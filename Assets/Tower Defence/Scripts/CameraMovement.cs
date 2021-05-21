using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;
    
    [Header("Position Settings")]
    [SerializeField]
    private float pivotHeight = 5;
    [SerializeField]
    private float MinDistance = 10;
    [SerializeField]
    private float MaxDistance = 500;
    [SerializeField]
    private float DefaultDistance = 100f;

    [Header("Rotation Settings")]
    //the maximum angle from the ground
    [SerializeField]
    private float maxAngle = 90;
    [SerializeField]
    private float minAngle = 10;
    [SerializeField]
    private float defaultAngle = 45;


    private void OnValidate()
    {
        gameObject.transform.rotation = Quaternion.Euler(Vector3.right * defaultAngle); 
        
        //if the camera is assigned, correctly set its position;
        if (camera)
        {
            if (camera.transform.parent == gameObject.transform)
                camera.transform.parent = gameObject.transform;
            camera.transform.localPosition = Vector3.back * DefaultDistance;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
