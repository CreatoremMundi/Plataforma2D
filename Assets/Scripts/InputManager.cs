using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estrutura para armazenar os botões que acabaram de ser pressionados (triggers)
/// </summary>
public struct InputTrigger
{
    public bool attack;
    public bool jump;
    public bool up;
    public bool down;

    public bool areaAttack;
    public bool dodge;
    public bool rangedAttack;
    public bool shield;

    public PlayerAction lastPressed;
}

/// <summary>
/// Estrutura para armazenar os botões que estão sendo mantidos pressionados
/// </summary>
public struct InputHold
{
    public bool shield;
    public bool down;
}

public class InputManager : MonoBehaviour {
    private static InputManager _instance;

    // Tempo (em segundos) que uma trigger fica ativa após pressionada
    float deactivationTime = 0.2f;
    // Contador de tempo para desativar a trigger
    float deactivationCounter = 0;

    private InputTrigger _triggers;
    private InputHold _holds;

    /// <summary>
    /// Valor do eixo Horizontal pressionado
    /// </summary>
    public float AxisHorizontal { get; private set; }
    /// <summary>
    /// Valor do eixo Vertical pressionado
    /// </summary>
    public float AxisVertical { get; private set; }
    /// <summary>
    /// Gatilhos que foram pressionados na útlima iteração
    /// </summary>
    public InputTrigger Triggers { get { return _triggers; } }
    /// <summary>
    /// Botões que estão sendo mantidos pressionados
    /// </summary>
    public InputHold Holds { get { return _holds; } }
    /// <summary>
    /// Instância do gerenciador de entradas.
    /// </summary>
    public static InputManager Instance { get { return _instance; } }

    void Awake()
    {
        // Inicia a instancia estática da classe
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // Valor padrão para a última tecla pressionada
        _triggers.lastPressed = PlayerAction.None;
    }

    void Update()
    {
        deactivationCounter += Time.deltaTime;

        // Desativa os gatilhos ao atingir o valor do contador
        if (deactivationCounter >= deactivationTime)
            DeactivateTriggers();

        // Valores dos direcionais
        AxisHorizontal = Input.GetAxis("Horizontal");
        AxisVertical = Input.GetAxis("Vertical");

        // Triggers
        if (Input.GetButtonDown("Attack"))
        {
            _triggers.attack = true;
            _triggers.lastPressed = PlayerAction.Attack;
            deactivationCounter = 0;
        }
            
        if (Input.GetButtonDown("Jump"))
        {
            _triggers.jump = true;
            _triggers.lastPressed = PlayerAction.Jump;
            deactivationCounter = 0;
        }

        if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") == 1)
        {
            _triggers.up = true;
            _triggers.lastPressed = PlayerAction.Grab;
            deactivationCounter = 0;
        }

        if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") == -1)
        {
            _triggers.down = true;
            _triggers.lastPressed = PlayerAction.Crouch;
            deactivationCounter = 0;
        }

        if (Input.GetButtonDown("Skill1"))
        {
            _triggers.dodge = true;
            _triggers.lastPressed = PlayerAction.SkillDodge;
            deactivationCounter = 0;
        }
            
        if (Input.GetButtonDown("Skill2"))
        {
            _triggers.rangedAttack = true;
            _triggers.lastPressed = PlayerAction.SkillRangedAttack;
            deactivationCounter = 0;
        }
            
        if (Input.GetButtonDown("Skill3"))
        {
            _triggers.areaAttack = true;
            _triggers.lastPressed = PlayerAction.SkillAreaAttack;
            deactivationCounter = 0;
        }

        if (Input.GetButtonDown("Skill4"))
        {
            _triggers.shield = true;
            _triggers.lastPressed = PlayerAction.SkillShield;
            deactivationCounter = 0;
        }
            
        // Holding Buttons 
        _holds.shield = Input.GetButton("Skill4");
        _holds.down = Input.GetAxisRaw("Vertical") == -1;
    }

    /// <summary>
    /// Desativa todos os botões de gatilho pressionados
    /// </summary>
    public void DeactivateTriggers()
    {
        _triggers.attack = false;
        _triggers.jump = false;
        _triggers.up = false;
        _triggers.down = false;
        _triggers.areaAttack = false;
        _triggers.dodge = false;
        _triggers.rangedAttack = false;
        _triggers.shield = false;

        _triggers.lastPressed = PlayerAction.None;
    }
}