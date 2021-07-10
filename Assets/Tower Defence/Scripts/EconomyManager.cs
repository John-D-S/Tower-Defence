using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structures;
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

        //the starting max amount of metal.
        public static int baseMaxMetal;
        //the starting max amount of energy.
        public static int baseMaxEnergy;
        
        //a list of all metal storage structures in the scene
        public static List<Storage> activeMetalStorageStructures = new List<Storage>();
        //a list of all energy storage structures in the scene
        public static List<Storage> activeEnergyStorageStructures = new List<Storage>();
        /// <summary>
        /// returns the sum of all max metals in active energy storageStructures.
        /// </summary>
        public static int MaxMetal
        {
            get
            {
                int returnValue = baseMaxMetal;
                foreach (Storage storageStructure in activeMetalStorageStructures)
                {
                    if (storageStructure.CanFunction)
                        returnValue += storageStructure.StorageCapacity;
                }
                return returnValue;
            }
        }
        /// <summary>
        /// returns the sum of all max metals in active energy storageStructures.
        /// </summary>
        public static int MaxEnergy
        {
            get
            {
                int returnValue = baseMaxEnergy;
                foreach (Storage storageStructure in activeEnergyStorageStructures)
                {
                    if (storageStructure.CanFunction)
                        returnValue += storageStructure.StorageCapacity;
                }
                return returnValue;
            }
        }

        /// <summary>
        /// increments the resource up or down by a certain amount and returns whether or not the resource was changed.
        /// </summary>
        /// <param name="incrementAmount">The amount to increment the resource by</param>
        /// <param name="resourceToIncrement">The resource to increment</param>
        /// <param name="maxResource">the maximum amount of said resource</param>
        public static bool TryIncrementResource(int incrementAmount, ref int resourceToIncrement, int maxResource)
        {
            //the current amount of the given resorce plus the amount to increment it by
            int resultantAmount = resourceToIncrement + incrementAmount;
            //if the current amount is less than the max resource and the resultant amount is greater than it, set the resource to the maxResource and return true
            if (resourceToIncrement < maxResource && resultantAmount >= maxResource)
            {
                resourceToIncrement = maxResource;
                return true;
            }//if the resultant amount is greater than or equal to 0 and the resultant amount is less than the max resource then set the resource to increment to resultantAmount
            else if (resultantAmount >= 0 && resultantAmount < maxResource)
            {
                resourceToIncrement = resultantAmount;
                return true;
            }//if the resultant amount is greater than the max resource and neither of the above conditions were met, just set resourceToIncrement to maxResource and return false.
            if (resultantAmount > maxResource)
                resourceToIncrement = maxResource;
            return false;
        }

        /// <summary>
        /// increments metal by _metalIncrementAmount
        /// </summary>
        public static bool TryIncrementMetal(int _metalIncrementAmount) => TryIncrementResource(_metalIncrementAmount, ref metal, MaxMetal);
        /// <summary>
        /// increments metal by _metalIncrementAmount
        /// </summary>        
        public static bool TryIncrementEnergy(int _energyIncrementAmount) => TryIncrementResource(_energyIncrementAmount, ref energy, MaxEnergy);
    }

    public class EconomyManager : MonoBehaviour
    {
        [Header("-- Economy Bars --")]
        [SerializeField, Tooltip("The HUD bar that visually displays the amount of metal stored")]
        private Image metalHudBar;
        [SerializeField, Tooltip("The text that displays the amount of metal stored")]
        private TextMeshProUGUI metalHudText;
        [SerializeField, Tooltip("The HUD bar that visually displays the amount of energy stored")]
        private Image energyHudBar;
        [SerializeField, Tooltip("The text that displays the amount of energy stored")]
        private TextMeshProUGUI energyHudText;

        [Header("-- Starting Values --")]
        [SerializeField, Tooltip("The amount of metal the player starts off with.")]
        private int startingMetal = 50;
        [SerializeField, Tooltip("The amount of energy the player starts off with.")]
        private int startingEnergy = 50;


        [SerializeField, Tooltip("The amount of Metal that is generated per second by default")]
        private int baseMetalPerSecond = 1;
        [SerializeField, Tooltip("The amount of energy that is generated per second by default")]
        private int baseEnergyPerSecond = 1;

        [Header("-- Base Resources --")]
        [SerializeField, Tooltip("The base amount of max metal storage the player has to start out with.")]
        private int baseMaxMetal = 100;
        [SerializeField, Tooltip("The base amount of max energy storage the player has to start out with.")]
        private int baseMaxEnergy = 100;

        /// <summary>
        /// Sets all the base/default economy values
        /// </summary>
        private void InitializeBaseMaxEconomy()
        {
            EconomyTracker.baseMaxMetal = baseMaxMetal;
            EconomyTracker.baseMaxEnergy = baseMaxEnergy;
            EconomyTracker.TryIncrementMetal(startingMetal);
            EconomyTracker.TryIncrementEnergy(startingEnergy);
        }

        //generate the base generation amount of eac resource each second
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
            //start the baseGenerateCoroutine so that the player can afford starting towers and initialise the economy variables
            StartCoroutine(BaseGenerate());
            InitializeBaseMaxEconomy();
        }

        private void Update()
        {
            //set how full the economy display bars are
            if (metalHudBar.sprite && energyHudBar.sprite)
            {
                metalHudBar.fillAmount = (float)EconomyTracker.metal / (float)EconomyTracker.MaxMetal;
                energyHudBar.fillAmount = (float)EconomyTracker.energy / (float)EconomyTracker.MaxEnergy;
            }
            //set what the economy display texts show.
            if (metalHudText && energyHudText)
            {
                metalHudText.text = $"Metal: {EconomyTracker.metal} / {EconomyTracker.MaxMetal}";
                energyHudText.text = $"Energy: {EconomyTracker.energy} / {EconomyTracker.MaxEnergy}";
            }
        }
    }
}