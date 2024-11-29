using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class Bow : WeaponBase
    {
        [SerializeField] LineRenderer bowString;
        Transform bowUpperEdge, bowLowerEdge;
        [SerializeField] Transform arrowHolder;

        private void Awake()
        {
            bowUpperEdge = transform.GetChild(0);
            bowLowerEdge = transform.GetChild(1);
            if (bowString.positionCount != 3)
            {
                bowString.positionCount = 3;
            }
            bowString.SetPosition(0, bowUpperEdge.localPosition);
            bowString.SetPosition(2, bowLowerEdge.localPosition);
        }

        private void Update()
        {
            bowString.SetPosition(1, arrowHolder.position - transform.position);
        }

        private void DrawArrow()
        {
            Projectile arrow = Instantiate(arrowPrefab, new Vector3(0.35f, 0, 0), Quaternion.Euler(new Vector3(0, 0, 90f)), arrowHolder);
            arrow.gameObject.SetActive(true);
        }
    }
}
