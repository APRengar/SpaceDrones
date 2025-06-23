using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float boostMultiplier = 2f;

    [Header("Rotation")]
    public float lookSpeed = 3f;
    public bool invertY = false;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.None 
                ? CursorLockMode.Locked 
                : CursorLockMode.None;
        }
    }

    private void HandleMovement()
    {
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? boostMultiplier : 1f);

        Vector3 move = new Vector3(
            Input.GetAxis("Horizontal"),
            (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0),
            Input.GetAxis("Vertical")
        );

        transform.Translate(move * speed * Time.deltaTime, Space.Self);
    }

    private void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed * (invertY ? 1f : -1f);

        rotationX += mouseX;
        rotationY += mouseY;
        rotationY = Mathf.Clamp(rotationY, -89f, 89f);

        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }
}