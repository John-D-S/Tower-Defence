using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.WeaponFunctions;

namespace Structures
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
                if (Economy.EconomyTracker.TryIncrementEnergy(-energyToRun))
                {
                    ShootProjectile();
                    StartCoroutine(FireCooldown());
                }
            }
        }

        private IEnumerator FireCooldown()
        {
            canFire = false;
            yield return new WaitForSeconds(1 / fireRate);
            canFire = true;
        }
        #endregion

        protected void UpdateTower()
        {
            Transform enemy = NearestVisibleTarget(turretBarrel, range, enemyLayer);
            if (enemy)
            {
                AimAtTarget(enemy, gameObject, ref turretBase, ref turretBarrel);
                Fire();
            }
        }

        private void Awake()
        {
            enemyLayer = LayerMask.GetMask("Enemy");
        }

        private void OnDestroy()
        {
            StructureOnDestroy();
        }
    }
}