namespace GameFlow.Upgrade.Base
{
    public enum UpgradeEffectType {
        #region Drone Upgrades

        IncreaseDroneBatteryCapacity,
        IncreaseDroneSpeed,
        IncreaseDroneStability,
        DecreaseDroneRepairCost,
        IncreaseDroneBatteryChargingSpeed,

        #endregion

        #region Banner Upgrades

        IncreaseBannerWeight,
        IncreaseBannerDurability,
        IncreaseBannerVisibility,
        DecreaseBannerRepairCost,

        #endregion
    }
}