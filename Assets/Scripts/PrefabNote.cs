using UnityEngine;

public class PrefabNote : MonoBehaviour {
    [TextArea(5, 10)]  // 最小5行、最大10行のテキストエリア
    public string note;
}
