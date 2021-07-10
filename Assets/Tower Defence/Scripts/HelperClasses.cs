using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperClasses : MonoBehaviour
{
    public static class WeaponFunctions
    {
        /// <summary>
        /// returns a quaternion which is randomly deviated from quaternion.identity by _spread degrees
        /// </summary>
        public static Quaternion RandomSpread(float _spread)
        {
            float randomXSpread = Random.Range(-_spread, _spread) * 0.5f;
            float randomYSpread = Random.Range(-_spread, _spread) * 0.5f;
            return Quaternion.Euler(randomXSpread, randomYSpread, 0);
        }

        /// <summary>
        /// returns the number of degrees tor rotate a transform on an axis to face the transform's local z axis towards a target
        /// </summary>
        public static float RotationTowardsTargetOnAxis(Transform objectToRotate, Vector3 target, Vector3 constraintAxis)
        {
            float angleToRotate = Vector3.SignedAngle(objectToRotate.forward, objectToRotate.position - target, constraintAxis);
            return angleToRotate;
        }

        /// <summary>
        /// aims the turret base and barrel toward the target.
        /// </summary>
        public static void AimAtTarget(Transform _target, GameObject _attatchedStructure, ref GameObject _turretBase, ref GameObject _turretBarrel)
        {
            Vector3 targetPos = _target.position;
            Vector3 transformedTargetPos = _attatchedStructure.transform.InverseTransformPoint(_target.position);

            Vector3 turretBaseTarget = new Vector3(targetPos.x, targetPos.y, targetPos.z);
            //the turret is aiming slightly above the position of the enemy so that it is less likely to be obscured by the terrain.
            Vector3 turretBarrelTarget = targetPos + Vector3.up * 0.5f;

            
            Quaternion turretBaseRotation = Quaternion.LookRotation(turretBaseTarget - _turretBase.transform.position, _attatchedStructure.transform.up);
            Quaternion turretBarrelRotation = Quaternion.LookRotation(turretBarrelTarget - _turretBarrel.transform.position, _attatchedStructure.transform.up);

            _turretBase.transform.rotation = turretBaseRotation;
            Vector3 turretBaseLocRot = _turretBase.transform.localRotation.eulerAngles;
            _turretBase.transform.localRotation = Quaternion.Euler(0, turretBaseLocRot.y, turretBaseLocRot.z);
            _turretBarrel.transform.rotation = turretBarrelRotation;
        }

        /// <summary>
        /// finds the nearest visible transform on the _TargetLayer with a collider
        /// </summary>
        public static Transform NearestVisibleTarget(GameObject _turretBarrel, float _range, LayerMask _targetLayer)
        {
            //check if there are any colliders within range
            if (Physics.CheckSphere(_turretBarrel.transform.position, _range, _targetLayer))
            {
                //iterate through each collider to find the nearest one
                Collider currentNearestVisibleCollider = null;
                float currentClosestDistance = Mathf.Infinity;
                Collider[] inRangeEnemyColliders = Physics.OverlapSphere(_turretBarrel.transform.position, _range, _targetLayer);
                foreach (Collider enemyCollider in inRangeEnemyColliders)
                {
                    float distance = Vector3.Distance(enemyCollider.transform.position, _turretBarrel.transform.position);
                    if (distance < currentClosestDistance)
                    {
                        //we only get colliders that we can get an unobstructed raycast to (the visible part of NearestVisibleTarget)
                        float enemyScale = enemyCollider.transform.localScale.y;
                        RaycastHit enemyHit;
                        //we are testing if we can see the top of the enemy because if we aim for the center, the terrain can get in the way easily
                        if (Physics.Raycast(_turretBarrel.transform.position, (enemyCollider.transform.position + Vector3.up * enemyScale * 0.5f) - _turretBarrel.transform.position, out enemyHit))
                        {
                            Debug.DrawLine(_turretBarrel.transform.position, (enemyCollider.transform.position + Vector3.up * enemyScale * 0.5f), Color.red);
                            if (enemyHit.collider == enemyCollider)
                            {
                                currentNearestVisibleCollider = enemyCollider;
                                currentClosestDistance = distance;
                            }
                        }
                    }
                }
                //return the nearest visible collider once they are all iterated through
                if (currentNearestVisibleCollider)
                {
                    return currentNearestVisibleCollider.transform;
                }
            }
            //if there are no visible targets in the radius, return null
            return null;
        }
    }

    public static class HelperFunctions
    {
        #region MouseRayHitPoint function and overloads
        /// <summary>
        /// Returns the point in space where the cursor is hovering over a collider.
        /// </summary>
        public static Vector3 MouseRayHitPoint()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return hit.point;
            }
            return MouseRayHitPoint(0f);
        }

        /// <summary>
        /// Returns the point in space where the cursor is hovering over a collider of an object with the specified layer. 
        /// </summary>
        /// <param name="_layerMask">The layer of the object you want to raycast to.</param>
        public static Vector3 MouseRayHitPoint(LayerMask _layerMask)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
            {
                return hit.point;
            }
            return MouseRayHitPoint(0f);
        }

        /// <summary>
        /// Returns the point in space where the cursor is hovering over an infinite, upward facing plane with a specified y height.
        /// </summary>
        /// <param name="TargetHeight">The height in space of the plane.</param>
        public static Vector3 MouseRayHitPoint(float TargetHeight)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.up * TargetHeight);
            float distance = 0;
            if (plane.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }
            else return Vector3.zero;
        }
        #endregion

        /// <summary>
        /// returns the layermask of the point under the cursor
        /// </summary>
        public static LayerMask MouseRayHitMask()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return hit.collider.gameObject.layer;
            }
            return 0;
        }

        /// <summary>
        /// returns the layermask of the point under the cursor as long as it is within the given _layermask
        /// </summary>
        public static LayerMask MouseRayHitMask(LayerMask _layerMask)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
            {
                return hit.collider.gameObject.layer;
            }
            return 0;
        }

        /// <summary>
        /// Converts a vector 2 into a point in space on the horizontal plane at y = 0.
        /// </summary>
        public static Vector3 ConvertToVector3(Vector2 input)
        {
            return new Vector3(input.x, 0, input.y);
        }

        public static Vector2 ConvertToVector2(Vector3 input)
        {
            return new Vector2(input.x, input.z);
        }

        /// <summary>
        /// Returns the Y height of colliders of objects with the Terrain layer at the given coordinates on the horizontal plane.
        /// </summary>
        public static float TargetHeight(Vector2 _targetPosition)
        {
            LayerMask terrain = LayerMask.GetMask("Terrain");
            RaycastHit hit;
            if (Physics.Raycast(ConvertToVector3(_targetPosition) + Vector3.up * 1000, Vector3.down, out hit, Mathf.Infinity, terrain))
            {
                return hit.point.y;
            }
            return 0;
        }

        /// <summary>
        /// Returns the Y height of colliders of objects with the Terrain layer at the given coordinates on the horizontal plane.
        /// </summary>
        public static float TargetHeight(Vector2 _targetPosition, float defaultHeight)
        {
            LayerMask terrain = LayerMask.GetMask("Terrain");
            RaycastHit hit;
            if (Physics.Raycast(ConvertToVector3(_targetPosition) + Vector3.up * 1000, Vector3.down, out hit, Mathf.Infinity, terrain))
            {
                return hit.point.y;
            }
            return defaultHeight;
        }

        /// <summary>
        /// returns the ground normal of the terrain at the position given on the x-z plane
        /// </summary>
        public static Vector3 GetGroundNormal(Vector2 _position)
        {
            LayerMask terrain = LayerMask.GetMask("Terrain");
            RaycastHit hit;
            if (Physics.Raycast(ConvertToVector3(_position) + Vector3.up * 1000, Vector3.down, out hit, Mathf.Infinity, terrain))
            {
                return hit.normal;
            }
            return Vector3.up;
        }

        /// <summary>
        /// returns the 2d forward direction rotated about the z axis by _angle
        /// </summary>
        public static Vector2 Angle2Direction(float _angle)
        {
            return ConvertToVector2(Quaternion.Euler(0, _angle, 0) * Vector3.forward);
        }

        /// <summary>
        /// converts a vector2 direction to the angle it would take to rotate it back to forward
        /// </summary>
        /// <param name="_direction"></param>
        /// <returns></returns>
        public static float Direction2Angle(Vector2 _direction)
        {
            float angle = Vector3.Angle(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(_direction.x, _direction.y, 0.0f));
            if (_direction.x < 0.0f)
            {
                angle = -angle;
                angle = angle + 360;
            }
            return angle;
        }

        /// <summary>
        /// set the layer of all children of the given gameobject
        /// </summary>
        public static void SetLayerOfAllChildren(GameObject objectToSetLayerOn, LayerMask layer)
        {
            objectToSetLayerOn.layer = layer;
            if (objectToSetLayerOn.transform.childCount > 0)
            {
                foreach (Transform child in objectToSetLayerOn.transform)
                {
                    SetLayerOfAllChildren(child.gameObject, layer);
                }
            }
        }

        /// <summary>
        /// set the tag of all children of the given gameobject
        /// </summary>
        public static void SetTagOfAllChildren(GameObject objectToSetTagOn, string tag)
        {
            objectToSetTagOn.tag = tag;
            if (objectToSetTagOn.transform.childCount > 0)
            {
                foreach (Transform child in objectToSetTagOn.transform)
                {
                    SetTagOfAllChildren(child.gameObject, tag);
                }
            }
        }
    }
}
