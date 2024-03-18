namespace Merino.Infrastructure
{
    public class MerinoApiClient
    {
        private static readonly HttpClient _client;

        static MerinoApiClient()
        {
            _client = new HttpClient();
        }

        public HttpClient client { get { return _client; } }
    }
}
