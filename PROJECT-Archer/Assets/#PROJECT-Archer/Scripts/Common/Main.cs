using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Archer
{
    // ���ǻ���: SceneType Enum�� ����ִ� ���� ���� Scene ������ �̸��� ��ġ�ؾ� �Ѵ�.
    public enum SceneType
    {
        None,
        Empty,
        Title,
        Ingame,
    }

    public class Main : SingletonBase<Main>
    {
        // #1. ���� ������ Scene�� �����ϴ� SceneController ����
        // #2. ������ �ʱ�ȭ ���� ����

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

            // To Do: �ʿ��� �ý��� ��� �ʱ�ȭ

            // To Do: ó�� �����ϴ� Scene���� ��ȯ

            ChangeScene(SceneType.Title);
        }

        public void SystemQuit()
        {
            // ���� ���� ���
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
