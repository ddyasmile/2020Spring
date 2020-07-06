using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;

    public float speed;
    public float upSpeedOnWall;
    float xVelocity;
    float yVelocity;

    public float jumpForce;
    bool doubleJumpable;

    public float groundCheckRadius;
    public LayerMask platform;
    public GameObject groundCheck;
    bool isOnGround;

    public float wallCheckRadius;
    public LayerMask wall;
    public GameObject wallCheck;
    bool isOnWall;

    bool playerDead;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        doubleJumpable = false;
        playerDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        isOnGround = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, platform);
        isOnWall = Physics2D.OverlapCircle(wallCheck.transform.position, wallCheckRadius, wall);

        anim.SetBool("isOnGround", isOnGround);
        anim.SetBool("isOnWall", isOnWall);

        if (!doubleJumpable && (isOnGround || isOnWall))
            doubleJumpable = true;

        Movement();
    }

    void Movement()
    {
        xVelocity = Input.GetAxisRaw("Horizontal");
        yVelocity = rb.velocity.y;

        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            yVelocity = Mathf.Abs(jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isOnGround && !isOnWall && doubleJumpable)
        {
            yVelocity = Mathf.Abs(jumpForce) / 1.3f;
            anim.SetTrigger("doubleJump");
            doubleJumpable = false;
        }

        if (isOnWall)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yVelocity = Mathf.Abs(jumpForce);
            }
            else
            {
                yVelocity = 0;
                Vector3 curPos = transform.position;
                transform.position = new Vector3(curPos.x, curPos.y + upSpeedOnWall * Time.deltaTime, curPos.z);
            }
            
        }

        rb.velocity = new Vector2(xVelocity * speed, yVelocity);

        anim.SetFloat("xSpeed", Mathf.Abs(rb.velocity.x));  //跑动动画
        anim.SetFloat("ySpeed", rb.velocity.y);

        if (xVelocity != 0)
            transform.localScale = new Vector3(xVelocity, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spike"))
        {
            anim.SetTrigger("dead");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fan"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 10.0f);
        }
    }

    public void PlayerDead()
    {
        playerDead = true;
        GameManager.GameOver(playerDead);
    }

    public void DoubleJump2Fall()
    {
        anim.SetTrigger("doubleJump2Fall");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wallCheck.transform.position, wallCheckRadius);
    }
}
