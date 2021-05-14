using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    private CinemachineInputProvider inputProvider;
    private CinemachineVirtualCamera virtualCamera;
    private Transform cameraTransform;
    [SerializeField] float zoomSpeed = 3f;
    [SerializeField] float zoomInMax = 5f;
    [SerializeField] float zoomOutMax = 20f;
    // Start is called before the first frame update
    void Start()
    {
        inputProvider = GetComponent<CinemachineInputProvider>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        cameraTransform = virtualCamera.VirtualCameraGameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float z = inputProvider.GetAxisValue(2);
        if (z != 0)
        {
            ZoomScreen(z);
        }
    }

    public void ZoomScreen(float increment)
    {
        float fov = virtualCamera.m_Lens.OrthographicSize;
        float target = Mathf.Clamp(fov + increment, zoomInMax, zoomOutMax);
        virtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(fov, target, zoomSpeed * Time.deltaTime);
    }


}
