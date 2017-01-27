using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(EnergyController))]
public class PlayerController : CharacterController {
    // Atributos
    public float speed;
    public float jumpForce;
    public float grabedSpeed;
    public float dodgeDistance;
    public float dodgeDuration;
    public GameObject prefabEnergyBall;
    public LayerMask groundLayer;
    public float groundCheckSize;
    public bool debugMode;

    private bool airJump = true;
    private bool gridArea = false;
    private Vector2 lookDirection;
    public AnimationClip clipAttack;

    // States
    private bool grounded;
    private bool shielding;
    private bool crouched;
    private bool grabed;

    // Components
    private Animator anim;
    private BoxCollider2D coll;
    private HealthController healthController;
    private EnergyController energyController;
    private Transform sword;
    private Transform energyBallSpawner;
    private GameObject areaAttack;
    private GameObject shield;

    public bool IsGrounded { get { return grounded; } }

    protected override void Start()
    {
        base.Start();

        // Inicia os componenetes
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        healthController = GetComponent<HealthController>();
        energyController = GetComponent<EnergyController>();
        sword = transform.FindChild("Sword");
        energyBallSpawner = transform.FindChild("EnergyBallSpawner");
        areaAttack = transform.FindChild("AreaAttack").gameObject;
        shield = transform.FindChild("Shield").gameObject;

        //clipAttack = (AnimationClip)AssetDatabase.LoadAssetAtPath("Assets/Animations/Player/PlayerAttack.anim", typeof(AnimationClip));

        healthController.OnTakeDamage += (amount) => {
            StopCoroutine("Dodge");
            IsLocked = false;
        };

        healthController.OnEnterInvulnerableMode += () =>
        {
            Blink(0.15f, healthController.invulnerableTime);
            Knockback(7, new Vector2(-lookDirection.x, 1));
            anim.SetTrigger("getHit");
        };

        healthController.OnReachZeroHealth += () =>
        {
            StartCoroutine("RestartScene");
        };
    }

    void FixedUpdate()
    {
        if (!IsLocked)
        {
            // Movimentação do personagem
            Move();

            // Último botão pressionado (para saber qual deverá ser a próxima ação)
            PlayerAction currentAction = InputManager.Instance.Triggers.lastPressed;

            // Só realiza ação se ela for permitida, já que dependendo do estado do jogador
            // algumas ações não podem ser realizada
            if (currentAction != PlayerAction.None && IsActionAllowed(currentAction))
            {
                switch (currentAction)
                {
                    case PlayerAction.Attack:
                        StartCoroutine(Attack());
                        break;


                    case PlayerAction.Grab:
                        if (gridArea)
                        {
                            StartGrab();
                        }
                        break;


                    case PlayerAction.Jump:
                        Jump();
                        break;


                    case PlayerAction.SkillAreaAttack:
                        if (energyController.Consume(50))
                        {
                            StartCoroutine(AreaAttack());
                        }
                        break;


                    case PlayerAction.SkillDodge:
                        if (energyController.Consume(10))
                        {
                            StartCoroutine("Dodge");
                        }
                        break;


                    case PlayerAction.SkillRangedAttack:
                        if (energyController.Consume(25))
                        {
                            StartCoroutine(RangeAttack());
                        }
                        break;


                    case PlayerAction.SkillShield:
                        StartCoroutine("Shield");
                        break;
                }

                InputManager.Instance.DeactivateTriggers();
            }
        }
    }

