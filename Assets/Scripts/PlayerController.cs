using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;
    public float rollSpeed;
    public LayerMask groundLayer;
    public Transform groundCheck;

    // Inputs
    private bool jump;
    private bool roll;
    private float axisHorizontal;

    // States
    private bool grounded;
    private bool rolling;

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

    void Update()
    {
        // Verifica os botões pressionado pelo jogador
        axisHorizontal = Input.GetAxis("Horizontal");
        jump = Input.GetButtonDown("Jump");
        roll = Input.GetKeyDown(KeyCode.S);

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
        grounded = Physics2D.CircleCast(groundCheck.position, 0.05f, Vector2.zero, 0, groundLayer);
        rolling = anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerRoll");

        // Atualiza a animação com caso ele esteja movimentando
        anim.SetFloat("axisHorizontal", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("grounded", grounded);

        if (rolling)
        {
            
            axisHorizontal += rollSpeed * Mathf.Sign(axisHorizontal);
        }
        else if (grounded && roll)
        {
            anim.SetTrigger("rolling");
        }
    }

    void FixedUpdate()
    {
        // Movimenta o personagem
        Vector2 movement = new Vector2(axisHorizontal * speed, rb2d.velocity.y);
        rb2d.velocity = movement;

        // Aplica a física de pulo
        if (jump && grounded && !rolling)
        {
            rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.05f);
    }
}
