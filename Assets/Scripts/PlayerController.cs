using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float JumpForce;
    private Animator anim;
    public LayerMask ground;
    public Collider2D coll;
    public Collider2D DisColl;
    private int Cherry;
    public Text CherryNum;
    private bool isHurt;
    private bool isGround;
    //public AudioSource jumpAudio, hurtAudio, cherryAudio;
    public Transform CellingCheck,GroundCheck;
    public int extraJump;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    if(!isHurt)
    {
        Movement();
    }
        SwitchAnim();
        isGround = Physics2D.OverlapCircle(GroundCheck.position, 0.2f, ground);
    }

    private void Update()
    {
        newJump();
        Crouch();
    }


    void Movement()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float facedircetion = Input.GetAxisRaw("Horizontal");

        //移动
        if(horizontalMove != 0)
        {
            rb.velocity = new Vector2(horizontalMove * speed * Time.fixedDeltaTime,rb.velocity.y);
            anim.SetFloat("running",Mathf.Abs(facedircetion));
        }
        if(facedircetion !=0)
        {
            transform.localScale = new Vector3(facedircetion,1,1);
        }

    }
    //切换动画
    void SwitchAnim()
    {
        if(rb.velocity.y < 0.1f && !coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling",true);
        }
        if(anim.GetBool("jumping"))
        {
            if(rb.velocity.y < 0)
            {
                anim.SetBool("jumping",false);
                anim.SetBool("falling",true);
            }
        }else if(isHurt)
        {
            anim.SetBool("hurt",true);
            anim.SetFloat("running",0);
            if(Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                anim.SetBool("hurt",false);
                isHurt = false;
            }
        }
        else if(coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling",false);
        }
    }
    //碰撞触发器
    void OnTriggerEnter2D(Collider2D collision)
    {//收集物品
        if(collision.tag == "Collection")
        {
            //cherryAudio.Play();
            SoundManager.instance.CherryAudio();
            Destroy(collision.gameObject);
            Cherry += 1;
            CherryNum.text = Cherry.ToString();
        }
        else if(collision.tag == "Collection2")
        {
            //cherryAudio.Play();
            SoundManager.instance.CherryAudio();
            Destroy(collision.gameObject);
            Cherry += 100;
            CherryNum.text = Cherry.ToString();
        }
        if(collision.tag == "DeadLine")
        {
            GetComponent<AudioSource>().enabled = false;
            Invoke("Restart",2f);
        }
    }
    //消灭敌人
    private void OnCollisionEnter2D(Collision2D  collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(anim.GetBool("falling"))
            {
            enemy.JumpOn();
            rb.velocity = new Vector2(rb.velocity.x , JumpForce * Time.deltaTime);
            anim.SetBool("jumping",true);
            }
            else if(transform.position.x < collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-10,rb.velocity.y);
                //hurtAudio.Play();
                SoundManager.instance.HurtAudio();
                isHurt = true;
            }
            else if(transform.position.x > collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(10, rb.velocity.y);
                //hurtAudio.Play();
                SoundManager.instance.HurtAudio();
                isHurt = true;
            }
        }
    }

    void Crouch()
    {
    if(!Physics2D.OverlapCircle(CellingCheck.position, 0.2f, ground))
    {
        if(Input.GetButton("Crouch"))
        {
            anim.SetBool("crouching",true);
            DisColl.enabled = false;
        }else
        {
            anim.SetBool("crouching",false);
            DisColl.enabled = true;
        }
    }
    }
//跳跃
    /*void Jump()
    {
        if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x , JumpForce * Time.fixedDeltaTime);
            jumpAudio.Play();
            anim.SetBool("jumping",true);
        }
    }*/

    void newJump()
    {
        if (isGround)
        {
            extraJump = 1;
        }
        if(Input.GetButtonDown("Jump") && extraJump > 0)
        {
            rb.velocity = Vector2.up * JumpForce;
            extraJump--;
            SoundManager.instance.JumpAudio();
            anim.SetBool("jumping", true);
        }
        if(Input.GetButtonDown("Jump")&& extraJump == 0 && isGround)
        {
            rb.velocity = Vector2.up * JumpForce;
            SoundManager.instance.JumpAudio();
            anim.SetBool("jumping", true);
        }
    }
    void Restart()
    {
     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
