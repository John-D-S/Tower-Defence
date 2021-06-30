using System.Collections;
using UnityEngine.EventSystems;
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

        private bool selected = false;

        [SerializeField]
        private Image structureIcon;
        [SerializeField]
        private Button structureButton;

        private StructurePlacer structurePlacer;

        public void SelectStructure()
        {
            structurePlacer.SelectNewStructure(ButtonStructureInfo.structure, this);
            selected = true;
        }

        public void DeselectStructure()
        {
            selected = false;
            EventSystem.current.SetSelectedGameObject(null);
        }   

        private void OnValidate()
        {
            structureButton ??= gameObject.GetComponent<Button>();
        }

        private void Awake()
        {
            structurePlacer = StaticObjectHolder.theStructurePlacer;
            structureButton.onClick.AddListener(SelectStructure);
        }

        private void Update()
        {
            if (selected && EventSystem.current.currentSelectedGameObject != structureButton.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(structureButton.gameObject);
            }
        }
    }
}
