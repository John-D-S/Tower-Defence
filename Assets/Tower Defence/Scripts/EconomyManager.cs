using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Economy
{
    public static class EconomyTracker
    {
        public static int metal;
        public static int energy;

        public static int maxMetal;
        public static int maxEnergy;

        public static bool TryIncrementResource(int incrementAmount, ref int resourceToIncrement, int maxResource)
        {
            int resultantAmount = resourceToIncrement + incrementAmount;
            if (resultantAmount < 0 || resultantAmount > maxResource)
                return false;
            else
                resourceToIncrement = resultantAmount;
            return true;
        }

        public static bool TryIncrementMetal(int _metalIncrementAmount) => TryIncrementResource(_metalIncrementAmount, ref metal, maxMetal);
        
        public static bool TryIncrementEnergy(int _energyIncrementAmount) => TryIncrementResource(_energyIncrementAmount, ref energy, maxEnergy);
    }

    public class EconomyManager : MonoBehaviour
    {
        [Header("-- Economy Bars --")]
        [SerializeField]
        private Image metalHudBar;
        [SerializeField]
        private Image energyHudBar;


        [Header("-- Starting Values --")]
        [SerializeField]
        private int startingMetal = 50;
        [SerializeField]
        private int startingEnergy = 50;


        [SerializeField]
        private int baseMetalPerSecond = 1;
        [SerializeField]
        private int baseEnergyPerSecond = 1;


        [SerializeField]
        private int baseMaxMetal = 100;
        [SerializeField]
        private int baseMaxEnergy = 100;

        private void InitializeBaseMaxEconomy()
        {
            EconomyTracker.maxMetal = baseMaxMetal;
            EconomyTracker.maxEnergy = baseMaxEnergy;
        }

        private IEnumerator BaseGenerate()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                EconomyTracker.TryIncrementMetal(baseMetalPerSecond);
                EconomyTracker.TryIncrementEnergy(baseEnergyPerSecond);
            }
        }

        private void Start()
        {
            StartCoroutine(BaseGenerate());
            InitializeBaseMaxEconomy();
        }

        private void Update()
        {
            if (metalHudBar.sprite && energyHudBar.sprite)
            {
                metalHudBar.fillAmount = (float)EconomyTracker.metal / (float)EconomyTracker.maxMetal;
                energyHudBar.fillAmount = (float)EconomyTracker.energy / (float)EconomyTracker.maxEnergy;
            }
        }
    }
}
