using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Structure
{
    [RequireComponent(typeof(Button))]
    public class StructureSelectionButton : MonoBehaviour
    {
        //[System.NonSerialized]
        public StructureInfo structureInfo;

        private Image structureIcon;
        private Button structureButton;

        private StructurePlacer structurePlacer;

        private void SelectStructure()
        {
            structurePlacer.SelectNewStructure(structureInfo.structure);
        }

        private void OnValidate()
        {
            structureButton ??= gameObject.GetComponent<Button>();

        }

        private void Awake()
        {
            structurePlacer = StaticObjects.theStructurePlacer;
        }

        private void Start()
        {
            
        }
    }
}
