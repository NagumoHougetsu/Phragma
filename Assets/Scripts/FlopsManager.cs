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
        [SerializeField] private Threshold snapdragon660_memoryThreshold = new Threshold(400f, 500f);
        [SerializeField] private Threshold snapdragon660_drawcallThreshold = new Threshold(150f, 200f);
        [SerializeField] private Threshold snapdragon660_setpasscallThreshold = new Threshold(100f, 150f);
        [SerializeField] private Threshold snapdragon660_batchThreshold = new Threshold(200f, 250f);
        [SerializeField] private Threshold snapdragon660_trianglesThreshold = new Threshold(200000f, 250000f);
        private float snapdragon660CpuFLOPS = 32.32f; // グレーアウト
        private float snapdragon660GpuFLOPS = 150f; // グレーアウト
        private float snapdragon660TotalFLOPS = 182.32f; // グレーアウト



    [Header("Snapdragon 675")]
        [SerializeField] private Text snapdragon675_fps;
        [SerializeField] private Text snapdragon675_memory;
        [SerializeField] private Text snapdragon675_drawcall;
        [SerializeField] private Text snapdragon675_setpasscall;
        [SerializeField] private Text snapdragon675_batch;
        [SerializeField] private Text snapdragon675_triangles;
        [SerializeField] private Threshold snapdragon675Threshold;
        [SerializeField] private Threshold snapdragon675_memoryThreshold = new Threshold(500f, 600f);
        [SerializeField] private Threshold snapdragon675_drawcallThreshold = new Threshold(180f, 220f);
        [SerializeField] private Threshold snapdragon675_setpasscallThreshold = new Threshold(120f, 160f);
        [SerializeField] private Threshold snapdragon675_batchThreshold = new Threshold(250f, 300f);
        [SerializeField] private Threshold snapdragon675_trianglesThreshold = new Threshold(250000f, 300000f);
        private float snapdragon675CpuFLOPS = 29.6f; // グレーアウト
        private float snapdragon675GpuFLOPS = 200f; // グレーアウト
        private float snapdragon675TotalFLOPS = 229.6f; // グレーアウト

    [Header("Snapdragon 730G")]
        [SerializeField] private Text snapdragon730G_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon730G_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon730G_drawcall;
        [SerializeField] private Text snapdragon730G_setpasscall;
        [SerializeField] private Text snapdragon730G_batch;
        [SerializeField] private Text snapdragon730G_triangles;
        [SerializeField] private Threshold snapdragon730GThreshold;
        [SerializeField] private Threshold snapdragon730G_memoryThreshold = new Threshold(700f, 800f);
        [SerializeField] private Threshold snapdragon730G_drawcallThreshold = new Threshold(200f, 250f);
        [SerializeField] private Threshold snapdragon730G_setpasscallThreshold = new Threshold(150f, 200f);
        [SerializeField] private Threshold snapdragon730G_batchThreshold = new Threshold(300f, 350f);
        [SerializeField] private Threshold snapdragon730G_trianglesThreshold = new Threshold(300000f, 400000f);
        private float snapdragon730GCpuFLOPS = 30.4f; // グレーアウト
        private float snapdragon730GGpuFLOPS = 250f; // グレーアウト
        private float snapdragon730GTotalFLOPS = 280.4f; // グレーアウト

    [Header("Snapdragon 765G")]
        [SerializeField] private Text snapdragon765G_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon765G_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon765G_drawcall;
        [SerializeField] private Text snapdragon765G_setpasscall;
        [SerializeField] private Text snapdragon765G_batch;
        [SerializeField] private Text snapdragon765G_triangles;
        [SerializeField] private Threshold snapdragon765GThreshold;
        [SerializeField] private Threshold snapdragon765G_memoryThreshold = new Threshold(800f, 900f);
        [SerializeField] private Threshold snapdragon765G_drawcallThreshold = new Threshold(220f, 270f);
        [SerializeField] private Threshold snapdragon765G_setpasscallThreshold = new Threshold(170f, 220f);
        [SerializeField] private Threshold snapdragon765G_batchThreshold = new Threshold(350f, 400f);
        [SerializeField] private Threshold snapdragon765G_trianglesThreshold = new Threshold(350000f, 450000f);
        private float snapdragon765GCpuFLOPS = 31.2f; // グレーアウト
        private float snapdragon765GGpuFLOPS = 400f; // グレーアウト
        private float snapdragon765GTotalFLOPS = 431.2f; // グレーアウト

    [Header("Snapdragon 778G")]
        [SerializeField] private Text snapdragon778G_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon778G_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon778G_drawcall;
        [SerializeField] private Text snapdragon778G_setpasscall;
        [SerializeField] private Text snapdragon778G_batch;
        [SerializeField] private Text snapdragon778G_triangles;
        [SerializeField] private Threshold snapdragon778GThreshold;
        [SerializeField] private Threshold snapdragon778G_memoryThreshold = new Threshold(900f, 1000f);
        [SerializeField] private Threshold snapdragon778G_drawcallThreshold = new Threshold(250f, 300f);
        [SerializeField] private Threshold snapdragon778G_setpasscallThreshold = new Threshold(200f, 250f);
        [SerializeField] private Threshold snapdragon778G_batchThreshold = new Threshold(400f, 450f);
        [SerializeField] private Threshold snapdragon778G_trianglesThreshold = new Threshold(400000f, 500000f);
        private float snapdragon778GCpuFLOPS = 33.6f; // グレーアウト
        private float snapdragon778GGpuFLOPS = 550f; // グレーアウト
        private float snapdragon778GTotalFLOPS = 583.6f; // グレーアウト

    [Header("Snapdragon 865")]
        [SerializeField] private Text snapdragon865_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon865_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon865_drawcall;
        [SerializeField] private Text snapdragon865_setpasscall;
        [SerializeField] private Text snapdragon865_batch;
        [SerializeField] private Text snapdragon865_triangles;
        [SerializeField] private Threshold snapdragon865Threshold;
        [SerializeField] private Threshold snapdragon865_memoryThreshold = new Threshold(1000f, 1100f);
        [SerializeField] private Threshold snapdragon865_drawcallThreshold = new Threshold(250f, 300f);
        [SerializeField] private Threshold snapdragon865_setpasscallThreshold = new Threshold(200f, 250f);
        [SerializeField] private Threshold snapdragon865_batchThreshold = new Threshold(400f, 450f);
        [SerializeField] private Threshold snapdragon865_trianglesThreshold = new Threshold(500000f, 700000f);
        private float snapdragon865CpuFLOPS = 37.12f; // グレーアウト
        private float snapdragon865GpuFLOPS = 800f; // グレーアウト
        private float snapdragon865TotalFLOPS = 837.12f; // グレーアウト

    [Header("Snapdragon 888")]
        [SerializeField] private Text snapdragon888_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon888_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon888_drawcall;
        [SerializeField] private Text snapdragon888_setpasscall;
        [SerializeField] private Text snapdragon888_batch;
        [SerializeField] private Text snapdragon888_triangles;
        [SerializeField] private Threshold snapdragon888Threshold;
        [SerializeField] private Threshold snapdragon888_memoryThreshold = new Threshold(1200f, 1400f);
        [SerializeField] private Threshold snapdragon888_drawcallThreshold = new Threshold(300f, 350f);
        [SerializeField] private Threshold snapdragon888_setpasscallThreshold = new Threshold(250f, 300f);
        [SerializeField] private Threshold snapdragon888_batchThreshold = new Threshold(500f, 600f);
        [SerializeField] private Threshold snapdragon888_trianglesThreshold = new Threshold(700000f, 900000f);
        private float snapdragon888CpuFLOPS = 34.6f; // グレーアウト
        private float snapdragon888GpuFLOPS = 1000f; // グレーアウト
        private float snapdragon888TotalFLOPS = 1034.6f; // グレーアウト

    [Header("Snapdragon 8Gen1")]
        [SerializeField] private Text snapdragon8Gen1_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen1_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen1_drawcall;
        [SerializeField] private Text snapdragon8Gen1_setpasscall;
        [SerializeField] private Text snapdragon8Gen1_batch;
        [SerializeField] private Text snapdragon8Gen1_triangles;
        [SerializeField] private Threshold snapdragon8Gen1Threshold;
        [SerializeField] private Threshold snapdragon8Gen1_memoryThreshold = new Threshold(1500f, 1600f);
        [SerializeField] private Threshold snapdragon8Gen1_drawcallThreshold = new Threshold(350f, 400f);
        [SerializeField] private Threshold snapdragon8Gen1_setpasscallThreshold = new Threshold(300f, 350f);
        [SerializeField] private Threshold snapdragon8Gen1_batchThreshold = new Threshold(600f, 700f);
        [SerializeField] private Threshold snapdragon8Gen1_trianglesThreshold = new Threshold(1000000f, 1200000f);
        private float snapdragon8Gen1CpuFLOPS = 37.44f; // グレーアウト
        private float snapdragon8Gen1GpuFLOPS = 1500f; // グレーアウト
        private float snapdragon8Gen1TotalFLOPS = 1537.44f; // グレーアウト

    [Header("Snapdragon 8Gen2")]
        [SerializeField] private Text snapdragon8Gen2_fps; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen2_memory; // 追加: シーン内でドラッグインドロップするTextコンポーネント
        [SerializeField] private Text snapdragon8Gen2_drawcall;
        [SerializeField] private Text snapdragon8Gen2_setpasscall;
        [SerializeField] private Text snapdragon8Gen2_batch;
        [SerializeField] private Text snapdragon8Gen2_triangles;
        [SerializeField] private Threshold snapdragon8Gen2Threshold;
        [SerializeField] private Threshold snapdragon8Gen2_memoryThreshold = new Threshold(1800f, 2000f);
        [SerializeField] private Threshold snapdragon8Gen2_drawcallThreshold = new Threshold(400f, 500f);
        [SerializeField] private Threshold snapdragon8Gen2_setpasscallThreshold = new Threshold(350f, 400f);
        [SerializeField] private Threshold snapdragon8Gen2_batchThreshold = new Threshold(700f, 900f);
        [SerializeField] private Threshold snapdragon8Gen2_trianglesThreshold = new Threshold(1500000f, 2000000f);
        private float snapdragon8Gen2CpuFLOPS = 32.0f; // グレーアウト
        private float snapdragon8Gen2GpuFLOPS = 1800f; // グレーアウト
        private float snapdragon8Gen2TotalFLOPS = 1832.0f; // グレーアウト


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
        float equivalentFPS = fps * (snapdragonTotalFLOPS / pcTotalFLOPS);
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
            // 各 SoC に対して換算 FPS を計算して表示
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

            // 各 SoC に対して換算 Memory を表示
            float memory = performanceMonitor.memory;
            snapdragon660_memory.text = $"{memory:N0}MB";
            snapdragon675_memory.text = $"{memory:N0}MB";
            snapdragon730G_memory.text = $"{memory:N0}MB";
            snapdragon765G_memory.text = $"{memory:N0}MB";
            snapdragon778G_memory.text = $"{memory:N0}MB";
            snapdragon865_memory.text = $"{memory:N0}MB";
            snapdragon888_memory.text = $"{memory:N0}MB";
            snapdragon8Gen1_memory.text = $"{memory:N0}MB";
            snapdragon8Gen2_memory.text = $"{memory:N0}MB";

            // 各 SoC に対して換算 drawcall を表示
            snapdragon660_drawcall.text = $"{drawCalls:N0}回";
            snapdragon675_drawcall.text = $"{drawCalls:N0}回";
            snapdragon730G_drawcall.text = $"{drawCalls:N0}回";
            snapdragon765G_drawcall.text = $"{drawCalls:N0}回";
            snapdragon778G_drawcall.text = $"{drawCalls:N0}回";
            snapdragon865_drawcall.text = $"{drawCalls:N0}回";
            snapdragon888_drawcall.text = $"{drawCalls:N0}回";
            snapdragon8Gen1_drawcall.text = $"{drawCalls:N0}回";
            snapdragon8Gen2_drawcall.text = $"{drawCalls:N0}回";

            // 各 SoC に対して換算 setpasscall を表示
            snapdragon660_setpasscall.text = $"{setPassCalls:N0}回";
            snapdragon675_setpasscall.text = $"{setPassCalls:N0}回";
            snapdragon730G_setpasscall.text = $"{setPassCalls:N0}回";
            snapdragon765G_setpasscall.text = $"{setPassCalls:N0}回";
            snapdragon778G_setpasscall.text = $"{setPassCalls:N0}回";
            snapdragon865_setpasscall.text = $"{setPassCalls:N0}回";
            snapdragon888_setpasscall.text = $"{setPassCalls:N0}回";
            snapdragon8Gen1_setpasscall.text = $"{setPassCalls:N0}回";
            snapdragon8Gen2_setpasscall.text = $"{setPassCalls:N0}回";

            // 各 SoC に対して換算 batch を表示
            snapdragon660_batch.text = $"{batches:N0}回";
            snapdragon675_batch.text = $"{batches:N0}回";
            snapdragon730G_batch.text = $"{batches:N0}回";
            snapdragon765G_batch.text = $"{batches:N0}回";
            snapdragon778G_batch.text = $"{batches:N0}回";
            snapdragon865_batch.text = $"{batches:N0}回";
            snapdragon888_batch.text = $"{batches:N0}回";
            snapdragon8Gen1_batch.text = $"{batches:N0}回";
            snapdragon8Gen2_batch.text = $"{batches:N0}回";

            // 各 SoC に対して換算 triangles を表示
            snapdragon660_triangles.text = $"{triangles:N0}tris";
            snapdragon675_triangles.text = $"{triangles:N0}tris";
            snapdragon730G_triangles.text = $"{triangles:N0}tris";
            snapdragon765G_triangles.text = $"{triangles:N0}tris";
            snapdragon778G_triangles.text = $"{triangles:N0}tris";
            snapdragon865_triangles.text = $"{triangles:N0}tris";
            snapdragon888_triangles.text = $"{triangles:N0}tris";
            snapdragon8Gen1_triangles.text = $"{triangles:N0}tris";
            snapdragon8Gen2_triangles.text = $"{triangles:N0}tris";
        }
    }
}
