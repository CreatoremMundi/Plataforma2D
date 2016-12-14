using UnityEngine;
using System.Collections;

public class EnergyBall : MonoBehaviour {
    public float maxDistance;

    private Vector2 startPos;

    private DamageController damageController;

	void Start () {
        damageController = GetComponent<DamageController>();
        damageController.OnHitTarget += SelfDestroy;

        startPos = transform.position;
	}

    void Update()
    {
        float distance = Vector2.Distance(transform.position, startPos);
        
        if(distance >= maxDistance)
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
