using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Economy;

namespace Structure
{
    public class Storage : Structure
    {
        [Header("-- Storage Settings --")]
        [SerializeField]
        private EconomyResource resourceToStore;
        [SerializeField]
        private int storageCapacity;
        public int StorageCapacity
        {
            get => storageCapacity;
        }

        private void Start()
        {
            switch (resourceToStore)
            {
                case EconomyResource.Metal:
                    EconomyTracker.activeMetalStorageStructures.Add(this);
                    break;
                case EconomyResource.Energy:
                    EconomyTracker.activeEnergyStorageStructures.Add(this);
                    break;
            }
        }

        private void OnDestroy()
        {
            if (resourceToStore == EconomyResource.Energy)
                EconomyTracker.activeEnergyStorageStructures.Remove(this);
            else
                EconomyTracker.activeMetalStorageStructures.Remove(this);
        }

        private void Update()
        {
            UpdateStructure();
        }
    }
}
