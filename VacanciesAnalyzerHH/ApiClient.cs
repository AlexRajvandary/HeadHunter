using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    internal class ApiClient : IDisposable
    {
        private const string baseAdress = "https://api.hh.ru";
        private CancellationTokenSource cancellationTokenSource;
        private readonly HttpClient httpClient;

        public ApiClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAdress);
            httpClient.DefaultRequestHeaders.Add("HH-User-Agent", "");

            cancellationTokenSource = new CancellationTokenSource();
        }

        public void Cancel()
        {
            cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }

        public async Task<HHResponse> GetVacancies(string textSearch, int numOfPage, int numOfPages)
        {
            cancellationTokenSource ??= new CancellationTokenSource();

            var param = new Dictionary<string, string>();
            param.TryAdd($"text", textSearch);
            param.TryAdd($"page", numOfPage.ToString());
            param.TryAdd($"per_page", numOfPages.ToString());

            try
            {
                var response = await httpClient.GetAsync(GetUrl("vacancies", param), cancellationTokenSource.Token);
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<HHResponse>(data);
            }
            catch (OperationCanceledException)
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
                throw;
            }
        }

        private static string GetUrl(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            return $"{baseAdress}/{query}";
        }

        private static string GetUrl(string query, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            if (parameters == null || parameters.Count == 0)
            {
                return null;
            }

            var param = new StringBuilder();
            param.Append('?');
            var i = 0;
            foreach (var parameter in parameters)
            {
                param.Append($"{parameter.Key}={parameter.Value}");

                if (i < parameters.Count)
                {
                    param.Append('&');
                }

                i++;
            }

            return $"{baseAdress}/{query}/{param}";
        }
    }
}
