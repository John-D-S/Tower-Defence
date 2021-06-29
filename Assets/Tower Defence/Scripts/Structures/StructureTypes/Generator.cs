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
            get
            {
                float returnValue = (1f / (float)resoucesGeneratedPerMinute) * 60f;
                return returnValue;
            }
        }

        void TryGenerateResource()
        {
            if (canGenerateResource && isConnectedToCore)
            {
                StartCoroutine(GenerateResource());
            }
        }

        private IEnumerator GenerateResource()
        {
            canGenerateResource = false;
            //Debug.Log($"Tried to Generate {generatedResource}");
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
    }
}

