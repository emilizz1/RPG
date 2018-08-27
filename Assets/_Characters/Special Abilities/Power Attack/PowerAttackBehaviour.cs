using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : AbilityBehaviour
    {
        public override void Use(GameObject target)
        {
            PlayAbilitySound();
            DealDamage(target);
            PlayParticalEffect();
            PlayAbilityAnimation();
        }

        private void DealDamage(GameObject target)
        {
            float damageToDeal = (config as PowerAttackConfig).GetExtraDamage();
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }
    }
}