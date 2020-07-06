using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;

    public float speed;

    public float jumpForce;
    bool doubleJumpable;

    public float groundCheckRadius;
    public LayerMask ground;
    public GameObject groundCheck;
    bool isOnGround;

    int cherryNum;

    public Text cherryNumText;

    public AudioSource cherryAudio;
    public AudioSource diamondAudio;

    public float diamondJumpForce;

    public Transform portalTarget;
    public AudioSource portalAudio;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        doubleJumpable = false;
        cherryNum = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isOnGround = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, ground);

        anim.SetBool("isOnGround", isOnGround);

        if (!doubleJumpable && isOnGround)
            doubleJumpable = true;

        Movement();
    }

    void Movement()
    {
        float xVelocity = Input.GetAxisRaw("Horizontal");
        float yVelocity = rb.velocity.y;

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            yVelocity = Mathf.Abs(jumpForce);
        }

        if (Input.GetButtonDown("Jump") && !isOnGround && doubleJumpable)
        {
            yVelocity = Mathf.Abs(jumpForce) / 1.3f;
            anim.SetTrigger("doubleJump");
            doubleJumpable = false;
        }

        anim.SetFloat("xSpeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("ySpeed", rb.velocity.y);

        rb.velocity = new Vector2(xVelocity * speed, yVelocity);

        if (xVelocity != 0)
            transform.localScale = new Vector3(xVelocity, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collection")
        {
            cherryAudio.Play();
            Destroy(collision.gameObject);
            cherryNum += 1;
            cherryNumText.text = cherryNum.ToString();
        }

        if (collision.tag == "Diamond")
        {
            diamondAudio.Play();
            rb.velocity = new Vector2(rb.velocity.x, diamondJumpForce);
        }

        if (collision.tag == "Portal")
        {
            portalAudio.Play();
            transform.position = portalTarget.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
    }

}
