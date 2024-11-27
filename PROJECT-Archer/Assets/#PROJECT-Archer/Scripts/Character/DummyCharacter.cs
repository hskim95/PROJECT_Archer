using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class DummyCharacter : MonoBehaviour, IDamage
    {
        public float health = 100;
        public Animator animator;

        public Collider[] ragdollColliders;
        public Rigidbody[] ragdollRigidbodies;
        public Collider rootCollider;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            Transform hipTransform = animator.GetBoneTransform(HumanBodyBones.Hips);

            ragdollColliders = hipTransform.GetComponentsInChildren<Collider>();
            ragdollRigidbodies = hipTransform.GetComponentsInChildren<Rigidbody>();
            rootCollider = GetComponent<Collider>();

            SetRagdollActive(false);
        }

        public void SetRagdollActive(bool isActive)
        {
            for (int i = 0; i < ragdollRigidbodies.Length; i++)
            {
                ragdollRigidbodies[i].isKinematic = !isActive;
            }

            rootCollider.enabled = !isActive;
        }

        public void ApplyDamage(float damage)
        {
            health -= damage;

            if (health <= 0)
            {
                // To Do : 레그돌 활성화
                SetRagdollActive(true);
                animator.enabled = false;
            }
        }
    }
}
