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
        [SerializeField] private Text snapdragon660_cpuTime;
        [SerializeField] private Text snapdragon660_gpuTime;
        [SerializeField] private Text snapdragon660_cpuGpuTime;
        private Threshold snapdragon660_memoryThreshold = new Threshold(400f, 500f);
        private Threshold snapdragon660_drawcallThreshold = new Threshold(150f, 200f);
        private Threshold snapdragon660_setpasscallThreshold = new Threshold(100f, 150f);
        private Threshold snapdragon660_batchThreshold = new Threshold(200f, 250f);
        private Threshold snapdragon660_trianglesThreshold = new Threshold(200000f, 250000f);
        private float snapdragon660CpuFLOPS = 48f; //GFLOPS
        private float snapdragon660GpuFLOPS = 165.6f; //GFLOPS
        private float snapdragon660TotalFLOPS = 3.0f; //GFLOPS



    [Header("Snapdragon 675")]
        [SerializeField] private Text snapdragon675_fps;
        [SerializeField] private Text snapdragon675_cpuTime;
        [SerializeField] private Text snapdragon675_gpuTime;
        [SerializeField] private Text snapdragon675_cpuGpuTime;
        private Threshold snapdragon675Threshold;
        private Threshold snapdragon675_memoryThreshold = new Threshold(500f, 600f);
        private Threshold snapdragon675_drawcallThreshold = new Threshold(180f, 220f);
        private Threshold snapdragon675_setpasscallThreshold = new Threshold(120f, 160f);
        private Threshold snapdragon675_batchThreshold = new Threshold(250f, 300f);
        private Threshold snapdragon675_trianglesThreshold = new Threshold(250000f, 300000f);
        private float snapdragon675CpuFLOPS = 36.4f;
        private float snapdragon675GpuFLOPS = 328.2f;
        private float snapdragon675TotalFLOPS = 3.8f;

    [Header("Snapdragon 730G")]
        [SerializeField] private Text snapdragon730G_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon730G_cpuTime; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon730G_gpuTime;
        [SerializeField] private Text snapdragon730G_cpuGpuTime;
        private Threshold snapdragon730GThreshold;
        private Threshold snapdragon730G_memoryThreshold = new Threshold(700f, 800f);
        private Threshold snapdragon730G_drawcallThreshold = new Threshold(200f, 250f);
        private Threshold snapdragon730G_setpasscallThreshold = new Threshold(150f, 200f);
        private Threshold snapdragon730G_batchThreshold = new Threshold(300f, 350f);
        private Threshold snapdragon730G_trianglesThreshold = new Threshold(300000f, 400000f);
        private float snapdragon730GCpuFLOPS = 39.2f;
        private float snapdragon730GGpuFLOPS = 422.4f;
        private float snapdragon730GTotalFLOPS = 4.5f;

    [Header("Snapdragon 765G")]
        [SerializeField] private Text snapdragon765G_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon765G_cpuTime; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon765G_gpuTime;
        [SerializeField] private Text snapdragon765G_cpuGpuTime;
        private Threshold snapdragon765GThreshold;
        private Threshold snapdragon765G_memoryThreshold = new Threshold(800f, 900f);
        private Threshold snapdragon765G_drawcallThreshold = new Threshold(220f, 270f);
        private Threshold snapdragon765G_setpasscallThreshold = new Threshold(170f, 220f);
        private Threshold snapdragon765G_batchThreshold = new Threshold(350f, 400f);
        private Threshold snapdragon765G_trianglesThreshold = new Threshold(350000f, 450000f);
        private float snapdragon765GCpuFLOPS = 76.8f;
        private float snapdragon765GGpuFLOPS = 576f;
        private float snapdragon765GTotalFLOPS = 5.5f; 

    [Header("Snapdragon 778G")]
        [SerializeField] private Text snapdragon778G_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon778G_cpuTime; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon778G_gpuTime;
        [SerializeField] private Text snapdragon778G_cpuGpuTime;
        private Threshold snapdragon778GThreshold;
        private Threshold snapdragon778G_memoryThreshold = new Threshold(900f, 1000f);
        private Threshold snapdragon778G_drawcallThreshold = new Threshold(250f, 300f);
        private Threshold snapdragon778G_setpasscallThreshold = new Threshold(200f, 250f);
        private Threshold snapdragon778G_batchThreshold = new Threshold(400f, 450f);
        private Threshold snapdragon778G_trianglesThreshold = new Threshold(400000f, 500000f);
        private float snapdragon778GCpuFLOPS = 139.2f; // グレーアウト
        private float snapdragon778GGpuFLOPS = 563.2f; // グレーアウト
        private float snapdragon778GTotalFLOPS = 6.3f; // グレーアウト

    [Header("Snapdragon 865")]
        [SerializeField] private Text snapdragon865_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon865_cpuTime; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon865_gpuTime;
        [SerializeField] private Text snapdragon865_cpuGpuTime;
        private Threshold snapdragon865Threshold;
        private Threshold snapdragon865_memoryThreshold = new Threshold(1000f, 1100f);
        private Threshold snapdragon865_drawcallThreshold = new Threshold(250f, 300f);
        private Threshold snapdragon865_setpasscallThreshold = new Threshold(200f, 250f);
        private Threshold snapdragon865_batchThreshold = new Threshold(400f, 450f);
        private Threshold snapdragon865_trianglesThreshold = new Threshold(500000f, 700000f);
        private float snapdragon865CpuFLOPS = 169.6f; // グレーアウト
        private float snapdragon865GpuFLOPS = 1029.1f; // グレーアウト
        private float snapdragon865TotalFLOPS = 6.3f; // グレーアウト

    [Header("Snapdragon 888")]
        [SerializeField] private Text snapdragon888_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon888_cpuTime; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon888_gpuTime;
        [SerializeField] private Text snapdragon888_cpuGpuTime;
        private Threshold snapdragon888Threshold;
        private Threshold snapdragon888_memoryThreshold = new Threshold(1200f, 1400f);
        private Threshold snapdragon888_drawcallThreshold = new Threshold(300f, 350f);
        private Threshold snapdragon888_setpasscallThreshold = new Threshold(250f, 300f);
        private Threshold snapdragon888_batchThreshold = new Threshold(500f, 600f);
        private Threshold snapdragon888_trianglesThreshold = new Threshold(700000f, 900000f);
        private float snapdragon888CpuFLOPS = 172.8f; // グレーアウト
        private float snapdragon888GpuFLOPS = 1290.2f; // グレーアウト
        private float snapdragon888TotalFLOPS = 9.5f; // グレーアウト

    [Header("Snapdragon 8Gen1")]
        [SerializeField] private Text snapdragon8Gen1_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen1_cpuTime; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen1_gpuTime;
        [SerializeField] private Text snapdragon8Gen1_cpuGpuTime;
        private Threshold snapdragon8Gen1Threshold;
        private Threshold snapdragon8Gen1_memoryThreshold = new Threshold(1500f, 1600f);
        private Threshold snapdragon8Gen1_drawcallThreshold = new Threshold(350f, 400f);
        private Threshold snapdragon8Gen1_setpasscallThreshold = new Threshold(300f, 350f);
        private Threshold snapdragon8Gen1_batchThreshold = new Threshold(600f, 700f);
        private Threshold snapdragon8Gen1_trianglesThreshold = new Threshold(1000000f, 1200000f);
        private float snapdragon8Gen1CpuFLOPS = 198.4f; // グレーアウト
        private float snapdragon8Gen1GpuFLOPS = 1720.3f; // グレーアウト
        private float snapdragon8Gen1TotalFLOPS = 11.0f; // グレーアウト

    [Header("Snapdragon 8Gen2")]
        [SerializeField] private Text snapdragon8Gen2_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen2_cpuTime; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen2_gpuTime;
        [SerializeField] private Text snapdragon8Gen2_cpuGpuTime;
        private Threshold snapdragon8Gen2Threshold;
        private Threshold snapdragon8Gen2_memoryThreshold = new Threshold(1800f, 2000f);
        private Threshold snapdragon8Gen2_drawcallThreshold = new Threshold(400f, 500f);
        private Threshold snapdragon8Gen2_setpasscallThreshold = new Threshold(350f, 400f);
        private Threshold snapdragon8Gen2_batchThreshold = new Threshold(700f, 900f);
        private Threshold snapdragon8Gen2_trianglesThreshold = new Threshold(1500000f, 2000000f);
        private float snapdragon8Gen2CpuFLOPS = 274.4f; // グレーアウト
        private float snapdragon8Gen2GpuFLOPS = 2208.8f; // グレーアウト
        private float snapdragon8Gen2TotalFLOPS = 12.0f; // グレーアウト

    //iOSのSoCの設定
    [Header("A10 Fusion")]
        [SerializeField] private Text a10_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text a10_cpuTime;
        [SerializeField] private Text a10_gpuTime;
        [SerializeField] private Text a10_cpuGpuTime;
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
        [SerializeField] private Text a11_cpuTime;
        [SerializeField] private Text a11_gpuTime;
        [SerializeField] private Text a11_cpuGpuTime;
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
        [SerializeField] private Text a12_cpuTime;
        [SerializeField] private Text a12_gpuTime;
        [SerializeField] private Text a12_cpuGpuTime;
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
        [SerializeField] private Text a13_cpuTime;
        [SerializeField] private Text a13_gpuTime;
        [SerializeField] private Text a13_cpuGpuTime;
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
        [SerializeField] private Text a14_cpuTime;
        [SerializeField] private Text a14_gpuTime;
        [SerializeField] private Text a14_cpuGpuTime;
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
        [SerializeField] private Text a15_cpuTime;
        [SerializeField] private Text a15_gpuTime;
        [SerializeField] private Text a15_cpuGpuTime;
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
        [SerializeField] private Text a16_cpuTime;
        [SerializeField] private Text a16_gpuTime;
        [SerializeField] private Text a16_cpuGpuTime;
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
        [SerializeField] private Text a17_cpuTime;
        [SerializeField] private Text a17_gpuTime;
        [SerializeField] private Text a17_cpuGpuTime;
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
        float equivalentFPS = fps * (snapdragonTotalFLOPS / pcTotalFLOPS) * 500;
        return equivalentFPS;
    }

    //CPU時間を換算するメソッド
    public float GetEquivalentCPU(float cpu, float cpuFlops){
        // PCのFLOPSを計算
        float pcCpuFlops = cpuClockFrequency * cpuWidth * cpuCores;
        float equivalentCpu = cpu * (pcCpuFlops / cpuFlops);
        return equivalentCpu;
    }
    //GPU時間を換算するメソッド
    public float GetEquivalentGPU(float gpu, float gpuFlops){
        // PCのFLOPSを計算
        float pcGpuFlops = gpuClockFrequency * gpuWidth * gpuShaderProcessors;
        float equivalentGpu = gpu * (pcGpuFlops / gpuFlops);
        return equivalentGpu;
    }

    //FPSを計算するメソッド
    public float GetEquivalentFPS(float cpuTime, float gpuTime){
        float equivalentFps = 1000 / (cpuTime + gpuTime);
        return equivalentFps;
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
            /*
            int drawCalls = UnityEditor.UnityStats.drawCalls;
            int setPassCalls = UnityEditor.UnityStats.setPassCalls;
            int batches = UnityEditor.UnityStats.batches;
            int triangles = UnityEditor.UnityStats.triangles;
            */
#endif
            
            
            // 各 SoC に対して換算 Memory を表示
            float cpuTime = performanceMonitor.cpuUsage;
            float cpu1 = GetEquivalentCPU(cpuTime, snapdragon660CpuFLOPS);
            snapdragon660_cpuTime.text = $"{cpu1:F2}m秒/f";
            float cpu2 = GetEquivalentCPU(cpuTime, snapdragon675CpuFLOPS);
            snapdragon675_cpuTime.text = $"{cpu2:F2}m秒/f";
            float cpu3 = GetEquivalentCPU(cpuTime, snapdragon730GCpuFLOPS);
            snapdragon730G_cpuTime.text = $"{cpu3:F2}m秒/f";
            float cpu4 = GetEquivalentCPU(cpuTime, snapdragon765GCpuFLOPS);
            snapdragon765G_cpuTime.text = $"{cpu4:F2}m秒/f";
            float cpu5 = GetEquivalentCPU(cpuTime, snapdragon778GCpuFLOPS);
            snapdragon778G_cpuTime.text = $"{cpu5:F2}m秒/f";
            float cpu6 = GetEquivalentCPU(cpuTime, snapdragon865CpuFLOPS);
            snapdragon865_cpuTime.text = $"{cpu6:F2}m秒/f";
            float cpu7 = GetEquivalentCPU(cpuTime, snapdragon888CpuFLOPS);
            snapdragon888_cpuTime.text = $"{cpu7:F2}m秒/f";
            float cpu8 = GetEquivalentCPU(cpuTime, snapdragon8Gen1CpuFLOPS);
            snapdragon8Gen1_cpuTime.text = $"{cpu8:F2}m秒/f";
            float cpu9 = GetEquivalentCPU(cpuTime, snapdragon8Gen2CpuFLOPS);
            snapdragon8Gen2_cpuTime.text = $"{cpu9:F2}m秒/f";

            // 各 SoC に対して換算 GPU処理時間 を表示
            float gpuTime = performanceMonitor.gpuUsage;
            float gpu1 = GetEquivalentGPU(gpuTime, snapdragon660GpuFLOPS);
            snapdragon660_gpuTime.text = $"{gpu1:F2}m秒/f";
            float gpu2 = GetEquivalentGPU(gpuTime, snapdragon675GpuFLOPS);
            snapdragon675_gpuTime.text = $"{gpu2:F2}m秒/f";
            float gpu3 = GetEquivalentGPU(gpuTime, snapdragon730GGpuFLOPS);
            snapdragon730G_gpuTime.text = $"{gpu3:F2}m秒/f";
            float gpu4 = GetEquivalentGPU(gpuTime, snapdragon765GGpuFLOPS);
            snapdragon765G_gpuTime.text = $"{gpu4:F2}m秒/f";
            float gpu5 = GetEquivalentGPU(gpuTime, snapdragon778GGpuFLOPS);
            snapdragon778G_gpuTime.text = $"{gpu5:F2}m秒/f";
            float gpu6 = GetEquivalentGPU(gpuTime, snapdragon865GpuFLOPS);
            snapdragon865_gpuTime.text = $"{gpu6:F2}m秒/f";
            float gpu7 = GetEquivalentGPU(gpuTime, snapdragon888GpuFLOPS);
            snapdragon888_gpuTime.text = $"{gpu7:F2}m秒/f";
            float gpu8 = GetEquivalentGPU(gpuTime, snapdragon8Gen1GpuFLOPS);
            snapdragon8Gen1_gpuTime.text = $"{gpu8:F2}m秒/f";
            float gpu9 = GetEquivalentGPU(gpuTime, snapdragon8Gen2GpuFLOPS);
            snapdragon8Gen2_gpuTime.text = $"{gpu9:F2}m秒/f";


            // 各 SoC に対して換算 CPU+GPU を表示
            snapdragon660_cpuGpuTime.text = $"{cpu1+gpu1:F2}m秒/f";
            snapdragon675_cpuGpuTime.text = $"{cpu2+gpu2:F2}m秒/f";
            snapdragon730G_cpuGpuTime.text = $"{cpu3+gpu3:F2}m秒/f";
            snapdragon765G_cpuGpuTime.text = $"{cpu4+gpu4:F2}m秒/f";
            snapdragon778G_cpuGpuTime.text = $"{cpu5+gpu5:F2}m秒/f";
            snapdragon865_cpuGpuTime.text = $"{cpu6+gpu6:F2}m秒/f";
            snapdragon888_cpuGpuTime.text = $"{cpu7+gpu7:F2}m秒/f";
            snapdragon8Gen1_cpuGpuTime.text = $"{cpu8+gpu8:F2}m秒/f";
            snapdragon8Gen2_cpuGpuTime.text = $"{cpu9+gpu9:F2}m秒/f";

            // 各Snapdragon SoC に対して換算 FPS を計算して表示
            float tempFps = GetEquivalentFPS(cpu1, gpu1);
            snapdragon660_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon660_fps, tempFps);

            tempFps = GetEquivalentFPS(cpu2, gpu2);
            snapdragon675_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon675_fps, tempFps);

            tempFps = GetEquivalentFPS(cpu3, gpu3);
            snapdragon730G_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon730G_fps, tempFps);

            tempFps = GetEquivalentFPS(cpu4, gpu4);
            snapdragon765G_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon765G_fps, tempFps);

            tempFps = GetEquivalentFPS(cpu5, gpu5);
            snapdragon778G_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon778G_fps, tempFps);
            
            tempFps = GetEquivalentFPS(cpu6, gpu6);
            snapdragon865_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon865_fps, tempFps);

            tempFps = GetEquivalentFPS(cpu7, gpu7);
            snapdragon888_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon888_fps, tempFps);

            tempFps = GetEquivalentFPS(cpu8, gpu8);
            snapdragon8Gen1_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon8Gen1_fps, tempFps);

            tempFps = GetEquivalentFPS(cpu9, gpu9);
            snapdragon8Gen2_fps.text = $"{tempFps:F1}fps";
            SetTextColorBasedOnFPS(snapdragon8Gen2_fps, tempFps);

            

            
            
        }
    }
}
