using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    [System.Serializable]
    public class UserDataDTO { }

    [System.Serializable]
    public class PlayerDataDTO : UserDataDTO
    {
        public string name;
        public int level;
        public Vector3 position;
        public float health;
    }
}
