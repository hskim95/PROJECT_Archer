using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Archer
{
    public class CharacterBase : MonoBehaviour, IDamage
    {
        public Animator characterAnimator;
        public UnityEngine.CharacterController unityCharacterController;
        public Transform cameraPivot;
        public Rigidbody[] ragdollRigidbodies;

        public CharacterStatData maxStat;
        [SerializeField] private CharacterStatData curStat;

        public bool IsAimingActive { get; set; } = false;

        public bool IsArmed { get; set; } = false;
        public bool IsRun { get; set; } = false;

        public bool IsAlive => CurrentHP > 0f;

        public bool IsNPC { get; set; } = false;

        public bool IsEquipDone => isEquipDone;
        [SerializeField] private bool isEquipDone = false;
        
        private bool isShooting = false;
        private bool isReloading = false;
        private bool isGrounded = false;

        public Vector3 AimingPoint
        {
            get => aimingPointTransform.position;
            set => aimingPointTransform.position = value;
        }

        public float CurrentHP
        {
            get => curStat.CharacterData.HP;
            set => curStat.CharacterData.HP = Mathf.Clamp(value, 0, maxStat.CharacterData.HP);
        }

        public float CurrentSP
        {
            get => curStat.CharacterData.SP;
            set => curStat.CharacterData.SP = Mathf.Clamp(value, 0, maxStat.CharacterData.SP);
        }

        public float targetRotation = 0f;
        public float moveSpeed;

        public float WalkSpeed => curStat.CharacterData.walkSpeed;
        public float RunSpeed => curStat.CharacterData.runSpeed;

        public float speed;
        public float armed;
        public float horizontal;
        public float vertical;
        public float runningBlend;

        public GameObject weaponHolder;
        public WeaponBase currentWeapon;
        public Transform aimingPointTransform;
        private Transform weaponTransform;
        public Transform returnTransform;

        public System.Action<float, float> OnDamaged;


        public void SetArmed(bool isArmed)
        {
            IsArmed = isArmed;
            if (IsArmed)
            {
                characterAnimator.SetTrigger("Equip Trigger");
            }
            else
            {
                isEquipDone = false;
                characterAnimator.SetTrigger("Holster Trigger");
            }
        }

        public void SetEquipState(int equipState)
        {
            bool isEquip = equipState == 1;
            if (isEquip)
            {
                weaponTransform.SetParent(weaponHolder.transform);
                weaponTransform.SetLocalPositionAndRotation(new Vector3(-0.08f, 0, 0), Quaternion.Euler(0, -90f, 0));
            }
            else
            {
                weaponTransform.SetParent(returnTransform);
                weaponTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        private void Awake()
        {
            TryGetComponent(out characterAnimator);
            TryGetComponent(out unityCharacterController);
            weaponTransform = currentWeapon.transform;

            currentWeapon.Owner = this;

            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            SetRagDollActive(false);

            SetEquipState(0);

            curStat = ScriptableObject.CreateInstance<CharacterStatData>();
            maxStat = GameDataModel.Singleton.PlayerCharacterStatData;

            StatInitalize();
        }

        private void StatInitalize()
        {
            curStat.CharacterData.HP = maxStat.CharacterData.HP;
            curStat.CharacterData.SP = maxStat.CharacterData.SP;
            curStat.CharacterData.walkSpeed = maxStat.CharacterData.walkSpeed;
            curStat.CharacterData.runSpeed = maxStat.CharacterData.runSpeed;
            curStat.CharacterData.RunSteminaCost = maxStat.CharacterData.RunSteminaCost;
            curStat.CharacterData.SteminaRecoverySpeed = maxStat.CharacterData.SteminaRecoverySpeed;
        }

        public void SetRagDollActive(bool isActive)
        {
            for (int i = 0; i < ragdollRigidbodies.Length; i++)
            {
                ragdollRigidbodies[i].isKinematic = !isActive;
            }

            unityCharacterController.enabled = !isActive;
            characterAnimator.enabled = !isActive;
        }

        private void Update()
        {
            if (IsAlive)
            {
                CheckGround();
                FreeFall();
                if (isShooting)
                {
                    bool isFireSuccess = currentWeapon.Fire();
                    if (!isFireSuccess)
                    {
                        if (currentWeapon.RemainAmmo <= 0 && !isReloading)
                        {
                            Reload();
                            SoundManager.Singleton.PlaySFX(SFXFileName.GunEmpty, currentWeapon.transform.position);
                        }
                    }
                }

                armed = Mathf.Lerp(armed, IsArmed ? 1f : 0f, Time.deltaTime * 10f);
                runningBlend = Mathf.Lerp(runningBlend, IsRun ? 1f : 0f, Time.deltaTime * 10f);

                characterAnimator.SetFloat("Speed", speed);
                characterAnimator.SetFloat("Armed", armed);
                characterAnimator.SetFloat("Horizontal", horizontal);
                characterAnimator.SetFloat("Vertical", vertical);
                characterAnimator.SetFloat("RunningBlend", runningBlend);

                CurrentSP += Time.deltaTime * curStat.CharacterData.SteminaRecoverySpeed;
            }
        }

        private float verticalAcceleration = 0f;
        public float groundOffset = 0.1f;
        public float checkRadius = 0.1f;
        public LayerMask groundLayers;

        public void CheckGround()
        {
            Ray ray = new Ray(transform.position + Vector3.up * groundOffset, Vector3.down);
            isGrounded = Physics.SphereCast(ray, checkRadius, 0.1f, groundLayers);
        }

        public void FreeFall()
        {
            verticalAcceleration = isGrounded ? 0 : -9.8f;
            unityCharacterController.Move(verticalAcceleration * Time.deltaTime * Vector3.up);
        }

        public void Move(Vector2 input, float yAxisAngle)
        {
            horizontal = input.x;
            vertical = input.y;

            // 실제 Transform이 움직이는 코드

            speed = input.magnitude > 0f ? 1f : 0f;

            float targetDistance = (IsRun ? RunSpeed : WalkSpeed) * Time.deltaTime;

            Vector3 movement; // = Vector3.zero

            if (IsArmed)
            {
                movement = transform.forward * vertical + transform.right * horizontal;
                if (IsRun)
                {
                    CurrentSP -= targetDistance * movement.magnitude * curStat.CharacterData.RunSteminaCost;
                }
            }
            else
            {
                movement = transform.forward * speed;

                if (input.magnitude > 0f)
                {
                    targetRotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + yAxisAngle;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref moveSpeed, 0.1f);
                    transform.rotation = Quaternion.Euler(0, rotation, 0);
                    CurrentSP -= targetDistance * 2f;
                }
            }
            unityCharacterController.Move(targetDistance * movement);
        }

        public void Rotate(float angle)
        {
            float rotation = transform.rotation.eulerAngles.y + angle;
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        public void Rotate(Vector3 targetPoint)
        {
            targetPoint.y = transform.position.y;
            Vector3 currentPos = transform.position;
            Vector3 direction = targetPoint - currentPos;
            float angle = Vector3.Angle(transform.forward, direction);
            Rotate(angle);
        }

        public void Shoot(bool isShoot)
        {
            isShooting = isEquipDone && isShoot;
        }

        public void Reload()
        {
            // To Do : 재장전 애니메이션을 Trigger 하는 코드
            isReloading = true;
            characterAnimator.SetTrigger("Reload Trigger");

            // Reload 사운드 출력
            //SoundManager.Singleton.PlaySFX(SFXFileName.GunReload, currentWeapon.transform.position);
        }

        public void ReloadComplete()
        {
            currentWeapon.Reload();
            isReloading = false;
        }

        public void EquipComplete()
        {
            Debug.Log("Equip motion complete!");
            isEquipDone = true;
        }

        public void ApplyDamage(float damage)
        {
            CurrentHP -= damage;

            if (!IsAlive)
            {
                SetRagDollActive(true);

                if (IsNPC)
                {
                    if (TryGetComponent(out CharacterController_AI ai))
                    {
                        ai.enabled = false;
                    }
                    Destroy(gameObject, 5f);
                }
                else
                {
                    if (TryGetComponent(out CharacterController controller))
                    {
                        controller.enabled = false;
                    }
                }
            }

            OnDamaged?.Invoke(maxStat.CharacterData.HP, curStat.CharacterData.HP);
        }

        public Transform GetBoneTransform(HumanBodyBones bone)
        {
            return characterAnimator ? characterAnimator.GetBoneTransform(bone) : transform;
        }
    }
}
