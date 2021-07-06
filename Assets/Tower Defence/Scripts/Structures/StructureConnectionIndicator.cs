using System.Collections;
using System.Collections.Generic;
using static StaticObjectHolder;
using UnityEngine;


namespace Structures
{
    [RequireComponent(typeof(MeshRenderer))]
    public class StructureConnectionIndicator : MonoBehaviour
    {
        [SerializeField]
        private Material connectedIndicatorMaterial;
        [SerializeField]
        private Material disconnectedIndicatorMaterial;
     
        private MeshRenderer connectionIndicator;
        private Structure AttatchedStructure;

        private void Start()
        {
            connectionIndicator = GetComponent<MeshRenderer>();
            AttatchedStructure = transform.parent.GetComponent<Structure>();
            AttatchedStructure.connectionIndicator = connectionIndicator;

            theVisibilityManager.structureConnectionIndicators.Add(this);
            gameObject.SetActive(theVisibilityManager.showStructureConnectionIndicators);
        }

        public void UpdateConnectionIndicator()
        {
            if (AttatchedStructure.Preview)
                connectionIndicator.enabled = false;
            else
                connectionIndicator.enabled = true;

            if (connectionIndicator)
            {
                //Debug.Log($"is connected to core: {isConnectedToCore}");
                if (AttatchedStructure.CanFunction)
                {
                    connectionIndicator.material = connectedIndicatorMaterial;
                }
                else
                {
                    connectionIndicator.material = disconnectedIndicatorMaterial;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateConnectionIndicator();
        }
    }
}
