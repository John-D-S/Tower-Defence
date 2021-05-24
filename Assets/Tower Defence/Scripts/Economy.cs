using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [Header("-- Starting Values --")]
        [SerializeField]
        private int startingMetal = 50;
        [SerializeField]
        private int startingEnergy = 50;


        [SerializeField]
        private int baseMetalPerSecond = 1;
        [SerializeField]
        private int baseEnergyPerSecond = 1;

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
        }
    }
}
