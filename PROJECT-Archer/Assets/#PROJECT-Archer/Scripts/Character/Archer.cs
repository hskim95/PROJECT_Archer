using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{

    public class Archer : CharacterBase
    {
        public Bow currentBow;
        public bool IsAimed
        { 
            get => isAimed;  
            set
            {
                isAimed = value;
                characterAnimator.SetBool("IsAimed", value);
            }
        }
        private bool isAimed = false;

        protected override void Awake()
        {
            base.Awake();
            currentBow = currentWeapon as Bow;
        }

        public void Shoot()
        {
            characterAnimator.SetTrigger("Shoot");
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}
