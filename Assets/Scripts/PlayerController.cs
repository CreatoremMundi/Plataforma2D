using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;
    public float rollTime;
    public float rollDistance;
    public float grabbingTime;
    public float grabSpeed;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Vector2 groundCheckSize;

    private float grabbingTimer;
    private float defaultGravity;

    // Inputs
    private bool pressAttack;
    private bool pressGrab;
    private bool pressRoll;
    private bool pressJump;
    private float axisHorizontal;
    private float axisVertical;

    // States
    private bool grounded;
    private bool rolling;
    private bool attacking;
    private bool grabbing;
    private bool grabed;

    private bool inputLocked;

    // Components
    private Animator anim;
    private BoxCollider2D coll;
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        defaultGravity = rb2d.gravityScale;
    }

    void FixedUpdate()
    {
        // Movimenta o personagem
        if (grabed)
        {
            rb2d.gravityScale = 0;

            Vector2 movement = new Vector2(axisHorizontal * grabSpeed, axisVertical * grabSpeed);
            rb2d.velocity = movement;
        }
        else
        {
            rb2d.gravityScale = defaultGravity;

            Vector2 movement = new Vector2(axisHorizontal * speed, rb2d.velocity.y);
            rb2d.velocity = movement;
        }
        

        // Aplica a física de pulo
        if (grounded && !rolling && !attacking)
        {
            if (pressJump)
            {
                Jump();
            }
            else if (pressRoll)
            {
                StartCoroutine("Roll");
            }
            else if (pressAttack)
            {
                StartCoroutine("Attack");
            }
        }
    }

    void Update()
    {
        // Verifica os botões pressionado pelo jogador
        if (!inputLocked)
        {
            axisHorizontal = Input.GetAxis("Horizontal");
            axisVertical = Input.GetAxis("Vertical");
            pressAttack = Input.GetButtonDown("Attack");
            pressGrab = Input.GetButton("Grab");
            pressRoll = Input.GetButtonDown("Roll");
            pressJump = Input.GetButtonDown("Jump");
        }
        else
        {
            axisHorizontal = 0;
            pressAttack = false;
            pressGrab = false;
            pressRoll = false;
            pressJump = false;
        }

        // Vira o personagem de acordo com a direção que ele está indo
        if (axisHorizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (axisHorizontal < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Verifica se está tocando o chão (para evitar pulos enquanto estiver no ar)
        grounded = Physics2D.BoxCast(groundCheck.position, groundCheckSize, 0, Vector2.right, 0, groundLayer) && rb2d.velocity.y == 0;

        // Atualiza a animação com caso ele esteja movimentando
        anim.SetFloat("axisHorizontal", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("grounded", grounded);
        anim.SetBool("grabed", grabed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector2 extends = new Vector2(groundCheckSize.x / 2, groundCheckSize.y / 2);

        Vector3 topLeft = new Vector3(groundCheck.position.x - extends.x, groundCheck.position.y + extends.y);
        Vector3 topRight = new Vector3(groundCheck.position.x + extends.x, groundCheck.position.y + extends.y);
        Vector3 bottomLeft = new Vector3(groundCheck.position.x - extends.x, groundCheck.position.y - extends.y);
        Vector3 bottomRight = new Vector3(groundCheck.position.x + extends.x, groundCheck.position.y - extends.y);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Grid") && (pressGrab) && !grounded)
        {
            grabed = true;
        }
        else
        {
            grabed = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Grid"))
        {
            grabed = false;
        }
    }

    private void Jump()
    {
        pressJump = false;

        rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

        anim.SetTrigger("jump");
    }

    IEnumerator Roll()
    {
        rolling = true;
        inputLocked = true;

        Vector2 startPos = rb2d.position;
        Vector2 endPos = rb2d.position;
        endPos.x = spriteRenderer.flipX ? rb2d.position.x - rollDistance : rb2d.position.x + rollDistance;

        float currentTime = 0;

        anim.SetTrigger("roll");
        while (true)
        {
            currentTime += Time.fixedDeltaTime;
            float t = currentTime / rollTime;

            Vector2 currentPos = Vector2.Lerp(startPos, endPos, t);
            rb2d.MovePosition(currentPos);

            if (t >= 1f)
                break;

            yield return null;
        }

        rolling = false;
        inputLocked = false;
    }

    IEnumerator Attack()
    {
        attacking = true;
        inputLocked = true;

        anim.SetTrigger("attack");

        inputLocked = false;
        attacking = false;
        
        return null;
    }
}
