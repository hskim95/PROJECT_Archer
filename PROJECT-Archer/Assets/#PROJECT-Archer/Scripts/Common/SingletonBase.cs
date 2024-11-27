using System;
using UnityEditor;
using UnityEngine;

namespace Archer
{
    public class SingletonBase<T> : MonoBehaviour where T : class
    {
        public static T Singleton => instance.Value;

        private static readonly Lazy<T> instance = new Lazy<T> (() =>
        {
            T instance = FindObjectOfType(typeof(T)) as T;

            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(T).ToString());
                instance = obj.AddComponent(typeof(T)) as T;

#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                {
                    DontDestroyOnLoad(obj);
                }
#else
                DontDestroyOnLoad(obj)
#endif
            }

            return instance;
        });

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
