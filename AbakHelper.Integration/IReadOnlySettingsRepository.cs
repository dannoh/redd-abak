
namespace AbakHelper.Integration
{
    public interface IReadOnlySettingsRepository
    {
        T GetComponentSettings<T>(ExportServiceBase component);
    }
}
