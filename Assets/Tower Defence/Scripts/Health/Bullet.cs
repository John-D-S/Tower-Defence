using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.WeaponFunctions;

public class Bullet : MonoBehaviour
{
    //these variables are set by the things that shoot them.
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

    [Header("-- Visual Bullet Settings --")]
    [SerializeField]
    private TrailRenderer trailRenderer;
    [SerializeField]
    private GameObject hitEffect;

    private void Start()
    {
        //set the width of the trail renderer to match the size of the bullet
        if (trailRenderer)
            trailRenderer.widthMultiplier = bulletRadius * 2;
        //start the coroutine that makes the bullet travel forward and act like a bullet. (disappearing when hitting and dealing damage to the thing it hits if it matches the target tag)
        StartCoroutine(ActLikeABullet(spawningCollider, gameObject, bulletRadius, bulletRange, bulletSpeed, spread, bulletDamage, targetTag, hitEffect));
    }

    /// <summary>
    /// I honestly can't remember why I made this static.
    /// </summary>
    public static IEnumerator ActLikeABullet(Collider spawningCollider, GameObject bullet, float bulletRadius, float bulletRange, float bulletSpeed, float bulletSpread, float bulletDamage, string targetTag, GameObject hitEffect)
    {
        //set the scale of the bullet
        bullet.transform.localScale = Vector3.one * bulletRadius * 2;
        //set the bullet's initial rotation according to spread
        bullet.transform.localRotation *= RandomSpread(bulletSpread);
        float distance = 0;
        //while the bullet is within bullet range
        while (distance < bulletRange)
        {
            yield return new WaitForFixedUpdate();
            float distanceDelta = Time.fixedDeltaTime * bulletSpeed;
            RaycastHit hitInfo;
            //if the bullet hits something
            if (Physics.SphereCast(new Ray(bullet.transform.position, bullet.transform.forward), bulletRadius, out hitInfo, distanceDelta))
            {
                //if the bullet hits the thing that it's supposed to, damage the thing
                if (hitInfo.collider.tag == targetTag)
                {
                    IKillable objectToDamage = hitInfo.collider.GetComponentInParent<IKillable>();
                    if (objectToDamage != null)
                    {
                        //damage the thing it hits
                        objectToDamage.Damage(bulletDamage);
                        //instantiate the hit effect and set its scale
                        GameObject instantiatedhitEffect = Instantiate(hitEffect, hitInfo.point, Quaternion.FromToRotation(Vector3.forward, Vector3.Lerp(bullet.transform.forward, hitInfo.normal, 0.6f)));
                        instantiatedhitEffect.transform.localScale = Vector3.one * bulletRadius * 2;
                        break;
                    }
                }
                else if (hitInfo.collider != spawningCollider) // if not, just makea the bullet a dissapear
                    break;
            }
            //move the bullet forward according to it's speed
            bullet.transform.position += bullet.transform.forward * distanceDelta;
            distance += distanceDelta;
        }
        //the bullet is destroyed when the above loop breaks
        Destroy(bullet);
    }
}
