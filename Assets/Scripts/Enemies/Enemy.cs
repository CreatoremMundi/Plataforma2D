using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthController))]
public class Enemy : MonoBehaviour {
    private HealthController healthController;

    void Start () {
        healthController = GetComponent<HealthController>();

        healthController.OnReachZeroHealth += Die;
	}
	
	private void Die()
    {
        Destroy(this.gameObject);
    }
}
