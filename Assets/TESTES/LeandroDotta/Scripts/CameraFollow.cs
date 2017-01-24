using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Bounds camBounds;

    void Start()
    {
        camBounds = GetComponent<Camera>().OrthographicBounds();
    }

    void LateUpdate()
    {
        Vector3 camPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        camPos.x = Mathf.Clamp(camPos.x, minX + camBounds.extents.x, maxX - camBounds.extents.x);
        camPos.y = Mathf.Clamp(camPos.y, minY + camBounds.extents.y, maxY - camBounds.extents.y);

        transform.position = camPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Vector2 topLeft = new Vector2(minX, maxY);
        Vector2 topRight = new Vector2(maxX, maxY);
        Vector2 bottomRight = new Vector2(maxX, minY);
        Vector2 bottomLeft = new Vector2(minX, minY);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
