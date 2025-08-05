using System;

namespace Systems.SaveLoad
{
    [Serializable]
    public class SecureSaveData
    {
        public string Data;
        public string Checksum;
        public long Timestamp;
        public string Identifier;
    }
}