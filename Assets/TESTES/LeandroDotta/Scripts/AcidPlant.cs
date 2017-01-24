using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPlant : MonoBehaviour {

    public float shootForce;
    public float shootDelay;
    public Vector2 shootDirection;
    public GameObject projectile;

    private float shootTimer;
    private Vector2 shootOrigin;
    private SpriteRenderer spriteRenderer;


	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (shootDirection.x < 0)
        {
            shootOrigin = new Vector2(spriteRenderer.bounds.min.x, spriteRenderer.bounds.center.y);
        }
        else
        {
            shootOrigin = new Vector2(spriteRenderer.bounds.max.x, spriteRenderer.bounds.center.y);
        }
    }

    void FixedUpdate()
    {
        if (shootTimer >= shootDelay)
        {
            shootTimer = 0;
            Shoot();
        }
    }
	
	void Update () {
        shootTimer += Time.deltaTime;
	}

    private void Shoot()
    {
        GameObject acidBall = Instantiate(projectile);
        acidBall.transform.position = shootOrigin;
        acidBall.SetActive(true);

        acidBall.GetComponent<Rigidbody2D>().AddForce(shootDirection * shootForce, ForceMode2D.Impulse);
    }
}
