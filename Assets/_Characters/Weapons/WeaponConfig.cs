using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Weapom"))]

    public class WeaponConfig : ScriptableObject
    {

        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] float timeBetweenAnimationCycles = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float additionalDamage = 10;
        [SerializeField] float damageDelay = 0.5f;
        [SerializeField] AudioClip[] hitSound; 

        public float GetTimeBetweenAnimationCycles()
        {
            return timeBetweenAnimationCycles;
        }

        public float GetDamageDelay()
        {
            return damageDelay;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAnimClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        public float GetAditionalDamage()
        {
            return additionalDamage;
        }

        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
        }

        public AudioClip GetRandomHitClips()
        {
            return hitSound[Random.Range(0,hitSound.Length)];
        }
    }
}