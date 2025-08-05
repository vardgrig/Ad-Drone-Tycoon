using GameFlow.Upgrade.Base;
using Zenject;

namespace GameFlow.Drone
{
    public interface IDroneService : IInitializable
    {
        void ApplyUpgrade(BaseUpgradeData upgrade);
        void RemoveUpgrade(BaseUpgradeData upgrade);
        float GetDroneSpeed();
        float GetBatteryCapacity();
        float GetStability();
        float GetRepairCostReduction();
        float GetChargingSpeed();
    }
}