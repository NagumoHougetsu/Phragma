using UnityEngine;
using UnityEngine.UI;

public class FlopsManager : MonoBehaviour
{
    // しきい値の構造体
     [System.Serializable]
    public class Threshold{
        public float SafeLimit;
        public float WarningLimit;
        public Threshold(float safeLimit = 30f, float warningLimit = 50f){
            SafeLimit = safeLimit;
            WarningLimit = warningLimit;
        }
    }

    // PCの設定
    [Header("PC")]
    [SerializeField] private float cpuClockFrequency = 2.6f; // クロック周波数 (GHz)
    [SerializeField] private int cpuWidth = 2; // 処理幅（通常は2）
    [SerializeField] private int cpuCores = 14; // コア数 (6パフォーマンスコア + 8効率コア)
    [SerializeField] private float gpuClockFrequency = 2.45f; // GPU クロック周波数 (GHz)
    [SerializeField] private int gpuWidth = 2; // 処理幅（通常は2）
    [SerializeField] private int gpuShaderProcessors = 3584; // シェーダープロセッサ数 (CUDAコア数)

    // SoCの設定
    [Header("Snapdragon 660")]
        [SerializeField] private Text snapdragon660_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon660_memory;
        [SerializeField] private Text snapdragon660_drawcall;
        [SerializeField] private Text snapdragon660_setpasscall;
        [SerializeField] private Text snapdragon660_batch;
        [SerializeField] private Text snapdragon660_triangles;
        private Threshold snapdragon660_memoryThreshold = new Threshold(400f, 500f);
        private Threshold snapdragon660_drawcallThreshold = new Threshold(150f, 200f);
        private Threshold snapdragon660_setpasscallThreshold = new Threshold(100f, 150f);
        private Threshold snapdragon660_batchThreshold = new Threshold(200f, 250f);
        private Threshold snapdragon660_trianglesThreshold = new Threshold(200000f, 250000f);
        private float snapdragon660CpuFLOPS = 2.5f; //GFLOPS
        private float snapdragon660GpuFLOPS = 0.5f; //GFLOPS
        private float snapdragon660TotalFLOPS = 3.0f; //GFLOPS



    [Header("Snapdragon 675")]
        [SerializeField] private Text snapdragon675_fps;
        [SerializeField] private Text snapdragon675_memory;
        [SerializeField] private Text snapdragon675_drawcall;
        [SerializeField] private Text snapdragon675_setpasscall;
        [SerializeField] private Text snapdragon675_batch;
        [SerializeField] private Text snapdragon675_triangles;
        private Threshold snapdragon675Threshold;
        private Threshold snapdragon675_memoryThreshold = new Threshold(500f, 600f);
        private Threshold snapdragon675_drawcallThreshold = new Threshold(180f, 220f);
        private Threshold snapdragon675_setpasscallThreshold = new Threshold(120f, 160f);
        private Threshold snapdragon675_batchThreshold = new Threshold(250f, 300f);
        private Threshold snapdragon675_trianglesThreshold = new Threshold(250000f, 300000f);
        private float snapdragon675CpuFLOPS = 3.0f;
        private float snapdragon675GpuFLOPS = 0.8f;
        private float snapdragon675TotalFLOPS = 3.8f;

    [Header("Snapdragon 730G")]
        [SerializeField] private Text snapdragon730G_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon730G_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon730G_drawcall;
        [SerializeField] private Text snapdragon730G_setpasscall;
        [SerializeField] private Text snapdragon730G_batch;
        [SerializeField] private Text snapdragon730G_triangles;
        private Threshold snapdragon730GThreshold;
        private Threshold snapdragon730G_memoryThreshold = new Threshold(700f, 800f);
        private Threshold snapdragon730G_drawcallThreshold = new Threshold(200f, 250f);
        private Threshold snapdragon730G_setpasscallThreshold = new Threshold(150f, 200f);
        private Threshold snapdragon730G_batchThreshold = new Threshold(300f, 350f);
        private Threshold snapdragon730G_trianglesThreshold = new Threshold(300000f, 400000f);
        private float snapdragon730GCpuFLOPS = 3.5f;
        private float snapdragon730GGpuFLOPS = 1.0f;
        private float snapdragon730GTotalFLOPS = 4.5f;

