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
            StartCoroutine(ShootBullet());
        }

        private Quaternion RandomSpread(float _spread)
        {
            float randomXSpread = Random.Range(-_spread, _spread) * 0.5f;
            float randomYSpread = Random.Range(-_spread, _spread) * 0.5f;
            return Quaternion.Euler(randomXSpread, randomYSpread, 0);
        }

        IEnumerator ShootBullet()
        {
            GameObject bulletInstance = Instantiate(bullet, turretBarrel.transform.position, turretBarrel.transform.rotation * RandomSpread(spread));
            bulletInstance.transform.localScale = Vector3.one * bulletRadius * 2;
            float distance = 0; 
            while (distance < range)
            {
                yield return new WaitForFixedUpdate();
                float distanceDelta = Time.deltaTime * bulletSpeed;
                RaycastHit hitInfo;
                if (Physics.SphereCast(new Ray(bulletInstance.transform.position, bulletInstance.transform.forward), bulletRadius, out hitInfo, distanceDelta))
                {
                    //if the bullet hits the thing that it's supposed to, damage the thing
                    if (hitInfo.collider.tag == bulletHitTargetTag)
                    {
                        IKillable objectToDamage = hitInfo.collider.GetComponentInParent<IKillable>();
                        if (objectToDamage != null)
                            objectToDamage.Damage(bulletDamage);
                    }
                    else if (hitInfo.collider != thisCollider)
                    {
                        break;
                    }
                }
                bulletInstance.transform.position += bulletInstance.transform.forward * distanceDelta;
                distance += distanceDelta;
            }
            Destroy(bulletInstance);
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