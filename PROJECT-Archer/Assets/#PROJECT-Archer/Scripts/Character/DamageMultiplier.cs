using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class DamageMultiplier : MonoBehaviour
    {

        // [] : Attribute��� �θ�
        // ������ �ڿ� get; set; �޼ҵ尡 ���� ���� property��� �θ�
        // get; set;�� �������� ���� ���� �Ϲ� ��� ����[:field]��� �θ�

        [field: SerializeField] public float DamageMultiply { get; set; }
    }
}
