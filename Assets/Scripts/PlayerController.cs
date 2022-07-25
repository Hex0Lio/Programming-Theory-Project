using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform playerObj;
    Rigidbody playerRb;

    Vector3 moveDir;
    float hInput;
    float vInput;
    bool isGrounded;
    bool readyToJump = true;

    [Header("Movement")]
    public float speed;
    public float groundDrag;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float checkRadius;
    public Transform checkPos;

    [Header("Jumping")]
    public float jumpHeight;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Step Climb")]
    public Transform lowerRay;
    public Transform upperRay;
    public float stepHeight;
    public float stepSmooth;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        upperRay.position = new Vector3(upperRay.position.x, stepHeight, upperRay.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        DoGroundCheck();
        InputListener();
    }

    void FixedUpdate()
    {
        //StepClimb();
        Move();
    }

    void InputListener()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && isGrounded && readyToJump) {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void DoGroundCheck()
    {
        isGrounded = Physics.CheckSphere(checkPos.position, checkRadius, groundMask);
    }

    void AddGroundDrag()
    {
        if (isGrounded)
            playerRb.drag = groundDrag;
        else
            playerRb.drag = 0f;
    }

    void ControlSpeed()
    {
        Vector3 flatVel = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
        if (flatVel.magnitude > speed) {
            Vector3 limitedVel = flatVel.normalized * speed;
            playerRb.velocity = new Vector3(limitedVel.x, playerRb.velocity.y, limitedVel.z);
        }
    }

    void Move()
    {
        moveDir = (playerObj.right * hInput + playerObj.forward * vInput).normalized;

        // On ground
        if (isGrounded)
            playerRb.MovePosition(transform.position + speed * Time.fixedDeltaTime * moveDir);
        //playerRb.AddForce(speed * 10f * moveDir, ForceMode.Force);
        //transform.Translate(speed * Time.deltaTime * moveDir, Space.World);
        // In the air
        else
            playerRb.MovePosition(transform.position + speed * airMultiplier * Time.fixedDeltaTime * moveDir);
        //playerRb.AddForce(speed * airMultiplier * 10f * moveDir, ForceMode.Force);
        //transform.Translate(speed * airMultiplier * Time.deltaTime * moveDir, Space.World);
    }

    void Jump()
    {
        float jumpForce = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight);
        playerRb.velocity = new Vector3(playerRb.velocity.x, jumpForce, playerRb.velocity.z);
    }
    void ResetJump() => readyToJump = true;

    void StepClimb()
    {
        //RaycastHit hitLower;
        if (Physics.Raycast(lowerRay.position, moveDir, 0.2f)) {
            //RaycastHit hitUpper;
            if (!Physics.Raycast(upperRay.position, moveDir, 0.3f)) {
                playerRb.position -= new Vector3(0, -stepSmooth * Time.deltaTime, 0);
            }
        }

        //RaycastHit hitLowerLeft;
        if (Physics.Raycast(lowerRay.position, Quaternion.AngleAxis(45, Vector3.up) * moveDir, 0.2f)) {
            //RaycastHit hitUpperLeft;
            if (!Physics.Raycast(upperRay.position, Quaternion.AngleAxis(45, Vector3.up) * moveDir, 0.3f)) {
                playerRb.position -= new Vector3(0, -stepSmooth * Time.deltaTime, 0);
            }
        }

        //RaycastHit hitLowerMinusRight;
        if (Physics.Raycast(lowerRay.position, Quaternion.AngleAxis(-45, Vector3.up) * moveDir, 0.2f)) {
            //RaycastHit hitUpperRight;
            if (!Physics.Raycast(upperRay.position, Quaternion.AngleAxis(-45, Vector3.up) * moveDir, 0.3f)) {
                playerRb.position -= new Vector3(0, -stepSmooth * Time.deltaTime, 0);
            }
        }
    }
}
