using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticObjectHolder;
using static HelperClasses.HelperFunctions;
using static HelperClasses.WeaponFunctions;


public class Enemy : MonoBehaviour, IKillable
{
    [Header("-- Movement Settings --")]
    private float acceleration;
    [SerializeField]
    private float maxVelocity = 10;
    //the amount the gameobject moves each physics update.
    private float velocity;

    [Header("-- AI Settings --")]
    [SerializeField]
    private float aiVisionRadius = 2.5f;
    [SerializeField]
    private float aiFieldOfView = 45f;
    [SerializeField]
    private int aiVisionPointNumber = 36;
    [SerializeField]
    private float aiTargetWeight = 2;
    [SerializeField]
    private string aiTargetTag = "EnemyTarget";

    private List<Collider> nearbyEnemies = new List<Collider>();
    private List<Collider> nearbyStructures = new List<Collider>();

    private Transform aiTarget;
    private Collider aiTargetCollider;

    [Header("-- Shooting Settings --")]
    [SerializeField, Tooltip("The Furthest enemies can be from the turret before it stops fireing")]
    private float range = 50;
    [SerializeField, Tooltip("The number of projectiles fired Per Second")]
    private float fireRate = 1;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private float bulletSpeed = 50f;
    [SerializeField]
    private float tipOfBarrelOffset = 4f;
    [SerializeField]
    private float spread = 5f;
    [SerializeField]
    private float bulletRadius = 0.25f;
    [SerializeField]
    private string bulletHitTargetTag;
    [SerializeField]
    private float bulletDamage = 10f;
    private bool canFire = true;

    [SerializeField]
    private GameObject turretBase;
    [SerializeField]
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

    private Vector2 direction2D;
    private Vector3 direction3D;

    [Header("Health Settings")]
    [SerializeField, Tooltip("how many minutes until the enemy self destructs")]
    private float lifeTime = 5;
    [SerializeField]
    HealthBar healthBar;

    private Vector3 GroundNormal
    {
        get
        {
            return GetGroundNormal(ConvertToVector2(transform.position));
        }
        set { }
    }

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
        }
    }
    public void Damage(float amount) => Health -= amount;
    public void Heal(float amount) => Health += amount;
    public void Die() => Destroy(gameObject);

    [SerializeField]
    private int rewardMetal = 10;

    [Header("-- Effect Settings --")]
    [SerializeField]
    private float fireEffectScale = 1;
    [SerializeField]
    private GameObject fireEffect;

    private Collider thisCollider;

    private void setRotationOnGround()
    {
        direction3D = ConvertToVector3(direction2D);
        direction3D = Quaternion.FromToRotation(Vector3.up, GroundNormal) * direction3D;
        transform.rotation = Quaternion.LookRotation(direction3D, GroundNormal);
    }

    float CalculateDirectionWeight(Vector2 _direction, Vector2 _directionToTarget)
    {
        float weight = 0;

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

    void CalculateDirection(int _pointNumber, float fieldOfView)
    {
        List<Vector2> possibleDirections = new List<Vector2>();

        float angleIncrement;
        float startingAngle;
        if (nearbyStructures.Count == 0)
        {
            angleIncrement = fieldOfView / _pointNumber;
            startingAngle = Direction2Angle(direction2D) - fieldOfView * 0.5f;
        }
        else
        {
            angleIncrement = 360 / _pointNumber;
            startingAngle = 0f;
        }

        Vector2 directionToTarget = ConvertToVector2(aiTarget.position - transform.position).normalized;

        for (int i = 0; i < _pointNumber; i++)
        {
            possibleDirections.Add(Angle2Direction(startingAngle + i * angleIncrement));
        }
        float currentHighestWeight = 0;
        Vector2 bestDirection = Vector2.zero;
        foreach (Vector2 direction in possibleDirections)
        {
            
            float directionWeight = CalculateDirectionWeight(direction, directionToTarget);
            Debug.DrawLine(transform.position, transform.position + ConvertToVector3(direction * Mathf.Pow(directionWeight, 2)), Color.red, Time.deltaTime, false);
            if (directionWeight > currentHighestWeight)
            {
                currentHighestWeight = directionWeight;
                bestDirection = direction;
            }
        }
        Debug.DrawLine(transform.position, transform.position + ConvertToVector3(bestDirection * aiVisionRadius), Color.green, Time.deltaTime, false);
        direction2D = Vector2.Lerp(direction2D, bestDirection, 0.1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + GroundNormal * 3);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + direction3D * 5);
    }

    private void AimAndShoot()
    {
        Transform target = NearestVisibleTarget(turretBarrel, range, LayerMask.GetMask("Structure"));
        if (target)
        {
            AimAtTarget(target, gameObject, ref turretBase, ref turretBarrel);
            if (canFire)
            {
                Vector3 bulletStartPosition = turretBarrel.transform.position + turretBarrel.transform.rotation * Vector3.forward * tipOfBarrelOffset;
                GameObject instantiatedFireEffect = Instantiate(fireEffect, bulletStartPosition, turretBarrel.transform.rotation);
                instantiatedFireEffect.transform.localScale = Vector3.one * fireEffectScale;
                Instantiate(bullet, bulletStartPosition, turretBarrel.transform.rotation);
                StartCoroutine(FireCooldown());
            }
        }
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(1 / fireRate);
        canFire = true;
    }

    void OnValidate()
    {
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
            else
                bullet = null;
        }
    }
    
    void FindNearbyObjects()
    {
        nearbyEnemies.Clear();
        nearbyStructures.Clear();
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

    private void Start()
    {
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
                aiTargetCollider = obj.GetComponent<Collider>();
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
        Economy.EconomyTracker.TryIncrementMetal(rewardMetal);
    }
}