using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
    private Vector2 defaultPosition;
    private Vector2 positionUp;
    private Vector2 positionDown;

    private Vector3 defaultRotation;
    private Vector3 rotationUp;
    private Vector3 rotationDown;

    void Awake()
    {
        Transform shieldUp = transform.parent.FindChild("ShieldUpPosition");
        Transform shieldDown = transform.parent.FindChild("ShieldDownPosition");

        defaultPosition = transform.localPosition;
        defaultRotation = transform.localRotation.eulerAngles;

        positionUp = shieldUp.localPosition;
        rotationUp = shieldUp.localRotation.eulerAngles;

        positionDown = shieldDown.localPosition;
        rotationDown = shieldDown.localRotation.eulerAngles;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
        }
    }

    public void ShieldUp()
    {
        transform.localPosition = positionUp;
        transform.localRotation = Quaternion.Euler(rotationUp);
    }

    public void ShieldDown()
    {
        transform.localPosition = positionDown;
        transform.localRotation = Quaternion.Euler(rotationDown);
    }

    public void ShieldHorizontally()
    {
        transform.localPosition = defaultPosition;
        transform.localRotation = Quaternion.Euler(defaultRotation);
    }
}
