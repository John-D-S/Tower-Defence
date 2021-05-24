using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public static class EconomyTracker
    {
        public static int metal;
        public static int energy;
    }

    public class EconomyManager : MonoBehaviour
    {
        [Header("-- Starting Values --")]
        [SerializeField]
        private int startingMetal = 100;
        [SerializeField]
        private int startingEnergy = 100;


        [SerializeField]
        private int baseMetalPerSecond = 1;
        [SerializeField]
        private int baseEnergyPerSecond = 1;

        private IEnumerator BaseGenerate()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                EconomyTracker.metal += baseMetalPerSecond;
                EconomyTracker.energy += baseEnergyPerSecond;
            }
        }

        private void Start()
        {

            StartCoroutine(BaseGenerate());
        }
    }
}
