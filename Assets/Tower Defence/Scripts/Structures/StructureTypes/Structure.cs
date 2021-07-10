using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;
using static StaticObjectHolder;
using Economy;

namespace Structures
{
    public abstract class Structure : MonoBehaviour, IKillable
    {
        [Header("-- Material Settings --")]
        [Tooltip("The material displayed on all components of the structure when it is placed")]
        public Material realMaterial;
        [Tooltip("The material displayed on all components of the structure when it is a preview which is allowed to be placed.")]
        public Material allowedPreviewMaterial;
        [Tooltip("The material displayed on all components of the structure when it is a preview which is not allowed to be placed.")]
        public Material disallowedPreviewMaterial;
        private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        [Header("-- Area Settings --")]
        [SerializeField, Tooltip("What tag is this structure is allowed to be placed on")]
        public string allowedGroundLayer = "Terrain";
        [SerializeField, Tooltip("How far away from the collider of another structure does this structure have to be before it can be placed")]
        private float radius = 0.5f;
        [SerializeField, Tooltip("about how tall is this structure, from the bottom to the top")]
        private float height = 3f;
        [System.NonSerialized]
        public bool isIntersecting;

        [SerializeField, Tooltip("How close do other structures have to be to this structure in order to be connected by it.")]
        private float coreConnectionRadius = 10f;

        [SerializeField, Tooltip("The collider of this structure")]
        protected Collider thisCollider;

        // the orb that hovers above the structure that shows whether or not it is connected
        [System.NonSerialized, HideInInspector]
        public MeshRenderer connectionIndicator;

        /// <summary>
        /// finds out if this structure can be placed by:
        /// 1: finding if the layer of the gameobject it is sitting on is allowed
        /// 2: finding if there is enough metal stored to afford 
        /// 3: making sure it is not intersecting another structure
        /// </summary>
        public bool CanBePlaced
        {
            get
            {
                Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
                bool isOnTopOfAllowedLayer = Physics.Raycast(ray, 1.1f, ~LayerMask.NameToLayer(allowedGroundLayer));
                return (isOnTopOfAllowedLayer && metalCostToBuild <= EconomyTracker.metal && !IntersectingOtherStructure());
            }
        }

