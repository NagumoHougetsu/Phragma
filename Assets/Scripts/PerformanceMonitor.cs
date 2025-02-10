using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

#if UNITY_EDITOR
using UnityEditor; // UnityStatsのために必要
#endif

public class PerformanceMonitor : MonoBehaviour{
    public Text performanceText;
    public Button windowsTabButton;
    public Button snapdragonTabButton;
    public Button iOSTabButton;

    private float timer = 0f;
    private int frameCount = 0;
    private float fps = 0;
    private Button currentButton;  // 現在選ばれているボタンを保持する
    private string currentTab = "Windows";  // 現在のタブ情報（デフォルトはWindows）

    void Start(){
        // 各ボタンに対するリスナーの設定
        windowsTabButton.onClick.AddListener(() => OnWindowsTabClick());
        snapdragonTabButton.onClick.AddListener(() => OnSnapdragonTabClick());
        iOSTabButton.onClick.AddListener(() => OniOSTabClick());
        // 最初にデフォルトのタブボタン（Windows）を選択
        currentButton = windowsTabButton;
        SetButtonColors();
    }

    void Update(){
        if (performanceText == null) return;

        timer += Time.deltaTime;
        frameCount++;

        if (timer >= 0.5f){
            fps = frameCount / timer;
            timer = 0;
            frameCount = 0;
        }

        long memory = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024); // MB単位

        // デフォルト値（ビルド環境用）
        int drawCalls = 0, setPassCalls = 0, batches = 0, triangles = 0, vertices = 0;

#if UNITY_EDITOR
        // Unityエディター専用情報
        drawCalls = UnityEditor.UnityStats.drawCalls;
        setPassCalls = UnityEditor.UnityStats.setPassCalls;
        batches = UnityEditor.UnityStats.batches;
        triangles = UnityEditor.UnityStats.triangles;
        vertices = UnityEditor.UnityStats.vertices;
#endif

        int materialCount = Resources.FindObjectsOfTypeAll<Material>().Length;
        int textureCount = Resources.FindObjectsOfTypeAll<Texture>().Length;
        int objectCount = Resources.FindObjectsOfTypeAll<GameObject>().Length;

        performanceText.supportRichText = true;  // 念のため

