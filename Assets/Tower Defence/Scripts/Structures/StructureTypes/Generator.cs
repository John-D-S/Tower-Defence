using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Economy;

namespace Structures
{
    public class Generator : Structure
    {
        [Header("-- Generation Settings --")]
        [SerializeField, Tooltip("The resource that this generator Generates")]
        private EconomyResource generatedResource;
        [SerializeField, Tooltip("The number of resouces generated at a time.")]
        private int numberOfResourcesGenerated = 2;
        [SerializeField, Tooltip("The seconds between each time resources are generated.")]
        private int secondsToGenerateResource = 10;

        [Header("-- Consumption Settings --")]
        [SerializeField, Tooltip("whether or not to consume the resource that isn't generatedResource")]
        private bool consumeResource;
        [SerializeField, Tooltip("the numer of resources to consume if consumeResource is true")]
        private int numberOfResourcesToConsume = 1;
        //this is like storage for resources that will be consumed in the future
        private int numberOfStoredResourcesToBeConsumed = 0;

        bool canGenerateResource = true;

        /// <summary>
        /// Generate the resource if you can
        /// </summary>
        void TryGenerateResource()
        {
            if (canGenerateResource && CanFunction)
            {
                StartCoroutine(GenerateResource());
            }
        }

        /// <summary>
        /// generates the resource
        /// </summary>
        private IEnumerator GenerateResource()
        {
            canGenerateResource = false;
            bool alreadyAtMaxResource = generatedResource == EconomyResource.Energy ? EconomyTracker.energy >= EconomyTracker.MaxEnergy : EconomyTracker.metal >= EconomyTracker.MaxMetal;
            // stores resources to be used when generating.
            if (consumeResource && !alreadyAtMaxResource)
            {
                if (generatedResource == EconomyResource.Energy)
                    if (EconomyTracker.TryIncrementMetal(-numberOfResourcesToConsume))
                        numberOfStoredResourcesToBeConsumed = numberOfResourcesToConsume;
                else
                    if (EconomyTracker.TryIncrementEnergy(-numberOfResourcesToConsume))
                        numberOfStoredResourcesToBeConsumed = numberOfResourcesToConsume;
            }
            // consumes the resource if there are enough resources in numberOfStoredResourcesToBeConsumed
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
            //wait for secondsToGenerateResources then set canGenerateResources to true
            yield return new WaitForSeconds(secondsToGenerateResource);
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