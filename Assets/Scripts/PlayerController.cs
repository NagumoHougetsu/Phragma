using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    public Vector3 initialCharacterPosition;

    private CameraController cameraController;
    private CharacterController characterController;
    private Animator animator; // Animatorを追加
    private float rotationSpeed = 100f;
    private bool wasRunning = false;

    void Start() {
        transform.position = initialCharacterPosition;

        cameraController = GetComponentInChildren<CameraController>();
        SceneManager sceneManager = GetComponent<SceneManager>();
        if (sceneManager != null && sceneManager.characterPrefab != null) {
            characterController = sceneManager.characterPrefab.GetComponent<CharacterController>();
            animator = sceneManager.characterPrefab.GetComponent<Animator>(); // Animator取得
        }

        Cursor.lockState = CursorLockMode.Locked;

        if (cameraController == null) {
            Debug.LogWarning("CameraController が見つかりませんでした！");
        }
        if (characterController == null) {
            Debug.LogWarning("CharacterController が見つかりませんでした！");
        }
        if (animator == null) {
            Debug.LogWarning("Animator が見つかりませんでした！");
        }
    }

    void Update() {
        if (characterController == null) return;

        float moveX = 0;
        float moveZ = 0;
        float rot = 0;

        if (Keyboard.current != null) {
            moveX = Keyboard.current.aKey.isPressed ? -1 : (Keyboard.current.dKey.isPressed ? 1 : 0);
            moveZ = Keyboard.current.wKey.isPressed ? 1 : (Keyboard.current.sKey.isPressed ? -1 : 0);
            rot = Keyboard.current.eKey.isPressed ? 1 : (Keyboard.current.qKey.isPressed ? -1 : 0);
        }

        // キャラクターの前方と右方向を基準に移動
        Vector3 move = characterController.transform.forward * moveZ + characterController.transform.right * moveX;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // 回転処理
        characterController.transform.Rotate(0, rot * rotationSpeed * Time.deltaTime, 0);

        bool isMoving = move.magnitude > 0.3f;  // 現在の移動状態

        // 前フレームと現在の状態を比較
        if (isMoving != wasRunning) {
            // 状態が変わった時にのみ遷移を行う
            if (animator != null) {
                animator.SetBool("IsRunning", isMoving);  // 移動しているかどうかで遷移
            }
            wasRunning = isMoving;  // 現在の状態を保存
        }
    }
}
