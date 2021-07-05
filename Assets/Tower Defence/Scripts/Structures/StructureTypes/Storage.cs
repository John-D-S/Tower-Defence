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

        [SerializeField, Tooltip("how many seconds after being disconnected is the storage structure no longer able to store resources")]
        private float timeUntilStorageDoesntWork = 5f;
        private bool canStoreResource = true;
        public bool CanStoreResource
        {
            get => canStoreResource;
        }
        private float timeSinceDisconnected = 0;
        void UpdateCanStoreResource()
        {
            if (isConnectedToCore)
            {
                canStoreResource = true;
                timeSinceDisconnected = 0;
            }
            else if (timeSinceDisconnected < timeUntilStorageDoesntWork)
            {
                timeSinceDisconnected += Time.fixedDeltaTime;
            }
            else
            {
                canStoreResource = false;
            }
        }

        private void Start()
        {
            StartStructure();
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
            StructureOnDestroy();
        }

        private void Update()
        {
            UpdateStructure();
        }

        private void FixedUpdate()
        {
            UpdateCanStoreResource();
        }
    }
}
