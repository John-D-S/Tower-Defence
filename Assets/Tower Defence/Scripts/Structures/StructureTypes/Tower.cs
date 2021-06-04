using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public abstract class Tower : Structure
    {
        [Header("-- Settings --")]
        [SerializeField, Tooltip("The Furthest enemies can be from the turret before it stops fireing")]
        protected float range = 50;
        [SerializeField, Tooltip("The number of projectiles fired Per Second")]
        protected float fireRate = 1;
        [SerializeField, Tooltip("The amount of metal consumed each time Fire is called")]
        protected int MetalConsumption = 0;

        [Header("-- Turret Parts --")]
        [SerializeField, Tooltip("The part of the turret that rotates left and right on the y axis")]
        private GameObject turretBase;
        [SerializeField, Tooltip("The part of the turret that rotates up and down on the x axis")]
        protected GameObject turretBarrel;

        private bool canFire = true;

        LayerMask enemyLayer;

        #region Shooting
        public abstract void ShootProjectile();

        private void Fire()
        {
            if (canFire)
            {
                ShootProjectile();
                StartCoroutine(FireCooldown());
            }
        }

        private IEnumerator FireCooldown()
        {
            canFire = false;
            yield return new WaitForSeconds(1 / fireRate);
            canFire = true;
        }
        #endregion

        protected Transform NearestVisibleEnemy()
        {

            if (Physics.CheckSphere(turretBarrel.transform.position, range, enemyLayer))
            {
                Collider currentNearestVisibleCollider = null;
                float currentClosestDistance = Mathf.Infinity;
                Collider[] inRangeEnemyColliders = Physics.OverlapSphere(turretBarrel.transform.position, range, enemyLayer);
                foreach (Collider enemyCollider in inRangeEnemyColliders)
                {
                    float distance = Vector3.Distance(enemyCollider.transform.position, transform.position);
                    if (distance < currentClosestDistance)
                    {
                        float enemyScale = enemyCollider.transform.localScale.y;
                        RaycastHit enemyHit;
                        //we are testing if we can see the top of the enemy because if we aim for the center, the terrain can get in the way easily
                        if (Physics.Raycast(turretBarrel.transform.position, (enemyCollider.transform.position + Vector3.up * enemyScale * 0.5f) - turretBarrel.transform.position, out enemyHit, range))
                        {
                            Debug.DrawLine(turretBarrel.transform.position, (enemyCollider.transform.position + Vector3.up * enemyScale * 0.5f), Color.red);
                            if (enemyHit.collider == enemyCollider)
                            {
                                currentNearestVisibleCollider = enemyCollider;
                                currentClosestDistance = distance;
                            }
                        }
                    }
                }
                if (currentNearestVisibleCollider)
                {
                    return currentNearestVisibleCollider.transform;
                }
            }
            return null;
        }

        protected void AimAtEnemy(Transform _enemy)
        {
            Vector3 turretBaseTarget = new Vector3(_enemy.position.x, turretBase.transform.position.y, _enemy.position.z);
            //the turret is aiming slightly above the position of the enemy so that it is less likely to be obscured by the terrain.
            Vector3 turretBarrelTarget = _enemy.transform.position + Vector3.up * 0.5f;
            
            turretBase.transform.LookAt(turretBaseTarget);
            turretBarrel.transform.LookAt(turretBarrelTarget);
        }

        protected void UpdateTower()
        {

            Transform enemy = NearestVisibleEnemy();
            if (enemy)
            {
                AimAtEnemy(enemy);
                Fire();
            }
            
        }

        private void Awake()
        {
            enemyLayer = LayerMask.GetMask("Enemy");
        }
    }
}