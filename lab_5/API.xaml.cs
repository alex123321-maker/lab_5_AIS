using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace lab_5
{
    /// <summary>
    /// Логика взаимодействия для API.xaml
    /// </summary>
    public partial class API : Window
    {
        public string accesstoken { get; set; }

        public API()
        {
            InitializeComponent();
        }
        private async Task<string> SendRequestAsync(string baseUrl, string method, Dictionary<string, string> parameters = null)
        {
            using (HttpClient client = new HttpClient())
            {
                // Создаем HttpRequestMessage
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}{method}");



                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);



                // Добавляем параметры к URL
                if (parameters != null)
                {
                    string queryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
                    requestMessage.RequestUri = new Uri(requestMessage.RequestUri + "?" + queryString);
                }

                // Отправляем запрос и получаем ответ
                HttpResponseMessage response = await client.SendAsync(requestMessage);

                // Проверяем успешность запроса
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Обработка ошибок
                    return $"Error: {response.StatusCode}";
                }
            }
        }

        private void DisplayJsonInTextBlock(string jsonString)
        {
            try
            {
                // Парсим JSON-строку в объект JToken
                JToken json = JToken.Parse(jsonString);

                // Сериализуем объект JToken в красиво отформатированную JSON-строку
                string formattedJson = json.ToString(Formatting.Indented);

                // Отображаем JSON в TextBlock
                textBox.Text = formattedJson;
            }
            catch (JsonException ex)
            {
                // Обработка ошибок парсинга JSON
                textBox.Text = $"{jsonString}";
            }
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string apiUrl = "https://osu.ppy.sh/api/v2";
                string method = "/users/34427975/osu";

                Dictionary<string, string> parameters = new Dictionary<string, string>
                {

                };

                // Заголовки запроса (если необходимо)


                string result = await SendRequestAsync(apiUrl, method, parameters);
                DisplayJsonInTextBlock(result);

            }
            catch (Exception ex)
            {
                // Обработка исключений
                textBox.Text = $"Error: {ex.Message}";
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string apiUrl = "https://osu.ppy.sh/api/v2";
                string method = "/users/34427975/beatmapsets/most_played";

                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    {"limit","10"}
                };

                // Заголовки запроса (если необходимо)


                string result = await SendRequestAsync(apiUrl, method, parameters);
                DisplayJsonInTextBlock(result);

            }
            catch (Exception ex)
            {
                // Обработка исключений
                textBox.Text = $"Error: {ex.Message}";
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                string apiUrl = "https://osu.ppy.sh/api/v2";
                string method;
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    
                };
                if (!string.IsNullOrEmpty(inputTextBox.Text))
                {
                    method = $"/beatmaps/lookup";
                    parameters.Add("id", $"{inputTextBox.Text}");

                }
                else
                {
                    inputTextBox.Text = "Введите id карты";
                    return;
                }


                // Заголовки запроса (если необходимо)


                string result = await SendRequestAsync(apiUrl, method, parameters);
                DisplayJsonInTextBlock(result);

            }
            catch (Exception ex)
            {
                // Обработка исключений
                textBox.Text = $"Error: {ex.Message}";
            }
        }
    }
}

