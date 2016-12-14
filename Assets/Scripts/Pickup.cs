using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pickup : MonoBehaviour 
   
{
    public float valor;
  
    

    public void OnDestroy()
    {
            
        Destroy(this.gameObject);

}

    [CustomEditor(typeof(Pickup))]
    public class PickupEditor : Editor
    {
        private Pickup objeto;
        private List<string> tags;

        void OnEnable()
        {
            objeto = (Pickup)target;
        }


    }
    }
    




    // Use this for initialization


// Update is called once per frame


