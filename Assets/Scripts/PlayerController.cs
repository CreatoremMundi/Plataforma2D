using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;
    public float rollTime;
    public float rollDistance;
    public LayerMask groundLayer;
    public Transform groundCheck;

    // Inputs
    private bool pressAttack;
    private bool pressGrab;
    private bool pressRoll;
    private bool pressJump;
    private float axisHorizontal;

    // States
    private bool grounded;
    private bool rolling;

    private bool inputLocked;

    // Components
    private Animator anim;
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // Movimenta o personagem
        Vector2 movement = new Vector2(axisHorizontal * speed, rb2d.velocity.y);
        rb2d.velocity = movement;

        // Aplica a física de pulo
        if (grounded && !rolling)
        {
            if (pressJump)
            {
                Jump();
            }
            else if (pressRoll)
            {
                StartCoroutine("Roll");
            }
        }       
    }

    void Update()
    {
        // Verifica os botões pressionado pelo jogador
        if (!inputLocked)
        {
            axisHorizontal = Input.GetAxis("Horizontal");
            pressAttack = Input.GetButtonDown("Attack");
            pressGrab = Input.GetButtonDown("Grab");
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
        grounded = Physics2D.CircleCast(groundCheck.position, 0.05f, Vector2.zero, 0, groundLayer) && rb2d.velocity.y == 0;

        // Atualiza a animação com caso ele esteja movimentando
        anim.SetFloat("axisHorizontal", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("grounded", grounded);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.05f);
    }

    private void Jump()
    {
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
}
