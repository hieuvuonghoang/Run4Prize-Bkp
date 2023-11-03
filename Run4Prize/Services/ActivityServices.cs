using Newtonsoft.Json;
using Run4Prize.Enum;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains.Actitvity;
using Run4Prize.Models.Domains.Configs;

namespace Run4Prize.Services
{
    public interface IActivityServices
    {
        Task<ActivityBodyResponse> GetDatas(string uID);
    }
    public class ActivityServices : IActivityServices
    {
        public readonly ILogger _logger;
        public readonly IHttpClientFactory _httpClientFactory;
        public readonly AppDBContext _appDBContext;
        public ActivityServices(ILogger<ActivityServices> logger, IHttpClientFactory httpClientFactory, AppDBContext appDBContext)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _appDBContext = appDBContext;
        }
        public async Task<ActivityBodyResponse> GetDatas(string uID)
        {
            using (var httpClient = _httpClientFactory.CreateClient(nameof(APIConfig)))
            {
                var url = $"/api/users/activities/{uID}?invalid=false&limit=&skip=0&type=";
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                var cookie = _appDBContext.Settings.Where(it => it.Type == (int)EnumSetting.Cookie).FirstOrDefault();
                if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                {
                    throw new Exception($"Error cookie or cookie value is null.");
                }
                requestMessage.Headers.Add("Cookie", cookie.Value);
                var response = await httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                try
                {
                    var result = JsonConvert.DeserializeObject<ActivityBodyResponse>(await response.Content.ReadAsStringAsync())!;
                    if (!result.success)
                        throw new Exception($"Error call api {httpClient.BaseAddress}{url}.");
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception($"DeserializeObject ActivityBodyResponse fail: {ex.Message}.");
                }
            }
        }
    }
}
