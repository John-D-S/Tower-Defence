using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structures
{
    public class StructureSelectionPanel : MonoBehaviour
    {
        //the list of structure selection buttons
        private List<StructureSelectionButton> structureSelectionButtons = new List<StructureSelectionButton>();

        private void Awake()
        {
            //set the structureSelectionPanel in the static object holder to this.
            StaticObjectHolder.theStructureSelectionPanel = this;
        }

        private void Start()
        {
            //initialise structureSelectionButtons.
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
