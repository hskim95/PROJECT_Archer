using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Archer
{
    public class TitleScene : SceneBase
    {
        public override bool IsAdditiveScene => false;

        public override IEnumerator OnStart()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(SceneType.Title.ToString(), LoadSceneMode);
            while (!async.isDone)
            {
                yield return null;

                float progress = async.progress / 0.9f;
                LoadingUI.Instance.SetProgress(progress);
            }

            // To Do : Show Title UI
            UIManager.Show<TitleUI>(UIList.TitleUI);
            SoundManager.Singleton.PlayMusic(MusicFileName.BGM_01);
        }

        public override IEnumerator OnEnd()
        {
            // To Do : Hide Title UI
            UIManager.Hide<TitleUI>(UIList.TitleUI);

            yield return null;
        }
    }
}
