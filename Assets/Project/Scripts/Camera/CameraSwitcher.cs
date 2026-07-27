using UnityEngine;
using UnityEngine.InputSystem; 

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject dynamicCamera;
    [SerializeField] private GameObject staticCamera;

    private bool isStaticActive = true;

    void Start()
    {
        SetCameraState(true);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
        {
            isStaticActive = !isStaticActive;
            SetCameraState(isStaticActive);
        }
    }

    private void SetCameraState(bool useStatic)
    {
        if (dynamicCamera != null && staticCamera != null)
        {
            dynamicCamera.SetActive(!useStatic);
            staticCamera.SetActive(useStatic);
        }
    }
}
