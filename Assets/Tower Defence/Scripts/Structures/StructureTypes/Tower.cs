using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public abstract class Tower : Structure
    {
        [Header("-- Settings --")]
        [SerializeField, Tooltip("The Furthest enemies can be from the turret before it stops fireing")]
        private float range = 50;
        [SerializeField, Tooltip("The number of projectiles fired Per Second")]
        private float fireRate = 1;
        [SerializeField, Tooltip("The amount of metal consumed each time Fire is called")]
        private int MetalConsumption = 0;

        [Header("-- Turret Parts --")]
        [SerializeField, Tooltip("The part of the turret that rotates left and right on the y axis")]
        private GameObject turretBase;
        [SerializeField, Tooltip("The part of the turret that rotates up and down on the x axis")]
        private GameObject turretBarrel;

        private bool canFire = true;

        LayerMask enemyLayer;

        #region Shooting
        public abstract void ShootProjectile();

        private void Fire()
        {
            if (canFire)
            {
                ShootProjectile();
                FireCooldown();
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
                        currentNearestVisibleCollider = enemyCollider;
                        currentClosestDistance = distance;
                    }
                }
                return currentNearestVisibleCollider.transform;
            }
            return null;
        }

        protected void AimAtEnemy(Transform _enemy)
        {
            Vector3 turretBaseTarget = new Vector3(_enemy.position.x, turretBase.transform.position.y, _enemy.position.z);
            Vector3 turretBarrelTarget = _enemy.transform.position;
            
            turretBase.transform.LookAt(turretBaseTarget);
            turretBarrel.transform.LookAt(turretBarrelTarget);
        }

        protected void UpdateTower()
        {
            Transform enemy = NearestVisibleEnemy();
            if (enemy)
            {
                AimAtEnemy(enemy);
            }
        }

        private void Awake()
        {
            enemyLayer = LayerMask.GetMask("Enemy");
        }
    }
}