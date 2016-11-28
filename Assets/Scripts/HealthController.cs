using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

    /// <summary>
    /// Saúde atual do objeto
    /// </summary>
    public float health;
    /// <summary>
    /// Saúde inicial do objeto
    /// </summary>
    public float startingHealth;
    /// <summary>
    /// Saúde máxima do objeto
    /// </summary>
    public float maxHealth = 100;

    /// <summary>
    /// Tempo que o objeto fica invulnerável após receber dano (em segundos).
    /// </summary>
    public float invulnerableTime;
    /// <summary>
    /// Contador de tempo de invulnerabilidade
    /// </summary>
    private float invulnerableCounter;

    /// <summary>
    /// Indica se o objeto está invulnerável ou não.
    /// </summary>
    [HideInInspector]
    public bool isInvulnerable;

    // Eventos
    public delegate void EnterInvulnerableModeAction();
    public event EnterInvulnerableModeAction OnEnterInvulnerableMode;
    public delegate void ExitInvulnerableModeAction();
    public event ExitInvulnerableModeAction OnExitInvulnerableMode;
    public delegate void TakeDamageAction(float amount);
    public event TakeDamageAction OnTakeDamage;
    public delegate void ReachZeroHealthAction();
    public event ReachZeroHealthAction OnReachZeroHealth;

    // Use this for initialization
    void Start () {
        // Atribui o valor inicial para a saúde do objeto.
        if (startingHealth <= 0)
            health = 1;
        else if (startingHealth > maxHealth)
            health = maxHealth;
        else
            health = startingHealth;
    }
	
	// Update is called once per frame
	void Update () {
        if (isInvulnerable)
        {
            // Conta o tempo em que está invulnerável
            invulnerableCounter += Time.deltaTime;

            // Ao atingir o tempo, sai do modo de invulnerabildade
            if (invulnerableCounter >= invulnerableTime)
            {
                invulnerableCounter = 0;
                isInvulnerable = false;

                // Chama o evento de sair do modo invulnerável
                OnExitInvulnerableMode();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isInvulnerable) // Se estiver no modo invulnerável, não recebe dano.
            return;

        // Remove a quantidade de saúde passada como parâmetro
        health -= amount;

        // Chama o evento ao receber dano
        OnTakeDamage(amount);

        // Chama evento ao zerar a saúde do objeto
        if (health <= 0)
        {
            OnReachZeroHealth();
        }

        // Altera o estado para invulnerável caso seja necessário
        if (invulnerableTime > 0)
        {
            isInvulnerable = true;

            // Chama o evento de entrar em modo invulnerável
            OnEnterInvulnerableMode();
        }
    }
}
