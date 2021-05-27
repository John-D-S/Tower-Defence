using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public class Gun : Tower
    {
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private float bulletSpeed;

        public override void ShootProjectile()
        {
            StartCoroutine(ShootBullet());
        }

        IEnumerator ShootBullet()
        {
            GameObject bulletInstance = Instantiate(bullet, turretBarrel.transform.position, turretBarrel.transform.rotation);
            float distance = 0;
            while (distance < range)
            {
                yield return new WaitForEndOfFrame();
                float distanceDelta = Time.deltaTime * bulletSpeed;
                bulletInstance.transform.position += bulletInstance.transform.forward * distanceDelta;
                distance += distanceDelta;
            }
        }

        private void Update()
        {
            UpdateStructure();
            if (!Preview)
            {
                UpdateTower();
            }
        }
    }
}