using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;
using Unity.Profiling;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class RuntimeProfilerOverlay : MonoBehaviour
{
    private GameObject canvasGO;
    private Text infoText;
    private Button toggleButton;
    private GameObject panel;
    private bool isVisible = true;

    private List<ProfilerRecorder> recorders = new();
    private Dictionary<string, ProfilerRecorder> metrics = new();
    private Dictionary<string, List<float>> metricHistories = new();
    private Dictionary<string, float> averagedValues = new();

    int frameCount = 0;
    float frameTimer = 0f; // FPS専用タイマー
    private float updateInterval = 0.1f;
    private float updateTimer = 0f;
    private const int maxHistoryLength = 100;
    private float avgUpdateTimer = 0f;
    private const float avgUpdateInterval = 5f;

    private readonly struct MetricInfo
    {
        public readonly ProfilerCategory Category;
        public readonly string Name;
        public readonly string Display;
        public MetricInfo(ProfilerCategory category, string name, string display)
        {
            Category = category;
            Name = name;
            Display = display;
        }
    }

    private readonly MetricInfo[] metricsToTrack = new MetricInfo[]
    {
        new(ProfilerCategory.Render, "SetPass Calls Count", "SetPassCalls"),
        new(ProfilerCategory.Render, "Draw Calls Count", "DrawCalls"),
        new(ProfilerCategory.Render, "Batches Count", "Batches"),
        new(ProfilerCategory.Render, "Triangles Count", "Triangles"),
        new(ProfilerCategory.Render, "Vertices Count", "Vertices"),
        new(ProfilerCategory.Render, "Static Batched Draw Calls Count", "DrawCalls"),
        new(ProfilerCategory.Render, "Static Batches Count", "Batches"),
        new(ProfilerCategory.Render, "Static Batched Triangles Count", "Triangles"),
        new(ProfilerCategory.Render, "Static Batched Vertices Count", "Vertices"),
        new(ProfilerCategory.Render, "Instanced Batched Draw Calls Count", "DrawCalls"),
        new(ProfilerCategory.Render, "Instanced Batches Count", "Batches"),
        new(ProfilerCategory.Render, "Instanced Batched Triangles Count", "Triangles"),
        new(ProfilerCategory.Render, "Instanced Batched Vertices Count", "Vertices"),
        new(ProfilerCategory.Render, "Shadow Casters Count", "ShadowCasters"),
        new(ProfilerCategory.Internal, "CPU Total Frame Time", "CPUTime"),
        new(ProfilerCategory.Internal, "GPU Frame Time", "GPUTime"),
        new(ProfilerCategory.Memory, "System Used Memory", "CPUMem"),
        new(ProfilerCategory.Memory, "Gfx Used Memory", "GPUMem"),
    };
    void Awake()
    {
        Application.targetFrameRate = 200;
        QualitySettings.vSyncCount = 0;
    }
    void OnEnable()
    {
        CreateCanvasUI();
        StartRecorders();
    }

    void OnDisable()
    {
        foreach (var recorder in recorders)
        {
            if (recorder.Valid)
                recorder.Dispose();
        }
    }

    void CreateCanvasUI()
    {
        canvasGO = new GameObject("PerfCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        panel = new GameObject("BackgroundPanel");
        panel.transform.SetParent(canvasGO.transform);
        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(0, -150);
        panelRect.sizeDelta = new Vector2(0, 400);

        GameObject textGO = new GameObject("InfoText");
        textGO.transform.SetParent(panel.transform);
        infoText = textGO.AddComponent<Text>();
        infoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        infoText.fontSize = 32;
        infoText.color = Color.white;
        infoText.alignment = TextAnchor.UpperLeft;
        RectTransform textRect = infoText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);

        GameObject btnGO = new GameObject("ToggleButton");
        btnGO.transform.SetParent(canvasGO.transform);
        toggleButton = btnGO.AddComponent<Button>();
        var btnImage = btnGO.AddComponent<Image>();
        btnImage.color = Color.gray;
        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(1, 1);
        btnRect.anchorMax = new Vector2(1, 1);
        btnRect.pivot = new Vector2(1, 1);
        btnRect.anchoredPosition = new Vector2(-10, -10);
        btnRect.sizeDelta = new Vector2(160, 60);

        GameObject btnTextGO = new GameObject("BtnText");
        btnTextGO.transform.SetParent(btnGO.transform);
        var btnText = btnTextGO.AddComponent<Text>();
        btnText.text = "Toggle";
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.fontSize = 24;
        btnText.color = Color.black;
        btnText.alignment = TextAnchor.MiddleCenter;
        RectTransform btnTextRect = btnText.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;

        toggleButton.onClick.AddListener(() => ToggleVisibility());
    }

    void ToggleVisibility()
    {
        isVisible = !isVisible;
        panel.SetActive(isVisible);
    }

    void StartRecorders()
    {
        foreach (var metric in metricsToTrack)
        {
            var recorder = ProfilerRecorder.StartNew(metric.Category, metric.Name);
            if (recorder.Valid)
            {
                recorders.Add(recorder);
                metrics[metric.Name] = recorder;
                metricHistories[metric.Name] = new List<float>();
                averagedValues[metric.Name] = 0;
            }
        }
        metricHistories["FPS"] = new List<float>();
        averagedValues["FPS"] = 0;
    }

    string FormatKilo(float value) => (value / 1000f).ToString("F1") + "K";
    string FormatTime(float ms) => (ms / 1_000_000f).ToString("F2") + "ms";
    string FormatMemory(float bytes) => (bytes / (1024f * 1024f * 1024f)).ToString("F2") + "G";

    void Update()
    {
        if (!isVisible) return;

        updateTimer += Time.unscaledDeltaTime;
        frameTimer += Time.unscaledDeltaTime;
        frameCount++;

        if (updateTimer < updateInterval) return;
        updateTimer = 0f;

        // ✅ FPSだけ「数フレーム合計」で割る方式に修正
        float fps = frameCount / frameTimer;
        frameTimer = 0f;
        frameCount = 0;

        var fpsHistory = metricHistories["FPS"];
        fpsHistory.Add(fps);
        if (fpsHistory.Count > maxHistoryLength) fpsHistory.RemoveAt(0);

        string info = "";

        if (Time.unscaledTime - avgUpdateTimer >= avgUpdateInterval)
        {
            averagedValues["FPS"] = fpsHistory.Count > 0 ? fpsHistory.Average() : 0;
            foreach (var key in metrics.Keys)
            {
                if (metricHistories[key].Count > 0)
                    averagedValues[key] = metricHistories[key].Average();
            }
            avgUpdateTimer = Time.unscaledTime;
        }

        info += $"FPS: {averagedValues["FPS"]:F1} (Min:{fpsHistory.Min():F1}, Max:{fpsHistory.Max():F1}, Now:{fps:F1})\n\n";

        void AddGroup(string title, string[] names)
        {
            info += $"【{title}】\n";
            foreach (var metric in metricsToTrack)
            {
                if (!names.Contains(metric.Name)) continue;
                if (!metrics.TryGetValue(metric.Name, out var rec) || !rec.Valid) continue;

                float value = rec.LastValue;
                var history = metricHistories[metric.Name];
                history.Add(value);
                if (history.Count > maxHistoryLength) history.RemoveAt(0);

                float avgVal = averagedValues.TryGetValue(metric.Name, out float avg) ? avg : 0;

                string nowStr = value.ToString("F0");
                string minStr = history.Min().ToString("F0");
                string maxStr = history.Max().ToString("F0");
                string avgStr = avgVal.ToString("F0");

                if (metric.Name.Contains("Triangles") || metric.Name.Contains("Vertices"))
                {
                    nowStr = FormatKilo(value);
                    minStr = FormatKilo(history.Min());
                    maxStr = FormatKilo(history.Max());
                    avgStr = FormatKilo(avgVal);
                }
                else if (metric.Name.Contains("Time"))
                {
                    nowStr = FormatTime(value);
                    minStr = FormatTime(history.Min());
                    maxStr = FormatTime(history.Max());
                    avgStr = FormatTime(avgVal);
                }
                else if (metric.Name.Contains("Memory"))
                {
                    nowStr = FormatMemory(value);
                    minStr = FormatMemory(history.Min());
                    maxStr = FormatMemory(history.Max());
                    avgStr = FormatMemory(avgVal);
                }

                info += $"{metric.Display}: {avgStr} (Min:{minStr}, Max:{maxStr}, Now:{nowStr})\n";
            }
            info += "\n";
        }

        AddGroup("Basic", new[] { "SetPass Calls Count", "Draw Calls Count", "Batches Count", "Triangles Count", "Vertices Count" });
        AddGroup("Static Batched", new[] { "Static Batched Draw Calls Count", "Static Batches Count", "Static Batched Triangles Count", "Static Batched Vertices Count" });
        AddGroup("Instanced Batched", new[] { "Instanced Batched Draw Calls Count", "Instanced Batches Count", "Instanced Batched Triangles Count", "Instanced Batched Vertices Count" });
        AddGroup("Usage", new[] { "CPU Total Frame Time", "GPU Frame Time", "System Used Memory", "Gfx Used Memory" });

        infoText.text = info;
        float preferredHeight = infoText.preferredHeight + 20f;
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(0, preferredHeight);
    }
}