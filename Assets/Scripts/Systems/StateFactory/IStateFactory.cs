using Systems.FSM;

namespace Systems.StateFactory
{
    public interface IStateFactory
    {
        T CreateState<T>() where T : class, IState;
        T CreateState<T>(object data) where T : class, IState;
    }
}