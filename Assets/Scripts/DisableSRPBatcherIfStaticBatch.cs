using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class DisableSRPBatcherIfStaticBatch : MonoBehaviour
{
    void OnEnable()
    {
        ApplyToEligibleRenderers();
    }

    void ApplyToEligibleRenderers()
    {
        var renderers = GetComponentsInChildren<Renderer>(true);
        int appliedCount = 0;

        foreach (var renderer in renderers)
        {
            GameObject go = renderer.gameObject;

#if UNITY_EDITOR
            // Editor判定：BatchingStaticかどうかチェック
            bool isBatchingStatic = GameObjectUtility.AreStaticEditorFlagsSet(go, StaticEditorFlags.BatchingStatic);
#else
            // 実行時は isStatic のみ確認（厳密な区別は不可）
            bool isBatchingStatic = go.isStatic;
#endif
            if (!isBatchingStatic) continue;

            // SRPバッチャー無効化：空のMPBをセット
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            renderer.SetPropertyBlock(mpb);
            appliedCount++;
        }

#if UNITY_EDITOR
        Debug.Log($"[DisableSRPBatcherIfStaticBatch] StaticBatch対象にMPB適用完了: {appliedCount}個", this);
#endif
    }
}
