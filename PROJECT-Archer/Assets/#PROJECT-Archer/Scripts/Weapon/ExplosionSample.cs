using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class ExplosionSample : MonoBehaviour
    {
        public LayerMask targetLayerMask;

        private void Start()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, targetLayerMask, QueryTriggerInteraction.Ignore); // QueryTrigger~~.Ignore : trigger 속성이 켜져 있는 collider는 무시

            List<Transform> executedTransforms = new List<Transform>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (executedTransforms.Contains(colliders[i].transform.root))
                {
                    continue;
                }

                if (colliders[i].transform.root.TryGetComponent(out IDamage damageInterface))
                {
                    damageInterface.ApplyDamage(500f);
                    colliders[i].attachedRigidbody.AddExplosionForce(1000f, transform.position, 5f, 1f, ForceMode.Impulse);

                    executedTransforms.Add(colliders[i].transform.root);
                }
            }
        }
    }
}
