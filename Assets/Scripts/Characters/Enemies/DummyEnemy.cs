using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Enemy {
    public float distance;
    private Vector2 origin;
    private AutoMovement movement;

    protected override void Start()
    {
        base.Start();

        movement = GetComponent<AutoMovement>();
        origin = transform.position;
    }

    void Update()
    {
        if(transform.position.x >= (origin.x + distance))
        {
            movement.direction = Vector2.left;
        }

        if(transform.position.x <= (origin.x - distance))
        {
            movement.direction = Vector2.right;
        }
    }
}
