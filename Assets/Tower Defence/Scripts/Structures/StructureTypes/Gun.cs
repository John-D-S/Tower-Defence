using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.WeaponFunctions;

namespace Structure
{
    public class Gun : Tower
    {
        [Header("-- Projectile Settings --")]
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private float bulletSpeed = 50f;
        [SerializeField]
        private float spread = 5f;
        [SerializeField]
        private float bulletRadius = 0.25f;
        [SerializeField]
        private string bulletHitTargetTag;
        [SerializeField]
        private float bulletDamage= 10f;

        private Collider thisCollider;

        public override void ShootProjectile()
        {
            StartCoroutine(ShootBullet(thisCollider, turretBarrel.transform.position, turretBarrel.transform.rotation, bulletSpeed, bullet, bulletRadius, bulletDamage, spread, range, bulletHitTargetTag));
        }

        private void Start()
        {
            StartStructure();
            thisCollider = gameObject.GetComponent<Collider>();
        }

        private void Update()
        {
            UpdateStructure();
            if (!Preview)
                UpdateTower();
        }
    }
}