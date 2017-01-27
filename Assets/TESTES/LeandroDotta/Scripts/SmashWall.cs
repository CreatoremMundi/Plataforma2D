using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmashWall : MonoBehaviour {

    public float fallSpeed;
    public float fallDelay;
    public float retreatSpeed;
    public float retreatDelay;
    public float startDelay;
    public float distance;

    private bool isDown;
    private float timer;
    public Vector2 direction;
    private Vector3 startPosition;
    private Vector3 endPosition;

    public bool IsMovingDown { get { return direction == Vector2.down; } }

    void Start()
    {
        startPosition = transform.position;
        endPosition = new Vector2(transform.position.x, transform.position.y - distance);
    }

    void Update()
    {
        if (startDelay >= 0)
        {
            // Aguarda um tempo para iniciar
            startDelay -= Time.deltaTime;
            return;
        }
        else
        {
            // Contador para o delay dos movimentos
            if (direction == Vector2.zero)
                timer += Time.deltaTime;

            // Atualiza a direção que o obstáculo vai se mover
            if (isDown && timer >= retreatDelay)
            {
                timer = 0;
                direction = Vector2.up;
            }

            if (!isDown && timer >= fallDelay)
            {
                timer = 0;
                direction = Vector2.down;
            }

            // Realiza a movimentação do obstáculo
            if (direction == Vector2.down)
            {
                transform.Translate(direction * fallSpeed * Time.deltaTime);
                transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, endPosition.y, startPosition.y));
            }
            else if (direction == Vector2.up)
            {
                transform.Translate(direction * retreatSpeed * Time.deltaTime);
                transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, endPosition.y, startPosition.y));
            }

            // Para a movimentação e atualiza a direção ao chegar no destino
            if (transform.position == startPosition || transform.position == endPosition)
            {
                direction = Vector2.zero;
                isDown = transform.position == endPosition;
            }
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("Player"))
        {
            PlayerController player = coll.collider.GetComponent<PlayerController>();

            if (IsMovingDown && player.IsGrounded)
                // TODO: Ao invés de reiniciar a cena, será necessário ativar a função de "Morrer" do "PlayerController"
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isEditor)
        {
            Collider2D coll = GetComponent<Collider2D>();

            Gizmos.color = Color.yellow;

            Gizmos.DrawLine(
                new Vector2(coll.bounds.min.x, coll.bounds.min.y - distance),
                new Vector2(coll.bounds.max.x, coll.bounds.min.y - distance)
            );
        }
    }
}
