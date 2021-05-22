using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    private Vector3 hitPoint = Vector3.zero;

    private void Update()
    {
        LayerMask terrain = LayerMask.GetMask("Terrain");
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, Mathf.Infinity, terrain))
        {
            hitPoint = hit.point;
        }
        else hitPoint = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 2);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hitPoint, 5);
    }
}
