using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticObjectHolder;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("-- HealthBarSettings --")]
    [SerializeField, Header("The Bar")]
    public SpriteRenderer Bar;
    [SerializeField, Header("The max width of the HealthBar")]
    private float healthBarMaxWidth;
    [SerializeField, Header("The height above the gameobject to put the healthbar.")]
    private float heightAboveParent;
    private float HealthBarWidth
    {
        get
        {
            if (Bar)
                return Bar.transform.localScale.x;
            else return healthBarMaxWidth;
        }
        set
        {
            if (Bar)
            {
                Bar.transform.localScale = new Vector3(value, Bar.transform.localScale.y, Bar.transform.localScale.z);
            }
        }
    }

    /// <summary>
    /// set the fill amount of the healthbar
    /// </summary>
    public void SetHealth(float health, float maxHealth)
    {
        HealthBarWidth = healthBarMaxWidth * health / maxHealth;
    }

    /// <summary>
    /// update how high the health bar is above it's parent based on heightAboveParent
    /// </summary>
    private void SetPosition()
    {
        if (gameObject.transform.parent)
            gameObject.transform.position = gameObject.transform.parent.position + Vector3.up * heightAboveParent;
    }

    private void OnValidate()
    {
        HealthBarWidth = healthBarMaxWidth;
        SetPosition();
    }

    private void Start()
    {
        //add this healthbar to the list of healthbars in theVisibility manager so that it's visibility will be updated properly according to theVisibilityManager
        theVisibilityManager.healthBars.Add(this);
        //set this healthbar to be active according to the current visible state of healthbars in theVisibilityManager
        gameObject.SetActive(theVisibilityManager.showHealthBars);
    }

    private void Update()
    {
        SetPosition();
        //this is far more performant than gameObject.transform.LookAt(Camera.main.transform);
        gameObject.transform.rotation = Camera.main.transform.rotation;
    }
}
