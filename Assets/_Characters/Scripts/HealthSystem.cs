using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] Image healthBar;
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] float deathVanishSeconds = 2f;

        const string DEATH_TRIGGER = "Death";

        float currentHealthPoints = 0;
        Animator animator;
        AudioSource audioSource;
        Character characterMovement;
        LevelUp levelUp;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }
        
        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<Character>();
            levelUp = FindObjectOfType<LevelUp>();
            currentHealthPoints = maxHealthPoints;
        }
        
        void Update()
        {
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if (healthBar)
            {
                healthBar.fillAmount = healthAsPercentage;
            }
        }

        public void TakeDamage(float damage)
        {
            bool characterDies = (currentHealthPoints - damage <= 0);
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }
        }

        private IEnumerator KillCharacter()
        {
            SendMessage("Kill");
            animator.SetTrigger(DEATH_TRIGGER);
            var clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.PlayOneShot(clip);
            var playerComponent = GetComponent<PlayerControl>();
            if(!GetComponent<PlayerControl>())
            {
                levelUp.AddKill();
            }
            yield return new WaitForSecondsRealtime(clip.length);
            if(playerComponent)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                Destroy(gameObject, deathVanishSeconds);
            }
        }

        public void Heal(float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
        }

        public void AddMaxHP(float amount)
        {
            maxHealthPoints += amount;
            currentHealthPoints = maxHealthPoints;
        }
    }
}