using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public enum AIState
    {
        Peaceful,
        Engaged,
    }

    public class CharacterController_AI : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            if (aiState == AIState.Engaged && target != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.transform.position);
            }
            else if (aiState == AIState.Peaceful && patrolWaypoints.Count > 0)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, patrolWaypoints[currentWaypointIndex].position);
            }
        }

        CharacterBase linkedCharacter;
        public UnityEngine.AI.NavMeshAgent navAgent;

        public AIState aiState = AIState.Peaceful;
        public float detectionRadius = 10f;
        public LayerMask detectionLayers;
        public LayerMask attackValidLayer;

        [SerializeField] CharacterBase target = null;

        // 정찰 기능에 사용할 변수
        public List<Transform> patrolWaypoints = new List<Transform>(); // 인스펙터 창에서 transform 연결 예정
        public int currentWaypointIndex = 0;

        private void Awake()
        {
            linkedCharacter = GetComponent<CharacterBase>();
            linkedCharacter.IsNPC = true;
            navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            navAgent.updateRotation = false;
        }

        private void Start()
        {
            navAgent.speed = linkedCharacter.WalkSpeed;
            linkedCharacter.SetArmed(true);
        }

        private void Update()
        {
            if (linkedCharacter.IsAlive)
            {
                UpdateDetecting();

                if (aiState == AIState.Peaceful)
                {
                    if (navAgent.remainingDistance <= navAgent.stoppingDistance)
                    {
                        OnDestinationArrival();
                    }
                    else
                    {
                        Move();
                    }
                }
                else if (aiState == AIState.Engaged)
                {
                    UpdateCombat();
                }
            }
        }

        private void Move()
        {
            if (navAgent.hasPath)
            {
                Vector3 moveDirection = (navAgent.steeringTarget - transform.position).normalized;
                Vector3 localDirection = linkedCharacter.transform.InverseTransformDirection(moveDirection);
                Vector2 input = new Vector2(localDirection.x, localDirection.z);
                linkedCharacter.Move(input, 0);
                linkedCharacter.transform.forward = moveDirection;
            }
        }

        private void UpdateDetecting()
        {
            if (aiState != AIState.Peaceful) return;

            Collider[] detectedColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayers, QueryTriggerInteraction.Ignore);
            if (detectedColliders.Length <= 0) return;

            for (int i = 0; i < detectedColliders.Length; i++)
            {
                if (detectedColliders[i].transform.root.TryGetComponent(out CharacterBase character) && character.gameObject.CompareTag("Player") && character.IsAlive)
                {
                    target = character;
                    Vector3 pivot = linkedCharacter.transform.position + Vector3.up;
                    Vector3 targetPosition = target.transform.position + Vector3.up;
                    Vector3 direction = (targetPosition - pivot).normalized;
                    Ray ray = new Ray(pivot, direction);

                    bool isRaycastHitTheTarget = false;

                    if (Physics.Raycast(ray, out RaycastHit hitInfo, detectionRadius, attackValidLayer, QueryTriggerInteraction.Ignore))
                    {
                        isRaycastHitTheTarget = hitInfo.transform.root.CompareTag("Player");
                    }

                    if (isRaycastHitTheTarget)
                    {
                        SetAIState(AIState.Engaged);
                    }
                    else
                    {
                        target = null;
                    }

                    break;
                }
            }
        }

        public void SetDestination(Vector3 destination) => navAgent.SetDestination(destination);

        public void OnDestinationArrival() => MoveToNextWaypoint();

        public void MoveToNextWaypoint()
        {
            if (patrolWaypoints.Count <= 0) return;

            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
            SetDestination(patrolWaypoints[currentWaypointIndex].position);
        }

        public void ChaseTarget()
        {
            linkedCharacter.IsRun = true;
            SetDestination(target.transform.position);
        }

        private void UpdateCombat()
        {
            if (aiState != AIState.Engaged || target == null) return;

            if (!target.IsAlive)
            {
                target = null;
                linkedCharacter.Shoot(false);
                SetAIState(AIState.Peaceful);
                return;
            }

            Vector3 pivot = linkedCharacter.currentWeapon.transform.position;
            Vector3 targetPosition = target.transform.position + Vector3.up;
            Vector3 direction = (targetPosition - pivot).normalized;
            Ray ray = new Ray(pivot, direction);

            bool isRaycastHitTheTarget = false;
            const float weaponRange = 7f;

            if (Physics.Raycast(ray, out RaycastHit hitInfo, weaponRange, attackValidLayer, QueryTriggerInteraction.Ignore))
            {
                isRaycastHitTheTarget = hitInfo.transform.root.CompareTag("Player");
            }

            if (isRaycastHitTheTarget)
            {            
                // To Do: 발견한 캐릭터(플레이어) 공격
                Transform targetChestTransform = target.GetBoneTransform(HumanBodyBones.Chest);
                linkedCharacter.AimingPoint = targetChestTransform.position;
                linkedCharacter.IsRun = false;
                SetDestination(transform.position);
                linkedCharacter.Move(Vector2.zero, 0f);
                linkedCharacter.Shoot(true);
            }
            else
            {
                ChaseTarget();
                Move();
            }
        }

        public void SetAIState(AIState state)
        {
            aiState = state;
            linkedCharacter.IsAimingActive = aiState == AIState.Engaged;
        }
    }
}