    [Header("Snapdragon 765G")]
        [SerializeField] private Text snapdragon765G_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon765G_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon765G_drawcall;
        [SerializeField] private Text snapdragon765G_setpasscall;
        [SerializeField] private Text snapdragon765G_batch;
        [SerializeField] private Text snapdragon765G_triangles;
        private Threshold snapdragon765GThreshold;
        private Threshold snapdragon765G_memoryThreshold = new Threshold(800f, 900f);
        private Threshold snapdragon765G_drawcallThreshold = new Threshold(220f, 270f);
        private Threshold snapdragon765G_setpasscallThreshold = new Threshold(170f, 220f);
        private Threshold snapdragon765G_batchThreshold = new Threshold(350f, 400f);
        private Threshold snapdragon765G_trianglesThreshold = new Threshold(350000f, 450000f);
        private float snapdragon765GCpuFLOPS = 4.0f;
        private float snapdragon765GGpuFLOPS = 1.5f;
        private float snapdragon765GTotalFLOPS = 5.5f; 

    [Header("Snapdragon 778G")]
        [SerializeField] private Text snapdragon778G_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon778G_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon778G_drawcall;
        [SerializeField] private Text snapdragon778G_setpasscall;
        [SerializeField] private Text snapdragon778G_batch;
        [SerializeField] private Text snapdragon778G_triangles;
        private Threshold snapdragon778GThreshold;
        private Threshold snapdragon778G_memoryThreshold = new Threshold(900f, 1000f);
        private Threshold snapdragon778G_drawcallThreshold = new Threshold(250f, 300f);
        private Threshold snapdragon778G_setpasscallThreshold = new Threshold(200f, 250f);
        private Threshold snapdragon778G_batchThreshold = new Threshold(400f, 450f);
        private Threshold snapdragon778G_trianglesThreshold = new Threshold(400000f, 500000f);
        private float snapdragon778GCpuFLOPS = 4.5f; // グレーアウト
        private float snapdragon778GGpuFLOPS = 1.8f; // グレーアウト
        private float snapdragon778GTotalFLOPS = 6.3f; // グレーアウト

    [Header("Snapdragon 865")]
        [SerializeField] private Text snapdragon865_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon865_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon865_drawcall;
        [SerializeField] private Text snapdragon865_setpasscall;
        [SerializeField] private Text snapdragon865_batch;
        [SerializeField] private Text snapdragon865_triangles;
        private Threshold snapdragon865Threshold;
        private Threshold snapdragon865_memoryThreshold = new Threshold(1000f, 1100f);
        private Threshold snapdragon865_drawcallThreshold = new Threshold(250f, 300f);
        private Threshold snapdragon865_setpasscallThreshold = new Threshold(200f, 250f);
        private Threshold snapdragon865_batchThreshold = new Threshold(400f, 450f);
        private Threshold snapdragon865_trianglesThreshold = new Threshold(500000f, 700000f);
        private float snapdragon865CpuFLOPS = 5.5f; // グレーアウト
        private float snapdragon865GpuFLOPS = 1.8f; // グレーアウト
        private float snapdragon865TotalFLOPS = 6.3f; // グレーアウト

