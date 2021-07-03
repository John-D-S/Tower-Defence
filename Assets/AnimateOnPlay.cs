using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnPlay : MonoBehaviour
{
    private Animation coreAnimation;
    
    void Start()
    {
        coreAnimation = GetComponent<Animation>();
        coreAnimation.Play();
    }
}
