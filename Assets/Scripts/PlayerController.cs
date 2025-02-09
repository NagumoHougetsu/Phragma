using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;

    private CameraController cameraController;
    private CharacterController characterController;
    private Vector2 rotation = Vector2.zero;

    void Start() {
        // 同じオブジェクト内の CameraController を取得
        cameraController = GetComponent<CameraController>();

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
        Move();
        LookAround();

        // モード切り替え
        if (Input.GetKeyDown(KeyCode.V)) {
            cameraController?.ToggleCameraMode();
        }
    }

    void Move() {
        if (characterController == null) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    void LookAround() {
        if (cameraController == null) return;

        rotation.x += Input.GetAxis("Mouse X") * lookSensitivity;
        rotation.y -= Input.GetAxis("Mouse Y") * lookSensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

        transform.localRotation = Quaternion.Euler(0, rotation.x, 0);
        cameraController.SetCameraRotation(rotation.y);
    }
}
