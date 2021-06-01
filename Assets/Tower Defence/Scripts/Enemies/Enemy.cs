using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;

public class Enemy : MonoBehaviour
{
    private float acceleration;
    [SerializeField]
    private float maxVelocity = 10;
    //the amount the gameobject moves each physics update.
    private float velocity;

    [SerializeField]
    private float aiVisionRadius;
    [SerializeField]
    private int AiVisionPointNumber = 36;
    [SerializeField]
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

    private void setRotationOnGround()
    {
        direction3D = ConvertToVector3(direction2D);
        direction3D = Quaternion.FromToRotation(Vector3.up, GroundNormal) * direction3D;
        transform.rotation = Quaternion.LookRotation(direction3D, GroundNormal);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + GroundNormal * 3);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + direction3D * 5);
    }

    private Vector2 Angle2Direction(float _angle)
    {
        return ConvertToVector2(Quaternion.Euler(0, _angle, 0) * Vector3.forward);
    }

    
    private void FixedUpdate()
    {
        CalculateDirection(AiVisionPointNumber);
        transform.position = new Vector3(transform.position.x, TargetHeight(ConvertToVector2(transform.position)), transform.position.z);
        transform.position = transform.position + transform.forward * maxVelocity * Time.deltaTime;
        setRotationOnGround();     
    }

    float CalculateDirectionWeight(Vector2 _direction, Vector2 _directionToTarget)
    {
        float weight = 0;

        float groundNormalYAtPoint = GetGroundNormal(ConvertToVector2(transform.position) + _direction * aiVisionRadius).y;
        weight += groundNormalYAtPoint;

        weight += (Vector2.Dot(_directionToTarget, _direction) + 1) * 0.25f;
        return weight;
    }

    void CalculateDirection(int _pointNumber)
    {
        List<Vector2> possibleDirections = new List<Vector2>();

        float angleIncrement = 360f / _pointNumber;
        Vector2 directionToTarget = ConvertToVector2(aiTarget.position - transform.position).normalized;
        for (int i = 0; i < _pointNumber; i++)
        {
            //Debug.Log(Angle2Direction(i * angleIncrement));
            possibleDirections.Add(Angle2Direction(i * angleIncrement));
        }
        float currentHighestWeight = 0;
        Vector2 bestDirection = Vector2.zero;
        foreach (Vector2 direction in possibleDirections)
        {
            
            float directionWeight = CalculateDirectionWeight(direction, directionToTarget);
            Debug.DrawLine(transform.position, transform.position + ConvertToVector3(direction * Mathf.Pow(directionWeight, 6)), Color.red, Time.deltaTime, false);
            if (directionWeight > currentHighestWeight)
            {
                currentHighestWeight = directionWeight;
                bestDirection = direction;
            }
        }
        Debug.DrawLine(transform.position, transform.position + ConvertToVector3(bestDirection * aiVisionRadius), Color.green);
        direction2D = Vector2.Lerp(direction2D, bestDirection, 0.1f);
    }

    private void OnValidate()
    {

    }
}
