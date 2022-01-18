using TrekBucketListApp.Model;
namespace TrekBucketListApp.Service
{
    public class TrekService : ITrekService
    {
        private IConfiguration Configuration;
        private string _BaseAddress { get; set; }
        private string _APIName { get; set; }
        private string _APIKey { get; set; }

        public TrekService(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        public async Task<List<ImageList>> GetTrekImages(string _foldername)
        {
            try
            {
                _BaseAddress = Configuration["Values:BaseAddress"];
                _APIName = Configuration["Values:APIName"];
                _APIKey =  Configuration["Values:APIKey"];

                HttpClient _client = new HttpClient();
                _client.BaseAddress = new Uri($"" + _BaseAddress + "");
                var TrekListUrl = new Uri($"/api/" + _APIName + "?code=" + _APIKey + "&nfolder=" + _foldername + "", UriKind.Relative);

                var res = await _client.GetAsync(TrekListUrl);
                res.EnsureSuccessStatusCode();
                

                // API Management Service
                /*HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "3f40a81b5df74617a54a9e88cfc1d74d");
                var TrekListUrl = new Uri($"https://demomgn.azure-api.net/FetchTreks/FetchTreks?nfolder="+_foldername+"");
                var res = await _client.GetAsync(TrekListUrl);
                */

                return await res.Content.ReadFromJsonAsync<List<ImageList>>();
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
        }
    }
}