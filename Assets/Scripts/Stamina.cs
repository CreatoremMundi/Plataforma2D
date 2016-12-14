using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;


public class Stamina : Pickup
{


    void OnTriggerEnter2D(Collider2D Player)


    {



        EnergyController energy = Player.GetComponent<EnergyController>();

        energy.Add(valor);

        base.OnDestroy();
    }



}