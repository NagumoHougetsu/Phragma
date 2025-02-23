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
    ProfilerRecorder drawCallsRecorder;//Drawcalls数
    ProfilerRecorder setPassCallsRecorder;//Setpasscalls数
    ProfilerRecorder totalBatchCountRecorder;//バッチ数
    ProfilerRecorder trianglesRecorder;//polygon数
    //Instance関連指標
    ProfilerRecorder instanceBatchDrawCallRecorder;//GPUインスタンスされたDrawcall数
    ProfilerRecorder instanceBatchCountRecorder;//GPUインスタンスされたBatch数
    ProfilerRecorder instanceBatchTrianglesRecorder;//GPUインスタンスされたpolygon数
    //アセット関連指標
    ProfilerRecorder usedTextureCountRecorder;//描画に使用されたTexture枚数
    ProfilerRecorder usedTextureMemoryRecorder;//描画に使用されたTextureメモリ
    //CPU処理時間
    ProfilerRecorder cpuTotalTimeRecorder;//総CPU処理時間
    ProfilerRecorder cpuMainThreadTimeRecorder;//CPUメインスレッド処理時間
    ProfilerRecorder cpuRenderThreadTimeRecorder;//CPUレンダースレッド処理時間
    ProfilerRecorder gpuTimeRecorder;//GPU描画にかかった時間/frame
    //Memory関連
    ProfilerRecorder totalMemoryRecorder;//総メモリ消費
    ProfilerRecorder loadedTextureCountRecorder;//描画だけでなくロードされているテクスチャのメモリ消費
    ProfilerRecorder loadedTextureMemoryRecorder;
    ProfilerRecorder loadedMeshCountRecorder;
    ProfilerRecorder loadedMeshMemoryRecorder;
    ProfilerRecorder loadedMaterialCountRecorder;
    ProfilerRecorder loadedMaterialMemoryRecorder;
    //GabageCollection
    ProfilerRecorder gcAlocCountRecorder;
    ProfilerRecorder gcAlocMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;
    ProfilerRecorder gcReservedMemoryRecorder;

    


    private float updateInterval = 1.0f; // 更新間隔（秒）
    private float lastUpdateTime = 0f;
    void Awake()
    {
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Profiler.enabled = true;
    #endif
    }

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
        // 現在選ばれているタブに応じて情報を表示
        if (currentTab == "Windows"){
            InvokeRepeating(nameof(UpdatePerformanceText), 0f, updateInterval);
        }
    }

    void OnEnable(){
        //RenderingPlofiler
        //全体的指標
        setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count", 1);
        drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 10);
        totalBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count", 10);
        trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count", 10);
        //Instance関連指標
        instanceBatchDrawCallRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Instanced Batched Draw Calls Count", 10);
        instanceBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Instanced Batches Count", 10);
        instanceBatchTrianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Instanced Batched Triangles Count", 10);
        //アセット関連指標
        usedTextureCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Used Textures Count", 50);
        usedTextureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Used Textures Bytes", 50);
        //CPU処理時間
        cpuTotalTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Total Frame Time", 30);
        cpuMainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Main Thread Frame Time", 30);
        cpuRenderThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Render Thread Frame Time", 30); 
        gpuTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "GPU Frame Time", 30); 
        //Memory関連
        totalMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory", 100);
        loadedTextureCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Texture Count", 100);
        loadedTextureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Texture Memory", 100);
        loadedMeshCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Mesh Count", 100);
        loadedMeshMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Mesh Memory", 100);
        loadedMaterialCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Material Count", 100);
        loadedMaterialMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Material Memory", 100);
        //GabageCollection
        gcAlocCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocation In Frame Count", 200);
        gcAlocMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 200);
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Used Memory", 200);
        gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory", 200);
    }

    void OnDisable(){
        //RenderingPlofiler
        //全体的指標
        setPassCallsRecorder.Dispose();
        drawCallsRecorder.Dispose();
        totalBatchCountRecorder.Dispose();
        trianglesRecorder.Dispose();
        //Instance関連指標
        instanceBatchDrawCallRecorder.Dispose();
        instanceBatchCountRecorder.Dispose();
        instanceBatchTrianglesRecorder.Dispose();
        //アセット関連指標
        usedTextureCountRecorder.Dispose();
        usedTextureMemoryRecorder.Dispose();
        //CPU処理時間
        cpuTotalTimeRecorder.Dispose();
        cpuMainThreadTimeRecorder.Dispose();
        cpuRenderThreadTimeRecorder.Dispose();
        gpuTimeRecorder.Dispose();
        //Memory関連
        cpuTotalTimeRecorder.Dispose();
        loadedTextureCountRecorder.Dispose();
        loadedTextureMemoryRecorder.Dispose();
        loadedMeshCountRecorder.Dispose();
        loadedMeshMemoryRecorder.Dispose();
        loadedMaterialCountRecorder.Dispose();
        loadedMaterialMemoryRecorder.Dispose();
        //GabageCollection
        gcAlocCountRecorder.Dispose();
        gcAlocMemoryRecorder.Dispose();
        gcMemoryRecorder.Dispose();
        gcReservedMemoryRecorder.Dispose();

    }

    void Update(){
        if (performanceText == null) return;
        // フレームごとに時間を加算
        timer += Time.unscaledDeltaTime;
        frameCount++;
        // 指定した時間ごとにFPSを更新
        if (timer >= updateInterval){
            fps = frameCount / timer; // FPSを算出
            timer = 0f;
            frameCount = 0;
            UpdatePerformanceText(); // FPSを更新
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
    
    void UpdatePerformanceText(){
        // ここで最新の値を取得して表示
        // 変化が5%以上あったら更新
        performanceText.text = GetWindowsPerformanceText(
            fps,
            drawCallsRecorder.LastValue, setPassCallsRecorder.LastValue, totalBatchCountRecorder.LastValue, trianglesRecorder.LastValue,
            instanceBatchDrawCallRecorder.LastValue, instanceBatchCountRecorder.LastValue, instanceBatchTrianglesRecorder.LastValue,
            usedTextureCountRecorder.LastValue, usedTextureMemoryRecorder.LastValue,                 
            cpuTotalTimeRecorder.LastValue, cpuMainThreadTimeRecorder.LastValue, cpuRenderThreadTimeRecorder.LastValue, gpuTimeRecorder.LastValue,
            totalMemoryRecorder.LastValue, loadedTextureCountRecorder.LastValue, loadedTextureMemoryRecorder.LastValue, 
            loadedMeshCountRecorder.LastValue,loadedMeshMemoryRecorder.LastValue, 
            loadedMaterialCountRecorder.LastValue, loadedMaterialMemoryRecorder.LastValue,
            gcAlocCountRecorder.LastValue, gcAlocMemoryRecorder.LastValue, gcMemoryRecorder.LastValue, gcReservedMemoryRecorder.LastValue
        );
    }

    string GetWindowsPerformanceText(float fps, 
                                    long drawcalls, long setpasscalls, long batches, long triangles, 
                                    long instanceDrawcalls, long instanceBatches, long instanceTriangles,
                                    long usedTextureCount, long usedTextureMemory,
                                    long cpuTotalTime, long cpuMainThreadTime, long cpuRenderThread, long gpuTime,
                                    long totalMemory, long loadedTextureCount, long loadedTextureMemory,
                                    long loadedMeshCount, long loadedMeshMemory,
                                    long loadedMaterialCount, long loadedMaterialMemory, 
                                    long gcAlocCount, long gcAlocMemory, long gcMemory, long gcReservedMemory){
        string text = $"【FPS】\n" +
                      $"{((int)fps).ToString("0").PadLeft(7)} Fps\n" +
                      $"{((int)setpasscalls).ToString("0").PadLeft(7)} Setpasscalls\n" +
                      $"{((int)drawcalls).ToString("0").PadLeft(7)} Drawcalls\n" +
                      $"{((int)instanceDrawcalls).ToString("0").PadLeft(7)} Drawcalls（GPUインスタンス）\n" +
                      $"{((int)batches).ToString("0").PadLeft(7)} Batches\n" +
                      $"{((int)instanceBatches).ToString("0").PadLeft(7)} Batches（GPUインスタンス）\n" +
                      $"{((int)triangles/1000).ToString("0").PadLeft(7)} K Triangles\n" +
                      $"{((int)instanceTriangles/1000).ToString("0").PadLeft(7)} K Triangles（GPUインスタンス）\n" +
                      $"【Texture】\n" +
                      $"{((int)usedTextureCount).ToString("0").PadLeft(7)} 枚（描画テクスチャ） \n" +  
                      $"{usedTextureMemory/1024/1024, 7:F2} MB（描画テクスチャMemory消費） \n" +
                      $"{((int)loadedTextureCount).ToString("0").PadLeft(7)} 枚（読込テクスチャ） \n" +  
                      $"{loadedTextureMemory/1024/1024, 7:F2} MB（読込テクスチャMemory消費) \n" +
                      $"【CPU処理時間】\n" +
                      $"{cpuTotalTime/1_000_000f, 7:F2} ミリ秒（総CPU処理時間) \n" +
                      $"{cpuMainThreadTime/ 1_000_000f, 7:F2} ミリ秒（CPUメインスレッド処理時間) \n" +
                      $"{cpuRenderThread/ 1_000_000f, 7:F2} ミリ秒（CPU描画スレッド処理時間) \n" +
                      $"{gpuTime/ 1_000_000f, 7:F2} ミリ秒（GPU処理時間) \n" +
                      $"【メモリ消費】\n" +
                      $"{totalMemory/1024/1024, 7:F2} MB（総Memory消費) \n" +
                      $"{((int)loadedMeshCount).ToString("N0").PadLeft(7)} 個（読込Mesh数） \n" + 
                      $"{loadedMeshMemory/1024/1024, 7:F2} MB（読込Meshメモリ消費) \n" +
                      $"{((int)loadedMaterialCount).ToString("N0").PadLeft(7)} 個（読込Material数） \n" +
                      $"{loadedMaterialMemory/1024/1024, 7:F2} MB（読込Materialメモリ消費) \n" +
                      $"【GabageCollection】\n" +
                      $"{((int)gcAlocCount).ToString("N0").PadLeft(7)} 回（発生GC回数/Frame） \n" + 
                      $"{gcAlocMemory/1024/1024, 7:F2} MB（確保GC Memory量/Frame) \n" +
                      $"{gcMemory/1024/1024, 7:F2} MB（使用中GC Memory) \n" +
                      $"{gcReservedMemory/1024/1024, 7:F2} MB（予約済GC Memory) ";
                      
                      
                      
        return text;
    }

}
