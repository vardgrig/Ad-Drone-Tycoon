using GameFlow.Currency;

namespace Signals
{
    public class PurchaseUpgradeSignal
    {
        public string UpgradeId { get; private set; }
        public PurchaseUpgradeSignal(string id)
        {
            UpgradeId = id;
        }
    }

    public class EquipUpgradeSignal
    {
        public string UpgradeId { get; private set; }
        public EquipUpgradeSignal(string id)
        {
            UpgradeId = id;
        }
    }

    public class ShowUpgradeInfoSignal
    {
        public string UpgradeId { get; private set; }
        public ShowUpgradeInfoSignal(string id)
        {
            UpgradeId = id;
        }
    }

    public struct RefreshUpgradeUISignal
    {
        public string UpgradeId { get; private set; }

        public RefreshUpgradeUISignal(string upgradeId = "ALL")
        {
            UpgradeId = upgradeId;
        }
    }

    public struct ShowMessageSignal
    {
        public string Message { get; private set; }
        public UnityEngine.Color Color { get; private set; }

        public ShowMessageSignal(string message, UnityEngine.Color color)
        {
            Message = message;
            Color = color;
        }
    }

    public struct PlaySoundSignal
    {
        public UnityEngine.AudioClip SoundClip { get; private set; }

        public PlaySoundSignal(UnityEngine.AudioClip soundClip)
        {
            SoundClip = soundClip;
        }
    }

    public struct UpgradePurchasedSignal
    {
        public string UpgradeId { get; private set; }

        public UpgradePurchasedSignal(string upgradeId)
        {
            UpgradeId = upgradeId;
        }
    }

    public struct UpgradeEquippedSignal
    {
        public string UpgradeId { get; private set; }

        public UpgradeEquippedSignal(string upgradeId)
        {
            UpgradeId = upgradeId;
        }
    }

    public struct UpgradeUnequippedSignal
    {
        public string UpgradeId { get; private set; }

        public UpgradeUnequippedSignal(string upgradeId)
        {
            UpgradeId = upgradeId;
        }
    }

    public struct MoneyChangedSignal
    {
        public Currency NewAmount { get; private set; }

        public MoneyChangedSignal(Currency newAmount)
        {
            NewAmount = newAmount;
        }
    }
}


/*
        public class PurchaseUpgradeSignal : Signal<PurchaseUpgradeSignal, string> { } // upgradeId
        public class EquipUpgradeSignal : Signal<EquipUpgradeSignal, string> { } // upgradeId
        public class ShowUpgradeInfoSignal : Signal<ShowUpgradeInfoSignal, string> { } // upgradeId

        public class RefreshUpgradeUISignal : Signal<RefreshUpgradeUISignal, string> { } // upgradeId or "ALL"
        public class ShowMessageSignal : Signal<ShowMessageSignal, string, UnityEngine.Color> { } // message, color
        public class PlaySoundSignal : Signal<PlaySoundSignal, UnityEngine.AudioClip> { } // sound clip

        public class UpgradePurchasedSignal : Signal<UpgradePurchasedSignal, string> { } // upgradeId
        public class UpgradeEquippedSignal : Signal<UpgradeEquippedSignal, string> { } // upgradeId
        public class UpgradeUnequippedSignal : Signal<UpgradeUnequippedSignal, string> { } // upgradeId
        public class MoneyChangedSignal : Signal<MoneyChangedSignal, Currency.Currency> { } // new amount
*/