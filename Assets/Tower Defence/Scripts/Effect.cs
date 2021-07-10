using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField, Tooltip("How many seconds the effect lasts for.")]
    private float effectDuration;

    /// <summary>
    /// will destroy this gameobject after effectDuration seconds
    /// </summary>
    private IEnumerator destroySelfAfterDuration()
    {
        yield return new WaitForSeconds(effectDuration);
        Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(destroySelfAfterDuration());
    }
}
