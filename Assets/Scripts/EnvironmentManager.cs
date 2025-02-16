using UnityEngine;
using UnityEngine.UI;  // UI要素を使うために必要

public class EnvironmentManager : MonoBehaviour{
    [Header("Environment Prefabs")]
    public GameObject[] environmentPrefabs; // Prefabのリスト
    private int currentIndex = 0;           // 現在表示されているPrefabのインデックス

    [Header("UI Elements")]
    public Button toggleUpButton;           // 上に切り替えボタン
    public Button toggleDownButton;         // 下に切り替えボタン

    void Start(){
        if (environmentPrefabs.Length > 0){
            DisplayPrefab(currentIndex);   // ゲーム開始時に最初のPrefabを表示
        }
        // ボタンの設定
        toggleUpButton.onClick.AddListener(ToggleUp);
        toggleDownButton.onClick.AddListener(ToggleDown);
    }

    // Prefabを表示するメソッド
    void DisplayPrefab(int index){
        // 全てのPrefabを非表示にする
        foreach (GameObject prefab in environmentPrefabs){
            prefab.SetActive(false);  // Prefabを非表示にする
        }
        // 指定されたインデックスのPrefabのみ表示
        if (environmentPrefabs.Length > 0){
            environmentPrefabs[index].SetActive(true);  // 指定されたPrefabを表示
        }
    }

    // 上に切り替えるメソッド
    public void ToggleUp(){
        if (environmentPrefabs.Length == 0) return;
        // インデックスを上に移動
        currentIndex = (currentIndex - 1 +  environmentPrefabs.Length) % environmentPrefabs.Length;
        DisplayPrefab(currentIndex); // 表示を更新
    }

    // 下に切り替えるメソッド
    public void ToggleDown(){
        if (environmentPrefabs.Length == 0) return;
        // インデックスを下に移動
        currentIndex = (currentIndex + 1) % environmentPrefabs.Length;
        DisplayPrefab(currentIndex); // 表示を更新
    }
}
