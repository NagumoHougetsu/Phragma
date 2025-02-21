using UnityEngine;
using UnityEngine.Rendering;

public class DisableSRPBatcher : MonoBehaviour
{
    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(mpb);

            // SRPバッチャーを無効化（異なるMaterialインスタンスを作成）
            Material newMat = new Material(renderer.sharedMaterial);
            newMat.enableInstancing = true; // GPUインスタンシングを有効化

            renderer.material = newMat;
            renderer.SetPropertyBlock(mpb);
        }
    }
}
