using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnvironmentManager : MonoBehaviour {
    [Header("Environment Prefabs")]
    public GameObject[] environmentPrefabs;
    private int currentIndex = 0;
    private GameObject currentActivePrefab = null;
    private bool isProcessing = false;

    [Header("UI Elements")]
    public Button toggleUpButton;
    public Button toggleDownButton;

    public GameObject CurrentActivePrefab => currentActivePrefab;

    void Awake() {
        // **シーン内の環境オブジェクトを削除**
        GameObject[] sceneObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in sceneObjects) {
            if (obj.CompareTag("Environment")) {
                Destroy(obj);
            }
        }

        // **最初の環境をロード**
        if (environmentPrefabs.Length > 0) {
            currentIndex = 0;
            LoadPrefab(currentIndex);
        }

        toggleUpButton.onClick.RemoveAllListeners();
        toggleDownButton.onClick.RemoveAllListeners();
        toggleUpButton.onClick.AddListener(ToggleUp);
        toggleDownButton.onClick.AddListener(ToggleDown);
    }

    void LoadPrefab(int index) {
        if (environmentPrefabs.Length == 0) return;
        if (environmentPrefabs[index] == null) {
            Debug.LogError($"Prefab at index {index} is NULL!");
            return;
        }

        StartCoroutine(SwitchPrefab(index));
    }

    IEnumerator SwitchPrefab(int index) {
        isProcessing = true;

        // **現在のPrefabを削除**
        if (currentActivePrefab != null) {
            GameObject oldPrefab = currentActivePrefab;
            currentActivePrefab = null; // 参照を切る

            Destroy(oldPrefab);
            yield return new WaitForSeconds(0.1f); // ちょっと待つ（安全策）
            Resources.UnloadUnusedAssets(); // メモリ解放
        }

        // **新しいプレハブを生成**
        currentActivePrefab = Instantiate(environmentPrefabs[index], transform);
        currentActivePrefab.SetActive(true);

        yield return null; // 1フレーム待つ（明示的に確実な処理順にする）

        isProcessing = false; // 最後に処理完了を明示
    }

    public void ToggleUp() {
        if (environmentPrefabs.Length == 0 || isProcessing) return;

        currentIndex = (currentIndex - 1 + environmentPrefabs.Length) % environmentPrefabs.Length;
        LoadPrefab(currentIndex);
    }

    public void ToggleDown() {
        if (environmentPrefabs.Length == 0 || isProcessing) return;

        currentIndex = (currentIndex + 1) % environmentPrefabs.Length;
        LoadPrefab(currentIndex);
    }
}
