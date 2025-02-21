using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;
using Unity.Profiling;
using System.Text;

#if UNITY_EDITOR
using UnityEditor; // UnityStatsのために必要 
#endif


public class PerformanceMonitor : MonoBehaviour{
    public Text performanceText;
    public Button windowsTabButton;
    public Button snapdragonTabButton;
    public Button iOSTabButton;

    public GameObject windowsPanel;
    public GameObject snapdragonPanel;
    public GameObject iOSPanel;

    private float timer = 0f;
    private int frameCount = 0;
    private Button currentButton;  // 現在選ばれているボタンを保持する
    private string currentTab = "Windows";  // 現在のタブ情報（デフォルトはWindows）

    private FlopsManager flopsManager;
    private ProfilerRecorder cpuRecorder;
    private ProfilerRecorder gpuRecorder;

    public float fps { get; private set; }
    public long memory { get; private set; }
    public int drawCalls { get; private set; }
    public int setPassCalls { get; private set; }
    public int batches { get; private set; }
    public int triangles { get; private set; }
    public int vertices { get; private set; }
    public int materialCount { get; private set; }
    public int textureCount { get; private set; }
    public int objectCount { get; private set; }
    public float cpuUsage { get; private set; }
    public float gpuUsage { get; private set; }

    //RenderingPlofiler
    //全体的指標
    ProfilerRecorder setPassCallsRecorder;
    ProfilerRecorder drawCallsRecorder;
    ProfilerRecorder totalBatchCountRecorder;
    ProfilerRecorder trianglesRecorder;
    //static関連指標
    ProfilerRecorder staticBatchDrawCallRecorder;
    ProfilerRecorder staticBatchCountRecorder;
    ProfilerRecorder staticBatchTrianglesRecorder;
    //Instance関連指標
    ProfilerRecorder instanceBatchDrawCallRecorder;
    ProfilerRecorder instanceBatchCountRecorder;
    ProfilerRecorder instanceBatchTrianglesRecorder;
    //アセット関連指標
    ProfilerRecorder textureCountRecorder;
    ProfilerRecorder textureMemoryRecorder;
    //CPU Usage Proflier
    //CPU処理時間
    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder renderThreadTimeRecorder;

    

    ProfilerRecorder verticesCountRecorder;

    void Start(){
        // FlopsManagerを取得
        flopsManager = FindObjectOfType<FlopsManager>();
        // 各ボタンに対するリスナーの設定
        windowsTabButton.onClick.AddListener(() => OnWindowsTabClick());
        snapdragonTabButton.onClick.AddListener(() => OnSnapdragonTabClick());
        iOSTabButton.onClick.AddListener(() => OniOSTabClick());
        // 最初にデフォルトのタブボタン（Windows）を選択
        currentButton = windowsTabButton;
        SetButtonColors();
        SetActivePanel(windowsPanel); // 初期状態でWindowsのパネルを表示
    }

