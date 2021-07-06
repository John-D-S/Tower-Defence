using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structures;

public class ExplodeOnDeath : MonoBehaviour
{
    [SerializeField]
    private float explosionSize;
    [SerializeField]
    private GameObject explosionEffect;
    [SerializeField, HideInInspector]
    private Structure structureComponent;

    private void OnValidate()
    {
        Structure component = GetComponent<Structure>();
        if (component)
        {
            structureComponent = component;
        }
    }

    private void OnDestroy()
    {
        if (!structureComponent || !structureComponent.Preview)
        {
            GameObject instantiatedExplosion = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
            instantiatedExplosion.transform.localScale = Vector3.one * explosionSize;
        }
    }
}
