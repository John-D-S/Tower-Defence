using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticObjectHolder;
using static HelperClasses.HelperFunctions;
using static HelperClasses.WeaponFunctions;


public class Enemy : MonoBehaviour, IKillable
{
    [Header("-- Movement Settings --")]
    [SerializeField, Tooltip("The Speed At which the enemy moves")]
    private float maxVelocity = 10;

    [Header("-- AI Settings --")]
    [SerializeField, Tooltip("The distance away from the enemy that they detect the gradient of the terrain")]
    private float aiVisionRadius = 2.5f;
    [SerializeField, Tooltip("The angle of the area in front of the enemy that the enemy can see.")]
    private float aiFieldOfView = 45f;
    [SerializeField, Tooltip("The number of points sampling the gradient of the terrain around the enemy")]
    private int aiVisionPointNumber = 36;
    [SerializeField, Tooltip("How much the enemy prioritizes travelling towards the core above avoiding steep terrain")]
    private float aiTargetWeight = 2;
    [SerializeField, Tooltip("The tag of the gameobject the enemy is trying to travel towards.")]
    private string aiTargetTag = "EnemyTarget";

    //a list of enemies within aiVisionRadius
    private List<Collider> nearbyEnemies = new List<Collider>();
    //a list of Structures within aiVisionRadius
    private List<Collider> nearbyStructures = new List<Collider>();

    //The transform of the core
    private Transform aiTarget;

    [Header("-- Shooting Settings --")]
    [SerializeField, Tooltip("The Furthest enemies can be from the turret before it stops fireing")]
    private float range = 50;
    [SerializeField, Tooltip("The number of projectiles fired Per Second")]
    private float fireRate = 1;
    [SerializeField, Tooltip("The bullet that the enemy shoots")]
    private GameObject bullet;
    [SerializeField, Tooltip("The speed of the bullet")]
    private float bulletSpeed = 50f;
    [SerializeField, Tooltip("How far forward from the pivot point of the barrel is the end of the barrel. The bullet will be spawned here")]
    private float tipOfBarrelOffset = 4f;
    [SerializeField, Tooltip("The amount of bullet spread in degrees")]
    private float spread = 5f;
    [SerializeField, Tooltip("The size of the bullet")]
    private float bulletRadius = 0.25f;
    [SerializeField, Tooltip("The tag of the gamobject the bullet is trying to hit")]
    private string bulletHitTargetTag;
    [SerializeField, Tooltip("How much damage the bullet does.")]
    private float bulletDamage = 10f;
    private bool canFire = true;

    [SerializeField, Tooltip("The part of the turret that rotates horizontally")]
    private GameObject turretBase;
    [SerializeField, Tooltip("The part that shoots the bullet")]
    private GameObject turretBarrel;

    [SerializeField, Tooltip("The damage the core takes when the enemy collides with it.")]
    private float coreDamage = 10f;
    public float CoreDamage
    {
        get
        {
            return coreDamage;
        }
        set {}
    }

    //what direction the tank is facing in 2d on the horizontal X-Z plane
    private Vector2 direction2D;
    //what direction the tank is facing in 3d space
    private Vector3 direction3D;

    [Header("Health Settings")]
    [SerializeField, Tooltip("how many minutes until the enemy self destructs")]
    private float lifeTime = 5;
    [SerializeField, Tooltip("The healthBar of the enemy")]
    HealthBar healthBar;

    /// <summary>
    /// the normal of the ground directly beneath the enemy
    /// </summary>
    private Vector3 GroundNormal
    {
        get
        {
            return GetGroundNormal(ConvertToVector2(transform.position));
        }
        set { }
    }

    [SerializeField, Tooltip("The maximum amount of health the enemy has.")]
    private float maxHealth = 100;
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    private float health;
    /// <summary>
    /// Use this to set the health of the enemy.
    /// </summary>
    public float Health 
    { 
        get => health;
        set
        {
            //if the health goes down below zero, die
            if (value < 0)
            {
                health = 0;
                Die();
            }
            else if (value > maxHealth)// if the health is greater than maxhealth, clamp it back down to max health
                health = maxHealth;
            else//otherwise just set the health to the value
                health = value;

            //set the healthbar to display the correct health
            if (healthBar)
            {
                healthBar.SetHealth(value, MaxHealth);
            }
        }
    }
    //use these to change the health
    public void Damage(float amount) => Health -= amount;
    public void Heal(float amount) => Health += amount;
    //this kills the enemy
    public void Die() => Destroy(gameObject);

    [SerializeField, Tooltip("The amount of metal to reward the player when the enemy dies")]
    private int rewardMetal = 10;

