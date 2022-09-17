using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerObj;
    [SerializeField] float sensitivity;
    [SerializeField] Vector3 startCameraRotation;

    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        xRotation = startCameraRotation.x;
        yRotation = startCameraRotation.y;

    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerObj.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
