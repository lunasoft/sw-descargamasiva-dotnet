using System.Threading.Tasks;

namespace sw.descargamasiva
{
    public interface ISatService<T>

    {
        string Generate(IParameters parameters);
        Task<T> Call(string xml = "", string authorization = "");
    }
}