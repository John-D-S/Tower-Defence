using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSlowly : MonoBehaviour
{
    [SerializeField, Tooltip("how Many degrees per second on average to rotate clockwise.")]
    private float rotationSpeed = 0.1f;

    private void Start()
    {
        transform.Rotate(Vector3.up * Random.Range(0, 360));
    }

    void Update()
    {
        transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
    }
}
