using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer Bar;
    [SerializeField]
    private float healthBarMaxWidth;
    [SerializeField]
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

    public void SetHealth(float health, float maxHealth)
    {
        HealthBarWidth = healthBarMaxWidth * health / maxHealth;
        Bar.transform.position = new Vector3(healthBarMaxWidth * 0.5f + healthBarMaxWidth * 0.5f * health / maxHealth, Bar.transform.position.y, Bar.transform.position.z);
    }

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


    private void Update()
    {
        SetPosition();
        gameObject.transform.LookAt(Camera.main.transform);
    }
}
