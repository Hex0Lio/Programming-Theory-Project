using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform playerObj;

    [Header("Variables")]
    public float speed;
    public float gravity;
    public float jumpHeight;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float checkRadius;
    public Transform checkPos;

    CharacterController controller;
    float hInput;
    float vInput;
    bool isGrounded;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        DoGroundCheck();

        InputListener();
        Move();

        if (Input.GetKey(KeyCode.Space) && isGrounded) Jump();

        ApplyGravity();
    }

    void ApplyGravity()
    {
        velocity.y -= gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight);
    }

    void DoGroundCheck()
    {
        isGrounded = Physics.CheckSphere(checkPos.position, checkRadius, groundMask);
    }

    void InputListener()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
    }

    void Move()
    {
        Vector3 moveDir = (playerObj.right * hInput + playerObj.forward * vInput).normalized;

        controller.Move(moveDir * speed * Time.deltaTime);
    }
}
