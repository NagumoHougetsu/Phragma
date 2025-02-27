using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour{
    [Header("Environment Prefabs")]
    public GameObject[] environmentPrefabs;
    private int currentIndex = 0;
    public GameObject currentActivePrefab = null;
    private bool isProcessing = false; // ボタン連打防止フラグ

    [Header("UI Elements")]
    public Button toggleUpButton;
    public Button toggleDownButton;

    void Awake(){
        if (environmentPrefabs.Length > 0){
            // すべて非表示にする
            foreach (var prefab in environmentPrefabs){
                prefab.SetActive(false);
            }

            // 最初のオブジェクトだけを表示
            currentIndex = 0;
            currentActivePrefab = environmentPrefabs[currentIndex];
            currentActivePrefab.SetActive(true);
        }

        // **ボタンのリスナーを確実に初期化**
        toggleUpButton.onClick.RemoveAllListeners();
        toggleDownButton.onClick.RemoveAllListeners();
        toggleUpButton.onClick.AddListener(ToggleUp);
        toggleDownButton.onClick.AddListener(ToggleDown);
    }

    void DisplayPrefab(int index){
        if (currentActivePrefab != null){
            currentActivePrefab.SetActive(false);
        }
        currentActivePrefab = environmentPrefabs[index];
        currentActivePrefab.SetActive(true);
    }

    public void ToggleUp(){
        if (environmentPrefabs.Length == 0 || isProcessing) return;
        isProcessing = true; // 連打防止

        int previousIndex = currentIndex;
        currentIndex = (currentIndex - 1 + environmentPrefabs.Length) % environmentPrefabs.Length;
        Invoke(nameof(ApplyPrefabChange), 0.05f); // 0.05秒後に適用
    }

    public void ToggleDown(){
        if (environmentPrefabs.Length == 0 || isProcessing) return;
        isProcessing = true; // 連打防止

        int previousIndex = currentIndex;
        currentIndex = (currentIndex + 1) % environmentPrefabs.Length;
        Invoke(nameof(ApplyPrefabChange), 0.05f); // 0.05秒後に適用
    }

    void ApplyPrefabChange(){
        DisplayPrefab(currentIndex);
        EnableButtons();
        isProcessing = false; // 連打防止解除
    }

    void EnableButtons(){
        toggleUpButton.interactable = true;
        toggleDownButton.interactable = true;
    }
}
