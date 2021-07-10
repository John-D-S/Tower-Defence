using System.Collections;
using System.Collections.Generic;
using static StaticObjectHolder;
using UnityEngine;


namespace Structures
{
    [RequireComponent(typeof(MeshRenderer))]
    public class StructureConnectionIndicator : MonoBehaviour
    {
        [Header("-- Connection Indicator Settings --")]
        [SerializeField, Tooltip("The material the connection indicator will have when connected")]
        private Material connectedIndicatorMaterial;
        [SerializeField, Tooltip("The material the connection indicator will have when disconnected")]
        private Material disconnectedIndicatorMaterial;
     
        //the meshrenderer which displays the connectionIndicator.
        private MeshRenderer connectionIndicator;
        //the structure script which the connection indicator is displaying the connection status of
        private Structure AttatchedStructure;

        private void Start()
        {
            //initialize variables
            connectionIndicator = GetComponent<MeshRenderer>();
            AttatchedStructure = transform.parent.GetComponent<Structure>();
            AttatchedStructure.connectionIndicator = connectionIndicator;

            theVisibilityManager.structureConnectionIndicators.Add(this);
            //set this gameobject active according to whether or not they are set to visible.
            gameObject.SetActive(theVisibilityManager.showStructureConnectionIndicators);
        }

        public void UpdateConnectionIndicator()
        {
            //if this connection indicator is attatched to a preview structure, it should not be visible
            if (AttatchedStructure.Preview)
                connectionIndicator.enabled = false;
            else
                connectionIndicator.enabled = true;

            //if the connectionIndicator is attatched to a structure that can function, set it to show that the structure is connected and visa versa
            if (connectionIndicator)
            {
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
