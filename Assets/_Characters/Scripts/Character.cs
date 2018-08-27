using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; //TODO consider re-wire..

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        [Header("Animator Settings")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar characterAvatar;
        [SerializeField] [Range (.1f,1f)] float animatorFowardCap = 1;

        [Header("Audio")]
        [SerializeField] float audioSourceSpatialBlend = 0.5f;
        [SerializeField] float pitch = 1f;

        [Header("Captule Collider")]
        [SerializeField] Vector3 colliderCenter;
        [SerializeField] float collidererRadius;
        [SerializeField] float colliderHeight;
        
        [Header("Movement")]
        [SerializeField] float moveSpeedMulyiplier = 1f;
        [SerializeField] float animationSpeedMultiplier = 1f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;

        [Header("Nav Mesh Agent")]
        [SerializeField] float stoppingDistance = 1.3f;
        [SerializeField] float navMeshSteeringSpeed = 1f;

        NavMeshAgent navMeshAgent;
        Animator animator;
        Rigidbody myRigidbody;
        float turnAmount;
        float forwardAmount;
        bool isAlive = true;

        private void Awake()
        {
            AddRequiredComponents();
        }

        private void AddRequiredComponents()
        {
            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = collidererRadius;
            capsuleCollider.height = colliderHeight;

            myRigidbody = gameObject.AddComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSourceSpatialBlend;
            audioSource.pitch = pitch;
            
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.stoppingDistance = stoppingDistance;
            navMeshAgent.speed = navMeshSteeringSpeed;
            navMeshAgent.autoBraking = false;
            
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;
        }

        void Update()
        {
            if(navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        private void OnAnimatorMove()
        {
            if(Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMulyiplier) / Time.deltaTime;
                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }
        }

        public float GetAnimSpeedMultiplier()
        {
            return animator.speed;
        }

        public void Kill()
        {
            isAlive = false;
        }

        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        public AnimatorOverrideController GetOverrideController()
        {
            return animatorOverrideController;
        }

        void Move(Vector3 movement)
        {
            SetFoweardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        void SetFoweardAndTurn(Vector3 movement)
        {
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }
            var localMove = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        void UpdateAnimator()
        {
            animator.SetFloat("Forward", forwardAmount * animatorFowardCap, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animationSpeedMultiplier;
        }

        void ApplyExtraTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        public void ReturnAnimationFowardCap()
        {
            animatorFowardCap = 1f;
        }
    }
}