        private bool preview = true;
        /// <summary>
        /// sets the layer, tags, collider, healthbar and material of the structure when set. returns whether or not it is a preview when gotten.
        /// </summary>
        public bool Preview
        {
            set
            {
                //set the preview value
                preview = value;
                //if the structure is being set to preview, set the layer and tag of all children of the structure to "previewStructure"
                //and deactivate the healthbar and collider of this structure because they are not needed when the structure is just a preview
                //also set the material to previewMaterial
                if (preview)
                {
                    SetLayerOfAllChildren(gameObject, LayerMask.NameToLayer("PreviewStructure"));
                    SetTagOfAllChildren(gameObject, "PreviewStructure");
                    if (thisCollider)
                        thisCollider.enabled = false;
                    if (healthBar)
                        healthBar.gameObject.SetActive(false);
                    SetMaterial(allowedPreviewMaterial);
                }
                else
                {
                    //when the structure is not a preview, it is being placed, and should have all those values relating to that accordingly
                    //this is basically the opposite of the above if statement.
                    SetLayerOfAllChildren(gameObject, LayerMask.NameToLayer("Structure"));
                    SetTagOfAllChildren(gameObject, "Structure");
                    if (thisCollider)
                        thisCollider.enabled = true;
                    if (healthBar)
                        healthBar.gameObject.SetActive(true);
                    currentStructures[gameObject] = this;
                    theCore.UpdateConnectedStructures();
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

        [SerializeField, Tooltip("The healthBar that displays the health of this particular structure.")]
        HealthBar healthBar;
        [SerializeField, Tooltip("The maximum health of this structure")]
        private float maxHealth = 100;
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        //this is set by default to -2 so that the first time Health is gotten, health is automatically set to maxHealth.
        private float health = -2;
        //this is used to get and set the value of health, and perform all appropriate functions at certain levels
        public float Health
        {
            get
            {
                //if the health is less than -1 when it is gotten, set it to maxHealth. this is used to initialise the health to max when it starts
                if (health < -1)
                {
                    health = maxHealth;
                }
                return health;
            }
            set
            {
                //if the value that the health is set to is less than zero, kill the structure.
                if (value < 0)
                {
                    health = 0;
                    Die();
                }
                else if (value > maxHealth) //if the health is for some reason, set to a value greater than max health, set it back to max health.
                    health = maxHealth;
                else //otherwise, just set the health to the value
                    health = value;

                //update the healthbar to display the correct value
                if (healthBar)
                {
                    healthBar.SetHealth(Health, MaxHealth);
                }
            }
        }

        [SerializeField, Tooltip("how many HP to heal per second")]
        private int regenerationAmount = 1;
        //whether or not the structure is currently regenerating
        private bool isRegenerating = false;

        [SerializeField, Tooltip("how many seconds after being disconnected is the structure no longer able to function")]
        private float timeUntilStructureDoesntWork = 5f;
        //stores whether or not this structure can function
        private bool canFunction = false;
        // this is used by other scripts to find out if a structure can or cannot function
        public bool CanFunction
        {
            get => canFunction;
        }
        //the amount of time since this structure has been disconnected, used to determine when to stop functioning.
        private float timeSinceDisconnected = 0;

        /// <summary>
        /// removes an amount of health from the health of the structure and triggers it to try and start regenerating
        /// </summary>
        /// <param name="amount">the amount of health to remove</param>
        public virtual void Damage(float amount)
        {
            Health -= amount;
            TryStartRegeneration();
        }
        /// <summary>
        /// heals the structure's health by amount
        /// </summary>
        public void Heal(float amount) => Health += amount;
        /// <summary>
        /// Destroys the gameobject
        /// </summary>
        public virtual void Die() => Destroy(gameObject);

        #region Regeneration
        /// <summary>
        /// sets regeneration to true and adds regenerationAmount to health every second, consuming regenerationAmount from metal every second.
        /// when the health is full, set isRegenerating back to false
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartRegeneration()
        {
            isRegenerating = true;
            while (health != maxHealth)
            {
                if (EconomyTracker.metal > regenerationAmount)
                {
                    Heal(regenerationAmount);
                    EconomyTracker.TryIncrementMetal(regenerationAmount);
                }
                yield return new WaitForSeconds(1);
            }
            isRegenerating = false;
        }

        /// <summary>
        /// will start the StartRegeneration() coroutine if the structure is not already regenerating.
        /// </summary>
        private void TryStartRegeneration()
        {
            if (!isRegenerating)
            {
                StartCoroutine(StartRegeneration());
            }
        }
        #endregion

        /// <summary>
        /// updates whether or not the function can function
        /// </summary>
        private void UpdateCanFunction()
        {
            //the structure cannot function if it's health is zero or below
            if (health > 0)
            {
                //if the structure is connected to the core, set canFunction to true and reset the amount of time since the structure has been disconected back to 0
                if (isConnectedToCore)
                {
                    canFunction = true;
                    timeSinceDisconnected = 0;
                }
                else if (timeSinceDisconnected < timeUntilStructureDoesntWork)//if the structue is not connected, count until timeUntilStructureDoesntWork has elapsed, then set canfunction to false.
                {
                    timeSinceDisconnected += Time.fixedDeltaTime;
                }
                else
                {
                    canFunction = false;
                }
            }
            else
            {
                canFunction = false;
            }
        }

        #region Core Connection
        /// <summary>
        /// a static dictionary of all structures that exist and their corresponding gameobject
        /// this is to save calling getcomponent everytime getting the structure script from a gameobject is needed.
        /// </summary>
        protected static Dictionary<GameObject, Structure> currentStructures = new Dictionary<GameObject, Structure>();
        
        /// <summary>
        /// removes all records from all structure scripts of the structureGameObject given so that it is not referenced
        /// </summary>
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

        /// <summary>
        /// removes the given structure from the list of nearby structures
        /// </summary>
        public void RemoveFromNearbyStructures(Structure structureToRemove)
        {
            if (nearbyStructures.Contains(structureToRemove))
            {
                nearbyStructures.Remove(structureToRemove);
            }
        }

        //a list of all non preview structures within coreConnectionRadius
        private List<Structure> nearbyStructures = new List<Structure>();
        /// <summary>
        /// finds all non-preview structures within coreConnectionRadius and adds them to the nearbyStructuresList
        /// </summary>
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

        /// <summary>
        /// sets this structure to be connected and calls ConnectNearbyStructures() after a fixed update
        /// </summary>
        protected void BecomeConnected()
        {
            currentStructures[gameObject] = this;
            isConnectedToCore = true;
            Invoke("ConnectNearbyStructures", Time.fixedDeltaTime);
        }

        /// <summary>
        /// tells all structures in nearby structures to become connected to the core if they are not already
        /// </summary>
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

        //whether or not the structure is connected to the core. (it should probably be a property)
        [HideInInspector]
        public bool isConnectedToCore;
        #endregion

        /// <summary>
        /// set the material of all meshrenderers in the children of this transform to _material unless that particular child is the connection indicator
        /// </summary>
        private void SetMaterial(Material _material)
        {
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                if (meshRenderer != connectionIndicator)
                    meshRenderer.material = _material;
            }
        }

        /// <summary>
        /// try to place the structure by consuming the cost of the metal and instantiating a non preview version of this gameobject at the position it is currently at
        /// </summary>
        public void TryPlaceStructure(Vector3 _position)
        {
            if (EconomyTracker.TryIncrementMetal(-metalCostToBuild))
            {
                GameObject newObject = Instantiate(gameObject, _position, Quaternion.identity);
                newObject.GetComponent<Structure>().InitializeMeshRendering();
                newObject.GetComponent<Structure>().Preview = false;
            }
        }

        /// <summary>
        /// determine whether or not this preview structure is placed and set the preview material accordingly.
        /// </summary>
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

        /// <summary>
        /// whether or not this structure is intersecting with another structure
        /// </summary>
        public virtual bool IntersectingOtherStructure()
        {
            if (Preview)
            {
                Vector3 point0 = transform.position + Vector3.up * (height * 0.5f - radius);
                Vector3 point1 = transform.position - Vector3.up * (height * 0.5f - radius);
                if (Physics.OverlapCapsule(point0, point1, radius, LayerMask.GetMask("Structure", "CoreCollider")).Length > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// add all the meshrenderers of all the children of this gameobject. to meshRenderers
        /// </summary>
        protected void InitializeMeshRendering()
        {
            meshRenderers.Clear();
            foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>(true))
            {
                meshRenderers.Add(renderer);
            }
        }

        //set Health to MaxHealth
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
            UpdateCanFunction();
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
            //remove the this destroyed structures and propagate through all structures to see if they're still connected
            if (!Preview)
            {
                RemoveStructureFromRecords(gameObject);
                if (theCore)
                {
                    theCore.UpdateConnectedStructures();
                }
            }
        }

        private void FixedUpdate()
        {
            UpdateCanFunction();
        }
    }
}