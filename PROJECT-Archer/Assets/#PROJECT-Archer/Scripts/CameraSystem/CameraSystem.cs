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

        // ������ VirtualCamera GameObject���� ��� �־�� ��
        // F1, F2, F3 Ű�� ���� ��, �� Ű�� �����ϴ� VirtualCamera GameObject�� �� �ְ�,
        // ������ ī�޶�� ���ش�.

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
            // �Ķ���ͷ� �Ѿ�� ī�޶� Ÿ�� ���� currentCameraType ������ �����Ѵ�.
            currentCameraType = newType;

            // �ϴ� ��� ī�޶� ���д�.
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

            // ���콺 ������ ��ư�� �ٿ� ��, �� �� ����
            if (Input.GetMouseButtonDown(1))
            {
                isZoom = !isZoom; // �� ��� ��ȯ
            }

            float targetFOV = isZoom ? 20f : 60f; // ��ǥ ȭ���� isZoom�� ���� ����
            if (Mathf.Abs(targetFOV - tpsCamera.m_Lens.FieldOfView) > 0.1f)
            {
                tpsCamera.m_Lens.FieldOfView = Mathf.Lerp(tpsCamera.m_Lens.FieldOfView, targetFOV, Time.deltaTime * 5f); // Lerp�� �̿��� �ε巴�� ��ȯ
            }
            else if (targetFOV != tpsCamera.m_Lens.FieldOfView)
            {
                tpsCamera.m_Lens.FieldOfView = targetFOV;
            }

            // Aiming Point ��� �ڵ�
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
