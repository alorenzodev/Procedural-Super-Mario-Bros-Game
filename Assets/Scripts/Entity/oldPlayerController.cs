using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oldPlayerController : MonoBehaviour
{
    //las variables globales públicas se pueden ver en Unity
    public Animator animator;

    public float normalSpeed = 6.0f;
    public float speed;
    public float maxSpeed = 10.0f;
    private Rigidbody2D rb;
    private float moveInput;

    public float jumpForce;
    private bool isGrounded;
    public Transform feetPos;
    public float circleRadius;
    public LayerMask whatIsGround;

    public float jumpTime;
    private float jumpTimeCounter;
    private bool isJumping;

    private static Transform p;

    public static Transform getTransform()
    {
        return p;
    }

    // Start is called before the first frame update
    void Start()
    {
        p = this.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, circleRadius);

        if (isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if(isJumping && Input.GetKey(KeyCode.Space))
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else if (jumpTimeCounter < 0)
            {
                isJumping = false;
            }

        }
        /*
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
            }
        */
        if (Input.GetKey(KeyCode.E))
        {

            speed = Mathf.Clamp(speed + Time.deltaTime*4, 6.0f, maxSpeed);
        }

        if (Input.GetKeyUp(KeyCode.E) || !Input.GetKey(KeyCode.E))
        {

            speed = Mathf.Clamp(speed - Time.deltaTime * 6, 6.0f, maxSpeed);
        }

        animator.SetFloat("horizontal", Mathf.Abs(Input.GetAxis("Horizontal")));
        animator.SetFloat("vertical", Mathf.Abs(Input.GetAxis("Vertical")));


    }

    void FixedUpdate()
    {
        moveInput = Input.GetAxis("Horizontal");

        if (moveInput > 0)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        rb.velocity = new Vector2(speed * moveInput, rb.velocity.y);

    }
}
