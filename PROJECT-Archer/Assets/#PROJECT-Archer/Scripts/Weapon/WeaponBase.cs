using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Archer
{
    public class WeaponBase : MonoBehaviour
    {
        public Cinemachine.CinemachineImpulseSource impulseSource; // �ѱ� �ݵ� ȿ���� �ֱ� ���� ��ü

        public CharacterBase Owner
        {
            get => owner;
            set => owner = value;
        }
        private CharacterBase owner; // ���⸦ ������ ĳ����

        public int RemainAmmo => currentAmmo;

        public Projectile bulletPrefab; // �Ѿ� ������ ��ü - �߻� �� ������ �Ѿ��� ���� GameObject
        public Transform firePoint; // �Ѿ� �߻� ��ġ �� ������ ��� �ִ� Transform

        public float fireRate; // ���� �ӵ� (�ð� ��) => ex) 0.1�� 0.1�ʿ� 1�߾� �߻��ϴ� ��
        public int clipSize; // źâ ũ��(�뷮)
        public float lastFireTime; // ������(���� �ֱ�)�� �߻� �ð� 
        private int currentAmmo; // ���� źâ�� ���� �Ѿ� ��

        public const float spread = 5f;

        private void Awake()
        {
            currentAmmo = clipSize;
            fireRate = Mathf.Max(fireRate, 0.1f); // �ּ� ���� �ӵ�(�ð� ��)�� 0.1
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

            // Muzzle Effect ���
            EffectManager.Instance.CreateEffect(EffectType.Muzzle_01, firePoint.position, firePoint.rotation);

            // Fire Sound ���
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
