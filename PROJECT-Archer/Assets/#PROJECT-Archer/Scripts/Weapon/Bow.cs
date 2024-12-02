using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class Bow : WeaponBase
    {
        public Bow Instance { get; private set; }

        [SerializeField] LineRenderer bowString;
        Transform bowUpperEdge, bowLowerEdge;
        [SerializeField] Transform arrowHolder;
        [SerializeField] GameObject arrowVisual;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            bowUpperEdge = transform.GetChild(0);
            bowLowerEdge = transform.GetChild(1);
            if (bowString.positionCount != 3)
            {
                bowString.positionCount = 3;
            }
            bowString.SetPosition(0, bowUpperEdge.position);
            bowString.SetPosition(2, bowLowerEdge.position);
        }

        private void Update()
        {
            bowString.SetPosition(0, bowUpperEdge.position);
            bowString.SetPosition(2, bowLowerEdge.position);
            if (Owner.IsAimingActive)
            {
                bowString.SetPosition(1, arrowHolder.position);
            }
            else
            {
                bowString.SetPosition(1, Vector3.Lerp(bowUpperEdge.position, bowLowerEdge.position, 0.5f));
            }
        }

        private void DrawArrow()
        {
            arrowVisual.SetActive(true);
        }

        public override bool Fire()
        {
            arrowVisual.SetActive(false);
            return base.Fire();
        }

        public override void Reload()
        {
            base.Reload();
            arrowVisual.SetActive(true);
        }
    }
}
