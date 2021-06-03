using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Economy;

namespace Structure
{
    public abstract class Structure : MonoBehaviour
    {
        [Header("-- Material Settings --")]
        [HideInInspector]
        public Material realMaterial;
        [HideInInspector]
        public Material allowedPreviewMaterial;
        [HideInInspector]
        public Material disallowedPreviewMaterial;
        private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        [Header("-- Area Settings --")]
        [SerializeField]
        private float radius = 0.5f;
        [SerializeField]
        private float height = 3f;
        [System.NonSerialized]
        public bool isIntersecting;

        private bool preview = true;
        public bool Preview
        {
            set
            {
                preview = value;
                if (preview)
                    gameObject.layer = LayerMask.NameToLayer("PreviewStructure");
                else
                    gameObject.layer = LayerMask.NameToLayer("Structure");
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    if (preview)
                        SetMaterial(allowedPreviewMaterial);
                    else
                        SetMaterial(realMaterial);
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

        private void SetMaterial(Material _material)
        {
            foreach (MeshRenderer meshRenderer in meshRenderers)
                meshRenderer.material = _material;
        }

        public void TryPlaceStructure(Vector3 _position)
        {
            if (EconomyTracker.TryIncrementMetal(-metalCostToBuild))
            {
                GameObject newObject = Instantiate(gameObject, _position, Quaternion.identity);
                newObject.GetComponent<Structure>().InitializeMeshRendering();
                newObject.GetComponent<Structure>().Preview = false;
            }
        }

        private void InitializeHealth() => health = maxHealth;

        private void SetAllowedDisallowedMaterial()
        {
            if (Preview)
            {
                if (IntersectingOtherStructure() && metalCostToBuild < EconomyTracker.metal)
                {
                    SetMaterial(disallowedPreviewMaterial);
                }
                else
                {
                    SetMaterial(allowedPreviewMaterial);
                }

            }
        }

        public virtual bool IntersectingOtherStructure()
        {
            if (Preview)
            {
                Vector3 point0 = transform.position + Vector3.up * (height * 0.5f - radius);
                Vector3 point1 = transform.position - Vector3.up * (height * 0.5f - radius);
                if (Physics.OverlapCapsule(point0, point1, radius, LayerMask.GetMask("Structure")).Length > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

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

        protected void StartStructure()
        {
            InitializeMeshRendering();
            InitializeHealth();
        }

        protected void UpdateStructure()
        {
            SetAllowedDisallowedMaterial();
        }
    }
}