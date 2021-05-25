using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public class StructureSelectionPanel : MonoBehaviour
    {
        private List<StructureSelectionButton> structureSelectionButtons = new List<StructureSelectionButton>();

        private void Awake()
        {
            StaticObjects.theStructureSelectionPanel = this;
        }

        private void Start()
        {
            foreach (Transform child in transform)
            {
                StructureSelectionButton structureSelectionButton = child.GetComponent<StructureSelectionButton>();
                if (structureSelectionButton)
                {
                    structureSelectionButtons.Add(structureSelectionButton);
                }
            }
        }
    }
}
