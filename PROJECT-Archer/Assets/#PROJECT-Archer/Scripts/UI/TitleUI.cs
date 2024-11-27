using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR // ��ó����
using UnityEditor;
#endif


namespace Archer
{
    public class TitleUI : UIBase
    {
        // Game Start Event
        public void OnClickGameStartButton()
        {
            Main.Singleton.ChangeScene(SceneType.Ingame);
        }

        // Game Exit Event
        public void OnClickExitButton()
        {
            Main.Singleton.SystemQuit();
        }
    }
}
