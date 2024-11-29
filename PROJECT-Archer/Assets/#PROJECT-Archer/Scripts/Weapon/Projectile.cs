using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class Projectile : MonoBehaviour
    {
        public float lifeTime; // �߻�ü�� Ȱ��ȭ �� ���� ����ִ� �ð� [���� �ð�]
        public float speed; // �߻� �ӵ�
        Rigidbody rigid;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
            rigid.AddForce(speed * transform.forward, ForceMode.VelocityChange);
            Destroy(gameObject, lifeTime); // ���� GameObject�� LifeTime�� ������ �ı�
        }


        private void OnCollisionEnter(Collision collision)
        {
            // To Do : �浹 �� � ������Ʈ�� �ε������� Ȯ��
            // �Ϲ� ������Ʈ�̸� ����Ʈ�� ��� || ĳ���� ������Ʈ�̸� ����Ʈ + ������ �ֱ�
            // Debug.Log($"<color=red>Bullet impact!</color> <color=green>Name: {collision.gameObject.name}</color>");

            Vector3 position = collision.GetContact(0).point;
            Quaternion rotation = Quaternion.LookRotation(collision.GetContact(0).normal);

            if (collision.gameObject.layer == LayerMask.NameToLayer("HitScanner"))
            {
                EffectManager.Instance.CreateEffect(EffectType.Impact_Wood, position, rotation);

                if (collision.transform.root.TryGetComponent(out IDamage damageInterface))
                {
                    float damageMultiple = 1f;
                    if (collision.gameObject.TryGetComponent(out DamageMultiplier multiplier))
                    {
                        damageMultiple = multiplier.DamageMultiply;
                    }

                    damageInterface.ApplyDamage(damageMultiple * 10f);
                    collision.rigidbody.AddForceAtPosition(speed * transform.forward, position, ForceMode.Impulse);
                    rigid.AddForce(-collision.GetContact(0).normal, ForceMode.Impulse);
                }
            }
            else // ĳ���Ͱ� �ƴ� �Ϳ� �浹���� ��
            {
                if (collision.transform.TryGetComponent(out IDamage damageInterface))
                {
                    damageInterface.ApplyDamage(10f);
                    Rigidbody rb = collision.rigidbody;
                    if (rb != null)
                    {
                        rb.AddForceAtPosition(speed * 0.1f * transform.forward, position, ForceMode.Impulse);
                    }
                }

                EffectType targetEffectType;
                if (collision.collider.material.name.Contains("Wood"))
                {
                    targetEffectType = EffectType.Impact_Wood;
                }
                else if (collision.collider.material.name.Contains("Metal"))
                {
                    targetEffectType = EffectType.Impact_Metal;
                }
                else if (collision.collider.material.name.Contains("Concrete"))
                {
                    targetEffectType = EffectType.Impact_Concrete;
                }
                else
                {
                    targetEffectType = EffectType.Impact_Stone;
                }
                EffectManager.Instance.CreateEffect(targetEffectType, position, rotation);
            }
            Destroy(gameObject);
        }
    }
}
