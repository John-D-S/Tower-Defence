using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public abstract class Tower : Structure
    {
        
        [SerializeField, Tooltip("The Furthest enemies can be from the turret before it stops fireing")]
        private float range = 50;
        [SerializeField, Tooltip("The number of projectiles fired Per Second")]
        private float fireRate = 1;
        [SerializeField, Tooltip("The amount of metal consumed each time Fire is called")]
        private int MetalConsumption = 0;

        [SerializeField, Tooltip("The part of the turret that rotates left and right on the y axis")]
        private GameObject turretBase;
        [SerializeField, Tooltip("The part of the turret that rotates up and down on the x axis")]
        private GameObject turretBarrel;

        private bool canFire = true;

        LayerMask enemy = LayerMask.GetMask("Enemy");

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

        public abstract GameObject Target();
        /*
        {
            if (Physics.CheckSphere(turretBarrel.transform.position, range, enemy))
            {
                Collider currentNearestVisibleCollider = null;
                Collider[] inRangeEnemyColliders = Physics.OverlapSphere(turretBarrel.transform.position, range, enemy);
                foreach (Collider enemyCollider in inRangeEnemyColliders)
                {
                    if
                }
                //Physics.SphereCast(turretBarrel.transform.position, range, )

            }
        }
        */
        private void AimAtEnemy()
        {

        }

        private void Update()
        {
            
        }
    }
}