    [Header("-- Effect Settings --")]
    [SerializeField, Tooltip("The size of the muzzleFlash")]
    private float fireEffectScale = 1;
    [SerializeField, Tooltip("the muzzleflash gameobject")]
    private GameObject fireEffect;

    private Collider thisCollider;

    /// <summary>
    /// rotate the enemy to sit correctly on the ground
    /// </summary>
    private void setRotationOnGround()
    {
        direction3D = ConvertToVector3(direction2D);
        direction3D = Quaternion.FromToRotation(Vector3.up, GroundNormal) * direction3D;
        transform.rotation = Quaternion.LookRotation(direction3D, GroundNormal);
    }

    /// <summary>
    /// Find out how desirable a certain direction is to travel in.
    /// </summary>
    /// <param name="_direction">The direction to calculate</param>
    /// <param name="_directionToTarget">What direction the target is in</param>
    /// <returns></returns>
    float CalculateDirectionWeight(Vector2 _direction, Vector2 _directionToTarget)
    {
        float weight = 0;

        //the ground normal at the point aivisonradius away from the enemy in _direction direction
        float groundNormalYAtPoint = GetGroundNormal(ConvertToVector2(transform.position) + _direction * aiVisionRadius).y;

        Vector2 groundNormalDirection = ConvertToVector2(GetGroundNormal(ConvertToVector2(transform.position) + _direction * aiVisionRadius)).normalized; 
        //the weight is slightly greater when going downhill than when going uphill
        float downhillBias = ((Vector2.Dot(groundNormalDirection, _direction) + 1) * 0.5f + 1) * 0.5f;
        weight += downhillBias;


        weight += Mathf.Pow(groundNormalYAtPoint, 2) * ((Vector2.Dot(_directionToTarget, _direction) + 1) * 0.5f + 1) * 0.5f * aiTargetWeight;
        
        //obstical avoidance;
        //this could be put into a function.
        if (nearbyEnemies.Count > 0f)
        {
            foreach (Collider enemyCollider in nearbyEnemies)
            {
                // this if statement is here because we might be trying to reference an object that was deleted in the last frame.
                if (enemyCollider)
                {
                    float distanceToEnemyCollider = Vector3.Distance(enemyCollider.transform.position, transform.position);
                    weight *= Mathf.Lerp((-Vector2.Dot(ConvertToVector2(enemyCollider.transform.position) - ConvertToVector2(transform.position), _direction) + 1), 1, distanceToEnemyCollider / aiVisionRadius);
                }
            }
        }

        //this does the same as the above if statement but for avoiding structures
        if (nearbyStructures.Count > 0f)
        {
            foreach (Collider structureCollider in nearbyStructures)
            {
                if (structureCollider)
                {
                    float distanceToStructureCollider = Vector3.Distance(structureCollider.transform.position, transform.position);
                    weight *= Mathf.Lerp((Vector2.Dot(ConvertToVector2(transform.position) - ConvertToVector2(structureCollider.transform.position), _direction) + 1), 1, Mathf.Pow(distanceToStructureCollider / aiVisionRadius, 2f));
                }
            }
        }

        return weight;
    }

    /// <summary>
    /// Figure out what direction to turn towards based on the enemy's surroundings and the direction to the core
    /// </summary>
    /// <param name="_pointNumber">The number of points to test around the enemy</param>
    /// <param name="fieldOfView">the angle infront of the enemy that points are tested</param>
    void CalculateDirection(int _pointNumber, float fieldOfView)
    {
        //a list of points to test
        List<Vector2> possibleDirections = new List<Vector2>();

        //the angle between points to test
        float angleIncrement;
        float startingAngle;

        //if there are not any nearby structures, calculate the direction from the points within the field of view infront of the enemy
        if (nearbyStructures.Count == 0)
        {
            angleIncrement = fieldOfView / _pointNumber;
            startingAngle = Direction2Angle(direction2D) - fieldOfView * 0.5f;
        }
        else //if there are structures nearby, calculate points all around the enemy
        {
            angleIncrement = 360 / _pointNumber;
            startingAngle = 0f;
        }

        //the direction from the enemy to the target
        Vector2 directionToTarget = ConvertToVector2(aiTarget.position - transform.position).normalized;

        //add all the possible directions to the list
        for (int i = 0; i < _pointNumber; i++)
        {
            possibleDirections.Add(Angle2Direction(startingAngle + i * angleIncrement));
        }
        //initialize the variables for finding the best direction.
        float currentHighestWeight = 0;
        Vector2 bestDirection = Vector2.zero;
        //calculate the weights for each direction in possible directions, then choose the best one to turn towards.
        foreach (Vector2 direction in possibleDirections)
        {
            float directionWeight = CalculateDirectionWeight(direction, directionToTarget);
            Debug.DrawLine(transform.position, transform.position + ConvertToVector3(direction * Mathf.Pow(directionWeight, 2)), Color.red, Time.deltaTime, false);
            //update the current heighest weight
            if (directionWeight > currentHighestWeight)
            {
                currentHighestWeight = directionWeight;
                bestDirection = direction;
            }
        }
        Debug.DrawLine(transform.position, transform.position + ConvertToVector3(bestDirection * aiVisionRadius), Color.green, Time.deltaTime, false);
        direction2D = Vector2.Lerp(direction2D, bestDirection, 0.1f);
    }

