using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField]
    private float effectDuration;

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
