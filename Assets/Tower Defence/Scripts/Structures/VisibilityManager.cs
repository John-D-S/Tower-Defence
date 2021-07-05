using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structure;
using static StaticObjectHolder;

public class VisibilityManager : MonoBehaviour
{
    public bool showHealthBars = true;
    public List<HealthBar> healthBars = new List<HealthBar>();

    public bool showStructureConnectionIndicators = true;
    public List<StructureConnectionIndicator> structureConnectionIndicators = new List<StructureConnectionIndicator>();

    private void Awake()
    {
        theVisibilityManager = this;
    }

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
