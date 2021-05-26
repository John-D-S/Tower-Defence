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

        public StructureInfo(Sprite _structureIcon, GameObject _structure)
        {
            structureIcon = _structureIcon;
            structure = _structure;
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
        private Material previewMaterial;

        private GameObject selectedStructure;
        private GameObject previewStructure;

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
                structureInfo.structure.GetComponent<Structure>().previewMaterial = previewMaterial;
            }
            Debug.Log(structureButtonInfos.Count + 1);
            for (int i = 0; i < structureButtonInfos.Count + 1; i++)
            {
                Debug.Log(i);
                GameObject currentSelectionButtonObject = Instantiate(structureButtonObject, structureSelectionPanel.transform);
                StructureSelectionButton currentSelectionButton = currentSelectionButtonObject.GetComponent<StructureSelectionButton>();
                if (i == 0)
                    currentSelectionButton.ButtonStructureInfo = new StructureInfo(cancelSelectionImage, null);
                else
                    currentSelectionButton.ButtonStructureInfo = structureButtonInfos[i - 1];
            }
        }

        public void SelectNewStructure(GameObject _structureToReplace)
        {
            selectedStructure = _structureToReplace;
            if (selectedStructure)
            {
                previewStructure = selectedStructure;
                previewStructure.GetComponent<Structure>();

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
            if (selectedStructure && ! previewStructure)
            {
                previewStructure = Instantiate(selectedStructure, MouseRayHitPoint(), Quaternion.identity, gameObject.transform);
                
            }
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
