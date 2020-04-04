namespace Brokers.DAL.Interfaces
{
    public interface IGenerator<T> where T : class, new()
    {
        T GenerateNew();
    }
}
