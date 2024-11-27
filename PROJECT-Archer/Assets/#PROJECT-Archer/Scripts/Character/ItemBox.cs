using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class ItemBox : MonoBehaviour, IInteractable
    {
        public string InteractionMessage => itemName;

        public string itemName;
        public void Interact()
        {
            // 로그를 출력한다.
            Debug.Log("ItemBox Interact !!!");

            // 아이템 박스 자기 자신 GameObject를 파괴한다.
            Destroy(gameObject);
        }
    }

}
