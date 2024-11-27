using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Archer
{
    public class IngameScene : SceneBase
    {
        public override bool IsAdditiveScene => false;

        public override IEnumerator OnStart()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(SceneType.Ingame.ToString(), LoadSceneMode);
            while (!async.isDone)
            {
                yield return null;

                float progress = async.progress / 0.9f; // progress에 따라 로딩 상황 업데이트
                LoadingUI.Instance.SetProgress(progress);
            }

            // To Do: Show Ingame UI
            UIManager.Show<InteractionUI>(UIList.InteractionUI);
            UIManager.Show<IngameUI>(UIList.IngameUI);
            UIManager.Show<CrossHairUI>(UIList.CrossHairUI);
            UIManager.Show<MinimapUI>(UIList.MinimapUI);
            UIManager.Show<IndicatorUI>(UIList.IndicatorUI);

            SoundManager.Singleton.PlayMusic(MusicFileName.BGM_02);
        }

        public override IEnumerator OnEnd()
        {
            // To Do: Hide Ingame UI
            UIManager.Hide<InteractionUI>(UIList.InteractionUI);
            UIManager.Hide<IngameUI>(UIList.IngameUI);
            UIManager.Hide<CrossHairUI>(UIList.CrossHairUI);
            UIManager.Hide<MinimapUI>(UIList.MinimapUI);
            UIManager.Hide<IndicatorUI>(UIList.IndicatorUI);

            yield return null;
        }
    }
}
