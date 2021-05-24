using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Structure
{
    public class StructurePlacer : MonoBehaviour
    {
        private StructureSelectionPanel structureSelectionPanel;
        private StructureInfo selectedStructureInfo;
        [System.NonSerialized]
        public GameObject selectedStructure;

        private void Awake()
        {
            StaticStructureObjects.theStructurePlacer = this;
        }

        private void Start()
        {
            structureSelectionPanel = StaticStructureObjects.theStructureSelectionPanel;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {

                }
            }
        }
    }
}
