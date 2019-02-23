using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO consider re-wire..
using RPG.Core;
using System;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(HealthSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 6f;
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointTime = 2f;
        [SerializeField] float waypointTolerance = 1;

        PlayerControl player = null;
        Character character;
        int nextWaypointIndex;
        float currentWeaponRange = 4f;
        float distanceToPlayer;
        bool isAlive = true;

        enum State { idle, attacking, chasing, patrolling}
        State state = State.idle;
        
        void Start()
        {
            character = GetComponent<Character>();
            player = FindObjectOfType<PlayerControl>();
        }

        void Update()
        {
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (isAlive)
            {
                bool inWeaponRange = distanceToPlayer <= currentWeaponRange;
                bool inChaseRange = distanceToPlayer > currentWeaponRange && distanceToPlayer <= chaseRadius;
                bool outsideChaseRange = distanceToPlayer > chaseRadius;
                
                if (inChaseRange)
                {
                    StopAllCoroutines();
                    weaponSystem.StopAttacking();
                    StartCoroutine(ChasePlayer());
                    character.ReturnAnimationFowardCap();
                }
                else if (inWeaponRange)
                {
                    StopAllCoroutines();
                    state = State.attacking;
                    weaponSystem.AttackTarget(player.gameObject);
                }
                else if (outsideChaseRange && state != State.patrolling)
                {
                    StopAllCoroutines();
                    weaponSystem.StopAttacking();
                    StartCoroutine(Patrol());
                }
            }
            else
            {
                StopAllCoroutines();
            }
        }

        IEnumerator Patrol()
        {
            state = State.patrolling;
            while (patrolPath != null)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPos);
                CycleWaypoint(nextWaypointPos);
                yield return new WaitForSecondsRealtime(waypointTime);
            }
        }

        private void CycleWaypoint(Vector3 nextWaypointPos)
        {
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        IEnumerator ChasePlayer()
        {
            state = State.chasing;
            while(distanceToPlayer >= currentWeaponRange || distanceToPlayer > chaseRadius)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        void Kill()
        {
            StopAllCoroutines();
            state = State.idle;
            isAlive = false;
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = new Color(255f, 0, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

            // Draw chase sphere 
            Gizmos.color = new Color(0, 0, 255, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}