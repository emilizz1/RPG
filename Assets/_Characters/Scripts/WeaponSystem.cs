using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{ 
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [SerializeField] WeaponConfig currentWeaponConfig;

        GameObject target;
        GameObject weaponObject;
        Animator animator;
        Character character;
        AudioSource audioSource;
        float lastHitTime;

        const string ATTACK_TRIGGER = "Attacking";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();
            audioSource = GetComponent<AudioSource>();
            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
        }

        void Update()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;
            if(target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            }
            else
            {
                targetIsDead = target.GetComponent<HealthSystem>().healthAsPercentage <= Mathf.Epsilon;
                targetIsOutOfRange = Vector3.Distance(transform.position, target.transform.position) > currentWeaponConfig.GetMaxAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

            if(characterIsDead || targetIsDead || targetIsOutOfRange)
            {
                StopAllCoroutines();
            }

            if(target != null && !targetIsDead)
            {
                transform.LookAt(target.transform);
            }
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject); //empty hands
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = weaponToUse.gripTransform.localPosition;
            weaponObject.transform.localRotation = weaponToUse.gripTransform.localRotation;
        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack; 
            StartCoroutine(AttackTargetRepeatadly()); 
        }

        IEnumerator AttackTargetRepeatadly()
        {
            
            bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            while (attackerStillAlive && targetStillAlive)
            {
                var animationClip = currentWeaponConfig.GetAnimClip();
                float animationClipTime = animationClip.length / character.GetAnimSpeedMultiplier();
                float timeToWait = animationClipTime + currentWeaponConfig.GetTimeBetweenAnimationCycles();
                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait; 
                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
            }
        }

        private void AttackTargetOnce()
        {
            transform.LookAt(target.transform);
            animator.SetTrigger(ATTACK_TRIGGER);
            float damageDelay = currentWeaponConfig.GetDamageDelay();
            SetAttackAnimation();
            audioSource.PlayOneShot(currentWeaponConfig.GetRandomHitClips());
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay (float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No dominant hand found on player");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominant hand scripts");
            return dominantHands[0].gameObject;
        }

        float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAditionalDamage();
        }
         
        void SetAttackAnimation()
        {
            if (!character.GetOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("please provide " + gameObject + "with animator override controller");
            }
            var animatorOverrideController = character.GetOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip();
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        public void AddBaseDamage(float amount)
        {
            baseDamage += amount;
        }
    }
}
