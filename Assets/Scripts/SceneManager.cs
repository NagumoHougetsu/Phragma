using UnityEngine;
using UnityEngine.Animations;

public class SceneManager : MonoBehaviour
{
    public GameObject backgroundPrefab;    // インスペクターで登録する背景（必要なら残す）
    public GameObject characterPrefab;     // インスペクターで登録するキャラクター
    private CameraController cameraController; // CameraController をインスペクター登録なしで取得

    void Awake(){
        // CameraController をこのオブジェクトから取得
        cameraController = GetComponent<CameraController>();
        SetupCameras();
    }

    void Start()
    {
        // CharacterController の確認・追加
        if (characterPrefab != null)
        {
            if (!characterPrefab.GetComponent<CharacterController>())
                characterPrefab.AddComponent<CharacterController>();
        }

        
    }

    private void SetupCameras()
    {
        if (cameraController.fpsCamera == null || cameraController.tpsCamera == null){
            Debug.LogError("FPS と TPS のカメラが設定されていません！");
            return;
        }

        if (characterPrefab != null){
            // PositionConstraintを追加
            AddPositionConstraint(cameraController.fpsCamera, characterPrefab.transform);
            AddPositionConstraint(cameraController.tpsCamera, characterPrefab.transform);

            // RotationConstraintを追加
            AddRotationConstraint(cameraController.fpsCamera, characterPrefab.transform);
            AddRotationConstraint(cameraController.tpsCamera, characterPrefab.transform);
        }
    }


    // PositionConstraintの追加
    // すでに存在する PositionConstraint にソースを追加
    private void AddPositionConstraint(Transform cameraTransform, Transform targetTransform)
    {
        if (cameraTransform != null && targetTransform != null)
        {
            var positionConstraint = cameraTransform.GetComponent<PositionConstraint>();

            if (positionConstraint != null)
            {
                // 新しいソースを追加
                var constraintSource = new ConstraintSource();
                constraintSource.sourceTransform = targetTransform;
                constraintSource.weight = 1f;

                // 既存のソースを更新
                positionConstraint.SetSource(0, constraintSource);

                // 有効化
                positionConstraint.constraintActive = true;
            }
            else
            {
                Debug.LogError("PositionConstraint が見つかりません！");
            }
        }
    }

    // すでに存在する RotationConstraint にソースを追加
    private void AddRotationConstraint(Transform cameraTransform, Transform targetTransform)
    {
        if (cameraTransform != null && targetTransform != null)
        {
            var rotationConstraint = cameraTransform.GetComponent<RotationConstraint>();

            if (rotationConstraint != null)
            {
                // 新しいソースを追加
                var constraintSource = new ConstraintSource();
                constraintSource.sourceTransform = targetTransform;
                constraintSource.weight = 1f;

                // 既存のソースを更新
                rotationConstraint.SetSource(0, constraintSource);

                // 有効化
                rotationConstraint.constraintActive = true;
            }
            else
            {
                Debug.LogError("RotationConstraint が見つかりません！");
            }
        }
    }

}
