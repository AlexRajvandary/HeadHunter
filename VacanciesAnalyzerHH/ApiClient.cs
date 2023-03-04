using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VacanciesAnalyzerHH
{
    internal class ApiClient
    {
        private const string baseAdress = "https://api.hh.ru";
        private HttpClient httpClient;

        public ApiClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAdress);
            httpClient.DefaultRequestHeaders.Add("HH-User-Agent", "");
        }

        public async Task<HttpResponseMessage> GetVacancies(Dictionary<string, string> parameters)
        {
            try
            {
                return await httpClient.GetAsync(GetUrl("vacancies", parameters));
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private string GetUrl(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            return $"{baseAdress}/{query}";
        }

        private string GetUrl(string query, Dictionary<string, string> parameters)
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
