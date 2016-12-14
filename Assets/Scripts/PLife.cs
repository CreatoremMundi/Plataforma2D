using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;


public class PLife : Pickup
{
    

    void OnTriggerEnter2D(Collider2D Player)


    {

        
        
        HealthController health = Player.GetComponent<HealthController>();
        health.Add(valor);
      
      
        base.OnDestroy();

 



    }


    

}




// Use this for initialization


// Update is called once per frame