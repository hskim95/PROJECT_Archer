using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem Instance { get; private set; } = null;

        public void SetCameraFollowTarget(Transform target)
        {
            tpsCamera.Follow = target;
            tpsCamera.LookAt = target;
        }

        // 각각의 VirtualCamera GameObject들을 들고 있어야 함
        // F1, F2, F3 키를 누를 때, 각 키에 대응하는 VirtualCamera GameObject를 켜 주고,
        // 나머지 카메라는 꺼준다.

        public Cinemachine.CinemachineVirtualCamera tpsCamera;

        public Vector3 AimingPoint { get; private set; }
        public LayerMask aimingLayerMask;

        private bool isZoom = false;

        public bool IsZoom => isZoom;

        private void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            float targetFOV = isZoom ? 20f : 60f; // 목표 화각을 isZoom에 따라 선택
            if (Mathf.Abs(targetFOV - tpsCamera.m_Lens.FieldOfView) > 0.1f)
            {
                tpsCamera.m_Lens.FieldOfView = Mathf.Lerp(tpsCamera.m_Lens.FieldOfView, targetFOV, Time.deltaTime * 5f); // Lerp를 이용해 부드럽게 전환
            }
            else if (targetFOV != tpsCamera.m_Lens.FieldOfView)
            {
                tpsCamera.m_Lens.FieldOfView = targetFOV;
            }

            // Aiming Point 계산 코드
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, aimingLayerMask, QueryTriggerInteraction.Ignore))
            {
                AimingPoint = hitInfo.point;

                if (hitInfo.transform.root.CompareTag("Enemy"))
                {
                    CrossHairUI.Instance.SetCrosshairColor(Color.red);
                }
                else
                {
                    CrossHairUI.Instance.SetCrosshairColor(Color.black);
                }
            }
            else
            {
                AimingPoint = ray.GetPoint(100f);
            }
        }
    }
}
