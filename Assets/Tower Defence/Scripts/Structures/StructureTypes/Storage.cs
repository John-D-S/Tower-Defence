using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Economy;

namespace Structures
{
    public class Storage : Structure
    {
        [Header("-- Storage Settings --")]
        [SerializeField, Tooltip("The resorce this storageStructure will store")]
        private EconomyResource resourceToStore;
        [SerializeField, Tooltip("The maximum amount of resouces that this structure can store")]
        private int storageCapacity;
        public int StorageCapacity
        {
            get => storageCapacity;
        }

        private void Start()
        {
            //initialise the strucuture
            StartStructure();
            //add this script to the appropriate list of storage structure types in EconomyTracker
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
            //when this structue is destroyed, remove it from the list that it is in in EconomyTracker, then destroy it
            if (resourceToStore == EconomyResource.Energy)
                EconomyTracker.activeEnergyStorageStructures.Remove(this);
            else
                EconomyTracker.activeMetalStorageStructures.Remove(this);
            StructureOnDestroy();
        }

        private void Update()
        {
            UpdateStructure();
        }
    }
}
