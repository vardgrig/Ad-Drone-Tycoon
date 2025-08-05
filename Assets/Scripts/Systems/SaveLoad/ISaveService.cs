namespace Systems.SaveLoad
{
    public interface ISaveService
    {
        void SaveData<T>(T data, string identifier);
        T LoadData<T>(string identifier) where T : class, new();
    }
}