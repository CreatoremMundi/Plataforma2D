using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthController))]
public class Enemy : MonoBehaviour {
    private HealthController healthController;

    protected virtual void Start () {
        healthController = GetComponent<HealthController>();

        healthController.OnReachZeroHealth += Die;
	}
	
	protected void Die()
    {
        Destroy(this.gameObject);
    }
}
