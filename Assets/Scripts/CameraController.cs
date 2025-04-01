using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private float mouseSpeed = 2;
    [SerializeField, Range(1, 10)] private float movementSpeed = 1;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right;
        }

        // Normalize to maintain a consistent speed in diagonal movement
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.MovePosition(transform.position + moveDirection * movementSpeed * 100 * Time.deltaTime * 5);
        }
        else
        {
            rb.MovePosition(transform.position + moveDirection * movementSpeed * 100 * Time.deltaTime);

        }

        yaw += mouseSpeed * Input.GetAxis("Mouse X");
        pitch -= mouseSpeed * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

}
