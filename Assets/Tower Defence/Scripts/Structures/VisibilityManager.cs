using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structures;
using static StaticObjectHolder;

public class VisibilityManager : MonoBehaviour
{
    //the variable that determines whether or not to display the health bars. when a healthbar script starts, it will set its gameobject to either active or inactive based on this
    [HideInInspector]
    public bool showHealthBars = true;
    //the list of healthbars in the world
    [HideInInspector]
    public List<HealthBar> healthBars = new List<HealthBar>();

    //the variables that determines whether or not to display the structure connection indicators
    [HideInInspector]
    public bool showStructureConnectionIndicators = true;
    //the list of all structure connection indicators in the world
    [HideInInspector]
    public List<StructureConnectionIndicator> structureConnectionIndicators = new List<StructureConnectionIndicator>();

    private void Awake()
    {
        //set the static theVisibilityManager to this gameobject (there is only ever 1)
        theVisibilityManager = this;
    }

    /// <summary>
    /// set all healthbars in the world to be active or inactive
    /// </summary>
    public void SetHealthBarsActive(bool active)
    {
        foreach (HealthBar healthBar in healthBars.ToArray())
        {
            if (healthBar != null)
                healthBar.gameObject.SetActive(active);
            else
                healthBars.Remove(healthBar);
        }
        showHealthBars = active;
    }

    /// <summary>
    /// set all structureConnectionIndicators in the world to be active or inactive
    /// </summary>
    public void SetStructureConnectionIndicatorsActive(bool active)
    {
        foreach (StructureConnectionIndicator connectionIndicator in structureConnectionIndicators.ToArray())
        {
            if (connectionIndicator != null)
                connectionIndicator.gameObject.SetActive(active);
            else
                structureConnectionIndicators.Remove(connectionIndicator);
        }
        showStructureConnectionIndicators = active;
    }
}
