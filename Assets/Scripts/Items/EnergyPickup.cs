using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPickup : Pickup
{
    protected override void Regenerate(Transform target)
    {
        EnergyController energy = target.GetComponent<EnergyController>();

        if (energy != null)
        {
            energy.Add(regenarationValue);
            Destroy(this.gameObject);
        }
    }
}
