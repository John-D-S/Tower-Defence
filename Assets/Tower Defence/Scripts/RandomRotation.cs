using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    void Start()
    {
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(currentEulerAngles.x, Random.Range(-180, 180), currentEulerAngles.z);
    }
}
