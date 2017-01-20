using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterController : MonoBehaviour
{
    private bool isBlinking;

    // Components
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb2d;

    // Properties
    public bool IsLocked { get; protected set; }

    // Events
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    ///     Faz o componente "SpriteRenderer" piscar durante um período de tempo
    /// </summary>
    /// <param name="delayBetweenBlinks">Intervalo de tempo entre as piscadas (em segundos)</param>
    /// <param name="duration">Duração do efeito (em segundos)</param>
    protected void Blink(float delayBetweenBlinks, float duration)
    {
        if (!isBlinking)
            StartCoroutine(BlinkCoroutine(delayBetweenBlinks, duration));
    }

    /// <summary>
    ///     Lança o personagem em uma direção através de um impulso no sistema de física.
    /// </summary>
    /// <param name="force">Força do Impulso</param>
    /// <param name="direction">Direção do inpulso</param>
    protected void Knockback(float force, Vector2 direction)
    {
        StartCoroutine(KnockbackCoroutine(force, direction));
    }

    #region Coroutines
    private IEnumerator BlinkCoroutine(float delayBetweenBlinks, float duration)
    {
        // Atualiza o estado para que não seja possível chamar a rotina enquanto já estiver sendo executada
        isBlinking = true;

        float counter = 0;
        bool toggleColor = false;
        Color defaultColor = spriteRenderer.color;

        // Irá piscar enquanto o contador não atingir o tempo máximo de duração
        while (counter <= duration)
        {
            // Alterna entre a cor padrão do componenete e transparente
            spriteRenderer.color = toggleColor ? Color.clear : defaultColor;
            toggleColor = !toggleColor;

            yield return new WaitForSeconds(delayBetweenBlinks);
            counter += delayBetweenBlinks + Time.deltaTime;
        }

        spriteRenderer.color = defaultColor;

        // Atualiza o estado.
        isBlinking = false;
    }

    IEnumerator KnockbackCoroutine(float force, Vector2 direction)
    {
        IsLocked = true;

        rb2d.velocity = new Vector2(0, 0);
        rb2d.AddForce(direction * force, ForceMode2D.Impulse);

        do
        {
            yield return new WaitForEndOfFrame();
        } while (rb2d.velocity.x != 0 && rb2d.velocity.y != 0);

        IsLocked = false;
    }
    #endregion
}
