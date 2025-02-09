using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    public Transform fpsCamera;  // FPS用カメラ
    public Transform tpsCamera;  // TPS用カメラ

    private bool isFPS = true;   // 初期はFPSモード

    private Vector3 fpsInitialPosition;  // FPSカメラの初期位置
    private Vector3 tpsInitialPosition;  // TPSカメラの初期位置

    private Transform playerTransform;  // プレイヤーのTransform

    void Start() {
        // プレイヤーのTransformを取得
        playerTransform = GetComponentInParent<Transform>();  // キャラクターのTransformを取得
    }

    void Update() {
        // Cキーでカメラ切り替え
        if (Keyboard.current.cKey.wasPressedThisFrame) {  // 新しい入力システムに対応
            ToggleCameraMode();
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
