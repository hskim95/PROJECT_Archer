using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public enum CameraType
    {
        TPS,
        QuaterView,
        FPS,
    }
    public class CameraSystem : SingletonBase<CameraSystem>
    {
        public void SetCameraFollowTarget(Transform target)
        {
            tpsCamera.Follow = target;
            quarterCamera.Follow = target;
            fpsCamera.Follow = target;
            tpsCamera.LookAt = target;
            quarterCamera.LookAt = target;
            fpsCamera.LookAt = target;
        }

        // 각각의 VirtualCamera GameObject들을 들고 있어야 함
        // F1, F2, F3 키를 누를 때, 각 키에 대응하는 VirtualCamera GameObject를 켜 주고,
        // 나머지 카메라는 꺼준다.

        public Cinemachine.CinemachineVirtualCamera tpsCamera;
        public Cinemachine.CinemachineVirtualCamera quarterCamera;
        public Cinemachine.CinemachineVirtualCamera fpsCamera;

        public Vector3 AimingPoint { get; private set; }
        public LayerMask aimingLayerMask;
        public CrossHairUI crossHairUI;
        UnityEngine.UI.Image[] images;

        private CameraType currentCameraType = CameraType.TPS;
        private bool isZoom = false;

        public bool IsZoom => isZoom;

        protected override void Awake()
        {
            base.Awake();
            images = crossHairUI.CrossHair.GetComponentsInChildren<UnityEngine.UI.Image>();
        }

        public void ChangeCameraType(CameraType newType)
        {
            // 파라미터로 넘어온 카메라 타입 값을 currentCameraType 변수에 저장한다.
            currentCameraType = newType;

            // 일단 모든 카메라를 꺼둔다.
            tpsCamera.gameObject.SetActive(false);
            quarterCamera.gameObject.SetActive(false);
            fpsCamera.gameObject.SetActive(false);

            switch (currentCameraType)
            {
                case CameraType.TPS:
                    tpsCamera.gameObject.SetActive(true);
                    break;
                case CameraType.QuaterView:
                    quarterCamera.gameObject.SetActive(true);
                    break;
                case CameraType.FPS:
                    fpsCamera.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ChangeCameraType(CameraType.TPS);
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                ChangeCameraType(CameraType.QuaterView);
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                ChangeCameraType(CameraType.FPS);
            }

            // 마우스 오른쪽 버튼이 다운 시, 한 번 실행
            if (Input.GetMouseButtonDown(1))
            {
                isZoom = !isZoom; // 줌 모드 전환
            }

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
                // Debug.Log($"AimingPoint: {AimingPoint}");
                if (hitInfo.transform.root.CompareTag("Enemy"))
                {
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].color = Color.red;
                    }
                }
                else
                {
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].color = Color.black;
                    }
                }
            }
            else
            {
                AimingPoint = ray.GetPoint(100f);
            }
        }
    }
}
