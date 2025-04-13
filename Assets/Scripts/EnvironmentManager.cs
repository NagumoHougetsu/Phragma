// Extended EnvironmentManager for Static Batching Compatibility

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    private List<GameObject> preloadedPrefabs = new List<GameObject>(); // ←クラスメンバにする！

    void Awake() {
        // **シーン内の環境オブジェクトを削除**
        GameObject[] sceneObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in sceneObjects) {
            if (obj.CompareTag("Environment")) {
                Destroy(obj);
            }
        }

        // Static計測したいときは、外部からInitStaticBatching呼ぶ

        toggleUpButton.onClick.RemoveAllListeners();
        toggleDownButton.onClick.RemoveAllListeners();
        toggleUpButton.onClick.AddListener(ToggleUp);
        toggleDownButton.onClick.AddListener(ToggleDown);
        if (environmentPrefabs.Length > 0)
        {
            InitStaticBatching();
        }
    }
    

    public void InitStaticBatching() {
        StartCoroutine(SetupStaticBatching());
    }

    IEnumerator SetupStaticBatching()
    {
        preloadedPrefabs.Clear();

        foreach (var prefab in environmentPrefabs)
        {
            if (prefab == null) continue;

            GameObject instance = Instantiate(prefab, transform);
            instance.SetActive(true); // ← Awake時に表示状態にしておくのが超重要！
            instance.tag = "Environment";
            instance.isStatic = true;

            preloadedPrefabs.Add(instance);
        }

        yield return new WaitForEndOfFrame(); // Batchingがここで確定

        // 👇ここで一括StaticBatchを明示的に行う（UnityEditor外でも効く）
        StaticBatchingUtility.Combine(gameObject);

        // ここで非表示にしてもOK（バッチ済み）
        for (int i = 0; i < preloadedPrefabs.Count; i++)
        {
            preloadedPrefabs[i].SetActive(i == currentIndex);
            if (i == currentIndex)
                currentActivePrefab = preloadedPrefabs[i];
        }
    }

    void SetActivePrefab(int index) {
        for (int i = 0; i < preloadedPrefabs.Count; i++) {
            preloadedPrefabs[i].SetActive(i == index);
            if (i == index) currentActivePrefab = preloadedPrefabs[i];
        }
    }
    public void ToggleUp() {
        Debug.Log($"[ToggleUp] preloaded count={preloadedPrefabs.Count}, index={currentIndex}");
        if (preloadedPrefabs.Count == 0 || isProcessing) return;
        StartCoroutine(SwitchEnvironment(-1));
    }

    public void ToggleDown() {
        Debug.Log($"[ToggleUp] preloaded count={preloadedPrefabs.Count}, index={currentIndex}");
        if (preloadedPrefabs.Count == 0 || isProcessing) return;
        StartCoroutine(SwitchEnvironment(+1));

    }
    private IEnumerator SwitchEnvironment(int direction) {
        isProcessing = true;

        int newIndex = (currentIndex + direction + environmentPrefabs.Length) % environmentPrefabs.Length;

        // ★ 未ロードならここで追加ロード
        if (newIndex >= preloadedPrefabs.Count && newIndex < environmentPrefabs.Length) {
            GameObject instance = Instantiate(environmentPrefabs[newIndex], transform);
            instance.tag = "Environment";
            instance.SetActive(false);
            preloadedPrefabs.Add(instance);
        }

        // 非表示化
        if (currentActivePrefab != null)
            currentActivePrefab.SetActive(false);

        // 表示切替
        preloadedPrefabs[newIndex].SetActive(true);
        currentActivePrefab = preloadedPrefabs[newIndex];
        currentIndex = newIndex;

        yield return new WaitForSeconds(0.2f);
        isProcessing = false;
    }
}
