using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField]
        private GameObject attatchedCamera;

        [Header("Pivot Position Settings")]
        [SerializeField]
        private float pivotHeight = 5;
        [SerializeField]
        private Vector2 defaultPivotPostion = Vector2.zero;

        [Header("Pivot Boundaries")]
        [SerializeField]
        Vector2 maxBoundaryCorner;
        [SerializeField]
        Vector2 minBoundaryCorner;

        [Header("Pivot Rotation Settings")]
        //the maximum angle from the ground
        [SerializeField]
        private float maxAngle = 90;
        [SerializeField]
        private float minAngle = 10;
        [SerializeField]
        private float defaultAngle = 45;

        [Header("Camera Distance Settings")]
        [SerializeField]
        private float MinDistance = 10;
        [SerializeField]
        private float MaxDistance = 500;
        [SerializeField]
        private float DefaultDistance = 100f;

        private Vector3 MouseDragAnchor;

        #region MouseRayHitPoint function and overload
        private Vector3 MouseRayHitPoint(LayerMask _layerMask)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
            {
                //Debug.Log("layerMask hit terrain");
                return hit.point;
            }
            //Debug.LogError("layerMask did not hit anything");
            return Vector3.zero;
        }

        private Vector3 MouseRayHitPoint()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return hit.transform.position;
            }
            else return Vector3.zero;
        }
        
        private Vector3 MouseRayHitPoint(float TargetHeight)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane hPlane = new Plane(Vector3.up, Vector3.up * TargetHeight);
            float distance = 0;
            if (hPlane.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }
            else return Vector3.zero;
        }
        #endregion

        float TargetHeight(Vector2 _targetPosition)
        {
            LayerMask terrain = LayerMask.GetMask("Terrain");
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(_targetPosition.x, 1000, _targetPosition.y), Vector3.down, out hit, Mathf.Infinity, terrain))
            {
                return hit.point.y;
            }
            Debug.LogError("TargetHeight didn't work");
            return 0;
        }

        void Move(Vector2 _offset)
        {
            gameObject.transform.position = new Vector3(transform.position.x + _offset.x, transform.position.y, transform.position.z + _offset.y);
        }

        void Rotate(Vector2 _rotationOffset)
        {
            Vector3 currentRotation = gameObject.transform.localEulerAngles;
            gameObject.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Clamp(currentRotation.x + _rotationOffset.x, minAngle, maxAngle), currentRotation.y, currentRotation.z));
            gameObject.transform.Rotate(0, _rotationOffset.x, 0);
        }

        private void OnValidate()
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.right * defaultAngle);
            gameObject.transform.position = new Vector3(defaultPivotPostion.x, TargetHeight(new Vector2(transform.position.x, transform.position.z)), defaultPivotPostion.y);
            //if the camera is assigned, correctly set its position;
            if (attatchedCamera)
            {
                if (attatchedCamera.transform.parent == gameObject.transform)
                    attatchedCamera.transform.parent = gameObject.transform;
                attatchedCamera.transform.localPosition = Vector3.back * DefaultDistance;
            }
        }

        Vector3 ConvertToV3(Vector2 input)
        {
            return new Vector3(input.x, 0, input.y);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(MouseDragAnchor, 2);
            Gizmos.color = Color.grey; 
            Gizmos.DrawSphere(transform.position, 2);
            Gizmos.color = Color.Lerp(Color.clear, Color.white, 0.5f);
            Gizmos.DrawWireCube((ConvertToV3(maxBoundaryCorner * 0.5f + minBoundaryCorner) + Vector3.up * 100f), new Vector3(maxBoundaryCorner.x - minBoundaryCorner.x, 200, maxBoundaryCorner.y - minBoundaryCorner.y));
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = new Vector3(transform.position.x, TargetHeight(new Vector2(transform.position.x, transform.position.z)), transform.position.z);
            if (Input.GetMouseButtonDown(0))
            {
                LayerMask terrain = LayerMask.GetMask("Terrain");
                MouseDragAnchor = MouseRayHitPoint(terrain);
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 mouseOffsetPosition = MouseRayHitPoint(MouseDragAnchor.y);
                Debug.Log("mouseOffsetPosition: " + mouseOffsetPosition);
                Debug.Log("mouseDragAnchor: " + MouseDragAnchor);
                Move(new Vector2(MouseDragAnchor.x - mouseOffsetPosition.x, MouseDragAnchor.z - mouseOffsetPosition.z));
            }

            if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0)
            {
                Vector2 rotation = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
            }
        }
    }
}