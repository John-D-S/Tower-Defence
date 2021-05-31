using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;

public class Enemy : MonoBehaviour
{
    private float maxVelocity;
    //the amount the gameobject moves each physics update.
    private float velocity;

    //this should always be set at the beginning of the physics update
    private Vector2 FacingDirection
    {
        get
        {
            return ConvertToVector2(transform.forward).normalized;
        }
        set
        {
            transform.rotation.SetLookRotation(ConvertToVector3(value));
            transform.rotation.SetFromToRotation(transform.up, GroundNormal);
        }
    }

    private Vector3 GroundNormal
    {
        get
        {
            return GetGroundNormal(ConvertToVector2(transform.position));
        }
        set { }
    }

    void Move()
    {
        Vector2 GroundNormal2D = ConvertToVector2(GroundNormal);
        float xMoveAmount = FacingDirection.x * velocity / Mathf.Abs(GroundNormal2D.x);
        float yMoveAmount = FacingDirection.y * velocity / Mathf.Abs(GroundNormal2D.y);

    }

    //void ApplyGravity()
}
