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

    public abstract class Structure : MonoBehaviour
    {
        [SerializeField, Tooltip("The amount of metal consumed when this structure is built")]
        int MetalCostToBuild;
        [SerializeField, Tooltip("The amount of energy consumed when this structure is activated")]
        int EnergyToRun;

        [SerializeField] 
        private float maxHealth;
        private float health;

        private void InitializeHealth() => health = maxHealth;

        private void ConsumeEnergy()
        {
            Economy.EconomyTracker.energy -= EnergyToRun;
        }

        private void Start()
        {
            InitializeHealth();
        }
    }
}