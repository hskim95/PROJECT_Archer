using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    [CreateAssetMenu(fileName = "Character Stat Data", menuName = "Archer/Character/Character Stat Data")] // Attribute
    public class CharacterStatData : ScriptableObject
    {
        public CharacterDataDTO CharacterData = new CharacterDataDTO();
    }
}