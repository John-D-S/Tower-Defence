using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structure;
using TMPro;

namespace Economy
{
    public enum EconomyResource
    {
        Metal,
        Energy
    }

    public static class EconomyTracker
    {
        public static int metal;
        public static int energy;

        public static int baseMaxMetal;
        public static int baseMaxEnergy;
        
        public static List<Storage> activeMetalStorageStructures = new List<Storage>();
        public static List<Storage> activeEnergyStorageStructures = new List<Storage>();
        public static int MaxMetal
        {
            get
            {
                int returnValue = baseMaxMetal;
                foreach (Storage storageStructure in activeMetalStorageStructures)
                {
                    returnValue += storageStructure.StorageCapacity;
                }
                return returnValue;
            }
        }
        public static int MaxEnergy
        {
            get
            {
                int returnValue = baseMaxEnergy;
                foreach (Storage storageStructure in activeEnergyStorageStructures)
                {
                    returnValue += storageStructure.StorageCapacity;
                }
                return returnValue;
            }
        }


        public static bool TryIncrementResource(int incrementAmount, ref int resourceToIncrement, int maxResource)
        {
            int resultantAmount = resourceToIncrement + incrementAmount;
            if (resultantAmount < 0 || resultantAmount > maxResource)
                return false;
            else
                resourceToIncrement = resultantAmount;
            return true;
        }

        public static bool TryIncrementMetal(int _metalIncrementAmount) => TryIncrementResource(_metalIncrementAmount, ref metal, MaxMetal);
        
        public static bool TryIncrementEnergy(int _energyIncrementAmount) => TryIncrementResource(_energyIncrementAmount, ref energy, MaxEnergy);
    }

    public class EconomyManager : MonoBehaviour
    {
        [Header("-- Economy Bars --")]
        [SerializeField]
        private Image metalHudBar;
        [SerializeField]
        private TextMeshProUGUI metalHudText;
        [SerializeField]
        private Image energyHudBar;
        [SerializeField]
        private TextMeshProUGUI energyHudText;

        [Header("-- Starting Values --")]
        [SerializeField]
        private int startingMetal = 50;
        [SerializeField]
        private int startingEnergy = 50;


        [SerializeField]
        private int baseMetalPerSecond = 1;
        [SerializeField]
        private int baseEnergyPerSecond = 1;

        [Header("-- Base Resources --")]
        [SerializeField]
        private int baseMaxMetal = 100;
        [SerializeField]
        private int baseMaxEnergy = 100;

        private void InitializeBaseMaxEconomy()
        {
            EconomyTracker.baseMaxMetal = baseMaxMetal;
            EconomyTracker.baseMaxEnergy = baseMaxEnergy;
            EconomyTracker.TryIncrementMetal(startingMetal);
            EconomyTracker.TryIncrementEnergy(startingEnergy);
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
                metalHudBar.fillAmount = (float)EconomyTracker.metal / (float)EconomyTracker.MaxMetal;
                energyHudBar.fillAmount = (float)EconomyTracker.energy / (float)EconomyTracker.MaxEnergy;
            }
            if (metalHudText && energyHudText)
            {
                metalHudText.text = $"Metal: {EconomyTracker.metal} / {EconomyTracker.MaxEnergy}";
                energyHudText.text = $"Energy: {EconomyTracker.energy} / {EconomyTracker.MaxEnergy}";
            }
        }
    }
}