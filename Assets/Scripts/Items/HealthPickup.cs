using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    protected override void Regenerate(Transform player)
    {
        HealthController health = player.GetComponent<HealthController>();

        if (health != null)
        {
            health.Add(regenarationValue);
            Destroy(this.gameObject);
        }
    }
}
