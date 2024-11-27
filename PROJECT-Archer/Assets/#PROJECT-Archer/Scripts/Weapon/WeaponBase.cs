using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Archer
{
    public class WeaponBase : MonoBehaviour
    {
        public Cinemachine.CinemachineImpulseSource impulseSource; // 총기 반동 효과를 넣기 위한 객체

        public CharacterBase Owner
        {
            get => owner;
            set => owner = value;
        }
        private CharacterBase owner; // 무기를 소유한 캐릭터

        public int RemainAmmo => currentAmmo;

        public Projectile bulletPrefab; // 총알 프리팹 객체 - 발사 시 복사할 총알의 원본 GameObject
        public Transform firePoint; // 총알 발사 위치 및 방향을 담고 있는 Transform

        public float fireRate; // 연사 속도 (시간 값) => ex) 0.1은 0.1초에 1발씩 발사하는 값
        public int clipSize; // 탄창 크기(용량)
        public float lastFireTime; // 마지막(가장 최근)에 발사 시간 
        private int currentAmmo; // 현재 탄창의 남은 총알 수

        public const float spread = 5f;

        private void Awake()
        {
            currentAmmo = clipSize;
            fireRate = Mathf.Max(fireRate, 0.1f); // 최소 연사 속도(시간 값)는 0.1
            TryGetComponent(out impulseSource);
        }

        public bool Fire()
        {
            if (currentAmmo <= 0 || Time.time - lastFireTime < fireRate)
            {
                return false;
            }

            Vector2 randomSpread = Random.insideUnitCircle;
            Vector2 spreadRotation = randomSpread * spread;


            lastFireTime = Time.time;
            Projectile bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(spreadRotation.x, 0f, spreadRotation.y));
            bullet.gameObject.SetActive(true);

            currentAmmo--;

            // Muzzle Effect 출력
            EffectManager.Instance.CreateEffect(EffectType.Muzzle_01, firePoint.position, firePoint.rotation);

            // Fire Sound 출력
            SoundManager.Singleton.PlaySFX(SFXFileName.GunFire, firePoint.position);

            if (!owner.IsNPC)
            {
                impulseSource.GenerateImpulse();
            }

            return true;
        }
        public void Reload() => currentAmmo = clipSize;
    }
}
