using UnityEngine;
using RPG.CameraUI;
using System.Collections;

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        SpecialAbilities abilities;
        Character character;
        WeaponSystem weaponSystem;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();
            RegisterForMouseEvent();
        }

        void Update()
        {
            ScanForAbilityKeyDown();
        }

        void RegisterForMouseEvent()
        {
            CameraRaycaster cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverPotentiallyWalkable += onMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void onMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                weaponSystem.StopAttacking();
                character.SetDestination(destination);
            }
        }

        void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }
            }
        }

        bool IsTargetInRange(EnemyAI enemy)
        {
            if (enemy)
            {
                float distanceToTarget = (enemy.transform.position - transform.position).magnitude;
                return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
            }
            else
            {
                return false;
            }
        }

        void OnMouseOverEnemy(EnemyAI enemy)
        {
            if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0, enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1) && !IsTargetInRange(enemy))
            {
                StartCoroutine(MoveAndPowerAttack(enemy));
            }
            else if (Input.GetMouseButton(0) && IsTargetInRange(enemy))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy))
            {
                StartCoroutine(MoveAndAttack(enemy));
            }
        }

        IEnumerator MoveToTarget(EnemyAI target)
        {
            character.SetDestination(target.transform.position);
            while (!IsTargetInRange(target))
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndAttack(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy));
            weaponSystem.AttackTarget(enemy.gameObject);
        }

        IEnumerator MoveAndPowerAttack(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy));
            abilities.AttemptSpecialAbility(0, enemy.gameObject);
        }
    }
}