using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class InteractionUI_ListItem : MonoBehaviour
    {
        public string InteractionText
        {
            set
            {
                itemNameText.text = value;
            }
        }

        public bool IsShowShortcut
        {
            set
            {
                shortcut.SetActive(value);
            }
        }

        public IInteractable interactionData { get; set; }

        public void Execute()
        {
            interactionData?.Interact();
        }

        public TMPro.TextMeshProUGUI itemNameText;
        public GameObject shortcut;
        
    }

}
