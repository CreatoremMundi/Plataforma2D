using System.Collections;
using UnityEngine;

/// <summary>
/// Representa a classe base para a criação de itens coletáveis que recuperam algum atributo do jogador.
/// </summary>
public abstract class Pickup : MonoBehaviour
{
    /// <summary>
    /// Valor que será recuperado ao coletar o objeto
    /// </summary>
    public float regenarationValue;

    /// <summary>
    /// Método deve ser implementado para aplicar a lógica que recupera o valor definido em "regenerationValue"
    /// para algum atributo do jogador, como vida ou energia.
    /// </summary>
    /// <param name="player">Objeto que representa o jogador.</param>
    protected abstract void Regenerate(Transform player);

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ao colidir com o jogador o método Regenerate é chamado.
        if(other.CompareTag("Player"))
        {
            Regenerate(other.transform);
        }
    }
}