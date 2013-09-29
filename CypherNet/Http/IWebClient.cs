namespace CypherNet.Http
{
    #region

    using System.Threading.Tasks;

    #endregion

    public interface IWebClient
    {
        Task<TResult> GetAsync<TResult>(string url);

        Task<TResult> PostAsync<TResult>(string url, object body);

        Task<TResult> PutAsync<TResult>(string url, object body);

        Task<TResult> DeleteAsync<TResult>(string url);
    }
}