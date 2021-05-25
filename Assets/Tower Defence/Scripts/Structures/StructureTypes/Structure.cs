using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Structure
{
    public abstract class Structure : MonoBehaviour
    {
        [SerializeField, Tooltip("The amount of metal consumed when this structure is built")]
        int MetalCostToBuild;
        [SerializeField, Tooltip("The amount of energy consumed when this structure is activated")]
        int EnergyToRun;

        private List<MeshRenderer> meshRenderers;

        private Material realMaterial;
        private Material previewMaterial;
        private bool preview = false;
        public bool Preview
        {
            set
            {
                preview = value;
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    if (preview)
                        meshRenderer.material = previewMaterial;
                    else
                        meshRenderer.material = realMaterial;
                }
            }
            get
            {
                return preview;
            }
        }

        [SerializeField] 
        private float maxHealth;
        private float health;

        private void InitializeHealth() => health = maxHealth;

        private void ConsumeEnergy()
        {
            Economy.EconomyTracker.energy -= EnergyToRun;
        }

        private void InitializeMeshRendering()
        {
            foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>(true))
            {
                meshRenderers.Add(renderer);
            }
        }

        private void Start()
        {
            InitializeHealth();
        }
    }
}