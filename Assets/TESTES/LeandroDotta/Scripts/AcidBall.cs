using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageController))]
public class AcidBall : MonoBehaviour {

    private Rigidbody2D rb2d;
    private DamageController damageController;
	
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();

        damageController = GetComponent<DamageController>();
        damageController.OnHitTarget += SelfDestroy;
    }
	
	void Update () {
        transform.right = rb2d.velocity.normalized;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            SelfDestroy();
        }
    }

    private void SelfDestroy()
    {
        //TODO Adicionar um delay ao destruir o objeto e exibir uma animação.
        Destroy(this.gameObject);
    }
}
