using UnityEngine;
using UnityEngine.UI;

public class QualitySettingsUI : MonoBehaviour
{
    public Dropdown qualityDropdown;

    void Start()
    {
        // QualitySettings のプリセット名を取得
        qualityDropdown.options.Clear();
        foreach (string name in QualitySettings.names)
        {
            qualityDropdown.options.Add(new Dropdown.OptionData(name));
        }

        // 現在の品質設定を UI に反映
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // 変更時の処理を登録
        qualityDropdown.onValueChanged.AddListener(SetQualityLevel);
    }

    public void SetQualityLevel(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
    }
}
