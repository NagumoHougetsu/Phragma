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
    ProfilerRecorder drawCallsRecorder;
    ProfilerRecorder setPassCallsRecorder;
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
    ProfilerRecorder totalTimeRecorder;
    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder renderThreadTimeRecorder;
    ProfilerRecorder gpuTimeRecorder;
    //Memory関連
    ProfilerRecorder totalMemoryRecorder;
    ProfilerRecorder textureCountRecorder2;
    ProfilerRecorder meshCountRecorder;
    ProfilerRecorder meshMemoryRecorder;
    ProfilerRecorder materialCountRecorder;
    ProfilerRecorder materialMemoryRecorder;
    //GabageCollection
    ProfilerRecorder gcAlocCountRecorder;
    ProfilerRecorder gcAlocMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;
    ProfilerRecorder gcReservedMemoryRecorder;

    


    private float updateInterval = 1.0f; // 更新間隔（秒）
    private float lastUpdateTime = 0f;

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
        setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count", 15);
        drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 15);
        totalBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count", 15);
        trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count", 15);
        //static関連指標
        staticBatchDrawCallRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Static Batched Draw Calls Count", 15);
        staticBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Static Batches Count", 15);
        staticBatchTrianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Static Batched Triangles Count", 15);
        //Instance関連指標
        instanceBatchDrawCallRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Instanced Batched Draw Calls Count", 15);
        instanceBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Instanced Batches Count", 15);
        instanceBatchTrianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Instanced Batched Triangles Count", 15);
        //アセット関連指標
        textureCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Used Textures Count");
        textureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Used Textures Bytes");
        //CPU処理時間
        totalTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Total Frame Time", 15);
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Main Thread Frame Time", 15);
        renderThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Render Thread Frame Time", 15); 
        gpuTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "GPU Frame Time", 15); 
        //Memory関連
        totalMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory", 15);
        textureCountRecorder2 = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Texture Count", 15);
        meshCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Mesh Count", 15);
        meshMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Mesh Memory", 15);
        materialCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Material Count", 15);
        materialMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Material Memory", 15);
        //GabageCollection
        gcAlocCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocation In Frame Count", 15);
        gcAlocMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 15);
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Used Memory", 15);
        gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory", 15);
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
        totalTimeRecorder.Dispose();
        mainThreadTimeRecorder.Dispose();
        renderThreadTimeRecorder.Dispose();
        gpuTimeRecorder.Dispose();
        //Memory関連
        totalMemoryRecorder.Dispose();
        textureCountRecorder2.Dispose();
        meshCountRecorder.Dispose();
        meshMemoryRecorder.Dispose();
        materialCountRecorder.Dispose();
        materialMemoryRecorder.Dispose();
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
            staticBatchDrawCallRecorder.LastValue, staticBatchCountRecorder.LastValue, staticBatchTrianglesRecorder.LastValue, 
            instanceBatchDrawCallRecorder.LastValue, instanceBatchCountRecorder.LastValue, instanceBatchTrianglesRecorder.LastValue,
            textureCountRecorder.LastValue, textureMemoryRecorder.LastValue,                 
            totalTimeRecorder.LastValue, mainThreadTimeRecorder.LastValue, renderThreadTimeRecorder.LastValue, gpuTimeRecorder.LastValue,
            totalMemoryRecorder.LastValue, textureCountRecorder2.LastValue, meshCountRecorder.LastValue,
            meshMemoryRecorder.LastValue, materialCountRecorder.LastValue, materialMemoryRecorder.LastValue,
            gcAlocCountRecorder.LastValue, gcAlocMemoryRecorder.LastValue, gcMemoryRecorder.LastValue, gcReservedMemoryRecorder.LastValue
        );
    }

    string GetWindowsPerformanceText(float fps, 
                                    long drawcalls, long setpasscalls, long batches, long triangles, 
                                    long staticDrawcalls , long staticBatches, long staticTriangles,
                                    long instanceDrawcalls, long instanceBatches, long instanceTriangles,
                                    long textureCount, long textureMemory, long renderThreadTime ,
                                    long totalTime, long mainThreadTime, long gpuTime,
                                    long totalMemory, long textureCount2, long meshCount, long meshMemory,
                                    long materialCount, long materialMemory, 
                                    long gcAlocCount, long gcAlocMemory, long gcMemory, long gcReservedMemory){
        string text = $"{((int)fps).ToString("N0").PadLeft(3)} fps\n" +
                      $"{((int)drawcalls).ToString("N0").PadLeft(3)} Drawcalls\n" +
                      $"{((int)setpasscalls).ToString("N0").PadLeft(3)} Setpasscalls\n" +
                      $"{((int)batches).ToString("N0").PadLeft(3)} Batches\n" +
                      $"{((int)triangles/1000).ToString("N0").PadLeft(3)} K Triangles\n" +
                      $"{((int)staticDrawcalls).ToString("N0").PadLeft(3)} Drawcalls(Statie)\n" +
                      $"{((int)staticBatches).ToString("N0").PadLeft(3)} Batches(Static)\n" +
                      $"{((int)staticTriangles/1000).ToString("N0").PadLeft(3)} K Triangles(Static)\n" +
                      $"{((int)instanceDrawcalls).ToString("N0").PadLeft(3)} Drawcalls(GPU)\n" +
                      $"{((int)instanceBatches).ToString("N0").PadLeft(3)} Batches(GPU)\n" +
                      $"{((int)instanceTriangles/1000).ToString("N0").PadLeft(3)} K Triangles(GPU)\n" +
                      $"{((int)textureCount).ToString("N0").PadLeft(3)} 枚 \n" +  
                      $"{textureMemory/1024/1024,5:0000.0} MB(Textureメモリ) \n" +
                      $"{renderThreadTime/1_000_000f,4:00.00} ミリ秒(CPU Total) \n" +
                      $"{totalTime/ 1_000_000f,4:00.00} ミリ秒(CPU Total) \n" +
                      $"{mainThreadTime/ 1_000_000f,4:00.00} ミリ秒(CPU mainThreadTime) \n" +
                      $"{gpuTime/ 1_000_000f,4:00.00} ミリ秒(GPU) \n" +
                      $"{totalMemory/1024/1024,5:0000.0} MB(Total Memory) \n" +
                      $"{((int)textureCount2).ToString("N0").PadLeft(3)} 枚 \n" +  
                      $"{((int)meshCount).ToString("N0").PadLeft(3)} Mesh \n" + 
                      $"{meshMemory/1024/1024,5:0000.0} MB(Mesh Memory) \n" +
                      $"{((int)materialCount).ToString("N0").PadLeft(3)} Material \n" +
                      $"{materialMemory/1024/1024,5:0000.0} MB(Material Memory) \n" +
                      $"{((int)gcAlocCount).ToString("N0").PadLeft(3)} 回（GC Allocated In Frame） \n" + 
                      $"{gcAlocMemory/1024/1024,5:0000.0} MB(GC Allocated In Frame) \n" +
                      $"{gcMemory/1024/1024,5:0000.0} MB(GC Used Memory) \n" +
                      $"{gcReservedMemory/1024/1024,5:0000.0} MB(GC Reserved Memory) ";
                      
                      
                      
        return text;
    }

}