    [Header("Snapdragon 888")]
        [SerializeField] private Text snapdragon888_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon888_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon888_drawcall;
        [SerializeField] private Text snapdragon888_setpasscall;
        [SerializeField] private Text snapdragon888_batch;
        [SerializeField] private Text snapdragon888_triangles;
        private Threshold snapdragon888Threshold;
        private Threshold snapdragon888_memoryThreshold = new Threshold(1200f, 1400f);
        private Threshold snapdragon888_drawcallThreshold = new Threshold(300f, 350f);
        private Threshold snapdragon888_setpasscallThreshold = new Threshold(250f, 300f);
        private Threshold snapdragon888_batchThreshold = new Threshold(500f, 600f);
        private Threshold snapdragon888_trianglesThreshold = new Threshold(700000f, 900000f);
        private float snapdragon888CpuFLOPS = 6.5f; // グレーアウト
        private float snapdragon888GpuFLOPS = 3.0f; // グレーアウト
        private float snapdragon888TotalFLOPS = 9.5f; // グレーアウト

    [Header("Snapdragon 8Gen1")]
        [SerializeField] private Text snapdragon8Gen1_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen1_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen1_drawcall;
        [SerializeField] private Text snapdragon8Gen1_setpasscall;
        [SerializeField] private Text snapdragon8Gen1_batch;
        [SerializeField] private Text snapdragon8Gen1_triangles;
        private Threshold snapdragon8Gen1Threshold;
        private Threshold snapdragon8Gen1_memoryThreshold = new Threshold(1500f, 1600f);
        private Threshold snapdragon8Gen1_drawcallThreshold = new Threshold(350f, 400f);
        private Threshold snapdragon8Gen1_setpasscallThreshold = new Threshold(300f, 350f);
        private Threshold snapdragon8Gen1_batchThreshold = new Threshold(600f, 700f);
        private Threshold snapdragon8Gen1_trianglesThreshold = new Threshold(1000000f, 1200000f);
        private float snapdragon8Gen1CpuFLOPS = 7.0f; // グレーアウト
        private float snapdragon8Gen1GpuFLOPS = 4.0f; // グレーアウト
        private float snapdragon8Gen1TotalFLOPS = 11.0f; // グレーアウト

    [Header("Snapdragon 8Gen2")]
        [SerializeField] private Text snapdragon8Gen2_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen2_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen2_drawcall;
        [SerializeField] private Text snapdragon8Gen2_setpasscall;
        [SerializeField] private Text snapdragon8Gen2_batch;
        [SerializeField] private Text snapdragon8Gen2_triangles;
        private Threshold snapdragon8Gen2Threshold;
        private Threshold snapdragon8Gen2_memoryThreshold = new Threshold(1800f, 2000f);
        private Threshold snapdragon8Gen2_drawcallThreshold = new Threshold(400f, 500f);
        private Threshold snapdragon8Gen2_setpasscallThreshold = new Threshold(350f, 400f);
        private Threshold snapdragon8Gen2_batchThreshold = new Threshold(700f, 900f);
        private Threshold snapdragon8Gen2_trianglesThreshold = new Threshold(1500000f, 2000000f);
        private float snapdragon8Gen2CpuFLOPS = 7.5f; // グレーアウト
        private float snapdragon8Gen2GpuFLOPS = 4.5f; // グレーアウト
        private float snapdragon8Gen2TotalFLOPS = 12.0f; // グレーアウト

    //iOSのSoCの設定
    [Header("A10 Fusion")]
        [SerializeField] private Text a10_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text a10_memory;
        [SerializeField] private Text a10_drawcall;
        [SerializeField] private Text a10_setpasscall;
        [SerializeField] private Text a10_batch;
        [SerializeField] private Text a10_triangles;
        private Threshold a10_memoryThreshold = new Threshold(400f, 500f);
        private Threshold a10_drawcallThreshold = new Threshold(150f, 200f);
        private Threshold a10_setpasscallThreshold = new Threshold(100f, 150f);
        private Threshold a10_batchThreshold = new Threshold(200f, 250f);
        private Threshold a10_trianglesThreshold = new Threshold(200000f, 250000f);
        private float a10CpuFLOPS = 2.4f;
        private float a10GpuFLOPS = 1.0f; 
        private float a10TotalFLOPS = 3.4f; 

