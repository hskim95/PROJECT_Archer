using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Archer
{
    public class WeaponBase : MonoBehaviour
    {
        public Cinemachine.CinemachineImpulseSource impulseSource; // 무기 반동 효과를 넣기 위한 객체

        public CharacterBase Owner
        {
            get => owner;
            set => owner = value;
        }
        private CharacterBase owner; // 무기를 소유한 캐릭터

        public int RemainAmmo => currentAmmo;

        public Projectile arrowPrefab; // 화살 프리팹 객체 - 발사 시 복사할 화살의 원본 GameObject
        public Transform firePoint; // 화살 발사 위치 및 방향을 담고 있는 Transform

        public float fireRate; // 연사 속도 (시간 값) => ex) 0.1은 0.1초에 1발씩 발사하는 값
        public const int clipSize = 1; // 탄창 크기(용량)
        public float lastFireTime; // 마지막(가장 최근)에 발사 시간 
        private int currentAmmo; // 현재 탄창의 남은 총알 수

        public const float spread = 5f;

        protected virtual void Awake()
        {
            currentAmmo = clipSize;
            fireRate = Mathf.Max(fireRate, 0.1f); // 최소 연사 속도(시간 값)는 0.1
            TryGetComponent(out impulseSource);
        }

        public virtual bool Fire()
        {
            if (currentAmmo <= 0 || Time.time - lastFireTime < fireRate)
            {
                return false;
            }

            Projectile newArrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            newArrow.gameObject.SetActive(true);

            lastFireTime = Time.time;
            currentAmmo--;

            // Muzzle Effect 출력
            //EffectManager.Instance.CreateEffect(EffectType.Muzzle_01, firePoint.position, firePoint.rotation);

            // Fire Sound 출력
            //SoundManager.Singleton.PlaySFX(SFXFileName.GunFire, firePoint.position);

            if (!owner.IsNPC)
            {
                if (impulseSource != null)
                {
                    impulseSource.GenerateImpulse();
                }
            }

            return true;
        }
        public virtual void Reload() => currentAmmo = clipSize;
    }
}
