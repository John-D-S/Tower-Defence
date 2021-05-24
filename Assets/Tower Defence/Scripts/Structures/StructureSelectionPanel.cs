using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public class StructureSelectionPanel : MonoBehaviour
    {
        private void Awake()
        {
            StaticStructureObjects.theStructureSelectionPanel = this;
        }
    }
}
