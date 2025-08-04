using Interfaces;
using Managers.SceneLoader;
using Zenject;

namespace Systems.FSM
{
    public class GameFsm : IGameFsm
    {
        public IState CurrentState { get; private set; }
        public void ChangeState(IState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

    }
}