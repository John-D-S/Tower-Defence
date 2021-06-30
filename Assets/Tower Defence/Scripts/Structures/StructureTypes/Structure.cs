using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;
using static StaticObjectHolder;
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
        [SerializeField, Tooltip("What tag is this structure is allowed to be placed on")]
        public string allowedGroundLayer = "Terrain";
        [SerializeField]
        private float radius = 0.5f;
        [SerializeField]
        private float height = 3f;
        [System.NonSerialized]
        public bool isIntersecting;

        [SerializeField]
        private float coreConnectionRadius = 10f;

        [SerializeField]
        protected Collider thisCollider;

        [System.NonSerialized, HideInInspector]
        public MeshRenderer connectionIndicator;

        public bool CanBePlaced
        {
            get
            {
                Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
                bool isOnTopOfAllowedLayer = Physics.Raycast(ray, 1.1f, ~LayerMask.NameToLayer(allowedGroundLayer));
                return (!IntersectingOtherStructure() && metalCostToBuild <= EconomyTracker.metal && isOnTopOfAllowedLayer);
            }
        }

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
                    currentStructures[gameObject] = this;
                    theCore.UpdateConnectedStructures();
                }

                if (preview)
                    SetMaterial(allowedPreviewMaterial);
                else
                    SetMaterial(realMaterial);
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

        protected static Dictionary<GameObject, Structure> currentStructures = new Dictionary<GameObject, Structure>();
        
        public static void RemoveStructureFromRecords(GameObject structureGameObject)
        {
            Structure structureToRemove = currentStructures[structureGameObject];
            foreach (KeyValuePair<GameObject, Structure> gOStructurePair in currentStructures)
            {
                if (gOStructurePair.Value != structureToRemove)
                {
                    gOStructurePair.Value.RemoveFromNearbyStructures(structureToRemove);
                }
            }
            currentStructures.Remove(structureGameObject);
        }

        public void RemoveFromNearbyStructures(Structure structureToRemove)
        {
            if (nearbyStructures.Contains(structureToRemove))
            {
                nearbyStructures.Remove(structureToRemove);
            }
        }

        private List<Structure> nearbyStructures = new List<Structure>();
        public void FindNearbyStructures()
        {
            //a list of all structure Colliders within checkConnectionRadius
            List<Collider> nearbyStructureColliders = new List<Collider>(Physics.OverlapSphere(transform.position, coreConnectionRadius, LayerMask.GetMask(new string[] { "Structure", "EnemyTarget" })));
            List<GameObject> nearbyStructureGameObjects = new List<GameObject>();
            foreach (Collider collider in nearbyStructureColliders)
            {
                nearbyStructureGameObjects.Add(collider.gameObject);
            }

            //adding new structures that have appeared within checkConnection Radius
            foreach (GameObject structureGameObject in nearbyStructureGameObjects)
            {
                //The structure component attatched to structureGameObject
                Structure structureComponent;
                //if this nearby structure is not yet recorded in nearbyStructures, add it.
                if (!nearbyStructures.Contains(currentStructures[structureGameObject]))
                {
                    structureComponent = structureGameObject.GetComponent<Structure>();
                    currentStructures[structureGameObject] = structureComponent;
                    nearbyStructures.Add(structureComponent);
                }
            }
        }

        protected void BecomeConnected()
        {
            currentStructures[gameObject] = this;
            isConnectedToCore = true;
            Invoke("ConnectNearbyStructures", Time.fixedDeltaTime);
        }

        private void ConnectNearbyStructures()
        {
            FindNearbyStructures();
            foreach (Structure structure in nearbyStructures)
            {
                if (!structure.isConnectedToCore)
                {
                    structure.BecomeConnected();
                }
            }
        }

        [HideInInspector]
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
            {
                if (meshRenderer != connectionIndicator)
                    meshRenderer.material = _material;
            }
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
                if (CanBePlaced)
                    SetMaterial(allowedPreviewMaterial);
                else
                    SetMaterial(disallowedPreviewMaterial);
            }
        }

        public virtual bool IntersectingOtherStructure()
        {
            if (Preview)
            {
                Vector3 point0 = transform.position + Vector3.up * (height * 0.5f - radius);
                Vector3 point1 = transform.position - Vector3.up * (height * 0.5f - radius);
                if (Physics.OverlapCapsule(point0, point1, radius, LayerMask.GetMask("Structure")).Length > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        protected void InitializeMeshRendering()
        {
            meshRenderers.Clear();
            foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>(true))
            {
                meshRenderers.Add(renderer);
            }
        }

        protected void InitializeHealth() => Health = maxHealth;

        private void OnValidate()
        {
            thisCollider = gameObject.GetComponentInChildren<Collider>();
            InitializeMeshRendering();
        }

        protected void StartStructure()
        {
            InitializeHealth();
            InitializeMeshRendering();
        }
        
        protected void UpdateStructure()
        {
            if (Preview)
            {
                SetAllowedDisallowedMaterial();
            }
        }

        private void OnDrawGizmos()
        {
            if (isConnectedToCore)
                Gizmos.color = Color.green * new Color(0, 1, 0, 0.25f);
            else
                Gizmos.color = Color.red * new Color(1, 0, 0, 0.25f);

            if (!Preview)
            {
                Gizmos.DrawSphere(transform.position, 5);
            }
        }

        protected void StructureOnDestroy()
        {
            if (!Preview)
            {
                RemoveStructureFromRecords(gameObject);
                if (theCore)
                {
                    theCore.UpdateConnectedStructures();
                }
            }
        }
    }
}