    void OnEnable(){
        //RenderingPlofiler
        //全体的指標
        setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
        drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        totalBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Total Batches Count");
        trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        //static関連指標
        staticBatchDrawCallRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Static Batched Draw Calls Count");
        staticBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Static Batches Count");
        staticBatchTrianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Static Batched Triangles Count");
        //Instance関連指標
        instanceBatchDrawCallRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Instanced Batched Draw Calls Count");
        instanceBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Instanced Batches Count");
        instanceBatchTrianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Instanced Batched Triangles Count");
        //アセット関連指標
        textureCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Used Textures Count");
        textureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Used Textures Memory");
        //CPU処理時間
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        renderThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Render Thread", 15); 

        // CPU Usage の ProfilerRecorder を開始
        cpuRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Scripts, "Timeline");
        // GPU Usage の ProfilerRecorder を開始
        gpuRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Total");
    }

    void OnDisable(){
        //RenderingPlofiler
        //全体的指標
        setPassCallsRecorder.Dispose();
        drawCallsRecorder.Dispose();
        totalBatchCountRecorder.Dispose();
        trianglesRecorder.Dispose();
        //static関連指標
        staticBatchDrawCallRecorder.Dispose();
        staticBatchCountRecorder.Dispose();
        staticBatchTrianglesRecorder.Dispose();
        //Instance関連指標
        instanceBatchDrawCallRecorder.Dispose();
        instanceBatchCountRecorder.Dispose();
        instanceBatchTrianglesRecorder.Dispose();
        //アセット関連指標
        textureCountRecorder.Dispose();
        textureMemoryRecorder.Dispose();
        //CPU処理時間
        mainThreadTimeRecorder.Dispose();
        renderThreadTimeRecorder.Dispose();

        cpuRecorder.Dispose();
        gpuRecorder.Dispose();
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
        memory = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024); // MB単位
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
        materialCount = Resources.FindObjectsOfTypeAll<Material>().Length;
        textureCount = Resources.FindObjectsOfTypeAll<Texture>().Length;
        objectCount = Resources.FindObjectsOfTypeAll<GameObject>().Length;
        // CPU Usage / GPU Usage の取得（数値はミリ秒単位なので Time.deltaTime で割合に換算）
        /*
        if (!cpuRecorder.Valid || !gpuRecorder.Valid){
            Debug.Log("ProfilerRecorder is NOT valid! Development Build を確認して");
        }
        */
        cpuUsage = cpuRecorder.LastValue / 1_000_000f;  // ミリ秒単位
        gpuUsage = gpuRecorder.LastValue / 1_000_000f;

        performanceText.supportRichText = true;  // 念のため
        // 現在選ばれているタブに応じて情報を表示
        if (currentTab == "Windows"){
            performanceText.text = GetWindowsPerformanceText(fps, memory, drawCalls, setPassCalls, batches, triangles, vertices, materialCount, textureCount, objectCount);
        }

    }

    private void SwitchTab(string tabName, Button clickedButton, GameObject panel){
        currentTab = tabName;
        currentButton = clickedButton;  // 現在選択されているボタンを更新
        SetButtonColors();  // ボタンの色を更新
        SetActivePanel(panel);  // 対応するパネルを表示
    }

    // 引数なしのメソッドを作成
    public void OnWindowsTabClick(){
        SwitchTab("Windows", windowsTabButton, windowsPanel);
    }

    public void OnSnapdragonTabClick(){
        SwitchTab("Snapdragon", snapdragonTabButton, snapdragonPanel);
    }

    public void OniOSTabClick(){
        SwitchTab("iOS", iOSTabButton, iOSPanel);
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

    void SetActivePanel(GameObject activePanel){
        // すべてのパネルを非表示にする
        windowsPanel.SetActive(false);
        snapdragonPanel.SetActive(false);
        iOSPanel.SetActive(false);

        // 選ばれたパネルだけを表示する
        activePanel.SetActive(true);
    }

    string GetWindowsPerformanceText(float fps, long memory, int drawCalls, int setPassCalls, 
                                    int batches, int triangles, int vertices, int materialCount, int textureCount, int objectCount){
        string text = $"FPS: {fps,21:F1} fps\n" +
                      $"DrawCalls数: {drawCallsRecorder.LastValue,13:N0} 回\n" +
                      $"SetPass数: {setPassCallsRecorder.LastValue,15:N0} 回\n" +
                      $"バッチ数: {totalBatchCountRecorder.LastValue,15:N0} 回\n" +
                      $"ポリゴン数: {trianglesRecorder.LastValue,15:N0} tris\n" +
                      $"StaticバッチDrawcall: {staticBatchDrawCallRecorder.LastValue,15:N0} 回\n" +
                      $"Staticバッチ数: {staticBatchCountRecorder.LastValue,15:N0} 回\n" +
                      $"Staticバッチポリゴン数: {staticBatchTrianglesRecorder.LastValue,15:N0} 回\n" +
                      $"InstanceバッチDrawcall: {instanceBatchDrawCallRecorder.LastValue,15:N0} 回\n" +
                      $"Instanceバッチ数: {instanceBatchCountRecorder.LastValue,15:N0} 回\n" +
                      $"Instanceバッチポリゴン数: {instanceBatchTrianglesRecorder.LastValue,15:N0} 回\n" +
                      $"テクスチャ数: {textureCountRecorder.LastValue,14:F1} 枚 \n" +
                      $"テクスチャ消費メモリ: {textureMemoryRecorder.LastValue,14:F1} MB";
        return text;
    }

}
