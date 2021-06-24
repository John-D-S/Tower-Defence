using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public class Core : Structure
    {
        private void Start()
        {
            //gameObject.tag = "EnemyTarget";
            //gameObject.layer = LayerMask.GetMask("EnemyTarget");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Damage(other.GetComponent<Enemy>().CoreDamage);
                Destroy(other);
            }
        }

        private void OnDestroy()
        {
            Debug.Log("RIP");
        }
    }
}
