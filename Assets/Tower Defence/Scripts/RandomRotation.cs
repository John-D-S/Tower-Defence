using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    // apply a random y rotation to this gameObject on start
    void Start()
    {
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(currentEulerAngles.x, Random.Range(-180, 180), currentEulerAngles.z);
    }
}
