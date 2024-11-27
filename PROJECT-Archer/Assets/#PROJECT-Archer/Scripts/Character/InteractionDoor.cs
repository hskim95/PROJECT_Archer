using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class InteractionDoor : MonoBehaviour, IInteractable
    {
        public string InteractionMessage => "Open";

        public Transform pivot;
        public bool isOpened = false;

        public void Interact()
        {
            isOpened = !isOpened;
        }

        private void Update()
        {
            Quaternion targetRotation;
            if (isOpened)
            {
                targetRotation = Quaternion.Euler(0, -90, 0);
            }
            else
            {
                targetRotation = Quaternion.Euler(0, 0, 0); 
            }

            pivot.localRotation = Quaternion.Slerp(pivot.localRotation, targetRotation, Time.deltaTime * 5f);
        }
    }

}
