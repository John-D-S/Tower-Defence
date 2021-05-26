using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Structure
{
    [RequireComponent(typeof(Button))]
    public class StructureSelectionButton : MonoBehaviour
    {
        public StructureInfo ButtonStructureInfo
        {
            set
            {
                structureIcon.sprite = value.structureIcon;
                structureInfo = value;
            }
            get
            {
                return structureInfo;
            }
        }
        private StructureInfo structureInfo;


        [SerializeField]
        private Image structureIcon;
        [SerializeField]
        private Button structureButton;

        private StructurePlacer structurePlacer;

        private void SelectStructure()
        {
            structurePlacer.SelectNewStructure(ButtonStructureInfo.structure);
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
            //structureIcon.sprite = StructureInfo.structureIcon;
        }
    }
}
