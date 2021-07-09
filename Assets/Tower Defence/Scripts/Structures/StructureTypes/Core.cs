using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using static StaticObjectHolder;

namespace Structures
{
    public class Core : Structure
    {
        [Header("-- CoreParts --")]
        [SerializeField, Tooltip("The Model of the core")]
        private GameObject CoreModel;
        [SerializeField, Tooltip("The source of the sound that the core plays")]
        private AudioSource coreSound;
        [SerializeField, Tooltip("The source of the sound that the rocket plays when it takes off")]
        private AudioSource rocketSound;

        [Header("-- CoreDeathSettings --")]
        [SerializeField, Tooltip("The death animation of the core")]
        private Animation coreDeathAnimation;
        [SerializeField, Tooltip("The particle system that makes the rocket exhaust")]
        private ParticleSystem RocketExhaustEffect;
        [SerializeField, Tooltip("The effect that plays to make the core explode")]
        private GameObject CoreExplosionEffect;
        [SerializeField, Tooltip("How long it takes for the core explosion to charge up")]
        private float coreExplosionWarmUpTime = 6.4f;
        [SerializeField, Tooltip("How long after the destruction of the structures to end the level.")]
        private float timeAfterCoreDestructionToEndLevel = 2.5f;

        bool isDead = false;

        bool allStructuresDestroyed = false;

        /// <summary>
        /// the thing that does all the stuff to display the core and all the structures being destroyed.
        /// </summary>
        IEnumerator DeathSequence()
        {
            float animationDuration = coreDeathAnimation.clip.length;
            StartCoroutine(TurnDownSound());
            coreDeathAnimation.Play();
            yield return new WaitForSeconds(2);
            rocketSound.Play();
            yield return new WaitForSeconds(animationDuration - 3f);
            RocketExhaustEffect.Stop(true);
            Instantiate(CoreExplosionEffect, gameObject.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(coreExplosionWarmUpTime);
            CoreModel.SetActive(false);
            StartCoroutine(DestroyAllStructures());
            while (allStructuresDestroyed == false)
            {
                yield return null;
            }
            yield return new WaitForSeconds(timeAfterCoreDestructionToEndLevel);
            Destroy(gameObject);
        }

        /// <summary>
        /// destroy all the structures currentStructures one by one every 0.05 then set the allStructuresDestroyed bool to true
        /// </summary>
        IEnumerator DestroyAllStructures()
        {
            List<GameObject> allStructures = new List<GameObject>(currentStructures.Keys);
            foreach (GameObject structureGO in allStructures)
            {
                if (structureGO && structureGO != gameObject)
                {
                    Destroy(structureGO);
                    yield return new WaitForSeconds(0.05f);
                }
            }
            allStructuresDestroyed = true;
        }

        /// <summary>
        /// lerp the sound down to 0 slowly
        /// </summary>
        IEnumerator TurnDownSound()
        {
            while (coreSound.volume > 0.001f)
            {
                coreSound.volume = Mathf.Lerp(coreSound.volume, 0, Time.fixedDeltaTime * 0.1f);
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// start the DeathSequence Coroutine that will inevitably end the game.
        /// </summary>
        public override void Die()
        {
            if (!isDead)
            {
                isDead = true;
                StartCoroutine(DeathSequence());
            }
        }
        
        /// <summary>
        /// remove the amount of damage from health
        /// </summary>
        public override void Damage(float amount)
        {
            Health -= amount;
        }

        private void Start()
        {
            //initialize all the things
            theCore = this;
            theCameraMovement.gameObject.transform.position = transform.position;
            InitializeHealth();
            InitializeMeshRendering();
            BecomeConnected();
        }

        /// <summary>
        /// disconnect all structures, then propagate through them all, starting with the core, connecting them.
        /// </summary>
        public void UpdateConnectedStructures()
        {
            //disconnect all structures from the core
            foreach (KeyValuePair<GameObject, Structure> gameObjectStructurePair in currentStructures)
            {
                gameObjectStructurePair.Value.isConnectedToCore = false;
            }
            // if the core is alive, connect all structures
            if (!isDead)
            {
                BecomeConnected();
            }
        }

        private void FixedUpdate()
        {
            //take damage from all enemies colliding with the core, then destroy the enemy
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
            //when the core dies, save the score and go back to the main menu.
            theScoreSystem.SaveScore();
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
}
