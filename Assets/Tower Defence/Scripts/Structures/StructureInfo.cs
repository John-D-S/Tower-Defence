using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Structure
{
    [CreateAssetMenu(fileName = "Structure Info", menuName = "Tower Defence/Structure Info", order = 100)]
    public class StructureInfo : ScriptableObject
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
    }
}
