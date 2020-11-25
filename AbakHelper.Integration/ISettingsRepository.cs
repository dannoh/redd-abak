namespace AbakHelper.Integration
{
    public interface ISettingsRepository : IReadOnlySettingsRepository
    {
        void Save<T>(ExportServiceBase component, T settings);
        Settings Get();
        void Save();
    }

    
}