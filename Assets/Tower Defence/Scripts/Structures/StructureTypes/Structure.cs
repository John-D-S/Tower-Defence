using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;
using Economy;

namespace Structure
{
    public abstract class Structure : MonoBehaviour, IKillable
    {
        [Header("-- Material Settings --")]
        public Material realMaterial;
        public Material allowedPreviewMaterial;
        public Material disallowedPreviewMaterial;
        private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        [Header("-- Area Settings --")]
        [SerializeField]
        private float radius = 0.5f;
        [SerializeField]
        private float height = 3f;
        [System.NonSerialized]
        public bool isIntersecting;

        [SerializeField]
        protected Collider thisCollider;

        private bool preview = true;
        public bool Preview
        {
            set
            {
                preview = value;
                if (preview)
                {
                    SetLayerOfAllChildren(gameObject, LayerMask.NameToLayer("PreviewStructure"));
                    SetTagOfAllChildren(gameObject, "PreviewStructure");
                    if (thisCollider)
                        thisCollider.enabled = false;
                    if (healthBar)
                        healthBar.gameObject.SetActive(false);
                }
                else
                {
                    SetLayerOfAllChildren(gameObject, LayerMask.NameToLayer("Structure"));
                    SetTagOfAllChildren(gameObject, "Structure");
                    if (thisCollider)
                        thisCollider.enabled = true;
                    if (healthBar)
                        healthBar.gameObject.SetActive(true);
                }
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

        [Header("-- Stats --")]
        [SerializeField, Tooltip("The amount of metal consumed when this structure is built")]
        private int metalCostToBuild;
        [SerializeField, Tooltip("The amount of energy consumed when this structure is activated")]
        protected int energyToRun;

        public bool IsConnectedToCore
        {
            //insert code to check if this structure is linked directly or by proxy to the core.
            get => true;
        }

        [SerializeField]
        HealthBar healthBar;
        [SerializeField]
        private float maxHealth = 100;
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        private float health;
        public float Health
        {
            get => health;
            set
            {
                if (value < 0)
                {
                    health = 0;
                    Die();
                }
                else if (value > maxHealth)
                    health = maxHealth;
                else
                    health = value;

                if (healthBar)
                {
                    healthBar.SetHealth(value, MaxHealth);
                }

                healthBar.SetHealth(Health, maxHealth);
            }
        }
        public void Damage(float amount) => Health -= amount;
        public void Heal(float amount) => Health += amount;
        public void Die() => Destroy(gameObject);

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

        private void SetAllowedDisallowedMaterial()
        {
            if (Preview)
            {
                if (IntersectingOtherStructure() && metalCostToBuild < EconomyTracker.metal)
                    SetMaterial(disallowedPreviewMaterial);
                else
                    SetMaterial(allowedPreviewMaterial);
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

        void initializeHealth() => Health = maxHealth;

        private void OnValidate()
        {
            thisCollider = gameObject.GetComponentInChildren<Collider>();
            InitializeMeshRendering();
        }

        protected void StartStructure()
        {
            initializeHealth();
            InitializeMeshRendering();
        }

        protected void UpdateStructure()
        {
            SetAllowedDisallowedMaterial();
        }
    }
}