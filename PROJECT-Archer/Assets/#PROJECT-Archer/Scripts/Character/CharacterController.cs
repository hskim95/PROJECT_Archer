using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class CharacterController : MonoBehaviour
    {
        public LayerMask interactionLayer;
        private Archer archerCharacter;

        private IInteractable[] interactableObjects;

        private void Awake()
        {
            TryGetComponent(out archerCharacter);
        }

        private void Start()
        {
            InputSystem.Singleton.OnClickSpace += CommandJump;
            InputSystem.Singleton.OnClickLeftMouseButtonDown += CommandShoot;
            InputSystem.Singleton.OnClickRightMouseButtonDown += CommandAimStart;
            InputSystem.Singleton.OnClickRightMouseButtonUp += CommandAimStop;

            InputSystem.Singleton.OnClickInteract += CommandInteract;
            InputSystem.Singleton.OnMouseScrollWheel += CommandMouseScrollWheel;

            if (CompareTag("Player"))
            {
                IngameUI.Instance.SetHP(archerCharacter.CurrentHP, archerCharacter.maxStat.CharacterData.HP);
                IngameUI.Instance.SetSP(archerCharacter.CurrentSP, archerCharacter.maxStat.CharacterData.SP);
            }

            CameraSystem.Instance.SetCameraFollowTarget(archerCharacter.cameraPivot);
        }

        private void CommandShoot()
        {
            if (!archerCharacter.IsArmed) return;
            archerCharacter.Shoot();
        }

        private void CommandAimStart()
        {
            if (!archerCharacter.IsArmed) return;
            archerCharacter.IsAimed = true;
        }

        private void CommandAimStop()
        {
            if (!archerCharacter.IsArmed) return;
            archerCharacter.IsAimed = false;
        }

        public void CommandMouseScrollWheel(float delta)
        {
            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            if (delta > 0)
            {
                interactionUI.SelectPrev();
            }
            else if (delta < 0)
            {
                interactionUI.SelectNext();
            }
        }

        private void CommandInteract()
        {
            if (interactableObjects != null && interactableObjects.Length > 0)
            {
                var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
                interactionUI.Execute();
                //interactableObjects[0].Interact();
            }
        }

        public void CheckOverlapInteractionObject()
        {
            Collider[] overlappedObjects = Physics.OverlapSphere(archerCharacter.transform.position, 2f, interactionLayer, QueryTriggerInteraction.Collide);

            List<IInteractable> interactables = new List<IInteractable>();
            for (int i = 0; i < overlappedObjects.Length; i++)
            {
                if (overlappedObjects[i].TryGetComponent(out IInteractable interaction))
                {
                    interactables.Add(interaction);
                }
            }
            interactableObjects = interactables.ToArray();

            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            interactionUI.SetInteractableObjects(interactableObjects);
        }

        private void Update()
        {
            if (archerCharacter.IsAlive)
            {
                CheckOverlapInteractionObject();

                if (archerCharacter.IsArmed)
                {
                    archerCharacter.Rotate(InputSystem.Singleton.Look.x);
                    archerCharacter.AimingPoint = CameraSystem.Instance.AimingPoint;
                }

                archerCharacter.Move(InputSystem.Singleton.Movement, archerCharacter.IsArmed ? Camera.main.transform.eulerAngles.y : archerCharacter.transform.rotation.y);

                archerCharacter.IsRun = InputSystem.Singleton.IsLeftShift;

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    archerCharacter.characterAnimator.SetTrigger("WeaponShift");
                    archerCharacter.SetArmed(!archerCharacter.IsArmed);
                    CrossHairUI crosshairUI = UIManager.Singleton.GetUI<CrossHairUI>(UIList.CrossHairUI);
                    crosshairUI.IsDrawCrosshair = archerCharacter.IsArmed;

                    /*
                    if (archerCharacter.IsArmed)
                    {
                        float differAngle = Camera.main.transform.eulerAngles.y - archerCharacter.transform.eulerAngles.y;
                        archerCharacter.Rotate(differAngle);
                    }
                    */
                }

            }

            if (CompareTag("Player"))
            {
                var ingameUI = UIManager.Singleton.GetUI<IngameUI>(UIList.IngameUI);
                ingameUI.SetHP(archerCharacter.CurrentHP, archerCharacter.maxStat.CharacterData.HP);
                ingameUI.SetSP(archerCharacter.CurrentSP, archerCharacter.maxStat.CharacterData.SP);
            }
        }

        public float bottomClamp = -90f;
        public float topClamp = 90f;
        public Transform cameraPivot;
        private float targetYaw;
        private float targetPitch;

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            if (InputSystem.Singleton.Look.sqrMagnitude > 0f)
            {
                float yaw = InputSystem.Singleton.Look.x;
                float pitch = InputSystem.Singleton.Look.y;

                targetYaw += yaw;
                targetPitch -= pitch;
            }

            targetYaw = ClampAngle(targetYaw, float.MinValue, float.MaxValue);
            targetPitch = ClampAngle(targetPitch, -90f, 90f);

            cameraPivot.rotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
        }

        private float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void CommandJump()
        {
            //character.Jump();
        }

        private void CommandAttack()
        {
            //character.Attack();
        }
    }

}
