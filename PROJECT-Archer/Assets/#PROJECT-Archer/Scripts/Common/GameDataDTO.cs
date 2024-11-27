using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    [System.Serializable]
    public class GameDataDTO { }

    [System.Serializable]
    public class CharacterDataDTO : GameDataDTO
    {
        public float HP = 100f;
        public float SP = 100f;

        public float walkSpeed = 1f;
        public float runSpeed = 3f;

        public float RunSteminaCost = 3f;
        public float SteminaRecoverySpeed = 5f;
    }
}
