using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Economy;

namespace Structures
{
    public class Generator : Structure
    {
        [Header("-- Generation Settings --")]
        [SerializeField]
        private EconomyResource generatedResource;
        [SerializeField]
        private int numberOfResourcesGenerated = 2;
        [SerializeField]
        private int secondsToGenerateResource = 10;
        [Header("-- Consumption Settings --")]
        [SerializeField]
        private bool consumeResource;
        [SerializeField]
        private int numberOfResourcesToConsume = 1;
        private int numberOfStoredResourcesToBeConsumed = 0;

        bool canGenerateResource = true;

        void TryGenerateResource()
        {
            if (canGenerateResource && CanFunction)
            {
                StartCoroutine(GenerateResource());
            }
        }

        private IEnumerator GenerateResource()
        {
            canGenerateResource = false;
            bool alreadyAtMaxResource = generatedResource == EconomyResource.Energy ? EconomyTracker.energy >= EconomyTracker.MaxEnergy : EconomyTracker.metal >= EconomyTracker.MaxMetal;
            if (consumeResource && !alreadyAtMaxResource)
            {
                if (generatedResource == EconomyResource.Energy)
                    if (EconomyTracker.TryIncrementMetal(-numberOfResourcesToConsume))
                        numberOfStoredResourcesToBeConsumed = numberOfResourcesToConsume;
                else
                    if (EconomyTracker.TryIncrementEnergy(-numberOfResourcesToConsume))
                        numberOfStoredResourcesToBeConsumed = numberOfResourcesToConsume;
            }
            if (!consumeResource || numberOfStoredResourcesToBeConsumed == numberOfResourcesToConsume)
            {
                if (generatedResource == EconomyResource.Energy)
                {
                    if (EconomyTracker.TryIncrementEnergy(numberOfResourcesGenerated))
                    {
                        numberOfStoredResourcesToBeConsumed = 0;
                    }
                }
                else
                {
                    if (EconomyTracker.TryIncrementMetal(numberOfResourcesGenerated))
                    {
                        numberOfStoredResourcesToBeConsumed = 0;
                    }
                }
            }
            yield return new WaitForSeconds(secondsToGenerateResource);
            //Debug.Log($"Tried to Generate {generatedResource}");
            canGenerateResource = true;
        }

        private void Start()
        {
            StartStructure();
        }

        void FixedUpdate()
        {
            if (!Preview)
            {
                TryGenerateResource();
            }
        }

        private void Update()
        {
            UpdateStructure();
        }

        private void OnDestroy()
        {
            StructureOnDestroy();
        }
    }
}

