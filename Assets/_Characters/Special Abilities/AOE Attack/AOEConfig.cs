using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
    [CreateAssetMenu(menuName = ("RPG/Special Ability/AOE Attack"))]
    public class AOEConfig : AbilityConfig
    {
        [Header("AOE Attack")]
        [SerializeField] float radius = 1f;
        [SerializeField] float damageToEachTarget = 10f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AOEbehaviour>();
        }

        public float GetRadius()
        {
            return radius;
        }

        public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }
    }
}