    /// <summary>
    /// Aim towards the nearest visible structure and shoot at it
    /// </summary>
    private void AimAndShoot()
    {
        //find the nearest target
        Transform target = NearestVisibleTarget(turretBarrel, range, LayerMask.GetMask("Structure"));
        //if the target exists, shoot at it.
        if (target)
        {
            //aim at the target
            AimAtTarget(target, gameObject, ref turretBase, ref turretBarrel);
            //if the enemy can fire
            if (canFire)
            {
                //instantiate the bullet and the muzzleflash and start the fireCooldownCoroutine.
                Vector3 bulletStartPosition = turretBarrel.transform.position + turretBarrel.transform.rotation * Vector3.forward * tipOfBarrelOffset;
                GameObject instantiatedFireEffect = Instantiate(fireEffect, bulletStartPosition, turretBarrel.transform.rotation);
                instantiatedFireEffect.transform.localScale = Vector3.one * fireEffectScale;
                Instantiate(bullet, bulletStartPosition, turretBarrel.transform.rotation);
                StartCoroutine(FireCooldown());
            }
        }
    }

    /// <summary>
    /// called when firing to enforce the firerate
    /// </summary>
    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(1 / fireRate);
        canFire = true;
    }

    /// <summary>
    /// find nearby structures and enemies.
    /// </summary>
    void FindNearbyObjects()
    {
        //clear the list
        nearbyEnemies.Clear();
        nearbyStructures.Clear();
        //add the colliders of enemies and structures within aivisionradius to their respective lists
        Collider[] nearbyEnemiesincludingThis = Physics.OverlapSphere(transform.position, aiVisionRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider enemyCollider in nearbyEnemiesincludingThis)
        {
            if (enemyCollider != thisCollider)
                nearbyEnemies.Add(enemyCollider);
        }
        Collider[] nearbyStructuresArray = Physics.OverlapSphere(transform.position, aiVisionRadius, LayerMask.GetMask("Structure", "EnemyTarget"));
        bool addCoreCollider = Physics.CheckSphere(transform.position, aiVisionRadius, LayerMask.GetMask(aiTargetTag));
        foreach (Collider structureCollider in nearbyStructuresArray)
        {
            nearbyStructures.Add(structureCollider);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + GroundNormal * 3);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + direction3D * 5);
    }
    
    void OnValidate()
    {
        //set the values of the bullet
        if (bullet)
        {
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript)
            {
                bulletScript.spawningCollider = GetComponent<Collider>();
                bulletScript.bulletSpeed = bulletSpeed;
                bulletScript.spread = spread;
                bulletScript.bulletRadius = bulletRadius;
                bulletScript.targetTag = bulletHitTargetTag;
                bulletScript.bulletDamage = bulletDamage;
                bulletScript.bulletRange = range;
            }
            else //if the bullet doesn't have a bullet script, remove it.
                bullet = null;
        }
    }

    private void Start()
    {
        //initialize the variables
        if (!thisCollider)
        {
            thisCollider = gameObject.GetComponent<Collider>();
        }
        health = maxHealth;
        foreach (GameObject obj in gameObject.scene.GetRootGameObjects())
        {
            if (obj.tag == aiTargetTag)
            {
                aiTarget = obj.transform;
                break;
            }
        }

        //memento mori
        Destroy(this.gameObject, lifeTime * 60f);
    }


    private void FixedUpdate()
    {
        if (theCore)
        {
            //AI and movement
            FindNearbyObjects();
            CalculateDirection(aiVisionPointNumber, aiFieldOfView);
            transform.position = new Vector3(transform.position.x, TargetHeight(ConvertToVector2(transform.position)), transform.position.z);
            transform.position = transform.position + transform.forward * maxVelocity * Time.deltaTime;
            setRotationOnGround();

            //shooting
            AimAndShoot();
        }
    }

    private void OnDestroy()
    {
        //reward the player with rewardmetal when the enemy dies.
        Economy.EconomyTracker.TryIncrementMetal(rewardMetal);
    }
}