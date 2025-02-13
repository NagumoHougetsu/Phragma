using UnityEngine;

[ExecuteAlways] // 再生中・エディターのどちらでも動作
public class FrustumCullingGizmos : MonoBehaviour
{
    private Camera activeCamera;  // 現在アクティブなカメラ
    private Renderer[] allRenderers; // シーン内の全オブジェクトのレンダラー

    void Start()
    {
        if (!Application.isPlaying) return; // 再生中のみ実行
        FindAllRenderers();
        DetectActiveCamera();
    }

    void Update()
    {
        if (!Application.isPlaying) return; // 再生中のみ実行

        DetectActiveCamera();
        if (activeCamera == null) return;
        ApplyFrustumCulling();
    }

    void FindAllRenderers()
    {
        allRenderers = FindObjectsOfType<Renderer>();
    }

    void DetectActiveCamera()
    {
        // 現在アクティブなカメラを検索
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.isActiveAndEnabled)  // アクティブかつ有効なカメラを選択
            {
                activeCamera = cam;
                break;
            }
        }
    }

    void ApplyFrustumCulling()
    {
        if (allRenderers == null) FindAllRenderers();
        if (activeCamera == null) return;

        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(activeCamera);

        foreach (Renderer rend in allRenderers)
        {
            if (rend == null) continue;

            bool isVisible = GeometryUtility.TestPlanesAABB(frustumPlanes, rend.bounds);
            rend.enabled = isVisible; // 視界外なら非表示
        }
    }
}
