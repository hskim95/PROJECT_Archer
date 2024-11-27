using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class DamageMultiplier : MonoBehaviour
    {

        // [] : Attribute라고 부름
        // 변수명 뒤에 get; set; 메소드가 붙은 것은 property라고 부름
        // get; set;을 정의하지 않은 것은 일반 멤버 변수[:field]라고 부름

        [field: SerializeField] public float DamageMultiply { get; set; }
    }
}
