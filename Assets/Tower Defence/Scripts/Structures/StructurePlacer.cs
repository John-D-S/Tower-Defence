using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static HelperClasses.HelperFunctions;

namespace Structures
{
    [System.Serializable]
    public class StructureInfo
    {
        [Header("-- Structure Button Settings --")]
        [Tooltip("The image displayed on the structure button")]
        public Sprite structureIcon;
        [Tooltip("The structure must have a script which inherits from structure.")]
        public GameObject structure;

        /// <summary>
        /// the initializer for structure info simply sets the structureIcon and the Structure
        /// </summary>
        public StructureInfo(Sprite _structureIcon, GameObject _structure)
        {
            structureIcon = _structureIcon;
            structure = _structure;
        }

        /// <summary>
        /// the script of structure
        /// </summary>
        public Structure StructureScript
        {
            get
            {
                return structureScript;
            }
            set { }
        }
        private Structure structureScript;

        //when this is changed in the script, update the structureScript and if the gameobject does not have a structure script attatched, set it to null
        private void OnValidate()
        {
            structureScript = structure.GetComponent<Structure>();
            if (structureScript == null)
                structure = null;
        }
    }

    public class StructurePlacer : MonoBehaviour
    {
        [Header("-- Button Options --")]
        [SerializeField, Tooltip("The image displayed on the button which clears your selection.")]
        private Sprite cancelSelectionImage;
        [SerializeField, Tooltip("The icon and structure for each button in the button selection panel.")]
        private List<StructureInfo> structureButtonInfos;
        [SerializeField, Tooltip("The prefab of the tower selection button. It must have the StructureSelectionButton component on it")]
        private GameObject structureButtonObject;

        [SerializeField, Tooltip("The panel where all the structure selection buttons are spawned")]
        private StructureSelectionPanel structureSelectionPanel;
        private List<StructureSelectionButton> structureSelectionButtons;

        [Header("-- Structure Options --")]
        [SerializeField, Tooltip("The allowed preview material the structures will have when they are preview holograms")]
        private Material allowedPreviewMaterial;
        [SerializeField, Tooltip("The disallowed preview material the structures will have when they are preview holograms")]
        private Material disallowedPreviewMaterial;

        private StructureSelectionButton selectedButtonObject;
        private GameObject selectedStructure;
        private GameObject previewStructure;

        //the actual, instantiated preview structure GameObject
        private GameObject previewStructureInstance;
        //the script of the instantiated preview structure.
        private Structure previewStructureInstanceScript;
        [Header("-- Structure Placement Effect Settings --")]
        [SerializeField, Tooltip("The effect which is instantiated whenever a structure is placed.")]
        private GameObject PlaceStructureEffect;

        IEnumerator Destroy(GameObject go)
        {
            yield return new WaitForEndOfFrame();
            //destroyImmediate is required for destroying Objects in Edit mode
            DestroyImmediate(go);
        }

        /// <summary>
        /// instantiate all the hud buttons for selecting structures to place in the world.
        /// </summary>
        private void InitializeStructureButtons()
        {
            //destroy allstructures currently in the structureSelectionPanel
            foreach (Transform child in structureSelectionPanel.transform)
            {
                //a coroutine is needed because using destroyimmediate in an iteration thing will cause a bug.
                StartCoroutine(Destroy(child.gameObject));
            }
            //set the allowed and disallowed preview material for each structure info in structue button infos
            foreach (StructureInfo structureInfo in structureButtonInfos)
            {
                Structure structureInfoStructure = structureInfo.structure.GetComponent<Structure>();
                structureInfoStructure.allowedPreviewMaterial = allowedPreviewMaterial;
                structureInfoStructure.disallowedPreviewMaterial = disallowedPreviewMaterial;
            }
            //instantiate each of the structure buttons in thestructureSelectionPanel
            for (int i = 0; i < structureButtonInfos.Count + 1; i++)
            {
                GameObject currentSelectionButtonObject = Instantiate(structureButtonObject, structureSelectionPanel.transform);
                StructureSelectionButton currentSelectionButton = currentSelectionButtonObject.GetComponent<StructureSelectionButton>();
                if (i == 0)
                    currentSelectionButton.ButtonStructureInfo = new StructureInfo(cancelSelectionImage, null);
                else
                    currentSelectionButton.ButtonStructureInfo = structureButtonInfos[i - 1];
            }
        }

        /// <summary>
        /// replace the selectedStructure with _structureToReplace, deselect the old structure button and select the new one
        /// </summary>
        public void SelectNewStructure(GameObject _structureToReplace, StructureSelectionButton _selectedButtonObject)
        {
            //deselect the currently selected button object
            if (selectedButtonObject)
                selectedButtonObject.DeselectStructure();
            //set the new selected Button object and the selectedStructure
            selectedButtonObject = _selectedButtonObject;
            selectedStructure = _structureToReplace;
            //make the selected structure a preview
            if (selectedStructure)
            {
                previewStructure = selectedStructure;
                previewStructure.GetComponent<Structure>().Preview = true;
            }
        }

        private void OnValidate()
        {
            StructureSelectionButton selectionButtonScript = structureButtonObject.GetComponent<StructureSelectionButton>();
            if (selectionButtonScript == null)
                structureButtonObject = null;
        }
        
        private void Awake()
        {
            StaticObjectHolder.theStructurePlacer = this;
            InitializeStructureButtons();
        }

        private void Update()
        {
            //if the pointer is not over UI
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //if there is aselected structure but not an instance of the preview structure
                if (selectedStructure && ! previewStructureInstance)
                {
                    //instantiate a new preview structure
                    previewStructureInstance = Instantiate(selectedStructure, MouseRayHitPoint(LayerMask.GetMask("Terrain", "Ore")), Quaternion.identity);
                    previewStructureInstanceScript = previewStructureInstance.GetComponent<Structure>();
                    previewStructureInstanceScript.Preview = true;
                } // if the selected structure exists and previewStructure Instance exists.
                else if (selectedStructure && previewStructureInstance)
                {
                    // set the position of the preview structure instance to the position of the cursor over the terrain.
                    previewStructureInstance.transform.position = MouseRayHitPoint(LayerMask.GetMask("Terrain", "Ore"));
                }

                // if the previewStructureInstance exists and you press the left mouse button and the structure can be placed
                if (previewStructureInstance && Input.GetMouseButtonDown(0) && previewStructureInstanceScript.CanBePlaced)
                {
                    Vector3 position = MouseRayHitPoint(LayerMask.GetMask("Terrain", "Ore"));
                    selectedStructure.GetComponent<Structure>().TryPlaceStructure(position);
                    Instantiate(PlaceStructureEffect, position, Quaternion.identity);
                }
            }
            else
            {
                //if the cursor is over ui, delete the previewStructureInstance
                if (previewStructureInstance)
                {
                    StartCoroutine(Destroy(previewStructureInstance));
                    previewStructureInstanceScript = null;
                }
            }
        }
    }
}