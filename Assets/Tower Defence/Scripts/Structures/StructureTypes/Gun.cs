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
        [SerializeField]
        private float spread;

        public override void ShootProjectile()
        {
            StartCoroutine(ShootBullet());
        }

        IEnumerator ShootBullet()
        {
            float randomXSpread = Random.Range(-spread, spread);
            float randomYSpread = Random.Range(-spread, spread);
            GameObject bulletInstance = Instantiate(bullet, turretBarrel.transform.position, turretBarrel.transform.rotation * Quaternion.Euler(randomXSpread, randomYSpread, 0));
            float distance = 0;
            while (distance < range)
            {
                yield return new WaitForFixedUpdate();
                float distanceDelta = Time.deltaTime * bulletSpeed;
                bulletInstance.transform.position += bulletInstance.transform.forward * distanceDelta;
                distance += distanceDelta;
            }
            Destroy(bulletInstance);
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