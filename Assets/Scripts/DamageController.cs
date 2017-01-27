using UnityEngine;
using System.Linq;

public class DamageController : MonoBehaviour {
    public float damage;

    [HideInInspector]
    public int targetTagsMask;
    [HideInInspector]
    public string[] targetTags;

    // Eventos
    public delegate void HitTargetAction();
    public event HitTargetAction OnHitTarget;

    void Start()
    {
        OnHitTarget += () => { return; };
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (targetTags.Contains(other.tag))
        {
            HealthController health = other.GetComponent<HealthController>();
            if(health != null)
            {
                health.TakeDamage(damage);
            }

            OnHitTarget();
        }
    }
}