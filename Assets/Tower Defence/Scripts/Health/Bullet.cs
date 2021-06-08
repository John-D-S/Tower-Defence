using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.WeaponFunctions;

public class Bullet : MonoBehaviour
{
    [System.NonSerialized]
    public Collider spawningCollider;
    [System.NonSerialized]
    public float bulletSpeed = 50f;
    [System.NonSerialized]
    public float spread = 5f;
    [System.NonSerialized]
    public float bulletRadius = 0.25f;
    [System.NonSerialized]
    public string bulletHitTargetTag;
    [System.NonSerialized]
    public float bulletDamage = 10f;
    [System.NonSerialized]
    public float bulletRange = 50f;

    [SerializeField]
    private TrailRenderer trailRenderer;

    private void Start()
    {
        if (trailRenderer)
            trailRenderer.widthMultiplier = bulletRadius * 2;
        StartCoroutine(ActLikeABullet(spawningCollider, gameObject, bulletRadius, bulletRange, bulletSpeed, spread, bulletDamage, bulletHitTargetTag));
    }

    public static IEnumerator ActLikeABullet(Collider spawningCollider, GameObject bullet, float bulletRadius, float bulletRange, float bulletSpeed, float bulletSpread, float bulletDamage, string targetTag)
    {
        bullet.transform.localScale = Vector3.one * bulletRadius * 2;
        bullet.transform.localRotation *= RandomSpread(bulletSpread);
        float distance = 0;
        while (distance < bulletRange)
        {
            yield return new WaitForFixedUpdate();
            float distanceDelta = Time.deltaTime * bulletSpeed;
            RaycastHit hitInfo;
            if (Physics.SphereCast(new Ray(bullet.transform.position, bullet.transform.forward), bulletRadius, out hitInfo, distanceDelta))
            {
                //if the bullet hits the thing that it's supposed to, damage the thing
                if (hitInfo.collider.tag == targetTag)
                {
                    IKillable objectToDamage = hitInfo.collider.GetComponentInParent<IKillable>();
                    if (objectToDamage != null)
                        objectToDamage.Damage(bulletDamage);
                }
                else if (hitInfo.collider != spawningCollider)
                {
                    break;
                }
            }
            bullet.transform.position += bullet.transform.forward * distanceDelta;
            distance += distanceDelta;
        }
        Destroy(bullet);
    }
}
