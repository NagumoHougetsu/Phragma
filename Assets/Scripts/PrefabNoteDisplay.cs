using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnvironmentManager))]
public class PrefabNoteDisplay : MonoBehaviour
{
    [Header("Note Text UI")]
    public Text noteTextUI;

    [Header("Prefab Name Text UI")] // 👈 追加：Prefab名を表示するテキスト
    public Text prefabNameTextUI;

    private EnvironmentManager environmentManager;
    private GameObject lastActivePrefab;

    void Awake()
    {
        environmentManager = GetComponent<EnvironmentManager>();
    }

    void Update()
    {
        var current = environmentManager.CurrentActivePrefab;

        if (current != null && current != lastActivePrefab)
        {
            // ノート表示
            var noteComponent = current.GetComponent<PrefabNote>();
            if (noteComponent != null && noteTextUI != null)
            {
                noteTextUI.text = noteComponent.note;
            }
            else if (noteTextUI != null)
            {
                noteTextUI.text = "(No note assigned to this prefab)";
            }

            // 👇追加：Prefab名表示
            if (prefabNameTextUI != null)
            {
                prefabNameTextUI.text = current.name;
            }

            lastActivePrefab = current;
        }
    }
}
