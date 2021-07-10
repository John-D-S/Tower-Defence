using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSlowly : MonoBehaviour
{
    [SerializeField, Tooltip("how Many degrees per second on average to rotate clockwise.")]
    private float rotationSpeed = 0.1f;
    [SerializeField, Tooltip("whether or not to apply a random y rotation when this gameobject starts")]
    bool RandomStartRotation = true;


    private void Start()
    {
        //apply the random start rotation
        if (RandomStartRotation)
        {
            transform.Rotate(Vector3.up * Random.Range(0, 360));
        }
    }

    void Update()
    {
        //rotate around the yaxis clockwise by the given amount of degrees per second
        transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
    }
}
