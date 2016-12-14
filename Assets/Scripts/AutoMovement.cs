using UnityEngine;
using System.Collections;

public class AutoMovement : MonoBehaviour {
    public float speed;
    public Vector2 direction = Vector2.right;

    private Rigidbody2D rb2d;

	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
	}
	
    void FixedUpdate()
    {
        if (rb2d != null)
        {
            rb2d.velocity = direction * speed;
        }
    }

	void Update () {
	
	}
}
