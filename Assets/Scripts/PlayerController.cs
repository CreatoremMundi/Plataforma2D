using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthController))]
public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;
    public LayerMask groundLayer;
    public float groundCheckSize;
    public bool debugMode;

    private bool airJump = true;

    // Inputs
    private bool pressAttack;
    private bool pressJump;
    private bool holdDown;
    private bool pressHorizontal;
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
    private HealthController health;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        health = GetComponent<HealthController>();

    }

    void FixedUpdate()
    {
        // Movimenta o personagem
        if (holdDown && grounded) // crouching
        {
        }
        else
        {
            Vector2 movement = new Vector2(axisHorizontal * speed, rb2d.velocity.y);
            rb2d.velocity = movement;
        }
        

        // Aplica a física de pulo
        if (pressJump)
        {
            if (grounded)
            {
                Jump();
            }
            else if (airJump)
            {
                airJump = false;
                Jump();
            }
        }
    }

    void Update()
    {
        // Verifica os botões pressionado pelo jogador
        if (!inputLocked)
        {
            axisHorizontal = Input.GetAxis("Horizontal");
            pressHorizontal = Input.GetButton("Horizontal");
            axisVertical = Input.GetAxis("Vertical");
            pressAttack = Input.GetButtonDown("Attack");
            pressJump = Input.GetButtonDown("Jump");
            holdDown = Input.GetAxisRaw("Vertical") == -1;
        }
        else
        {
            axisHorizontal = 0;
            axisVertical = 0;
            pressAttack = false;
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
        //grounded = Physics2D.BoxCast(groundCheck.position, groundCheckSize, 0, Vector2.right, 0, groundLayer) && rb2d.velocity.y == 0;
        Bounds groundCheckBounds = CalculateGroundCheckBounds();
        grounded = Physics2D.BoxCast(groundCheckBounds.center, groundCheckBounds.size, 0, Vector2.right, 0, groundLayer) && rb2d.velocity.y == 0;
        if (grounded)
            airJump = true;

        // Atualiza a animação com caso ele esteja movimentando
        anim.SetFloat("velocityX", Mathf.Abs(rb2d.velocity.x));
        anim.SetFloat("axisVertical", axisVertical);
        anim.SetBool("grounded", grounded);
        anim.SetBool("holdDown", holdDown);
        anim.SetBool("pressHorizontal", pressHorizontal);

        if (debugMode)
        {
            Debug();
        }
    }

    void OnDrawGizmosSelected()
    {
        coll = GetComponent<BoxCollider2D>();

        Gizmos.color = Color.red;

        // Gizmos para a área de "ground check"
        Bounds bounds = CalculateGroundCheckBounds();
        Vector3 topLeft = new Vector3(bounds.min.x, bounds.max.y);
        Vector3 topRight = new Vector3(bounds.max.x, bounds.max.y);
        Vector3 bottomLeft = new Vector3(bounds.min.x, bounds.min.y);
        Vector3 bottomRight = new Vector3(bounds.max.x, bounds.min.y);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    void OnTriggerStay2D(Collider2D other)
    {
    }

    void OnTriggerExit2D(Collider2D other)
    {
    }

    private void Jump()
    {
        anim.SetTrigger("jump");
        pressJump = false;

        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    //IEnumerator Roll()
    //{
    //    rolling = true;
    //    inputLocked = true;

    //    Vector2 startPos = rb2d.position;
    //    Vector2 endPos = rb2d.position;
    //    endPos.x = spriteRenderer.flipX ? rb2d.position.x - rollDistance : rb2d.position.x + rollDistance;

    //    float currentTime = 0;

    //    anim.SetTrigger("roll");
    //    while (true)
    //    {
    //        currentTime += Time.fixedDeltaTime;
    //        float t = currentTime / rollTime;

    //        Vector2 currentPos = Vector2.Lerp(startPos, endPos, t);
    //        rb2d.MovePosition(currentPos);

    //        if (t >= 1f)
    //            break;

    //        yield return null;
    //    }

    //    rolling = false;
    //    inputLocked = false;
    //}

    //IEnumerator Attack()
    //{
    //    attacking = true;
    //    inputLocked = true;

    //    anim.SetTrigger("attack");

    //    inputLocked = false;
    //    attacking = false;

    //    return null;
    //}

    Bounds CalculateGroundCheckBounds()
    {
        Vector2 center = new Vector2(transform.position.x, coll.bounds.min.y);
        Vector2 size = new Vector2(coll.bounds.max.x - coll.bounds.min.x, groundCheckSize);

        return new Bounds(center, size);
    }

    void Debug()
    {
        GameObject debugScreen = GameObject.Find("DebugScreen");

        if (debugScreen)
        {
            DebugCanvas canvas = debugScreen.GetComponent<DebugCanvas>();

            canvas.Show(0, string.Format("Grounded: {0}", grounded ? "Yes" : "No"));
            canvas.Show(1, string.Format("Holding Down: {0}", holdDown ? "Yes" : "No"));
        }
    }
}
