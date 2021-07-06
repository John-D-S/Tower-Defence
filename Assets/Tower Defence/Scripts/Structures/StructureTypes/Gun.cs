using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.WeaponFunctions;

namespace Structures
{
    public class Gun : Tower
    {
        [Header("-- Projectile Settings --")]
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private float bulletSpeed = 50f;
        [SerializeField]
        private float tipOfBarrelOffset = 4f;
        [SerializeField]
        private float spread = 5f;
        [SerializeField]
        private float bulletRadius = 0.25f;
        [SerializeField]
        private string bulletHitTargetTag;
        [SerializeField]
        private float bulletDamage= 10f;

        [Header("-- Effect Settings --")]
        [SerializeField]
        private GameObject fireEffect;

        private void OnValidate()
        {
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
                else
                    bullet = null;
            }
        }

        public override void ShootProjectile()
        {
            Vector3 bulletStartPosition = turretBarrel.transform.position + turretBarrel.transform.rotation * Vector3.forward * tipOfBarrelOffset;
            Instantiate(fireEffect, bulletStartPosition, turretBarrel.transform.rotation);
            Instantiate(bullet, bulletStartPosition, turretBarrel.transform.rotation);
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