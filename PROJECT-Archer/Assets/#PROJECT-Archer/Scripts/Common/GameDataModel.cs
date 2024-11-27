using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class GameDataModel : SingletonBase<GameDataModel>
    {
        [field: SerializeField] public CharacterStatData PlayerCharacterStatData { get; private set; }

        public void Initialize()
        {
            //// For Test - Save Character Stat Data with JSON
            //CharacterDataDTO sampleData = new CharacterDataDTO()
            //{
            //    HP = 100,
            //    SP = 10,

            //    walkSpeed = 1f,
            //    runSpeed = 1.5f,

            //    RunSteminaCost = 1f,
            //    SteminaRecoverySpeed = 0.5f,
            //};

            //string toJson = JsonUtility.ToJson(sampleData, true);
            //FileManager.WriteFileFromString("Assets/#PROJECT Archer/Resources/sampleData.json", toJson);

            // LoadPlayerCharacterStatData();
        }

        public void LoadPlayerCharacterStatData()
        {
            TextAsset sampleDataText = Resources.Load<TextAsset>("sampleData");
            var loadedDataDTO = JsonUtility.FromJson<CharacterDataDTO>(sampleDataText.text);

            var newRuntimeData = ScriptableObject.CreateInstance<CharacterStatData>();
            newRuntimeData.CharacterData = loadedDataDTO;
            PlayerCharacterStatData = newRuntimeData;
        }
    }
}