        [Header("A11 Bionic")]
        [SerializeField] private Text a11_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text a11_memory;
        [SerializeField] private Text a11_drawcall;
        [SerializeField] private Text a11_setpasscall;
        [SerializeField] private Text a11_batch;
        [SerializeField] private Text a11_triangles;
        private Threshold a11_memoryThreshold = new Threshold(500f, 600f);
        private Threshold a11_drawcallThreshold = new Threshold(180f, 220f);
        private Threshold a11_setpasscallThreshold = new Threshold(120f, 160f);
        private Threshold a11_batchThreshold = new Threshold(250f, 300f);
        private Threshold a11_trianglesThreshold = new Threshold(250000f, 300000f);
        private float a11CpuFLOPS = 2.8f;
        private float a11GpuFLOPS = 2.0f; 
        private float a11TotalFLOPS = 4.8f; 

        [Header("A12 Bionic")]
        [SerializeField] private Text a12_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text a12_memory;
        [SerializeField] private Text a12_drawcall;
        [SerializeField] private Text a12_setpasscall;
        [SerializeField] private Text a12_batch;
        [SerializeField] private Text a12_triangles;
        private Threshold a12_memoryThreshold = new Threshold(700f, 800f);
        private Threshold a12_drawcallThreshold = new Threshold(200f, 250f);
        private Threshold a12_setpasscallThreshold = new Threshold(150f, 200f);
        private Threshold a12_batchThreshold = new Threshold(300f, 350f);
        private Threshold a12_trianglesThreshold = new Threshold(300000f, 400000f);
        private float a12CpuFLOPS = 5.0f;
        private float a12GpuFLOPS = 3.5f; 
        private float a12TotalFLOPS = 8.5f; 

        [Header("A13 Bionic")]
        [SerializeField] private Text a13_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text a13_memory;
        [SerializeField] private Text a13_drawcall;
        [SerializeField] private Text a13_setpasscall;
        [SerializeField] private Text a13_batch;
        [SerializeField] private Text a13_triangles;
        private Threshold a13_memoryThreshold = new Threshold(800f, 900f);
        private Threshold a13_drawcallThreshold = new Threshold(220f, 270f);
        private Threshold a13_setpasscallThreshold = new Threshold(170f, 220f);
        private Threshold a13_batchThreshold = new Threshold(350f, 400f);
        private Threshold a13_trianglesThreshold = new Threshold(350000f, 450000f);
        private float a13CpuFLOPS = 6.0f;
        private float a13GpuFLOPS = 5.0f; 
        private float a13TotalFLOPS = 11.0f; 

        [Header("A14 Bionic")]
        [SerializeField] private Text a14_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text a14_memory;
        [SerializeField] private Text a14_drawcall;
        [SerializeField] private Text a14_setpasscall;
        [SerializeField] private Text a14_batch;
        [SerializeField] private Text a14_triangles;
        private Threshold a14_memoryThreshold = new Threshold(900f, 1000f);
        private Threshold a14_drawcallThreshold = new Threshold(250f, 300f);
        private Threshold a14_setpasscallThreshold = new Threshold(200f, 250f);
        private Threshold a14_batchThreshold = new Threshold(400f, 450f);
        private Threshold a14_trianglesThreshold = new Threshold(400000f, 500000f);
        private float a14CpuFLOPS = 7.0f;
        private float a14GpuFLOPS = 6.0f; 
        private float a14TotalFLOPS = 13.0f; 

        [Header("A15 Bionic")]
        [SerializeField] private Text a15_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text a15_memory;
        [SerializeField] private Text a15_drawcall;
        [SerializeField] private Text a15_setpasscall;
        [SerializeField] private Text a15_batch;
        [SerializeField] private Text a15_triangles;
        private Threshold a15_memoryThreshold = new Threshold(1200f, 1400f);
        private Threshold a15_drawcallThreshold = new Threshold(300f, 350f);
        private Threshold a15_setpasscallThreshold = new Threshold(250f, 300f);
        private Threshold a15_batchThreshold = new Threshold(500f, 600f);
        private Threshold a15_trianglesThreshold = new Threshold(700000f, 900000f);
        private float a15CpuFLOPS = 7.5f;
        private float a15GpuFLOPS = 6.5f; 
        private float a15TotalFLOPS = 14.0f; 

