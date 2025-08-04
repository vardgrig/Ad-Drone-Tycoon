using System;
using UnityEngine.SceneManagement;
using Zenject;

namespace Managers.SceneLoader
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ZenjectSceneLoader _zenjectSceneLoader;

        public SceneLoader(ZenjectSceneLoader zenjectSceneLoader)
        {
            _zenjectSceneLoader = zenjectSceneLoader;
        }

        public void LoadScene(string sceneName, 
            Action<DiContainer> extraBindings = null, 
            Action<DiContainer> onLoaded = null)
        {
            _zenjectSceneLoader.LoadScene(sceneName, LoadSceneMode.Single, container =>
            {
                extraBindings?.Invoke(container);

            }, extraBindingsLate: onLoaded);
        }
    }
}