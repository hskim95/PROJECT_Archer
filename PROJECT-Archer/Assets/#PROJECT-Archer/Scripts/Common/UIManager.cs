using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Archer
{
    public class UIManager : SingletonBase<UIManager>
    {
        public static T Show<T>(UIList uiType) where T : UIBase
        {
            var uiInstance = Singleton.GetUI<T>(uiType);
            uiInstance?.Show();

            return uiInstance;
        }

        public static T Hide<T>(UIList uiType) where T : UIBase
        {
            var uiInstance = Singleton.GetUI<T>(uiType);
            uiInstance?.Hide();

            return uiInstance;
        }

        private Transform panelRoot;
        private Transform popupRoot;

        private Dictionary<UIList, UIBase> panels = new Dictionary<UIList, UIBase>();
        private Dictionary<UIList, UIBase> popups = new Dictionary<UIList, UIBase>();

        private bool isInitialized = false;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            CreateNewRoot(UIList.PANEL_START);
            CreateNewRoot(UIList.POPUP_START);
            InitializeDictionary(nameof(panels));
            InitializeDictionary(nameof(popups));

            for (int index = (int)UIList.POPUP_START; index < (int)UIList.POPUP_END; index++)
            {
                panels.Add((UIList)index, null);
            }
        }

        private void InitializeDictionary(string dictName)
        {
            int start, end;
            if (dictName == nameof(panels))
            {
                start = (int)UIList.PANEL_START;
                end = (int)UIList.PANEL_END;

                for (int index = start; index < end; index++)
                {
                    panels.Add((UIList)index, null);
                }
            }
            else if (dictName == nameof(popups))
            {
                start = (int)UIList.POPUP_START;
                end = (int)UIList.POPUP_END;

                for (int index = start; index < end; index++)
                {
                    popups.Add((UIList)index, null);
                }
            }
            else return;

        }

        private void CreateNewRoot(UIList uiType)
        {
            Debug.Log("Create New Root.");

            GameObject goRoot;

            void InitializeRoot(ref Transform root)
            {
                root = goRoot.transform;
                root.SetParent(transform);
                root.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                root.localScale = Vector3.one;
            }

            if (uiType == UIList.PANEL_START)
            {
                goRoot = new GameObject("Panel Root");
                InitializeRoot(ref panelRoot);
            }
            else if (uiType == UIList.POPUP_START)
            {
                goRoot = new GameObject("Popup Root");
                InitializeRoot(ref popupRoot);
            }
        }

        // 위에 선언한 Dictionary 2개 panels/popups에 담겨있는 UIList에 해당하는 UI 객체를 반환하는 함수
        // 만약 Dictionary로 선언한 panels/popups에 해당하는 UI 객체가 없다면
        // Resources.Load를 통해 해당 UI Prefab을 복제 생성한다.
        public T GetUI<T>(UIList uiType, bool reload = false) where T : UIBase
        {
            if (UIList.PANEL_START < uiType && uiType < UIList.PANEL_END)
            {
                if (panels.ContainsKey(uiType))
                {
                    if (reload && panels[uiType] != null)
                    {
                        Destroy(panels[uiType].gameObject);
                        panels[uiType] = null;
                    }

                    if (panels[uiType] == null)
                    {
                        string path = $"UI/UI.{uiType}";
                        UIBase loadedPrefab = Resources.Load<UIBase>(path);
                        if (loadedPrefab == null) return null;
                        else
                        {
                            T result = loadedPrefab.GetComponent<T>();
                            if (result == null) return null;
                        } 

                        panels[uiType] = Instantiate(loadedPrefab, panelRoot);
                        panels[uiType].gameObject.SetActive(false);

                        return panels[uiType].GetComponent<T>();
                    }
                    else
                    {
                        return panels[uiType].GetComponent<T>();
                    }
                }
            }

            if (UIList.POPUP_START < uiType && uiType < UIList.POPUP_END)
            {
                if (popups.ContainsKey(uiType))
                {
                    if (reload && popups[uiType] != null)
                    {
                        Destroy(popups[uiType].gameObject);
                        popups[uiType] = null;
                    }

                    if (popups[uiType] == null)
                    {
                        string path = $"UI/UI.{uiType}";
                        UIBase loadedPrefab = Resources.Load<UIBase>(path);
                        if (loadedPrefab == null)
                            return null;

                        T result = loadedPrefab.GetComponent<T>();
                        if (result == null)
                            return null;

                        popups[uiType] = Instantiate(loadedPrefab, popupRoot);
                        popups[uiType].gameObject.SetActive(false);

                        return popups[uiType].GetComponent<T>();
                    }
                    else
                    {
                        return popups[uiType].GetComponent<T>();
                    }
                }
            }

            return null;
        }
    }
}
