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
        private float moveSpeed = 1;
        [SerializeField]
        private Vector2 defaultPivotPostion = Vector2.zero;

        private Vector3 MouseDragAnchor;
        
        [Header("Pivot Boundaries")]
        [SerializeField]
        Vector2 maxBoundaryCorner;
        [SerializeField]
        Vector2 minBoundaryCorner;

        [Header("Pivot Rotation Settings")]
        //the maximum angle from the ground
        [SerializeField]
        private float maxAngle = 89.99f;
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
        [SerializeField]
        private float zoomAmount = 5;

        private float targetCameraDistance = 100;

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
            return 0;
        }

        void Move(Vector2 _offset)
        {
            gameObject.transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + _offset.x, transform.position.y, transform.position.z + _offset.y), 0.1f);
        }

        void Rotate(Vector2 _rotationOffset)
        {
            Vector3 rotateValue = new Vector3(-_rotationOffset.y, _rotationOffset.x, 0);
            transform.eulerAngles += rotateValue;
        }

        Vector3 ConvertToV3(Vector2 input)
        {
            return new Vector3(input.x, 0, input.y);
        }

        private void OnValidate()
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.right * defaultAngle);
            gameObject.transform.position = new Vector3(defaultPivotPostion.x, TargetHeight(new Vector2(transform.position.x, transform.position.z)) + pivotHeight, defaultPivotPostion.y);
            //if the camera is assigned, correctly set its position;
            if (attatchedCamera)
            {
                if (attatchedCamera.transform.parent == gameObject.transform)
                    attatchedCamera.transform.parent = gameObject.transform;
                attatchedCamera.transform.localPosition = Vector3.back * DefaultDistance;
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position + transform.forward * 5, transform.position - transform.forward * 5, Color.blue);
            Debug.DrawLine(transform.position + transform.right * 5, transform.position - transform.right * 5, Color.red);
            Debug.DrawLine(transform.position + transform.up * 5, transform.position - transform.up * 5, Color.green);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(MouseDragAnchor, 2);
            Gizmos.color = Color.grey; 
            Gizmos.DrawWireSphere(transform.position, 2);
            Gizmos.color = Color.Lerp(Color.clear, Color.white, 0.5f);
            Gizmos.DrawWireCube((ConvertToV3(maxBoundaryCorner * 0.5f + minBoundaryCorner) + Vector3.up * 100f), new Vector3(maxBoundaryCorner.x - minBoundaryCorner.x, 200, maxBoundaryCorner.y - minBoundaryCorner.y));
            Gizmos.DrawCube(MouseDragAnchor, new Vector3(5, 0, 5));
            Gizmos.DrawSphere(gameObject.transform.position + Vector3.down * pivotHeight, 5);
        }

        void EnforceBoundaries()
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minBoundaryCorner.x, maxBoundaryCorner.x), transform.position.y, Mathf.Clamp(transform.position.z, minBoundaryCorner.y, maxBoundaryCorner.y));
            transform.eulerAngles = new Vector3(Mathf.Clamp(transform.eulerAngles.x, minAngle, maxAngle), transform.eulerAngles.y, transform.eulerAngles.z);
        }

        // Update is called once per frame
        void Update()
        {
            
            if (Input.GetMouseButtonDown(1))
            {
                LayerMask terrain = LayerMask.GetMask("Terrain");
                MouseDragAnchor = MouseRayHitPoint(terrain);
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 mouseOffsetPosition = MouseRayHitPoint(MouseDragAnchor.y);
                Move(new Vector2(MouseDragAnchor.x - mouseOffsetPosition.x, MouseDragAnchor.z - mouseOffsetPosition.z));
            }

            if (!Input.GetMouseButton(1))
            {
                Vector3 flattenedForwardVector = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
                Vector3 targetOffset = (transform.right * Input.GetAxisRaw("Horizontal") + flattenedForwardVector * Input.GetAxisRaw("Vertical")) * moveSpeed;
                transform.position = Vector3.Lerp(transform.position, transform.position + targetOffset, 0.1f);
                //gameObject.transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed, 0, Input.GetAxisRaw("Vertical") * moveSpeed).normalized);
            }

            if (Input.GetMouseButtonDown(2))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Input.GetMouseButton(2))
            {
                Vector2 rotation = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                Rotate(rotation);
            }
            else if (Input.GetMouseButtonUp(2))
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                targetCameraDistance -= targetCameraDistance / (Input.mouseScrollDelta.y * zoomAmount);
                targetCameraDistance = Mathf.Clamp(targetCameraDistance, MinDistance, MaxDistance);
            }
            
            attatchedCamera.transform.localPosition = Vector3.Lerp(attatchedCamera.transform.localPosition, Vector3.back * targetCameraDistance, 0.1f);
            
            EnforceBoundaries();
            transform.position = new Vector3(transform.position.x, TargetHeight(new Vector2(transform.position.x, transform.position.z)) + pivotHeight, transform.position.z);
            
        }
    }
}