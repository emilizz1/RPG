using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    //[RequireComponent(typeof(Image))]
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float energyRegenPerSecond = 1f;
        [SerializeField] AudioClip outOfEnergy;
         
        float currentEnergyPoints;
        AudioSource audioSource;

        float energyAsPercent { get { return currentEnergyPoints / maxEnergyPoints;} }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            currentEnergyPoints = maxEnergyPoints;
            AttackInitialAbilities();
            UpdateEnergyBar();
        }

        void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergyPoints();
                UpdateEnergyBar();
             }
        }

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        void AttackInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        void AddEnergyPoints()
        {
            var pointsToAdd = energyRegenPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            //var energyComponent = GetComponent<SpecialAbilities>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();
            if (energyCost <= currentEnergyPoints)
            {
                EnergyUsed(energyCost); //read from SO 
                abilities[abilityIndex].Use(target);
            }
            else
            {
                audioSource.PlayOneShot(outOfEnergy);
            }
        }

        public void EnergyUsed(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBar();
        }

        void UpdateEnergyBar()
        {
            if (energyBar)
            {
                energyBar.fillAmount = energyAsPercent;
            }
        }

        public void AddMaxEnergy(float amount)
        {
            maxEnergyPoints += amount;
            currentEnergyPoints = maxEnergyPoints;
        }
    }
}
