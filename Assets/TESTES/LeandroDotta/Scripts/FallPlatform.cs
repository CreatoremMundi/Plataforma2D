using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlatform : MonoBehaviour {
    public float fallDelay;
    public float rebuildDelay;

    private bool inProcess;
    private Collider2D coll;
    private Animator anim;

    void Start()
    {
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("Player") && !inProcess)
        {
            StartCoroutine(Break());
        }
    }

    private IEnumerator Break()
    {
        inProcess = true;

        // Desabilita o colisor e iniciar a animação após o tempo para cair
        yield return new WaitForSeconds(fallDelay);
        coll.enabled = false;
        anim.SetTrigger("fall");

        // Habilita o colisor e exibe novamente a animação inicial após o tempo de retorno
        yield return new WaitForSeconds(rebuildDelay);
        coll.enabled = true;
        anim.SetTrigger("normal");

        inProcess = false;
    }
}
