using GameFlow.Contracts;
using Interfaces;
using Systems.FSM;
using Zenject;

namespace Systems.StateFactory
{
    public class StateFactory : IStateFactory
    {
        private readonly DiContainer _container;
    
        public StateFactory(DiContainer container)
        {
            _container = container;
        }
    
        public T CreateState<T>() where T : class, IState
        {
            return _container.Resolve<T>();
        }
    
        public T CreateState<T>(object data) where T : class, IState
        {
            var subContainer = _container.CreateSubContainer();
        
            if (data is ContractData contractData)
            {
                subContainer.Bind<ContractData>().FromInstance(contractData).AsSingle();
            }
        
            return subContainer.Resolve<T>();
        }
    }
}