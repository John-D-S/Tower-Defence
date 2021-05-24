using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Structure
{
    [RequireComponent(typeof(Button))]
    public class StructureSelectionButton : MonoBehaviour
    {
        [SerializeField]
        private StructureInfo structureInfo;

        private StructurePlacer structurePlacer = StaticStructureObjects.theStructurePlacer;
        private Image structureIcon;
        private Button structureButton;

        private void Start()
        {
            //structureSelectionPanel = StaticStructureObjects.theStructureSelectionPanel;
            structureButton = gameObject.GetComponent<Button>();
            foreach (Transform child in transform)
            {
                Image image = child.GetComponent<Image>();
                if (image)
                    structureIcon = image;
            }
            if (structureIcon)
            {
                structureIcon.sprite = structureInfo.structureIcon;
            }
        }
    }
}
