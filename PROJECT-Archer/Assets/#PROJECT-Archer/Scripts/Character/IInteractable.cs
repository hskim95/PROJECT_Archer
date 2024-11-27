using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public interface IInteractable
    {
        public string InteractionMessage { get; }

        public void Interact();
    }

}