        // 現在選ばれているタブに応じて情報を表示
        if (currentTab == "Windows"){
            performanceText.text = GetWindowsPerformanceText(fps, memory, drawCalls, setPassCalls, batches, triangles, vertices, materialCount, textureCount, objectCount);
        }else if (currentTab == "Snapdragon"){
            performanceText.text = GetSnapdragonPerformanceText(fps, memory, drawCalls, setPassCalls, batches, triangles, vertices, materialCount, textureCount, objectCount);
        }else if (currentTab == "iOS"){
            performanceText.text = GetiOSPerformanceText(fps, memory, drawCalls, setPassCalls, batches, triangles, vertices, materialCount, textureCount, objectCount);
        }
    }

    private void SwitchTab(string tabName, Button clickedButton){
        currentTab = tabName;
        currentButton = clickedButton;  // 現在選択されているボタンを更新
        SetButtonColors();  // ボタンの色を更新
    }

    // 引数なしのメソッドを作成
    public void OnWindowsTabClick(){
        SwitchTab("Windows", windowsTabButton);
    }

    public void OnSnapdragonTabClick(){
        SwitchTab("Snapdragon", snapdragonTabButton);
    }

    public void OniOSTabClick(){
        SwitchTab("iOS", iOSTabButton);
    }

    void SetButtonColors(){
        // すべてのボタンを暗くする
        SetButtonColor(windowsTabButton, Color.gray);
        SetButtonColor(snapdragonTabButton, Color.gray);
        SetButtonColor(iOSTabButton, Color.gray);

        // 現在選ばれているボタンを明るくする
        SetButtonColor(currentButton, Color.white);
    }

    void SetButtonColor(Button button, Color color){
        var colors = button.colors;
        colors.normalColor = color;
        button.colors = colors;  // ボタンの色を更新
    }

    string GetWindowsPerformanceText(float fps, long memory, int drawCalls, int setPassCalls, int batches, int triangles, int vertices, int materialCount, int textureCount, int objectCount)
    {
        return $"FPS: {fps,23:F1} fps\n" +
               $"メモリ消費: {memory,15:N0} MB\n" +
               $"DrawCalls数: {drawCalls,13:N0} 回\n" +
               $"SetPass数: {setPassCalls,15:N0} 回\n" +
               $"Batches数: {batches,15:N0} 個\n" +
               $"表示ポリゴン数: {triangles,11:N0} ポリゴン\n" +
               $"表示頂点数: {vertices,15:N0} 頂点\n" +
               $"使用マテリアル数: {materialCount,9:N0} 個\n" +
               $"使用テクスチャ数: {textureCount,9:N0} 枚\n" +
               $"表示オブジェクト数: {objectCount,7:N0} 個";
    }

    string GetSnapdragonPerformanceText(float fps, long memory, int drawCalls, int setPassCalls, int batches, int triangles, int vertices, int materialCount, int textureCount, int objectCount)
    {
        return $"FPS: {fps,23:F1} fps\n" +
               $"メモリ消費: {memory,15:N0} MB\n" +
               $"DrawCalls数: {drawCalls,13:N0} 回\n" +
               $"SetPass数: {setPassCalls,15:N0} 回\n" +
               $"Batches数: {batches,15:N0} 個\n" +
               $"表示ポリゴン数: {triangles,11:N0} ポリゴン\n" +
               $"表示頂点数: {vertices,15:N0} 頂点\n" +
               $"使用マテリアル数: {materialCount,9:N0} 個\n" +
               $"使用テクスチャ数: {textureCount,9:N0} 枚\n" +
               $"表示オブジェクト数: {objectCount,7:N0} 個\n" +
               $"-----------------------------------------------------\n" +
               $"|Item    |Snadra660|Snadra765G|Snadra845|Snadra8GenX|\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|DC(safe)|    -100 |    -200  |    -300 |  300-400  |\n" +
               $"|DC(full)| 100-200 | 200-300  | 300-400 |  400-500  |\n" +
               $"|DC(risk)| 200-    | 300-     | 400-    |  500-     |\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|SC(safe)|  30- 60 |  50-100  | 100-150 |  150-200  |\n" +
               $"|SC(full)|  60-100 | 100-150  | 150-200 |  200-300  |\n" +
               $"|SC(risk)| 100-    | 150-     | 200-    |  300-     |\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|BT(safe)|  50-100 |    -150  |    -200 |     -250  |\n" +
               $"|BT(full)| 100-150 | 150-200  | 200-250 |  250-300  |\n" +
               $"|BT(risk)| 150-    | 200-     | 250-    |  300-     |\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|MM(safe)|   -1.0GB|   -1.5GB |   -2.0GB|    -2.0GB |\n" +
               $"|MM(full)|1.0-1.5GB|1.5-2.0GB |2.0-2.5GB| 2.0-3.0GB |\n" +
               $"|MM(risk)|1.5GB-   |2.0GB-    |2.5GB-   | 3.0GB-    |\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|TR(safe)|    -100K|    -200K |   -300K |    -500K  |\n" +
               $"|TR(full)| 100-150K| 200-300K |300-400K | 500-600K  |\n" +
               $"|TR(risk)| 150K-   | 300K-    |400K-    | 600K-     |\n" +
               $"-----------------------------------------------------";
    }

    string GetiOSPerformanceText(float fps, long memory, int drawCalls, int setPassCalls, int batches, int triangles, int vertices, int materialCount, int textureCount, int objectCount)
    {
        return $"FPS: {fps,23:F1} fps\n" +
               $"メモリ消費: {memory,15:N0} MB\n" +
               $"DrawCalls数: {drawCalls,13:N0} 回\n" +
               $"SetPass数: {setPassCalls,15:N0} 回\n" +
               $"Batches数: {batches,15:N0} 個\n" +
               $"表示ポリゴン数: {triangles,11:N0} ポリゴン\n" +
               $"表示頂点数: {vertices,15:N0} 頂点\n" +
               $"使用マテリアル数: {materialCount,9:N0} 個\n" +
               $"使用テクスチャ数: {textureCount,9:N0} 枚\n" +
               $"表示オブジェクト数: {objectCount,7:N0} 個\n" +
               $"-----------------------------------------------------\n" +
               $"|Item    |A11Bionic|A13Bionic |A14Bionic| A16Bionic |\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|DC(safe)|    -100 |    -200  |    -300 |  300-400  |\n" +
               $"|DC(full)| 100-200 | 200-300  | 300-400 |  400-500  |\n" +
               $"|DC(risk)| 200-    | 300-     | 400-    |  500-     |\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|SC(safe)|  30- 60 |  50-100  | 100-150 |  150-200  |\n" +
               $"|SC(full)|  60-100 | 100-150  | 150-200 |  200-300  |\n" +
               $"|SC(risk)| 100-    | 150-     | 200-    |  300-     |\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|BT(safe)|  50-100 |    -150  |    -200 |     -250  |\n" +
               $"|BT(full)| 100-150 | 150-200  | 200-250 |  250-300  |\n" +
               $"|BT(risk)| 150-    | 200-     | 250-    |  300-     |\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|MM(safe)|   -1.0GB|   -1.5GB |   -2.0GB|    -2.0GB |\n" +
               $"|MM(full)|1.0-1.5GB|1.5-2.0GB |2.0-2.5GB| 2.0-3.0GB |\n" +
               $"|MM(risk)|1.5GB-   |2.0GB-    |2.5GB-   | 3.0GB-    |\n" +
               $"|--------|---------|----------|---------|-----------|\n" +
               $"|TR(safe)|    -100K|    -200K |   -300K |    -500K  |\n" +
               $"|TR(full)| 100-150K| 200-300K |300-400K | 500-600K  |\n" +
               $"|TR(risk)| 150K-   | 300K-    |400K-    | 600K-     |\n" +
               $"-----------------------------------------------------";
    }
}
