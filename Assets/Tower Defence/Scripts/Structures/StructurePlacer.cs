using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Controls.ControlHelperFunctions;

namespace Structure
{
    public class StructurePlacer : MonoBehaviour
    {
        [SerializeField]
        private Material previewMaterial;

        private StructureSelectionPanel structureSelectionPanel;
        private StructureInfo selectedStructureInfo;
        [System.NonSerialized]
        public GameObject selectedStructure;
        private GameObject selectedStructurePreview;

        private GameObject previewStructure;

        public void SwitchSelectedStructures(GameObject _structureToReplace)
        {
            selectedStructure = _structureToReplace;
            selectedStructurePreview = selectedStructure;

            selectedStructurePreview.GetComponent<Structure>();
        
        }

        private void Awake()
        {
            StaticObjects.theStructurePlacer = this;
        }

        private void Start()
        {
            structureSelectionPanel = StaticObjects.theStructureSelectionPanel;
        }

        private void Update()
        {
            if (selectedStructure && ! previewStructure)
            {
                previewStructure = Instantiate(selectedStructure, MouseRayHitPoint(), Quaternion.identity, gameObject.transform);
                
            }
            if (Input.GetMouseButtonDown(0))
            {
                //if the pointer is not over any gui
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (selectedStructure)
                    {

                    }
                }
            }
        }
    }
}
