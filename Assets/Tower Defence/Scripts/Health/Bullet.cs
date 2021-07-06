using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.WeaponFunctions;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public Collider spawningCollider;
    [HideInInspector]
    public float bulletSpeed;
    [HideInInspector]
    public float spread;
    [HideInInspector]
    public float bulletRadius;
    [HideInInspector]
    public string targetTag;
    [HideInInspector]
    public float bulletDamage;
    [HideInInspector] 
    public float bulletRange;

    [SerializeField]
    private TrailRenderer trailRenderer;
    [SerializeField]
    private GameObject hitEffect;

    private void Start()
    {
        if (trailRenderer)
            trailRenderer.widthMultiplier = bulletRadius * 2;
        StartCoroutine(ActLikeABullet(spawningCollider, gameObject, bulletRadius, bulletRange, bulletSpeed, spread, bulletDamage, targetTag, hitEffect));
    }

    public static IEnumerator ActLikeABullet(Collider spawningCollider, GameObject bullet, float bulletRadius, float bulletRange, float bulletSpeed, float bulletSpread, float bulletDamage, string targetTag, GameObject hitEffect)
    {
        bullet.transform.localScale = Vector3.one * bulletRadius * 2;
        bullet.transform.localRotation *= RandomSpread(bulletSpread);
        float distance = 0;
        while (distance < bulletRange)
        {
            yield return new WaitForFixedUpdate();
            float distanceDelta = Time.fixedDeltaTime * bulletSpeed;
            RaycastHit hitInfo;
            if (Physics.SphereCast(new Ray(bullet.transform.position, bullet.transform.forward), bulletRadius, out hitInfo, distanceDelta))
            {
                //if the bullet hits the thing that it's supposed to, damage the thing
                if (hitInfo.collider.tag == targetTag)
                {
                    IKillable objectToDamage = hitInfo.collider.GetComponentInParent<IKillable>();
                    if (objectToDamage != null)
                    {
                        objectToDamage.Damage(bulletDamage);
                        GameObject instantiatedhitEffect = Instantiate(hitEffect, hitInfo.point, Quaternion.FromToRotation(Vector3.forward, Vector3.Lerp(bullet.transform.forward, hitInfo.normal, 0.6f)));
                        instantiatedhitEffect.transform.localScale = Vector3.one * bulletRadius * 2;
                        break;
                    }
                }
                else if (hitInfo.collider != spawningCollider)
                {
                    GameObject instantiatedhitEffect = Instantiate(hitEffect, hitInfo.point, Quaternion.FromToRotation(Vector3.forward, Vector3.Lerp(bullet.transform.forward, hitInfo.normal, 0.6f)));
                    instantiatedhitEffect.transform.localScale = Vector3.one * bulletRadius * 2;
                    break;
                }
            }
            bullet.transform.position += bullet.transform.forward * distanceDelta;
            distance += distanceDelta;
        }
        Destroy(bullet);
    }
}
