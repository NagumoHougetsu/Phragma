using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    public Transform fpsCamera;  // FPS用カメラ
    public Transform tpsCamera;  // TPS用カメラ

    private bool isFPS = true;   // 初期はFPSモード

    private Transform playerTransform;  // プレイヤーのTransform
    private Vector3 tpsOffset;         // TPS時のカメラのオフセット
    private Vector3 fpsOffset;         // FPS時のカメラのオフセット

    private float tpsDistance = 3f;    // TPSカメラのキャラからの距離
    private float tpsHeight = 1.6f;      // TPSカメラの高さ
    private float fpsDistance = 0.5f;  // FPSカメラのキャラからの距離
    private float fpsHeight = 1.6f;    // FPSカメラの高さ

    private PlayerController playerController;  // SceneManagerクラスへの参照

    void Awake(){
        // 初期のTPSカメラオフセット設定
        tpsOffset = new Vector3(0, tpsHeight, -tpsDistance);
        fpsOffset = new Vector3(0, fpsHeight, fpsDistance); // FPS用オフセット設定

        SetCameraMode(isFPS);
    }

    void Start() {
        // SceneManagerを取得（親オブジェクト内で同じオブジェクトにアタッチされていると仮定）
        playerController = GetComponentInParent<PlayerController>();

        if (playerController != null && playerController.characterPrefab != null) {
            // SceneManagerからキャラクターのTransformを取得
            playerTransform = playerController.characterPrefab.transform;
        } else {
            Debug.LogError("SceneManagerまたはcharacterPrefabが設定されていません！");
        }

       
    }

    void Update() {
        // Cキーでカメラ切り替え
        if (Keyboard.current.cKey.wasPressedThisFrame) {
            ToggleCameraMode();
        }

        Quaternion rotation = Quaternion.Euler(0, playerTransform.rotation.eulerAngles.y, 0);

        if (isFPS) {
            // FPSモード：キャラクターの前方に少しオフセットして配置
            Vector3 cameraPosition = playerTransform.position + rotation * fpsOffset;
            fpsCamera.position = cameraPosition;
            fpsCamera.rotation = playerTransform.rotation;  // プレイヤーの向きに一致
        } else {
            // TPSモード：キャラクターの背後にオフセットして配置
            Vector3 cameraPosition = playerTransform.position + rotation * tpsOffset;
            tpsCamera.position = cameraPosition;
            tpsCamera.LookAt(playerTransform);  // プレイヤーを見つめる
        }
    }


    public void ToggleCameraMode() {
        isFPS = !isFPS;
        SetCameraMode(isFPS);
    }

    void SetCameraMode(bool fpsMode) {
        if (fpsCamera != null) {
            fpsCamera.gameObject.SetActive(fpsMode);
        }

        if (tpsCamera != null) {
            tpsCamera.gameObject.SetActive(!fpsMode);
        }
    }
}
