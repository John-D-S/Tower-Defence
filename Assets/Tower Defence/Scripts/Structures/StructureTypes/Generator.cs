using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Economy;

namespace Structure
{
    public class Generator : Structure
    {
        [SerializeField]
        private EconomyResource generatedResource;
        [SerializeField]
        private int resoucesGeneratedPerMinute;

        bool canGenerateResource = true;

        private float SecondsToGenerateResource
        {
            get => resoucesGeneratedPerMinute * 0.0166666f;
        }

        void TryGenerateResource()
        {
            if (canGenerateResource)
            {
                StartCoroutine(GenerateResource());
            }
        }

        private IEnumerator GenerateResource()
        {
            canGenerateResource = false;
            if (generatedResource == EconomyResource.Energy)
            {
                EconomyTracker.TryIncrementEnergy(resoucesGeneratedPerMinute);
            }
            else
            {
                EconomyTracker.TryIncrementMetal(resoucesGeneratedPerMinute);
            }
            yield return new WaitForSeconds(SecondsToGenerateResource);
        }

        void FixedUpdate()
        {
            TryGenerateResource();
        }
    }
}

