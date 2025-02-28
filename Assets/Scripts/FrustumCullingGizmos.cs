using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class FrustumCullingGizmos : MonoBehaviour
{
    private Camera activeCamera;
    private Renderer[] allRenderers;
    public Button toggleButton;
    private bool isFrustumCullingEnabled = true;
    private Image buttonImage;

    void Start()
    {
        FindAllRenderers();
        DetectActiveCamera();

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleFrustumCulling);
            buttonImage = toggleButton.GetComponent<Image>();
            UpdateButtonColor();
        }
    }

    void Update()
    {
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
        if (allRenderers == null || Time.frameCount % 30 == 0) // 定期的に再取得
        {
            FindAllRenderers();
        }
        if (activeCamera == null) return;

        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(activeCamera);

        foreach (Renderer rend in allRenderers)
        {
            if (rend == null) continue;

            bool isVisible = GeometryUtility.TestPlanesAABB(frustumPlanes, rend.bounds);
            rend.enabled = isVisible;

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rend);
            #endif
        }
    }

    public void ToggleFrustumCulling()
    {
        isFrustumCullingEnabled = !isFrustumCullingEnabled;
        Debug.Log($"フラスタムカリング: {(isFrustumCullingEnabled ? "ON" : "OFF")}");

        if (!isFrustumCullingEnabled && allRenderers != null)
        {
            RestoreRenderers();
        }

        UpdateButtonColor();
    }

    void UpdateButtonColor()
    {
        if (buttonImage != null)
        {
            buttonImage.color = isFrustumCullingEnabled ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (allRenderers == null) return;

        foreach (Renderer rend in allRenderers)
        {
            if (rend == null) continue;

            bool isVisible = rend.enabled;
            Gizmos.color = isVisible ? Color.green : Color.red;
            Gizmos.DrawWireCube(rend.bounds.center, rend.bounds.size);
        }
    }
    #endif

    void RestoreRenderers()
    {
        // MeshRendererの表示を元に戻す
        if (allRenderers != null)
        {
            foreach (Renderer rend in allRenderers)
            {
                if (rend != null) rend.enabled = true;
            }
        }

        // SkinnedMeshRendererも表示を戻す
        SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsOfType<SkinnedMeshRenderer>(true); // 非アクティブも検索
        foreach (SkinnedMeshRenderer skinnedRenderer in skinnedMeshRenderers)
        {
            if (skinnedRenderer != null)
            {
                skinnedRenderer.enabled = true;
                if (!skinnedRenderer.gameObject.activeInHierarchy)
                {
                    skinnedRenderer.gameObject.SetActive(true);
                }
            }
        }
    }

    void OnDisable()
    {
        RestoreRenderers();
    }

    void OnDestroy()
    {
        RestoreRenderers();
    }
}
