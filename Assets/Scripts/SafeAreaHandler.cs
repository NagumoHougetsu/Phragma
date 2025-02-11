using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect currentSafeArea = Rect.zero;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void Update()
    {
        // デバイスの回転やサイズ変更を考慮
        if (currentSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        currentSafeArea = Screen.safeArea;

        // Safe Areaを0~1の正規化された値に変換
        Vector2 anchorMin = currentSafeArea.position;
        Vector2 anchorMax = currentSafeArea.position + currentSafeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
