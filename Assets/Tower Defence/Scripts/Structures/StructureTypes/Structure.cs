using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public abstract class Structure : MonoBehaviour
    {
        private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        private Material realMaterial;
        public Material previewMaterial;
        private bool preview = true;
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
        private float maxHealth = 50;
        private float health;

        [SerializeField, Tooltip("The amount of metal consumed when this structure is built")]
        int metalCostToBuild;
        [SerializeField, Tooltip("The amount of energy consumed when this structure is activated")]
        int energyToRun;

        private void InitializeHealth() => health = maxHealth;


        private void InitializeMeshRendering()
        {
            meshRenderers.Clear();
            foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>(true))
            {
                meshRenderers.Add(renderer);
            }
        }

        private void ConsumeEnergy()
        {
            Economy.EconomyTracker.TryIncrementEnergy(-energyToRun);
        }

        private void OnValidate()
        {
            InitializeMeshRendering();
        }

        private void Start()
        {
            InitializeHealth();
        }
    }
}