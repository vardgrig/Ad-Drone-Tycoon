using System;
using Systems.FSM;
using Zenject;

namespace Managers.SceneLoader
{
    public interface ISceneLoader
    {
        void LoadScene(string sceneName, Action<DiContainer> extraBindings = null, Action<DiContainer> onLoaded = null);
    }
    
    public interface IGameFsm
    {
        void ChangeState(IState state);
        IState CurrentState { get; }
    }
}