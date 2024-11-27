using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class GizmoController : MonoBehaviour
    {
        public float distance = 3f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Vector3 startPos = transform.position + Vector3.up;
            Vector3 endPos = startPos + (transform.forward * distance);

            Gizmos.DrawLine(startPos, endPos);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(endPos, 1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(endPos + Vector3.right, new Vector3(3f, 1f, 2f));
        }
    }

}
