using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Menu.TheMenuHandler;

namespace Structures
{
    [RequireComponent(typeof(Button))]
    public class StructureSelectionButton : MonoBehaviour
    {
        /// <summary>
        /// the structure info of this structureSelectionButton
        /// </summary>
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

        [SerializeField, Tooltip("The image displayed on the structure button")]
        private Image structureIcon;
        [SerializeField, Tooltip("The structure must have a script which inherits from structure.")]
        private Button structureButton;

        private StructurePlacer structurePlacer;

        /// <summary>
        /// Set the selected structure in the structurePlacer to this one.
        /// </summary>
        public void SelectStructure()
        {
            structurePlacer.SelectNewStructure(ButtonStructureInfo.structure, this);
            selected = true;
        }

        /// <summary>
        /// deselect this structure button in the structurePlacer.
        /// </summary>
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
            //initialise the structurePlacer and add a listener to the attatched button.
            structurePlacer = StaticObjectHolder.theStructurePlacer;
            structureButton.onClick.AddListener(SelectStructure);
        }

        private void Update()
        {
            //if this button is selected and the game is paused, deselect this button
            if (selected && theMenuHandler.Paused)
            {
                DeselectStructure();
            }
            //if this structure button is selected and the structureinfo does not equal null and the current selected canvas gameobject is not this one, select this button.
            if (selected && structureInfo.structure != null && EventSystem.current.currentSelectedGameObject != structureButton.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(structureButton.gameObject);
            }
        }
    }
}
