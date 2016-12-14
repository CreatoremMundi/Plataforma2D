using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

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

    // Propriedades
    /// <summary>
    /// Indica se o objeto está invulnerável ou não.
    /// </summary>
    public bool IsInvulnerable { get; private set; }
    /// <summary>
    /// Saúde atual do objeto
    /// </summary>
    public float CurrentHealth { get; set; }

    // Eventos
    public delegate void EnterInvulnerableModeAction();
    public event EnterInvulnerableModeAction OnEnterInvulnerableMode;
    public delegate void ExitInvulnerableModeAction();
    public event ExitInvulnerableModeAction OnExitInvulnerableMode;
    public delegate void TakeDamageAction(float amount);
    public event TakeDamageAction OnTakeDamage;
    public delegate void ReachZeroHealthAction();
    public event ReachZeroHealthAction OnReachZeroHealth;

    void Start () {
        // Atribui o valor inicial para a saúde do objeto.
        if (startingHealth <= 0)
            CurrentHealth = 1;
        else if (startingHealth > maxHealth)
            CurrentHealth = maxHealth;
        else
            CurrentHealth = startingHealth;

        // Implementação padrão dos delegates
        OnEnterInvulnerableMode += () => { return; };
        OnExitInvulnerableMode += () => { return; };
        OnTakeDamage += (amount) => { return; };
        OnReachZeroHealth += () => { return; };

    }
	
	void Update () {
        if (IsInvulnerable)
        {
            // Conta o tempo em que está invulnerável
            invulnerableCounter += Time.deltaTime;

            // Ao atingir o tempo, sai do modo de invulnerabildade
            if (invulnerableCounter >= invulnerableTime)
            {
                invulnerableCounter = 0;
                IsInvulnerable = false;

                // Chama o evento de sair do modo invulnerável
                OnExitInvulnerableMode();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsInvulnerable) // Se estiver no modo invulnerável, não recebe dano.
            return;

        // Remove a quantidade de saúde passada como parâmetro
        CurrentHealth -= amount;

        // Chama o evento ao receber dano
        OnTakeDamage(amount);

        // Chama evento ao zerar a saúde do objeto
        if (CurrentHealth <= 0)
        {
            OnReachZeroHealth();
        }

        // Altera o estado para invulnerável caso seja necessário
        if (invulnerableTime > 0)
        {
            IsInvulnerable = true;

            // Chama o evento de entrar em modo invulnerável
            OnEnterInvulnerableMode();
        }
    }

    public void Add(float amount)
    {
        if (Mathf.Sign(amount) == -1)
            throw new System.Exception("O valor para ser adicionado à saúde não pode ser negativo");

        float newHealth = CurrentHealth + amount;

        if(newHealth > maxHealth)
        {
            CurrentHealth = maxHealth;
        }else
        {
            CurrentHealth = newHealth;
        }
    }
}
