using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Controls.ControlHelperFunctions;

namespace Structure
{
    [System.Serializable]
    public class StructureInfo
    {
        public Sprite structureIcon;
        [Tooltip("The structure must have a script which inherits from structure.")]
        public GameObject structure;

        public StructureInfo(Sprite _structureIcon, GameObject _structure)
        {
            structureIcon = _structureIcon;
            structure = _structure;
        }

        public Structure StructureScript
        {
            get
            {
                return structureScript;
            }
            set { }
        }
        private Structure structureScript;

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
        
        [SerializeField]
        private StructureSelectionPanel structureSelectionPanel;
        private List<StructureSelectionButton> structureSelectionButtons;
        
        [Header("-- Structure Options --")]
        [SerializeField]
        private Material allowedPreviewMaterial;
        [SerializeField]
        private Material disallowedPreviewMaterial;

        private StructureSelectionButton selectedButtonObject;
        private GameObject selectedStructure;
        private GameObject previewStructure;

        private GameObject previewStructureInstance;
        private Structure previewStructureInstanceScript;

        IEnumerator Destroy(GameObject go)
        {
            yield return new WaitForEndOfFrame();
            //destroyImmediate is required for destroying Objects in Edit mode
            DestroyImmediate(go);
        }

        private void InitializeStructureButtons()
        {
            foreach (Transform child in structureSelectionPanel.transform)
            {
                //a coroutine is needed because using destroyimmediate in an iteration thing will cause a bug.
                StartCoroutine(Destroy(child.gameObject));
            }
            foreach (StructureInfo structureInfo in structureButtonInfos)
            {
                Structure structureInfoStructure = structureInfo.structure.GetComponent<Structure>();
                structureInfoStructure.allowedPreviewMaterial = allowedPreviewMaterial;
                structureInfoStructure.disallowedPreviewMaterial = disallowedPreviewMaterial;
            }
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

        public void SelectNewStructure(GameObject _structureToReplace, StructureSelectionButton _selectedButtonObject)
        {
            if (selectedButtonObject)
                selectedButtonObject.DeselectStructure();
            selectedButtonObject = _selectedButtonObject;
            selectedStructure = _structureToReplace;
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
            StaticObjects.theStructurePlacer = this;
            InitializeStructureButtons();
        }

        private void Update()
        {
            //if the pointer is not over any gui
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log(true);
                if (selectedStructure && ! previewStructureInstance)
                {
                    Debug.Log("Instantiated New Preview Structure");
                    previewStructureInstance = Instantiate(selectedStructure, MouseRayHitPoint(LayerMask.GetMask("Terrain")), Quaternion.identity);
                    previewStructureInstanceScript = previewStructureInstance.GetComponent<Structure>();
                }
                else if (selectedStructure && previewStructureInstance)
                {
                    Debug.Log("Moved Current Preview Structure");
                    previewStructureInstance.transform.position = MouseRayHitPoint(LayerMask.GetMask("Terrain"));
                }


                if (previewStructureInstance)
                {
                    Debug.Log(previewStructureInstanceScript.IntersectingOtherStructure());
                    if (!previewStructureInstanceScript.IntersectingOtherStructure())
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            selectedStructure.GetComponent<Structure>().TryPlaceStructure(MouseRayHitPoint(LayerMask.GetMask("Terrain")));
                        }
                    }
                }
            }
            else
            {
                if (previewStructureInstance)
                {
                    StartCoroutine(Destroy(previewStructureInstance));
                    previewStructureInstanceScript = null;
                }
            }
        }
    }
}