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
            
            // プロパティブロックを設定して、マテリアルを上書きせずにインスタンシング
            renderer.SetPropertyBlock(mpb);
        }
    }
}

