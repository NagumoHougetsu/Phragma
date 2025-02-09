using UnityEngine;

public class CameraController : MonoBehaviour{
    public Transform fpsCamera;
    public Transform tpsCamera;

    private bool isFPS = true;

    void Start(){
        SetCameraMode(isFPS);
    }

    public void ToggleCameraMode(){
        isFPS = !isFPS;
        SetCameraMode(isFPS);
    }

    void SetCameraMode(bool fpsMode){
        if (fpsCamera != null)
            fpsCamera.gameObject.SetActive(fpsMode);
        if (tpsCamera != null)
            tpsCamera.gameObject.SetActive(!fpsMode);
    }

    public void SetCameraRotation(float pitch){
        if (isFPS){
            fpsCamera.localRotation = Quaternion.Euler(pitch, 0, 0);
        }else{
            tpsCamera.localRotation = Quaternion.Euler(pitch, 0, 0);
        }
    }
}
