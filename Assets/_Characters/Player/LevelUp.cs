using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class LevelUp : MonoBehaviour
    {
        [SerializeField] ParticleSystem levelUpPS;
        [SerializeField] AudioClip levelUpSound;
        [SerializeField] int[] levelUpThreshold;
        [SerializeField] float[] levelUpDamage;
        [SerializeField] float[] levelUpHealth;
        [SerializeField] float[] levelUpEnergy;

        int currentLevel = 0;
        int currentKillCount = 0;
        HealthSystem healthSystem;
        WeaponSystem weaponSystem;
        SpecialAbilities specialAbilities;
        AudioSource audioSource;

        void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
            weaponSystem = GetComponent<WeaponSystem>();
            specialAbilities = GetComponent<SpecialAbilities>();
            audioSource = GetComponent<AudioSource>();
        }

        public void AddKill()
        {
            currentKillCount++;
            if (currentKillCount >= levelUpThreshold[currentLevel])
            {
                LevelingUp();
                currentLevel++;
                currentKillCount = 0;
            }
        }

        void LevelingUp()
        {
            weaponSystem.AddBaseDamage(levelUpDamage[currentLevel]);
            healthSystem.AddMaxHP(levelUpHealth[currentLevel]);
            specialAbilities.AddMaxEnergy(levelUpEnergy[currentLevel]);
            audioSource.PlayOneShot(levelUpSound);
            levelUpPS.Play();
        }
    }
}
