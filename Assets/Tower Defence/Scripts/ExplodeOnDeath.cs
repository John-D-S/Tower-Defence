using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structures;
using static StaticObjectHolder;

public class ExplodeOnDeath : MonoBehaviour
{
    [Header("-- Explosion Settings --")]
    [SerializeField, Tooltip("How big the explosion should be")]
    private float explosionSize;
    [SerializeField, Tooltip("What GameObject to spawn that contains the explosion effect")]
    private GameObject explosionEffect;
    //this is to make sure preview structures do not explode.
    [SerializeField, HideInInspector]
    private Structure structureComponent;

    private void OnValidate()
    {
        //if the structure component exists on this gameobject, set it 
        Structure component = GetComponent<Structure>();
        if (component)
        {
            structureComponent = component;
        }
    }

    private void OnDestroy()
    {
        //only explode if there is no structure component or if the structure component is in a preview state.
        if (!structureComponent || !structureComponent.Preview)
        {
            GameObject instantiatedExplosion = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
            instantiatedExplosion.transform.localScale = Vector3.one * explosionSize;
        }
    }
}
