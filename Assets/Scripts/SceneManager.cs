using UnityEngine;
using UnityEngine.Animations;

public class SceneManager : MonoBehaviour
{
    public GameObject backgroundPrefab;    // インスペクターで登録する背景（必要なら残す）
    public GameObject characterPrefab;     // インスペクターで登録するキャラクター
    private CameraController cameraController; // CameraController をインスペクター登録なしで取得

}