        [Header("A16 Bionic")]
        [SerializeField] private Text a16_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text a16_memory;
        [SerializeField] private Text a16_drawcall;
        [SerializeField] private Text a16_setpasscall;
        [SerializeField] private Text a16_batch;
        [SerializeField] private Text a16_triangles;
        private Threshold a16_memoryThreshold = new Threshold(1500f, 1600f);
        private Threshold a16_drawcallThreshold = new Threshold(350f, 400f);
        private Threshold a16_setpasscallThreshold = new Threshold(300f, 350f);
        private Threshold a16_batchThreshold = new Threshold(600f, 700f);
        private Threshold a16_trianglesThreshold = new Threshold(1000000f, 1200000f);
        private float a16CpuFLOPS = 8.5f;
        private float a16GpuFLOPS = 7.0f; 
        private float a16TotalFLOPS = 15.5f;

        [Header("A17 Pro")]
        [SerializeField] private Text a17_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text a17_memory;
        [SerializeField] private Text a17_drawcall;
        [SerializeField] private Text a17_setpasscall;
        [SerializeField] private Text a17_batch;
        [SerializeField] private Text a17_triangles;
        private Threshold a17_memoryThreshold = new Threshold(1800f, 2000f);
        private Threshold a17_drawcallThreshold = new Threshold(400f, 500f);
        private Threshold a17_setpasscallThreshold = new Threshold(350f, 400f);
        private Threshold a17_batchThreshold = new Threshold(700f, 900f);
        private Threshold a17_trianglesThreshold = new Threshold(1500000f, 2000000f);
        private float a17CpuFLOPS = 10.0f;
        private float a17GpuFLOPS = 8.0f; 
        private float a17TotalFLOPS = 18.0f;

     // PerformanceMonitor クラスのインスタンスを参照
    private PerformanceMonitor performanceMonitor;

    private void Start(){
        // 同じオブジェクトにアタッチされているPerformanceMonitorコンポーネントを取得
        performanceMonitor = GetComponent<PerformanceMonitor>();
        
        // PerformanceMonitorが正常に取得できていない場合のチェック
        if (performanceMonitor == null){
            Debug.LogError("PerformanceMonitor component not found on the same GameObject.");
        }
    }
    // FPS換算を計算するメソッド
    public float GetSnapdragonEquivalentFPS(float fps, float snapdragonTotalFLOPS){
        // PCのFLOPSを計算
        float pcTotalFLOPS = (cpuClockFrequency * cpuWidth * cpuCores) + (gpuClockFrequency * gpuWidth * gpuShaderProcessors);
        // FPSの換算を計算
        float equivalentFPS = fps * (snapdragonTotalFLOPS / pcTotalFLOPS) * 2500;
        return equivalentFPS;
    }

    // FPSに応じた色を変更するメソッド
    private void SetTextColorBasedOnFPS(Text textComponent, float fps){
        if (fps < 30){
            textComponent.color = Color.red; // FPSが30未満 → 赤
        }else if (fps >= 30 && fps < 60){
            textComponent.color = Color.yellow; // FPSが30以上60未満 → 黄色
        }else{
            textComponent.color = Color.blue; // FPSが60以上 → 青
        }
    }

    // 各パラメーターテキストの色を変更するメソッド
    private void SetTextColorBasedOnThreshold(Text textComponent, double value, Threshold threshold){
        if (value < threshold.SafeLimit){
            textComponent.color = Color.blue; // FPSが30未満 → 赤
        }else if (value >= threshold.SafeLimit && value < threshold.WarningLimit){
            textComponent.color = Color.yellow; // FPSが30以上60未満 → 黄色
        }else{
            textComponent.color = Color.red; // FPSが60以上 → 青
        }
    }

