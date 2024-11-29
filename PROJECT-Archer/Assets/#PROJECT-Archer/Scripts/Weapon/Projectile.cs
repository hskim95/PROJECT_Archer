using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class Projectile : MonoBehaviour
    {
        public float lifeTime; // 발사체가 활성화 된 이후 살아있는 시간 [제한 시간]
        public float speed; // 발사 속도
        Rigidbody rigid;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
            rigid.AddForce(speed * transform.forward, ForceMode.VelocityChange);
            Destroy(gameObject, lifeTime); // 본인 GameObject를 LifeTime이 지나서 파괴
        }


        private void OnCollisionEnter(Collision collision)
        {
            // To Do : 충돌 시 어떤 오브젝트에 부딪혔는지 확인
            // 일반 오브젝트이면 이펙트만 출력 || 캐릭터 오브젝트이면 이펙트 + 데미지 주기
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
            else // 캐릭터가 아닌 것에 충돌했을 때
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
