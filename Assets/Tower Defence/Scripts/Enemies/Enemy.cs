using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;

public class Enemy : MonoBehaviour, IKillable
{
    private float acceleration;
    [SerializeField]
    private float maxVelocity = 10;
    //the amount the gameobject moves each physics update.
    private float velocity;

    [SerializeField]
    private float aiVisionRadius = 2.5f;
    [SerializeField]
    private float aiFieldOfView =  45f;
    [SerializeField]
    private int AiVisionPointNumber = 36;

    [SerializeField]
    private float aiTargetWeight = 2;

    private Transform aiTarget;

    private Vector2 direction2D;
    private Vector3 direction3D;

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
    public float Health { get => health; set => health = value; }

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

    private void Start()
    {
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
        CalculateDirection(AiVisionPointNumber, aiFieldOfView);
        transform.position = new Vector3(transform.position.x, TargetHeight(ConvertToVector2(transform.position)), transform.position.z);
        transform.position = transform.position + transform.forward * maxVelocity * Time.deltaTime;
        setRotationOnGround();     
    }

    public void Damage()
    {
        throw new System.NotImplementedException();
    }

    public void Kill()
    {
        throw new System.NotImplementedException();
    }
}
