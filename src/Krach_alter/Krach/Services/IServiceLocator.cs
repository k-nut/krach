namespace Krach.Services
{
    public interface IServiceLocator
    {
        T GetInstance<T>() where T : class;
    }
}