using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Archer
{
    public class BootStrapper : MonoBehaviour
    {
        public static bool IsSystemLoaded { get; private set; } = false;

        private static readonly List<string> AutoBootStrapperScenes = new List<string>()
        {
            "Ingame",
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void SystemBoot()
        {
            IsSystemLoaded = false;

#if UNITY_EDITOR
            var activeScene = EditorSceneManager.GetActiveScene();
            for (int i = 0; i < AutoBootStrapperScenes.Count; i++)
            {
                if (activeScene.name.Equals(AutoBootStrapperScenes[i]))
                {
                    InternalBoot();
                    break;
                }
            }
#else
            InternalBoot();
#endif
            IsSystemLoaded = true;
        }

        private static void InternalBoot()
        {
            UIManager.Singleton.Initialize();
            SoundManager.Singleton.Initialize();
            GameDataModel.Singleton.Initialize();
            UserDataModel.Singleton.Initialize();

            // TO Do: Add more system initialize
            var activeScene = SceneManager.GetActiveScene();
            bool isValidSceneName = false;

            foreach (var scene in AutoBootStrapperScenes)
            {
                if (activeScene.name.Equals(scene))
                {
                    isValidSceneName = true;
                    break;
                }
            }

            if (isValidSceneName)
            {
                UIManager.Show<IngameUI>(UIList.IngameUI);
                UIManager.Show<InteractionUI>(UIList.InteractionUI);
                UIManager.Show<CrossHairUI>(UIList.CrossHairUI);
                UIManager.Show<MinimapUI>(UIList.MinimapUI);
                UIManager.Show<IndicatorUI>(UIList.IndicatorUI);

                SoundManager.Singleton.PlayMusic(MusicFileName.BGM_02);
            }
        }
    }
}
