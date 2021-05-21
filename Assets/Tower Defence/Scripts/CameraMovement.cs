using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
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

        float TargetHeight(Vector2 _targetPosition)
        {
            LayerMask terrain = LayerMask.GetMask("Terrain");
            RaycastHit hit;
            if (Physics.Raycast(gameObject.transform.position + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, terrain))
            {
                return hit.point.y;
            }
            else return 0;
        }

        void Move(Vector2 _offset)
        {
            gameObject.transform.position = new Vector3(_offset.x, TargetHeight(_offset), _offset.y);
        }

        void Rotate(Vector2 _rotation)
        {
            gameObject.transform.rotation = Quaternion.Euler(Mathf.Clamp(_rotation.x), _rotation.y, 0);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0)
            {
                Vector2 rotation = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
            }

            
        }
    }
}