using System.Collections;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public float regenarationValue;

    protected abstract void Regenerate(Transform target);

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Regenerate(other.transform);
        }
    }
}