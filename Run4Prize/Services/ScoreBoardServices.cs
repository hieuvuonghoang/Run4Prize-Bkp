﻿using Newtonsoft.Json;
using Run4Prize.Enum;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains.Configs;
using Run4Prize.Models.Domains.ScoreBoard;

namespace Run4Prize.Services
{
    public interface IScoreBoardServices
    {
        Task<ScroeBoardBodyResponse> GetDatas(string uID);
    }
    public class ScoreBoardServices : IScoreBoardServices
    {
        public readonly ILogger _logger;
        public readonly IHttpClientFactory _httpClientFactory;
        public readonly AppDBContext _appDBContext;
        public ScoreBoardServices(ILogger<ScoreBoardServices> logger, IHttpClientFactory httpClientFactory, AppDBContext appDBContext)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _appDBContext = appDBContext;
        }
        public async Task<ScroeBoardBodyResponse> GetDatas(string uID)
        {
            using (var httpClient = _httpClientFactory.CreateClient(nameof(APIConfig)))
            {
                var url = $"/scoreboard?limit=10&skip=0&team={uID}";
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
                    var result = JsonConvert.DeserializeObject<ScroeBoardBodyResponse>(await response.Content.ReadAsStringAsync())!;
                    if (!result.success)
                        throw new Exception($"Error call api {httpClient.BaseAddress}{url}.");
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception($"DeserializeObject ScroeBoardBodyResponse fail: {ex.Message}.");
                }
            }
        }
    }
}
