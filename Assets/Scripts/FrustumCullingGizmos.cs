using UnityEngine;
using UnityEngine.UI; // UIを扱うために必要

[ExecuteAlways]
public class FrustumCullingGizmos : MonoBehaviour
{
    private Camera activeCamera;
    private Renderer[] allRenderers;
    public Button toggleButton; // カリングON/OFFの切り替えボタン
    private bool isFrustumCullingEnabled = true; // カリングの有効状態
    private Image buttonImage; // ボタンのImageコンポーネント

    void Start()
    {
        if (!Application.isPlaying) return;

        FindAllRenderers();
        DetectActiveCamera();

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleFrustumCulling);
            buttonImage = toggleButton.GetComponent<Image>(); // ボタンのImageを取得
            UpdateButtonColor(); // 初期状態を反映
        }
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        DetectActiveCamera();
        if (activeCamera == null || !isFrustumCullingEnabled) return;

        ApplyFrustumCulling();
    }

    void FindAllRenderers()
    {
        allRenderers = FindObjectsOfType<Renderer>();
    }

    void DetectActiveCamera()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.isActiveAndEnabled)
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
            rend.enabled = isVisible;
        }
    }

    public void ToggleFrustumCulling()
    {
        isFrustumCullingEnabled = !isFrustumCullingEnabled;
        Debug.Log($"フラスタムカリング: {(isFrustumCullingEnabled ? "ON" : "OFF")}");

        // カリングOFF時は全オブジェクトを表示する
        if (!isFrustumCullingEnabled && allRenderers != null)
        {
            foreach (Renderer rend in allRenderers)
            {
                if (rend != null) rend.enabled = true;
            }
        }

        UpdateButtonColor();
    }

    void UpdateButtonColor()
    {
        if (buttonImage != null)
        {
            buttonImage.color = isFrustumCullingEnabled ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f); // OFFならグレー
        }
    }
}
