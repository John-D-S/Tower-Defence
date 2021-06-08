using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private float aiFieldOfView =  45f;
    [SerializeField]
    private int aiVisionPointNumber = 36;

    [SerializeField]
    private float aiTargetWeight = 2;

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

    private Transform aiTarget;

    private Vector2 direction2D;
    private Vector3 direction3D;

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
        return weight;
    }

    void CalculateDirection(int _pointNumber, float fieldOfView)
    {
        List<Vector2> possibleDirections = new List<Vector2>();

        float angleIncrement = fieldOfView / _pointNumber;
        float startingAngle = Direction2Angle(direction2D) - fieldOfView * 0.5f;
        Vector2 directionToTarget = ConvertToVector2(aiTarget.position - transform.position).normalized;
        for (int i = 0; i < _pointNumber; i++)
        {
            //Debug.Log(Angle2Direction(i * angleIncrement));
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
        Debug.DrawLine(transform.position, transform.position + ConvertToVector3(bestDirection * aiVisionRadius), Color.green);
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
                Instantiate(bullet, turretBarrel.transform.position, turretBarrel.transform.rotation);
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
                bulletScript.bulletHitTargetTag = bulletHitTargetTag;
                bulletScript.bulletDamage = bulletDamage;
                bulletScript.bulletRange = range;
            }
            else
                bullet = null;
        }
    }
    
    private void Start()
    {
        if (!thisCollider)
        {
            thisCollider = gameObject.GetComponent<Collider>();
            Debug.Log("collider is now set");
        }
        health = maxHealth;
        foreach (GameObject obj in gameObject.scene.GetRootGameObjects())
        {
            if (obj.tag == "EnemyTarget")
            {
                aiTarget = obj.transform;
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        //AI and movement
        CalculateDirection(aiVisionPointNumber, aiFieldOfView);
        transform.position = new Vector3(transform.position.x, TargetHeight(ConvertToVector2(transform.position)), transform.position.z);
        transform.position = transform.position + transform.forward * maxVelocity * Time.deltaTime;
        setRotationOnGround();

        //shooting
        AimAndShoot();
    }
}