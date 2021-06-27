 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticObjects;

namespace Structure
{
    public class Core : Structure
    {
        private void Start()
        {
            theCore = this;
            theCameraMovement.gameObject.transform.position = transform.position;
            StartStructure();
        }

        protected override void UpdateConnectedToCore()
        {
            isConnectedToCore = true;
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
