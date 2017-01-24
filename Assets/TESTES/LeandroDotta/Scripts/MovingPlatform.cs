using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    public float speed;
    public float distance;
    [Range(-1,1, order = 2)]
    public int direction = -1;
    private Vector3 originPos;
    private Vector3 minPos;
    private Vector3 maxPos;
    private Collider2D platformColl;

    private Vector2 lastPosition;
    private Transform player;

    void Start()
    {
        originPos = transform.position;
        UpdatePositions();

        platformColl = GetComponent<Collider2D>();

        lastPosition = transform.position;
    }

    void Update()
    {
        UpdatePositions();

        transform.Translate(new Vector3(direction * speed * Time.deltaTime, 0));
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x), transform.position.y);

        Vector2 moved = new Vector2(transform.position.x - lastPosition.x, transform.position.y - lastPosition.y);

        if(player != null)
        {
            player.Translate(moved);
        }

        lastPosition = transform.position;

        if(transform.position == minPos || transform.position == maxPos)
        {
            direction = -direction;
        }


    }

    void OnCollisionStay2D(Collision2D coll)
    {
        // indica se o contato entre as colisões acontece na parte de cima da plataforma
        // para garantir que o jogador não fique enroscado ao tocar nas laterais ou em baixo.
        bool isTop = false;

        Vector2 contactPoint = coll.contacts[0].point;
        Bounds platformBounds = platformColl.bounds;

        isTop = contactPoint.y >= platformBounds.max.y && // está acima da plataforma (eixo y)
                contactPoint.x >= platformBounds.min.x && contactPoint.x <= platformBounds.max.x; // está entre os limites da plataforma (no eixo x)

        if (isTop)
        {
            player = coll.collider.transform;
        }else
        {
            player = null;
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        player = null;
    }

    private void UpdatePositions()
    {
        minPos = new Vector2(originPos.x - distance, originPos.y);
        maxPos = new Vector2(originPos.x + distance, originPos.y);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Bounds collBounds = GetComponent<Collider2D>().bounds;

        Vector2 topLeft = Vector2.zero;
        Vector2 topRight = Vector2.zero;
        Vector2 bottomLeft = Vector2.zero;
        Vector2 bottomRight = Vector2.zero;

        if (Application.isPlaying)
        {
            topLeft = new Vector2(minPos.x - collBounds.extents.x, collBounds.max.y);
            topRight = new Vector2(maxPos.x + collBounds.extents.x, collBounds.max.y);
            bottomLeft = new Vector2(minPos.x - collBounds.extents.x, collBounds.min.y);
            bottomRight = new Vector2(maxPos.x + collBounds.extents.x, collBounds.min.y);
        }
        else if (Application.isEditor)
        {
            topLeft = new Vector2(collBounds.min.x - distance, collBounds.max.y);
            topRight = new Vector2(collBounds.max.x + distance, collBounds.max.y);
            bottomLeft = new Vector2(collBounds.min.x - distance, collBounds.min.y);
            bottomRight = new Vector2(collBounds.max.x + distance, collBounds.min.y);
        }

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