    void Update(){
        // PerformanceMonitor のFPSを取得
        if (performanceMonitor != null){
#if UNITY_EDITOR
        // Unityエディター専用情報
            int drawCalls = UnityEditor.UnityStats.drawCalls;
            int setPassCalls = UnityEditor.UnityStats.setPassCalls;
            int batches = UnityEditor.UnityStats.batches;
            int triangles = UnityEditor.UnityStats.triangles;
#endif
            float fps = performanceMonitor.fps;
            // 各Snapdragon SoC に対して換算 FPS を計算して表示
            float tempFps = GetSnapdragonEquivalentFPS(fps, snapdragon660TotalFLOPS);
            snapdragon660_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon660_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, snapdragon675TotalFLOPS);
            snapdragon675_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon675_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, snapdragon730GTotalFLOPS);
            snapdragon730G_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon730G_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, snapdragon765GTotalFLOPS);
            snapdragon765G_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon765G_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, snapdragon778GTotalFLOPS);
            snapdragon778G_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon778G_fps, tempFps);
            
            tempFps = GetSnapdragonEquivalentFPS(fps, snapdragon865TotalFLOPS);
            snapdragon865_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon865_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, snapdragon888TotalFLOPS);
            snapdragon888_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon888_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, snapdragon8Gen1TotalFLOPS);
            snapdragon8Gen1_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon8Gen1_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, snapdragon8Gen2TotalFLOPS);
            snapdragon8Gen2_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon8Gen2_fps, tempFps);
            /*
            // 各 SoC に対して換算 Memory を表示
            float memory = performanceMonitor.memory;
            snapdragon660_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(snapdragon660_memory, memory, snapdragon660_memoryThreshold);
            snapdragon675_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(snapdragon675_memory, memory, snapdragon675_memoryThreshold);
            snapdragon730G_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(snapdragon730G_memory, memory, snapdragon730G_memoryThreshold);
            snapdragon765G_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(snapdragon765G_memory, memory, snapdragon765G_memoryThreshold);
            snapdragon778G_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(snapdragon778G_memory, memory, snapdragon778G_memoryThreshold);
            snapdragon865_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(snapdragon865_memory, memory, snapdragon865_memoryThreshold);
            snapdragon888_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(snapdragon888_memory, memory, snapdragon888_memoryThreshold);
            snapdragon8Gen1_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(snapdragon8Gen1_memory, memory, snapdragon8Gen1_memoryThreshold);
            snapdragon8Gen2_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(snapdragon8Gen2_memory, memory, snapdragon8Gen2_memoryThreshold);

            // 各 SoC に対して換算 drawcall を表示
            snapdragon660_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon660_drawcall, drawCalls, snapdragon660_drawcallThreshold);
            snapdragon675_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon675_drawcall, drawCalls, snapdragon675_drawcallThreshold);
            snapdragon730G_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon730G_drawcall, drawCalls, snapdragon730G_drawcallThreshold);
            snapdragon765G_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon765G_drawcall, drawCalls, snapdragon765G_drawcallThreshold);
            snapdragon778G_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon778G_drawcall, drawCalls, snapdragon778G_drawcallThreshold);
            snapdragon865_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon865_drawcall, drawCalls, snapdragon865_drawcallThreshold);
            snapdragon888_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon888_drawcall, drawCalls, snapdragon888_drawcallThreshold);
            snapdragon8Gen1_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon8Gen1_drawcall, drawCalls, snapdragon8Gen1_drawcallThreshold);
            snapdragon8Gen2_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon8Gen2_drawcall, drawCalls, snapdragon8Gen2_drawcallThreshold);

            // 各 SoC に対して換算 setpasscall を表示
            snapdragon660_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon660_setpasscall, setPassCalls, snapdragon660_setpasscallThreshold);
            snapdragon675_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon675_setpasscall, setPassCalls, snapdragon675_setpasscallThreshold);
            snapdragon730G_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon730G_setpasscall, setPassCalls, snapdragon730G_setpasscallThreshold);
            snapdragon765G_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon765G_setpasscall, setPassCalls, snapdragon765G_setpasscallThreshold);
            snapdragon778G_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon778G_setpasscall, setPassCalls, snapdragon778G_setpasscallThreshold);
            snapdragon865_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon865_setpasscall, setPassCalls, snapdragon865_setpasscallThreshold);
            snapdragon888_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon888_setpasscall, setPassCalls, snapdragon888_setpasscallThreshold);
            snapdragon8Gen1_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon8Gen1_setpasscall, setPassCalls, snapdragon8Gen1_setpasscallThreshold);
            snapdragon8Gen2_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(snapdragon8Gen2_setpasscall, setPassCalls, snapdragon8Gen2_setpasscallThreshold);

            // 各 SoC に対して換算 batch を表示
            snapdragon660_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(snapdragon660_batch, batches, snapdragon660_batchThreshold);
            snapdragon675_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(snapdragon675_batch, batches, snapdragon675_batchThreshold);
            snapdragon730G_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(snapdragon730G_batch, batches, snapdragon730G_batchThreshold);
            snapdragon765G_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(snapdragon765G_batch, batches, snapdragon765G_batchThreshold);
            snapdragon778G_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(snapdragon778G_batch, batches, snapdragon778G_batchThreshold);
            snapdragon865_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(snapdragon865_batch, batches, snapdragon865_batchThreshold);
            snapdragon888_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(snapdragon888_batch, batches, snapdragon888_batchThreshold);
            snapdragon8Gen1_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(snapdragon8Gen1_batch, batches, snapdragon8Gen1_batchThreshold);
            snapdragon8Gen2_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(snapdragon8Gen2_batch, batches, snapdragon8Gen2_batchThreshold);

            // 各 SoC に対して換算 triangles を表示
            snapdragon660_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(snapdragon660_triangles, triangles, snapdragon660_trianglesThreshold);
            snapdragon675_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(snapdragon675_triangles, triangles, snapdragon675_trianglesThreshold);
            snapdragon730G_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(snapdragon730G_triangles, triangles, snapdragon730G_trianglesThreshold);
            snapdragon765G_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(snapdragon765G_triangles, triangles, snapdragon765G_trianglesThreshold);
            snapdragon778G_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(snapdragon778G_triangles, triangles, snapdragon778G_trianglesThreshold);
            snapdragon865_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(snapdragon865_triangles, triangles, snapdragon865_trianglesThreshold);
            snapdragon888_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(snapdragon888_triangles, triangles, snapdragon888_trianglesThreshold);
            snapdragon8Gen1_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(snapdragon8Gen1_triangles, triangles, snapdragon8Gen1_trianglesThreshold);
            snapdragon8Gen2_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(snapdragon8Gen2_triangles, triangles, snapdragon8Gen2_trianglesThreshold);

            // 各Apple SoC に対して換算 FPS を計算して表示
            tempFps = GetSnapdragonEquivalentFPS(fps, a10TotalFLOPS);
            a10_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(a10_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, a11TotalFLOPS);
            a11_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(a11_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, a12TotalFLOPS);
            a12_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(a12_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, a13TotalFLOPS);
            a13_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(a13_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, a14TotalFLOPS);
            a14_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(a14_fps, tempFps);
            
            tempFps = GetSnapdragonEquivalentFPS(fps, a15TotalFLOPS);
            a15_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(a15_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, a16TotalFLOPS);
            a16_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(a16_fps, tempFps);

            tempFps = GetSnapdragonEquivalentFPS(fps, a17TotalFLOPS);
            a17_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(a17_fps, tempFps);

            // 各 SoC に対して換算 Memory を表示
            memory = performanceMonitor.memory;
            a10_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(a10_memory, memory, a10_memoryThreshold);
            a11_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(a11_memory, memory, a11_memoryThreshold);
            a12_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(a12_memory, memory, a12_memoryThreshold);
            a13_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(a13_memory, memory, a13_memoryThreshold);
            a14_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(a14_memory, memory, a14_memoryThreshold);
            a15_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(a15_memory, memory, a15_memoryThreshold);
            a16_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(a16_memory, memory, a16_memoryThreshold);
            a17_memory.text = $"{memory:N0}MB";
            SetTextColorBasedOnThreshold(a17_memory, memory, a17_memoryThreshold);

            // 各 SoC に対して換算 drawcall を表示
            a10_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(a10_drawcall, drawCalls, a10_drawcallThreshold);
            a11_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(a11_drawcall, drawCalls, a11_drawcallThreshold);
            a12_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(a12_drawcall, drawCalls, a12_drawcallThreshold);
            a13_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(a13_drawcall, drawCalls, a13_drawcallThreshold);
            a14_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(a14_drawcall, drawCalls, a14_drawcallThreshold);
            a15_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(a15_drawcall, drawCalls, a15_drawcallThreshold);
            a16_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(a16_drawcall, drawCalls, a16_drawcallThreshold);
            a17_drawcall.text = $"{drawCalls:N0}回";
            SetTextColorBasedOnThreshold(a17_drawcall, drawCalls, a17_drawcallThreshold);

            // 各 SoC に対して換算 setpasscall を表示
            a10_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(a10_setpasscall, setPassCalls, a10_setpasscallThreshold);
            a11_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(a11_setpasscall, setPassCalls, a11_setpasscallThreshold);
            a12_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(a12_setpasscall, setPassCalls, a12_setpasscallThreshold);
            a13_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(a13_setpasscall, setPassCalls, a13_setpasscallThreshold);
            a14_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(a14_setpasscall, setPassCalls, a14_setpasscallThreshold);
            a15_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(a15_setpasscall, setPassCalls, a15_setpasscallThreshold);
            a16_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(a16_setpasscall, setPassCalls, a16_setpasscallThreshold);
            a17_setpasscall.text = $"{setPassCalls:N0}回";
            SetTextColorBasedOnThreshold(a17_setpasscall, setPassCalls, a17_setpasscallThreshold);
           
            // 各 SoC に対して換算 batch を表示
            a10_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(a10_batch, batches, a10_batchThreshold);
            a11_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(a11_batch, batches, a11_batchThreshold);
            a12_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(a12_batch, batches, a12_batchThreshold);
            a13_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(a13_batch, batches, a13_batchThreshold);
            a14_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(a14_batch, batches, a14_batchThreshold);
            a15_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(a15_batch, batches, a15_batchThreshold);
            a16_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(a16_batch, batches, a16_batchThreshold);
            a17_batch.text = $"{batches:N0}回";
            SetTextColorBasedOnThreshold(a17_batch, batches, a17_batchThreshold);

            // 各 SoC に対して換算 triangles を表示
            a10_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(a10_triangles, triangles, a10_trianglesThreshold);
            a11_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(a11_triangles, triangles, a11_trianglesThreshold);
            a12_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(a12_triangles, triangles, a12_trianglesThreshold);
            a13_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(a13_triangles, triangles, a13_trianglesThreshold);
            a14_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(a14_triangles, triangles, a14_trianglesThreshold);
            a15_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(a15_triangles, triangles, a15_trianglesThreshold);
            a16_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(a16_triangles, triangles, a16_trianglesThreshold);
            a17_triangles.text = $"{triangles:N0}tris";
            SetTextColorBasedOnThreshold(a17_triangles, triangles, a17_trianglesThreshold);
            */
        }
    }
}
