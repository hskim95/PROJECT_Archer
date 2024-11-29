using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{

    public class Archer : CharacterBase
    {
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

        public void Shoot()
        {
            characterAnimator.SetTrigger("Shoot");
        }
    }
}
