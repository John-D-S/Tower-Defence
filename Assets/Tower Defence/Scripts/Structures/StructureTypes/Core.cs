 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static StaticObjectHolder;

namespace Structure
{
    public class Core : Structure
    {
        [SerializeField]
        private Animation coreDeathAnimation;

        bool isDead = false;

        IEnumerator DeathSequence(float duration)
        {
            coreDeathAnimation.Play();
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }

        public override void Die()
        {
            if (!isDead)
            {
                isDead = true;
                float deathAnimationDuration = coreDeathAnimation.clip.length;
                StartCoroutine(DeathSequence(deathAnimationDuration));
            }
        }

        public override void Damage(float amount)
        {
            Health -= amount;
        }

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
            theScoreSystem.SaveScore();
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
}
