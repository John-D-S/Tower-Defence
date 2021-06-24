using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Economy;

namespace Structure
{
    public class Generator : Structure
    {
        [Header("-- Generation Settings --")]
        [SerializeField]
        private EconomyResource generatedResource;
        [SerializeField]
        private int resoucesGeneratedPerMinute;

        bool canGenerateResource = true;

        private float SecondsToGenerateResource
        {
            get => (1 / resoucesGeneratedPerMinute) * 60f;
        }

        void TryGenerateResource()
        {
            if (canGenerateResource)
            {
                StartCoroutine(GenerateResource());
            }
            Debug.Log(SecondsToGenerateResource);
        }

        private IEnumerator GenerateResource()
        {
            canGenerateResource = false;
            Debug.Log($"Tried to Generate {generatedResource}");
            if (generatedResource == EconomyResource.Energy)
            {
                EconomyTracker.TryIncrementEnergy(1);
            }
            else
            {
                EconomyTracker.TryIncrementMetal(1);
            }
            yield return new WaitForSeconds(SecondsToGenerateResource);
            canGenerateResource = true;
        }

        void FixedUpdate()
        {
            if (!Preview)
            {
                TryGenerateResource();
            }
        }
    }
}

