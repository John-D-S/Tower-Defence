 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static StaticObjects;

namespace Structure
{
    public class Core : Structure
    {
        private void Start()
        {
            theCore = this;
            theCameraMovement.gameObject.transform.position = transform.position;
            InitializeHealth();
            InitializeMeshRendering();
            BecomeConnected();
        }

        public void UpdateConnectedStructures()
        {
            foreach (KeyValuePair<GameObject, Structure> gameObjectStructurePair in currentStructures)
            {
                gameObjectStructurePair.Value.isConnectedToCore = false;
            }
            BecomeConnected();
        }

        private void FixedUpdate()
        {
            Collider[] collidingEnemies = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Enemy"));
            if (collidingEnemies.Length > 0)
            {
                foreach (Collider enemyCollider in collidingEnemies)
                {
                    Damage(enemyCollider.GetComponent<Enemy>().CoreDamage);
                    Destroy(enemyCollider.gameObject);
                }
            }
        }
        
        private void OnDestroy()
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
}
