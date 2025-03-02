using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private Vector2 lastTwoFingerPos;

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

    void Update() {
        if (characterController == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        float rot = 0;

        if (Input.GetKey(KeyCode.Q)) rot = -1;
        if (Input.GetKey(KeyCode.E)) rot = 1;

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (IsTouchOverUI(touch)) return;

            if (touch.phase == TouchPhase.Moved) {
                moveX = touch.deltaPosition.x * 0.01f;
                moveZ = touch.deltaPosition.y * 0.01f;
            }
        }

        // 🔥 2本指スワイプでの回転判定を緩和
        if (Input.touchCount == 2) {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            Vector2 currentTwoFingerPos = (touch0.position + touch1.position) / 2;

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began) {
                lastTwoFingerPos = currentTwoFingerPos;
            } else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved) {
                float deltaX = currentTwoFingerPos.x - lastTwoFingerPos.x;

                if (Mathf.Abs(deltaX) > 5f) { // ← 🔥 閾値を20px → 5pxに変更して判定を緩和
                    rot = deltaX * 0.05f;  // ← 🔥 回転速度をなめらかに調整
                }

                lastTwoFingerPos = currentTwoFingerPos;
            }
        }

        // 🔥 よりスムーズな回転処理
        if (rot != 0) {
            Debug.Log($"Rotation Input: {rot * rotationSpeed * Time.deltaTime}");
            characterPrefab.transform.Rotate(Vector3.up * rot * rotationSpeed * Time.deltaTime);
        }

        // 🔥 移動方向を characterPrefab の向きに合わせる
        Vector3 moveDirection = (characterPrefab.transform.forward * moveZ + characterPrefab.transform.right * moveX).normalized;
        
        if (moveDirection.magnitude < 0.1f) moveDirection = Vector3.zero;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 🔥 アニメーション処理
        bool isMoving = moveDirection.magnitude > 0.1f;
        if (isMoving != wasRunning) {
            if (animator != null) {
                animator.SetBool("IsRunning", isMoving);
            }
            wasRunning = isMoving;
        }
    }

    // ✅ UIをタップしているかを判定
    private bool IsTouchOverUI(Touch touch) {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current) {
            position = touch.position
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results) {
            if (result.gameObject.GetComponent<Button>() != null ||
                result.gameObject.GetComponent<Slider>() != null ||
                result.gameObject.GetComponent<Scrollbar>() != null ||
                result.gameObject.GetComponent<Dropdown>() != null ||
                result.gameObject.GetComponent<Toggle>() != null) {
                return true;
            }
        }
        return false;
    }
}
