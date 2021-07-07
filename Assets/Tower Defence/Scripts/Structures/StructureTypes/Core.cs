 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static StaticObjectHolder;

namespace Structures
{
    public class Core : Structure
    {
        [Header("-- CoreDeathSettings --")]
        [SerializeField]
        private Animation coreDeathAnimation;
        [SerializeField]
        private GameObject CoreModel;
        [SerializeField]
        private ParticleSystem RocketExhaustEffect;
        [SerializeField]
        private GameObject CoreExplosionEffect;
        [SerializeField]
        private float coreExplosionWarmUpTime = 6.4f;
        [SerializeField]
        private float timeAfterCoreDestructionToEndLevel = 5f;

        bool isDead = false;

        IEnumerator DeathSequence()
        {
            float duration = coreDeathAnimation.clip.length;
            coreDeathAnimation.Play();
            yield return new WaitForSeconds(duration - 1f);
            RocketExhaustEffect.Stop(true);
            Instantiate(CoreExplosionEffect, gameObject.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(coreExplosionWarmUpTime);
            CoreModel.SetActive(false);
            yield return new WaitForSeconds(timeAfterCoreDestructionToEndLevel);
            Destroy(gameObject);
        }

        public override void Die()
        {
            if (!isDead)
            {
                isDead = true;
                StartCoroutine(DeathSequence());
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
