using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(EnergyController))]
public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;

    public float dodgeDistance;
    public float dodgeDuration;

    public LayerMask groundLayer;
    public float groundCheckSize;
    public bool debugMode;

    private bool airJump = true;
    private Vector2 lookDirection;

    // Inputs
    private bool pressDodge;
    private bool pressJump;
    private bool holdDown;
    private bool pressHorizontal;
    private float axisHorizontal;
    private float axisVertical;

    // States
    private bool grounded;
    private bool locked;

    // Components
    private Animator anim;
    private BoxCollider2D coll;
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private HealthController healthController;
    private EnergyController energyController;

    // Propriedades
    public bool IsLocked { get; set; }

    void Start()
    {
        // Inicia os componenetes
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        healthController = GetComponent<HealthController>();
        energyController = GetComponent<EnergyController>();

        healthController.OnTakeDamage += (amount) => {
            StopCoroutine("Dodge");
            IsLocked = false;
        };

        healthController.OnEnterInvulnerableMode += () =>
        {
            StartCoroutine(Blink(0.15f, healthController.invulnerableTime));
            StartCoroutine(Knockback(7));
            anim.SetTrigger("getHit");
        };
    }

    void FixedUpdate()
    {
        if (!IsLocked)
        {
            if (!(holdDown && grounded)) // crouching
            {
                // Movimenta o personagem
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
            else if (pressDodge)
            {
                StartCoroutine("Dodge");
            }
        }
    }

    void Update()
    {
        // Verifica os botões pressionado pelo jogador
        if (IsLocked)
        {
            axisHorizontal = 0;
            axisVertical = 0;
            pressHorizontal = false;
            pressDodge = false;
            pressJump = false;
            holdDown = false;
        }
        else
        {
            axisHorizontal = Input.GetAxis("Horizontal");
            pressHorizontal = Input.GetButton("Horizontal");
            axisVertical = Input.GetAxis("Vertical");
            pressDodge = Input.GetButtonDown("Dodge");
            pressJump = Input.GetButtonDown("Jump");
            holdDown = Input.GetAxisRaw("Vertical") == -1;
        }
        

        // Verifica se está tocando o chão (para evitar pulos enquanto estiver no ar)
        Bounds groundCheckBounds = CalculateGroundCheckBounds();
        grounded = Physics2D.BoxCast(groundCheckBounds.center, groundCheckBounds.size, 0, Vector2.right, 0, groundLayer) && rb2d.velocity.y == 0;
        if (grounded)
            airJump = true;

        // Atualiza os estados das animações
        anim.SetFloat("velocityX", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("grounded", grounded);
        anim.SetBool("holdDown", holdDown);
        anim.SetBool("pressHorizontal", pressHorizontal);

        // Vira o personagem de acordo com a direção que ele está indo
        Flip();

        if (debugMode)
        {
            Debug();
        }
    }

    /// <summary>
    /// Aplica a física de pulo para o jogador
    /// </summary>
    private void Jump()
    {
        // Atualiza a animação
        anim.SetTrigger("jump");
        // Desabilita o input da ação para evitar um impulso extra
        pressJump = false;

        // Antes de aplicar o impulso, é zerado o movimento vertical que o objeto está fazendo
        // para evitar que o impsulso do pulo duplu seja maior, já que ele vai adicionar a força 
        // junto ao movimento que já está sendo executado
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Vira o personagem de acordo com a direção que ele está indo
    /// </summary>
    private void Flip()
    {
        if (axisHorizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (axisHorizontal < 0)
        {
            spriteRenderer.flipX = true;
        }

        lookDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
    }

    Bounds CalculateGroundCheckBounds()
    {
        Vector2 center = new Vector2(transform.position.x + coll.offset.x, coll.bounds.min.y);
        Vector2 size = new Vector2(coll.bounds.max.x - coll.bounds.min.x, groundCheckSize);

        return new Bounds(center, size);
    }

    IEnumerator Blink(float delayBetweenBlinks, float duration)
    {
        float counter = 0;
        bool toggleColor = false;
        Color defaultColor = spriteRenderer.color;

        while (counter <= duration)
        {
            spriteRenderer.color = toggleColor ? Color.clear : defaultColor;
            toggleColor = !toggleColor;

            yield return new WaitForSeconds(delayBetweenBlinks);
            counter += delayBetweenBlinks + Time.deltaTime;
        }

        spriteRenderer.color = defaultColor;
    }

    IEnumerator Dodge()
    {
        pressDodge = false;
        float x = lookDirection == Vector2.right ? rb2d.position.x + dodgeDistance : rb2d.position.x - dodgeDistance;
        Vector2 startPos = rb2d.position;
        Vector2 endPos = new Vector2(x, rb2d.position.y);

        float currentTime = 0;
        float t = 0;

        IsLocked = true;

        while (t < 1f)
        {
            currentTime += Time.deltaTime;
            t = currentTime / dodgeDuration;

            Vector2 currentPos = Vector2.Lerp(startPos, endPos, t);
            rb2d.MovePosition(currentPos);

            yield return null;
        }

        IsLocked = false;
    }

    IEnumerator Knockback(float force)
    {
        rb2d.velocity = new Vector2(0, 0);
        rb2d.AddForce(new Vector2(-lookDirection.x, 1) * force, ForceMode2D.Impulse);

        IsLocked = true;
        do
        {
            yield return new WaitForEndOfFrame();
        } while (rb2d.velocity.x != 0 && rb2d.velocity.y != 0);

        IsLocked = false;
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

    void Debug()
    {
        GameObject debugScreen = GameObject.Find("DebugScreen");

        if (debugScreen)
        {
            DebugCanvas canvas = debugScreen.GetComponent<DebugCanvas>();

            canvas.Show(0, string.Format("Grounded: {0}", grounded ? "Yes" : "No"));
            canvas.Show(1, string.Format("Holding Down: {0}", holdDown ? "Yes" : "No"));
            canvas.Show(2, string.Format("Health: {0}", healthController.CurrentHealth));
            canvas.Show(3, string.Format("Energy: {0}", energyController.CurrentEnergy));
            canvas.Show(4, Time.time.ToString());
        }
    }
}
