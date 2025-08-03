using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public float moveSpeed;

    private Rigidbody2D rb;
    private BoxCollider2D box;
    public Animator animator;
    private float previousInput = 0f;
    private float moveInput;

    private bool wasHitting = false;
    private bool isHitting = false;

    private Vector3 originalScale;
    private Vector3 rotatedScale;

    private bool isFacingRight = false;

    public float gravity;

    private float dy = 0f;
    private float lastDy;

    private int floorsCounter = 0;

    public float jumpSpeed = 10f;
    public float floorCorrectionSpeed = 0.1f;
    private bool correctFloor = false;

    private float floorOffset = 0f;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;


    void Awake()
    {

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        originalScale = transform.localScale;
        rotatedScale = originalScale;
        rotatedScale.x *= -1;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        UpdateJump(Time.deltaTime);
        //FixedUpdateGravity(Time.deltaTime);
        //FixedUpdateInput(Time.deltaTime);
        UpdateFacing();
        UpdateAnimatorMoving();
        UpdateAnimatorHitting();
        previousInput = moveInput;
    }

    void FixedUpdate()
    {
        FixedUpdateGravity(Time.fixedDeltaTime);
        //moveInput = Input.GetAxisRaw("Horizontal");
        FixedUpdatePosition(Time.fixedDeltaTime);
        

        //previousInput = moveInput;
    }

    void FixedUpdatePosition(float time)
    {
         // -1, 0, or 1 for A/D
        lastDy = dy * time;
        Vector2 moveDelta = new Vector2(moveInput * moveSpeed * time, dy * time);
        if (correctFloor)
        {

            moveDelta.y += floorOffset;
            //rb.MovePosition(rb.poition + moveDelta);
            correctFloor = false;        
        }
        rb.MovePosition(rb.position + moveDelta);
    }

    void UpdateFacing()
    {
        if (moveInput > 0 && !isFacingRight)
        {
            transform.localScale = rotatedScale;
            isFacingRight = true;
        }
        else
            if (moveInput < 0 && isFacingRight)
        {
            transform.localScale = originalScale;
            isFacingRight = false;
        }
    }


    void UpdateAnimatorMoving()
    {
        if (moveInput != previousInput)
        {
            bool isMoving = moveInput != 0f;
            animator.SetBool("moving", isMoving);
        }
    }

    void UpdateAnimatorHitting()
    {
        bool isHitting = Input.GetMouseButton(0); // true while LMB is held

        if (isHitting != wasHitting)
        {
            animator.SetBool("hitting", isHitting);
        }

        wasHitting = isHitting;
    }


    void FixedUpdateGravity(float time)
    {
        if (floorsCounter > 0)
        {
            dy = Mathf.Max(dy,0f);
        }
        else
        {
            dy += gravity * time;
        }


        //Vector2 gravityDelta = new Vector2(0f, dy* Time.fixedDeltaTime);
        //rb.MovePosition(rb.position + gravityDelta);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"collided with {collision.collider}");
        // Check if the object we collided with is tagged as "floor"
        if (collision.collider.CompareTag("Floor"))
        {
            if (floorsCounter == 0)
            {
                // we have to correct collision with floor:
                correctFloor = true;
                float bottom = box.bounds.min.y;
                float floorY = collision.collider.bounds.max.y;
                floorOffset = floorY - bottom;

            }
            floorsCounter += 1;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the object we collided with is tagged as "floor"
        if (collision.collider.CompareTag("Floor"))
        {
            floorsCounter -= 1;
        }
    }


    void UpdateJump(float time)
    {
        if (floorsCounter > 0)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= time;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpBufferCounter = jumpBufferTime;
            // Handle jump input


        }
        else
        {
            jumpBufferCounter -= time;
            if (dy > 0f && Input.GetKeyUp(KeyCode.W))
            {
                dy *= 0.5f;
                coyoteTimeCounter = 0f;
            }


        }

            if (coyoteTimeCounter >= 0f && jumpBufferCounter >= 0f)
            {
                dy = jumpSpeed;
                jumpBufferCounter = 0f;
            }


    }

}