using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Archer
{
    public class WeaponBase : MonoBehaviour
    {
        public Cinemachine.CinemachineImpulseSource impulseSource; // ���� �ݵ� ȿ���� �ֱ� ���� ��ü

        public CharacterBase Owner
        {
            get => owner;
            set => owner = value;
        }
        private CharacterBase owner; // ���⸦ ������ ĳ����

        public int RemainAmmo => currentAmmo;

        public Projectile arrowPrefab; // ȭ�� ������ ��ü - �߻� �� ������ ȭ���� ���� GameObject
        public Transform firePoint; // ȭ�� �߻� ��ġ �� ������ ��� �ִ� Transform

        public float fireRate; // ���� �ӵ� (�ð� ��) => ex) 0.1�� 0.1�ʿ� 1�߾� �߻��ϴ� ��
        public const int clipSize = 1; // źâ ũ��(�뷮)
        public float lastFireTime; // ������(���� �ֱ�)�� �߻� �ð� 
        private int currentAmmo; // ���� źâ�� ���� �Ѿ� ��

        public const float spread = 5f;

        protected virtual void Awake()
        {
            currentAmmo = clipSize;
            fireRate = Mathf.Max(fireRate, 0.1f); // �ּ� ���� �ӵ�(�ð� ��)�� 0.1
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

            // Muzzle Effect ���
            //EffectManager.Instance.CreateEffect(EffectType.Muzzle_01, firePoint.position, firePoint.rotation);

            // Fire Sound ���
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
