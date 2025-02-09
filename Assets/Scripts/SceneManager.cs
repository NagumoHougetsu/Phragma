using UnityEngine;

public class SceneManager : MonoBehaviour{
    public GameObject backgroundPrefab;    // インスペクターで登録する背景（必要なら残す）
    public GameObject characterPrefab;     // インスペクターで登録するキャラクター
    public CameraController cameraController; // インスペクターで登録する CameraController

    void Start(){
        // PlayerController と CharacterController の確認・追加
        if (characterPrefab != null){
            if (!characterPrefab.GetComponent<PlayerController>())
                characterPrefab.AddComponent<PlayerController>();

            if (!characterPrefab.GetComponent<CharacterController>())
                characterPrefab.AddComponent<CharacterController>();

            characterPrefab.GetComponent<PlayerController>().cameraController = cameraController;
        }

        // FPS / TPS カメラの追従設定
        SetupCameras();
    }

    private void SetupCameras(){
        
        if (cameraController == null){
            Debug.LogWarning("CameraController がアサインされていません！");
            return;
        }

        if (cameraController.fpsCamera == null || cameraController.tpsCamera == null){
            Debug.LogError("FPS と TPS のカメラがインスペクターで設定されていません！");
            return;
        }

        if (characterPrefab != null){
            cameraController.fpsCamera.SetParent(characterPrefab.transform);
            cameraController.fpsCamera.localPosition = new Vector3(0, 1.8f, 0);

            cameraController.tpsCamera.SetParent(characterPrefab.transform);
            cameraController.tpsCamera.localPosition = new Vector3(0, 2f, -4f);
        }
    }
}
