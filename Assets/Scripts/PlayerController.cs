using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // ← Button判定用

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    public Vector3 initialCharacterPosition;

    public GameObject characterPrefab;

    private CameraController cameraController;
    private CharacterController characterController;
    private Animator animator;
    private float rotationSpeed = 100f;
    private bool wasRunning = false;

    private Vector2 lastTwoFingerPos; // 二本指スワイプの前回位置

    void Start() {
        transform.position = initialCharacterPosition;

        cameraController = GetComponentInChildren<CameraController>();

        if (characterPrefab != null) {
            characterController = characterPrefab.GetComponent<CharacterController>();
            animator = characterPrefab.GetComponent<Animator>();
        }

        Cursor.lockState = CursorLockMode.Locked;

        if (cameraController == null) Debug.LogWarning("CameraController が見つかりませんでした！");
        if (characterController == null) Debug.LogWarning("CharacterController が見つかりませんでした！");
        if (animator == null) Debug.LogWarning("Animator が見つかりませんでした！");
    }

    void FixedUpdate() {
        if (characterController == null) return;

        // 🎮 PC用移動
        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveZ = Input.GetAxisRaw("Vertical");
        float rot = 0;

        if (Input.GetKey(KeyCode.Q)) rot = -1;
        if (Input.GetKey(KeyCode.E)) rot = 1;

        // 📱 スマホ用スワイプ（ボタンを触っている間は無効）
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            // 🔹 UIのボタンやスライダーを触っているときはスワイプ無効
            if (IsTouchOverUI(touch)) return;

            // 🔄 画面スワイプで移動
            if (touch.phase == TouchPhase.Moved) {
                moveX = touch.deltaPosition.x * 0.01f;
                moveZ = touch.deltaPosition.y * 0.01f;
            }
        }

        // 📱 スマホ用回転（二本指スワイプ）
        if (Input.touchCount == 2) {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 currentTwoFingerPos = (touch0.position + touch1.position) / 2;

            // 🔹 初回タッチ位置を記録
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began) {
                lastTwoFingerPos = currentTwoFingerPos;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved) {
                float deltaX = currentTwoFingerPos.x - lastTwoFingerPos.x;

                if (Mathf.Abs(deltaX) > 20f) { // 一定距離スワイプしたら
                    rot = (deltaX > 0) ? 1 : -1; // 右スワイプ → 時計回り、左スワイプ → 反時計回り
                }

                lastTwoFingerPos = currentTwoFingerPos; // 位置を更新
            }
        }

        // 🔄 スムーズな回転処理
        if (rot != 0) {
            Quaternion newRotation = Quaternion.Euler(0, rot * rotationSpeed * Time.fixedDeltaTime, 0);
            characterController.transform.rotation *= newRotation;
        }

        // 🎮 移動処理（PC & スマホ共通）
        Vector3 moveDirection = (characterController.transform.forward * moveZ + characterController.transform.right * moveX).normalized;
        if (moveDirection.magnitude < 0.1f) moveDirection = Vector3.zero;
        characterController.Move(moveDirection * moveSpeed * Time.fixedDeltaTime);

        bool isMoving = moveDirection.magnitude > 0.1f;
        if (isMoving != wasRunning) {
            if (animator != null) {
                animator.SetBool("IsRunning", isMoving);
            }
            wasRunning = isMoving;
        }
    }

    // ✅ ボタンやスライダーがあるUIをタップしているかチェック
    private bool IsTouchOverUI(Touch touch) {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.position;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results) {
            if (result.gameObject.GetComponent<Button>() != null || 
                result.gameObject.GetComponent<Slider>() != null) {
                return true;  // ボタン or スライダーを触っていたら無効
            }
        }
        return false;
    }
}