    void Update()
    {
        // Verifica se está tocando o chão (para evitar pulos enquanto estiver no ar)
        Bounds groundCheckBounds = CalculateGroundCheckBounds();
        grounded = Physics2D.BoxCast(groundCheckBounds.center, groundCheckBounds.size, 0, Vector2.right, 0, groundLayer);
        if (grounded || grabed)
            airJump = true;

        // Atualiza o estado de "abaixado"
        crouched = InputManager.Instance.Holds.down && grounded && !shielding;

        // Sai da grade/escada ao tocar no chão
        if (grabed && grounded)
            StopGrab();

        // Atualiza os estados das animações
        anim.SetFloat("velocityX", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("grounded", grounded);
        anim.SetBool("crouched", crouched);
        anim.SetBool("grabed", grabed);

        // Vira o personagem de acordo com a direção que ele está indo
        Flip();

        if (debugMode)
        {
            Debug();
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Grid"))
        {
            gridArea = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.CompareTag("Grid"))
        {
            gridArea = false;
            StopGrab();
        }
    }

    /// <summary>
    /// Atualiza o movimento do jogador nas direções permitidas.
    /// </summary>
    private void Move()
    {
        Vector2 movement = rb2d.velocity;

        if (IsActionAllowed(PlayerAction.MoveHorizontally))
            movement.x = InputManager.Instance.AxisHorizontal * (grabed ? grabedSpeed : speed);

        if (IsActionAllowed(PlayerAction.MoveVertically))
            movement.y = InputManager.Instance.AxisVertical * (grabed ? grabedSpeed : speed);

        rb2d.velocity = movement;
    }

    /// <summary>
    /// Aplica a física de pulo para o jogador
    /// </summary>
    private void Jump()
    {
        if (!grounded && !grabed && airJump)
            airJump = false;

        StopGrab();

        // Atualiza a animação
        anim.SetTrigger("jump");

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
        Vector3 scale = transform.localScale;

        if (InputManager.Instance.AxisHorizontal > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (InputManager.Instance.AxisHorizontal < 0)
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
        lookDirection = (Mathf.Sign(transform.localScale.x) == 1) ? Vector2.right : Vector2.left;
    }

    Bounds CalculateGroundCheckBounds()
    {
        Vector2 center = new Vector2(transform.position.x + coll.offset.x, coll.bounds.min.y);
        Vector2 size = new Vector2(coll.bounds.max.x - coll.bounds.min.x, groundCheckSize);

        return new Bounds(center, size);
    }


    IEnumerator Attack()
    {
        anim.SetTrigger("attack");
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        IsLocked = true;
        yield return new WaitForSeconds(clipAttack.length);
        IsLocked = false;
    }

    IEnumerator AreaAttack()
    {
        IsLocked = true;
        areaAttack.SetActive(true);
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        yield return new WaitForSeconds(0.5f);
        areaAttack.SetActive(false);
        IsLocked = false;
    }

    IEnumerator Shield()
    {
        shielding = true;
        shield.SetActive(true);
        Shield shieldController = shield.transform.FindChild("ShieldObject").GetComponent<Shield>();
        energyController.Consume(1);
        float counter = 0;
        while (InputManager.Instance.Holds.shield && energyController.CurrentEnergy > 0)
        {
            counter += Time.deltaTime;
            if(counter >= 0.2f)
            {
                counter = 0;
                energyController.Consume(1);
            }

            if (InputManager.Instance.AxisVertical > 0)
            {
                shieldController.ShieldUp();
            }
            else if (InputManager.Instance.AxisVertical < 0 && !grounded)
            {
                shieldController.ShieldDown();
            }
            else
            {
                shieldController.ShieldHorizontally();
            }

            yield return null;
        }
        shield.SetActive(false);
        shielding = false;
    }

    IEnumerator Dodge()
    {
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

    IEnumerator RangeAttack()
    {
        GameObject energyBall = Instantiate(prefabEnergyBall);
        energyBall.transform.position = energyBallSpawner.position;
        energyBall.GetComponent<AutoMovement>().direction = lookDirection;
        energyBall.SetActive(true);

        yield return null;
    }

    IEnumerator RestartScene()
    {
        IsLocked = true;
        anim.SetTrigger("died");
        
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void StartGrab()
    {
        grabed = true;
        rb2d.isKinematic = true;

        // Move o personagem um pouco para cima para que ele saia do chão. Isso é necessário pois quando 
        // ele toca o chão, ele sai do modo "Grabed"
        rb2d.MovePosition(new Vector2(rb2d.position.x, rb2d.position.y + groundCheckSize));
    }

    private void StopGrab()
    {
        grabed = false;
        rb2d.isKinematic = false;
    }

    public bool IsActionAllowed(PlayerAction action)
    {
        if (grabed)
        {
            return
                action == PlayerAction.MoveHorizontally ||
                action == PlayerAction.MoveVertically ||
                action == PlayerAction.Jump ||
                action == PlayerAction.SkillDodge;
        }
        else if (crouched)
        {
            return
                action == PlayerAction.Jump ||
                action == PlayerAction.Attack ||
                action == PlayerAction.SkillAreaAttack ||
                action == PlayerAction.SkillDodge ||
                action == PlayerAction.SkillRangedAttack ||
                action == PlayerAction.SkillShield;
        }
        else if (shielding)
        {
            return
                action == PlayerAction.Jump && (grounded || airJump);
        }
        else if (grounded)
        {
            return
                action == PlayerAction.MoveHorizontally ||
                action == PlayerAction.Crouch ||
                action == PlayerAction.Grab ||
                action == PlayerAction.Jump ||
                action == PlayerAction.Attack ||
                action == PlayerAction.SkillAreaAttack ||
                action == PlayerAction.SkillDodge ||
                action == PlayerAction.SkillRangedAttack ||
                action == PlayerAction.SkillShield;
        }
        else if (!grounded)
        {
            return
                action == PlayerAction.MoveHorizontally ||
                action == PlayerAction.Grab ||
                (action == PlayerAction.Jump && airJump) ||
                action == PlayerAction.Attack ||
                action == PlayerAction.SkillDodge ||
                action == PlayerAction.SkillRangedAttack ||
                action == PlayerAction.SkillShield;
        }

        return false;
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
            canvas.Show(1, string.Format("Crouched: {0}", crouched ? "Yes" : "No"));
            canvas.Show(2, string.Format("Health: {0}", healthController.CurrentHealth));
            canvas.Show(3, string.Format("Energy: {0}", energyController.CurrentEnergy));
            canvas.Show(4, string.Format("Tempo: {0}",Time.time.ToString()));
            canvas.Show(5, string.Format("Grid Area: {0}", gridArea ? "Yes" : "No"));
            canvas.Show(6, string.Format("Grabed: {0}", grabed ? "Yes" : "No"));
            canvas.Show(7, string.Format("Air Jump: {0}", airJump ? "Yes" : "No"));
        }
    }
}
