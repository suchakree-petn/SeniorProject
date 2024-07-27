
namespace DataPersistence
{
    public interface IDataPersistence<T>
    {
        void LoadData(T data);
        void SaveData(ref T data);
    }
}

