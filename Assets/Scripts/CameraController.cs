using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    private float mouseSpeed = 3.5f;
    private float movementSpeed = 6;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    Rigidbody rb;
    private bool lockCamera = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(150, 50, 0);
        rb = GetComponent<Rigidbody>();
    }

    public void UnlockCamera()
    {
        lockCamera = false;
    }

    public void LockCamera()
    {
        lockCamera = true;
    }

    void Update()
    {
        if (!lockCamera)
        {
            Vector3 moveDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
            if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
            if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;
            if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;

            if (moveDirection != Vector3.zero)
                moveDirection.Normalize();

            float speed = movementSpeed * 100 * Time.unscaledDeltaTime * (Input.GetKey(KeyCode.LeftShift) ? 5 : 1);
            rb.MovePosition(transform.position + moveDirection * speed);

            yaw += Input.GetAxis("Mouse X") * mouseSpeed;
            pitch -= Input.GetAxis("Mouse Y") * mouseSpeed;
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}
