using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

namespace Archer
{
    public class CharacterController : MonoBehaviour
    {
        public LayerMask interactionLayer;
        private CharacterBase character;

        private IInteractable[] interactableObjects;

        private void Awake()
        {
            TryGetComponent(out character);
        }

        private void Start()
        {
            InputSystem.Singleton.OnClickSpace += CommandJump;
            //InputSystem.Singleton.OnClickLeftMouseButtonDown += CommandAttack;
            InputSystem.Singleton.OnClickLeftMouseButtonDown += CommandRapidfireStart;
            InputSystem.Singleton.OnClickLeftMouseButtonUp += CommandRapidfireStop;
            InputSystem.Singleton.OnClickRightMouseButtonDown += CommandAimfireStart;
            InputSystem.Singleton.OnClickRightMouseButtonUp += CommandAimfireStop;

            InputSystem.Singleton.OnClickInteract += CommandInteract;
            InputSystem.Singleton.OnMouseScrollWheel += CommandMouseScrollWheel;

            if (CompareTag("Player"))
            {
                IngameUI.Instance.SetHP(character.CurrentHP, character.maxStat.CharacterData.HP);
                IngameUI.Instance.SetSP(character.CurrentSP, character.maxStat.CharacterData.SP);
            }

            CameraSystem.Singleton.SetCameraFollowTarget(character.cameraPivot);
        }

        private void CommandRapidfireStart()
        {
            character.Shoot(true);
        }

        private void CommandRapidfireStop()
        {
            character.Shoot(false);
        }

        private void CommandAimfireStart()
        {
            character.Shoot(true);
        }

        private void CommandAimfireStop()
        {
            character.Shoot(false);
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
            Collider[] overlappedObjects = Physics.OverlapSphere(character.transform.position, 2f, interactionLayer, QueryTriggerInteraction.Collide);

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
            if (character.IsAlive)
            {
                CheckOverlapInteractionObject();

                if (character.IsArmed)
                {
                    character.Rotate(InputSystem.Singleton.Look.x);
                    character.AimingPoint = CameraSystem.Singleton.AimingPoint;
                }

                character.Move(InputSystem.Singleton.Movement, character.IsArmed ? Camera.main.transform.eulerAngles.y : character.transform.rotation.y);

                //character.SetRunning(InputSystem.Instance.IsLeftShift); -> 예전 코드
                character.IsRun = InputSystem.Singleton.IsLeftShift;

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    character.SetArmed(!character.IsArmed);
                    if (character.IsArmed)
                    {
                        float differAngle = Camera.main.transform.eulerAngles.y - character.transform.eulerAngles.y;
                        character.Rotate(differAngle);
                    }
                }

            }

            if (CompareTag("Player"))
            {
                var ingameUI = UIManager.Singleton.GetUI<IngameUI>(UIList.IngameUI);
                ingameUI.SetHP(character.CurrentHP, character.maxStat.CharacterData.HP);
                ingameUI.SetSP(character.CurrentSP, character.maxStat.CharacterData.SP);
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
