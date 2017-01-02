/// <summary>
/// Representa as ações existente para o jogador. Cada item representa uma ação.
/// </summary>
public enum PlayerAction
{
    /// <summary>
    /// Nenhuma ação
    /// </summary>
    None,

    /// <summary>
    /// Movimentação Horizontal
    /// </summary>
    MoveHorizontally,

    /// <summary>
    /// Movimentação Vertical
    /// </summary>
    MoveVertically,

    /// <summary>
    /// Ataque comum
    /// </summary>
    Attack,

    /// <summary>
    /// Ação para abaixar-se
    /// </summary>
    Crouch,

    /// <summary>
    /// Ação para agarrar-se a uma estrutura (escada, corda etc).
    /// </summary>
    Grab,

    /// <summary>
    /// Ação de salto
    /// </summary>
    Jump,

    /// <summary>
    /// Habilidade de ataque em área.
    /// </summary>
    SkillAreaAttack,

    /// <summary>
    /// Habilidade de esquiva.
    /// </summary>
    SkillDodge,

    /// <summary>
    /// Habilidade de ataque à distância.
    /// </summary>
    SkillRangedAttack,

    /// <summary>
    /// Habilidade de escudo.
    /// </summary>
    SkillShield
}