using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public class Core : Structure
    {
        private void Start()
        {
            //Preview = false;
        }

        private void FixedUpdate()
        {
            Collider[] collidingEnemies = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Enemy"));
            if (collidingEnemies.Length > 0)
            {
                Debug.Log("there be enemies");
                foreach (Collider enemyCollider in collidingEnemies)
                {
                    Debug.Log(enemyCollider.tag);
                    Damage(enemyCollider.GetComponent<Enemy>().CoreDamage);
                    Destroy(enemyCollider.gameObject);
                }
            }
        }
        /*
        private void OnTriggerStay(Collider other)
        {
            Debug.Log(other.tag);
            if (other.CompareTag("Enemy"))
            {
                Damage(other.GetComponent<Enemy>().CoreDamage);
                Destroy(other);
            }
        }
        */
        private void OnDestroy()
        {
            Debug.Log("RIP");
        }
    }
}
