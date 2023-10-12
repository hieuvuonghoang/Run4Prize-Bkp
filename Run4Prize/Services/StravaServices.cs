using com.strava.v3.api.Activities;
using com.strava.v3.api.Athletes;
using com.strava.v3.api.Authentication;
using com.strava.v3.api.Clients;
using com.strava.v3.api.Common;
using com.strava.v3.api.Utilities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Run4Prize.Models.Domains;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Run4Prize.Services
{
    public class StravaServices : IStravaServices
    {
        public static string URL_GET_ACCESS_TOKEN = "https://www.strava.com/oauth/token";
        public static string URL_GET_ACTIVITIES = "https://www.strava.com/api/v3/athlete/activities";

        private readonly Run4PrizeAppConfig _run4PrizeAppConfig;
        private readonly IHttpClientFactory _httpClientFactory;

        public StravaServices(IHttpClientFactory httpClientFactory, IOptions<Run4PrizeAppConfig> run4PrizeAppConfig)
        {
            _httpClientFactory = httpClientFactory;
            _run4PrizeAppConfig = run4PrizeAppConfig.Value;
        }
        public async Task<AccessToken> Authentication(string code)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("client_id", _run4PrizeAppConfig.client_id);
            dict.Add("client_secret", _run4PrizeAppConfig.client_secret);
            dict.Add("code", code);
            dict.Add("grant_type", _run4PrizeAppConfig.grant_type);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "")
            {
                Content = new FormUrlEncodedContent(dict)
            };
            var httpClient = _httpClientFactory.CreateClient(URL_GET_ACCESS_TOKEN);
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var str = await httpResponseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AccessToken>(str);
            }
            else
            {
                throw new Exception("Authentication Fail");
            }
        }

        public async Task<List<ActivitySummary>> GetActivities(string code, DateTime fromDateTime)
        {
            var auth = new StaticAuthentication(code);
            var client = new StravaClient(auth);
            var activites = await client.Activities.GetActivitiesAfterAsync(fromDateTime);
            return activites;
        }

    }
}

