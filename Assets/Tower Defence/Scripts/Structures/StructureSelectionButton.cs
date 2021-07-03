using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Menu.TheMenuHandler;

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
            if (selected && theMenuHandler.Paused)
            {
                Debug.Log("DeselectStructure should have been called");
                DeselectStructure();
            }
            if (selected && structureInfo.structure != null && EventSystem.current.currentSelectedGameObject != structureButton.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(structureButton.gameObject);
            }
        }
    }
}
