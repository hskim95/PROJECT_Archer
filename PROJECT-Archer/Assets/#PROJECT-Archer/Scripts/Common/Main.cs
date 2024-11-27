using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Archer
{
    // 주의사항: SceneType Enum에 들어있는 값은 실제 Scene 파일의 이름과 일치해야 한다.
    public enum SceneType
    {
        None,
        Empty,
        Title,
        Ingame,
    }

    public class Main : SingletonBase<Main>
    {
        // #1. 현재 게임의 Scene을 관리하는 SceneController 역할
        // #2. 게임의 초기화 구역 역할

        public SceneType CurrentSceneType => currentSceneType;

        [SerializeField] private SceneType currentSceneType = SceneType.None;

        private void Start()
        {
            Initialize();
        }

        // #2.
        public void Initialize()
        {
            StartCoroutine(MainSystemInitialize());
        }

        IEnumerator MainSystemInitialize()
        {
            yield return null;

            // To Do: 필요한 시스템 모두 초기화

            // To Do: 처음 시작하는 Scene으로 전환

            ChangeScene(SceneType.Title);
        }

        public void SystemQuit()
        {
            // 게임 종료 기능
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public bool IsOnProgressSceneChanging { get; private set; } = false;

        public SceneBase CurrentSceneController => currentSceneController;

        private SceneBase currentSceneController = null;

        public void ChangeScene(SceneType sceneType, System.Action sceneLoadedCallback = null)
        {
            if (currentSceneType == sceneType)
            {
                return;
            }

            switch (sceneType)
            {
                case SceneType.Title:
                    ChangeScene<TitleScene>(sceneType, sceneLoadedCallback);
                    break;
                case SceneType.Ingame:
                    ChangeScene<IngameScene>(sceneType, sceneLoadedCallback);
                    break;
                default:
                    Debug.LogAssertion("Change Scene Error");
                    break;
            }
        }

        private void ChangeScene<T>(SceneType sceneType, System.Action sceneLoadedCallback = null) where T : SceneBase
        {
            if (IsOnProgressSceneChanging) return;
            StartCoroutine(ChangeSceneAsync<T>(sceneType, sceneLoadedCallback));
        }

        private IEnumerator ChangeSceneAsync<T>(SceneType sceneType, System.Action sceneLoadedCallback = null) where T : SceneBase
        {
            IsOnProgressSceneChanging = true;

            // To Do : Show Loading UI
            var loadingUI = UIManager.Show<LoadingUI>(UIList.LoadingUI);
            loadingUI.SetProgress(0f);

            if (currentSceneController != null)
            {
                yield return currentSceneController.OnEnd();
                Destroy(currentSceneController.gameObject);
            }

            loadingUI.SetProgress(0.1f);
            yield return null;

            AsyncOperation emptyOperation = SceneManager.LoadSceneAsync(SceneType.Empty.ToString(), LoadSceneMode.Single);
            yield return new WaitUntil(() => emptyOperation.isDone);

            loadingUI.SetProgress(0.3f);
            yield return null;

            GameObject sceneGo = new GameObject(typeof(T).Name);
            sceneGo.transform.SetParent(transform);
            currentSceneController = sceneGo.AddComponent<T>();
            currentSceneType = sceneType;

            yield return StartCoroutine(currentSceneController.OnStart());


            loadingUI.SetProgress(1f);
            yield return null;

            // To Do: Hide Loading UI
            UIManager.Hide<LoadingUI>(UIList.LoadingUI);

            IsOnProgressSceneChanging = false;
            sceneLoadedCallback?.Invoke();
        }
    }
}
