using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace DredgeHardMode.MainMenu
{
    internal class CustomSceneManager : MonoBehaviour
    {
        public static Action GameSceneLoaded;
        public static Action MainMenuSceneLoaded;

        public void Awake()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
            OnSceneChanged(default, SceneManager.GetActiveScene());
        }

        public void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private static void OnSceneChanged(Scene prev, Scene current)
        {
            var sceneManagerObject = new GameObject(nameof(CustomSceneManager) + "Objects");
            //WinchCore.Log.Info(current.name);
            switch (current.name)
            {
                case Scenes.Title:
                    foreach (var type in TypeHelper.GetTypesWithAttribute(typeof(AddToMainMenuSceneAttribute)))
                    {
                        sceneManagerObject.AddComponent(type);
                    }
                    DredgeHardMode.Instance.ShouldBeHard = false;
                    MainMenuSceneLoaded?.Invoke();
                    break;

            }
        }
    }

    public static class TypeHelper
    {
        public static IEnumerable<Type> GetTypesWithAttribute(Type attribute) =>
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(attribute, true)
            where attributes != null && attributes.Length > 0
            select t;
    }

    internal static class Scenes
    {
        public const string Title = nameof(Title);
        public const string Game = nameof(Game);

        public static string StringFromLoadSceneReference(AssetReference loadSceneReference)
        {
            if (loadSceneReference == GameManager.Instance._sceneLoader.titleSceneReference)
            {
                return Title;
            }
            if (loadSceneReference == GameManager.Instance._sceneLoader.gameSceneReference)
            {
                return Game;
            }
            return null;
        }
    }
}
