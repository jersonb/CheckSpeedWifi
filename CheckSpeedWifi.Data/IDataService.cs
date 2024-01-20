
namespace CheckSpeedWifi.Data
{
    public interface IDataService
    {
        Task CreateDatabase();
        Task Insert(decimal download, decimal upload, decimal ping, object audit);
    }
}