using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.WeaponFunctions;

namespace Structures
{
    public class Gun : Tower
    {
        [Header("-- Projectile Settings --")]
        [SerializeField, Tooltip("The bullet to shoot.")]
        private GameObject bullet;
        [SerializeField, Tooltip("The speed of the bullet")]
        private float bulletSpeed = 50f;
        [SerializeField, Tooltip("How far forward from the pivot point of the barrel is the end of the barrel. The bullet will be spawned here")]
        private float tipOfBarrelOffset = 4f;
        [SerializeField, Tooltip("The amount of bullet spread in degrees")]
        private float spread = 5f;
        [SerializeField, Tooltip("The size of the bullet")]
        private float bulletRadius = 0.25f;
        [SerializeField, Tooltip("The tag of the gamobject the bullet is trying to hit")]
        private string bulletHitTargetTag;
        [SerializeField, Tooltip("How much damage the bullet does.")]
        private float bulletDamage= 10f;

        [Header("-- Effect Settings --")]
        [SerializeField, Tooltip("The muzzleflash effect")]
        private GameObject fireEffect;

        /// <summary>
        /// shoot the projectile from the tip of the barrel and spawn the fireEffect
        /// </summary>
        public override void ShootProjectile()
        {
            Vector3 bulletStartPosition = turretBarrel.transform.position + turretBarrel.transform.rotation * Vector3.forward * tipOfBarrelOffset;
            Instantiate(fireEffect, bulletStartPosition, turretBarrel.transform.rotation);
            Instantiate(bullet, bulletStartPosition, turretBarrel.transform.rotation);
        }

        private void OnValidate()
        {
            //set the values of the bullet
            if (bullet)
            {
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript)
                {
                    bulletScript.spawningCollider = GetComponent<Collider>();
                    bulletScript.bulletSpeed = bulletSpeed;
                    bulletScript.spread = spread;
                    bulletScript.bulletRadius = bulletRadius;
                    bulletScript.targetTag = bulletHitTargetTag;
                    bulletScript.bulletDamage = bulletDamage;
                    bulletScript.bulletRange = range;
                }
                else //if the bullet does't have a bullet script, remove it.
                    bullet = null;
            }
        }

        private void Start()
        {
            StartStructure();
        }

        private void Update()
        {
            UpdateStructure();
            if (!Preview && CanFunction)
                UpdateTower();
        }
    }
}