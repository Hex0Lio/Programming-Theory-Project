using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerObj;
    Rigidbody playerRb;

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float groundDrag;
    Vector3 moveDir;
    float hInput;
    float vInput;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float checkRadius;
    [SerializeField] Transform checkPos;
    bool isGrounded;

    [Header("Jumping")]
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMultiplier;
    bool readyToJump = true;

    [Header("Step Climb")]
    [SerializeField] Transform lowerRay;
    [SerializeField] Transform upperRay;
    [SerializeField] float stepHeight;
    [SerializeField] float stepSmooth;

    [Header("Slope Climb")]
    [SerializeField] float maxClimbAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

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
        
        AddGroundDrag();
        ControlSpeed();
    }

    void FixedUpdate()
    {
        Move();
        StepClimb();
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

    void DoGroundCheck() => isGrounded = Physics.CheckSphere(checkPos.position, checkRadius, groundMask);

    void AddGroundDrag()
    {
        if (isGrounded)
            playerRb.drag = groundDrag;
        else
            playerRb.drag = 0f;
    }
    void ControlSpeed()
    {
        if (IsOnSlope() && !exitingSlope) {
            if (playerRb.velocity.magnitude > speed) {
                playerRb.velocity = playerRb.velocity.normalized * speed;
            }
        } else {
            Vector3 flatVel = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
            if (flatVel.magnitude > speed) {
                Vector3 limitedVel = flatVel.normalized * speed;
                playerRb.velocity = new Vector3(limitedVel.x, playerRb.velocity.y, limitedVel.z);
            }
        }
    }

    void Move()
    {
        moveDir = (playerObj.right * hInput + playerObj.forward * vInput).normalized;

        // On slope
        if (isGrounded && IsOnSlope() && !exitingSlope) {
            playerRb.AddForce(speed * 10f * GetSlopeMoveDir(), ForceMode.Force);

            if (playerRb.velocity.y > 0) playerRb.AddForce(Vector3.down * 10f, ForceMode.Force);
        }
        // On ground
        else if (isGrounded)
            playerRb.AddForce(speed * 10f * moveDir, ForceMode.Force);
        // In the air
        else
            playerRb.AddForce(speed * airMultiplier * 10f * moveDir, ForceMode.Force);

        playerRb.useGravity = !IsOnSlope();
    }

    void Jump()
    {
        exitingSlope = true;

        float jumpForce = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight) * 1.3f;
        playerRb.velocity = new Vector3(playerRb.velocity.x, jumpForce, playerRb.velocity.z);
    }
    void ResetJump()
    {
        exitingSlope = false;
        readyToJump = true;
    }

    void StepClimb()
    {
        Vector3[] directions = {
            moveDir,
            Quaternion.AngleAxis(45, Vector3.up) * moveDir,
            Quaternion.AngleAxis(-45, Vector3.up) * moveDir
        };

        foreach (Vector3 dir in directions) {
            RaycastHit hitLower;
            if (Physics.Raycast(lowerRay.position, dir, out hitLower, 0.75f)) {
                float angle = Vector3.Angle(Vector3.up, hitLower.normal);
                if (!Physics.Raycast(upperRay.position, dir, 1) && angle == 90) {
                    playerRb.position -= new Vector3(0, -stepSmooth * Time.deltaTime, 0);
                }
            }
        }
    }

    bool IsOnSlope()
    {
        if (Physics.SphereCast(transform.position, 0.5f, Vector3.down, out slopeHit, 1)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if (angle <= maxClimbAngle && angle != 0) {
                return true;
            }
        }

        return false;
    }
    Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }
}
