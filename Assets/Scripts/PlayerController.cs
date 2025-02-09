using UnityEngine;
using UnityEngine.InputSystem;  // InputSystemを使うために追加

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    public Vector3 initialCharacterPosition;  // 初期位置用

    private CameraController cameraController;  // CameraControllerへの参照
    private CharacterController characterController;
    private float rotationSpeed = 100f;  // 回転速度
    private float rotation = 0f;

    void Start() {
        // 初期位置設定（シーン開始時にキャラの位置を初期化）
        transform.position = initialCharacterPosition;

        // 同じオブジェクト内の CameraController を取得
        cameraController = GetComponentInChildren<CameraController>();  // 子オブジェクトからCameraControllerを取得

        // SceneManager から characterPrefab を参照して CharacterController を取得
        SceneManager sceneManager = GetComponent<SceneManager>();
        if (sceneManager != null && sceneManager.characterPrefab != null) {
            characterController = sceneManager.characterPrefab.GetComponent<CharacterController>();
        }

        // カーソルロック
        Cursor.lockState = CursorLockMode.Locked;

        // エラー処理
        if (cameraController == null) {
            Debug.LogWarning("CameraController が見つかりませんでした！");
        }

        if (characterController == null) {
            Debug.LogWarning("CharacterController が見つかりませんでした！");
        }
    }

    void Update() {
        // 移動入力の確認
        float moveX = 0;
        float moveZ = 0;
        float rot = 0;

        // Keyboard inputを取得
        if (Keyboard.current != null) {
            moveX = Keyboard.current.aKey.isPressed ? -1 : (Keyboard.current.dKey.isPressed ? 1 : 0);
            moveZ = Keyboard.current.wKey.isPressed ? 1 : (Keyboard.current.sKey.isPressed ? -1 : 0);
            rot = Keyboard.current.eKey.isPressed ? -1 : (Keyboard.current.qKey.isPressed ? 1 : 0);
        }

        Debug.Log($"MoveX: {moveX}, MoveZ: {moveZ}, Rotate: {rot}");  // 入力を確認

        // ここから移動処理
        if (characterController == null) return;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // 回転入力処理
        characterController.transform.Rotate(0, rot * rotationSpeed * Time.deltaTime, 0);  
        
        
    }

    void LateUpdate() {
        // カメラがキャラクターの位置に追従する
        if (cameraController != null) {
            // キャラクターの位置にカメラを追従させる
            cameraController.transform.position = transform.position;

            // カメラの回転を調整する
            cameraController.transform.rotation = transform.rotation;  // キャラクターの回転に合わせる
        }
    }
}
