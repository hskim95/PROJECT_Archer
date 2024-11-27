using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class UserDataModel : SingletonBase<UserDataModel>
    {
        [field: SerializeField] public PlayerDataDTO PlayerData { get; private set; }

        public void Initialize()
        {
            LoadPlayerData();
        }

        public void SavePlayerData(PlayerDataDTO data)
        {
            string playerDataToJson = JsonUtility.ToJson(data, true);
            Debug.Log(playerDataToJson);

            PlayerPrefs.SetString(typeof(PlayerDataDTO).Name, playerDataToJson);
        }

        public void LoadPlayerData()
        {
            // To Do: Load Player Data to "PlayerData" property
            string loadedPlayerDataString = PlayerPrefs.GetString(typeof(PlayerDataDTO).Name, string.Empty);
            Debug.Log(loadedPlayerDataString);

            PlayerData = JsonUtility.FromJson<PlayerDataDTO>(loadedPlayerDataString);
        }
    }
}
