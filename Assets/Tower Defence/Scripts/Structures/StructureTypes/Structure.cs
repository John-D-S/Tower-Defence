using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;
using static StaticObjects;
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
        
        #region Core Connection
        private float checkConnectionRadius = 20;

        static Dictionary<GameObject, Structure> currentStructures = new Dictionary<GameObject, Structure>();
        
        private List<Structure> nearbyStructures = new List<Structure>();
        
        private bool nearbyStructuresChecked;
        private IEnumerator TemporarilySetNearbyStructuresCheckedTrue()
        {
            nearbyStructuresChecked = true;
            yield return new WaitForFixedUpdate();
            nearbyStructuresChecked = false;
        }

        private bool NearbyStructuresHaveBeenChecked
        {
            get
            {
                return nearbyStructuresChecked;
            }
        }

        public void FindNearbyStructures()
        {
            //a list of all structure Colliders within checkConnectionRadius
            List<Collider> nearbyStructureColliders = new List<Collider>(Physics.OverlapSphere(transform.position, checkConnectionRadius, LayerMask.GetMask(new string[] { "Structure", "EnemyTarget" })));
            List<GameObject> nearbyStructureGameObjects = new List<GameObject>();
            foreach (Collider collider in nearbyStructureColliders)
            {
                nearbyStructureGameObjects.Add(collider.gameObject);
            }

            //removing old structures
            foreach (Structure structure in nearbyStructures)
            {
                //if a structure that used to be nearby is no longer there, remove it from the list of nearby structures
                if (!nearbyStructureGameObjects.Contains(structure.gameObject))
                {
                    nearbyStructures.Remove(structure);
                }
            }

            //adding new structures
            foreach (Collider structureCollider in nearbyStructureColliders)
            {
                //the structure collider's gameobject
                GameObject structureGameObject = structureCollider.gameObject;
                //The structure component attatched to structureGameObject
                Structure structureColliderStructure;
                //if this nearby structure is not yet recorded in current structures, add it.
                if (!currentStructures.ContainsKey(structureCollider.gameObject))
                {
                    structureColliderStructure = structureCollider.gameObject.GetComponent<Structure>();
                    currentStructures[structureCollider.gameObject] = structureColliderStructure;
                }
                else
                    structureColliderStructure = currentStructures[structureGameObject];
            }

        }

        /// <summary>
        /// checks if any nearby structures are activated. If so, set self to activated and 
        /// </summary>
        protected virtual void UpdateConnectedToCore()
        {
            StartCoroutine(TemporarilySetNearbyStructuresCheckedTrue());
            FindNearbyStructures();
            foreach (Structure structure in nearbyStructures)
            {
                if (structure.isConnectedToCore)
                {
                    isConnectedToCore = true;
                }
            }
            foreach (Structure structure in nearbyStructures)
            {
                if (!structure.nearbyStructuresChecked)
                {
                    structure.UpdateConnectedToCore();
                }
            }
        }
        public bool isConnectedToCore;
        #endregion


        [SerializeField]
        HealthBar healthBar;
        [SerializeField]
        private float maxHealth = 100;
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        private float health = -2;
        public float Health
        {
            get
            {
                if (health < -1)
                {
                    health = maxHealth;
                }
                return health;
            }
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
            theCore.UpdateConnectedToCore();
        }

        protected void UpdateStructure()
        {
            SetAllowedDisallowedMaterial();
        }

        private void OnDestroy()
        {
            Core.
        }
    }
}