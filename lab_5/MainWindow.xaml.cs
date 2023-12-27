using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web; // Для использования HttpUtility
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CefSharp.Web;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;

namespace lab_5
{
    public partial class MainWindow : Window
    {
        private bool isWindowOpened = false;
        readonly string appId = "28519";


        public MainWindow()
        {
            InitializeComponent();
            Browser.LoadingStateChanged += Browser_LoadingStateChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var uriStr = @"https://osu.ppy.sh/oauth/authorize?client_id=" + appId +
                @"&response_type=code&scope=public identify&redirect_uri=http://localhost:4000";
            Browser.Load(uriStr);
        }

        private async void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false )
            {
                string url = e.Browser.MainFrame.Url;
                Uri uri = new Uri(url);
                NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);

                string code = queryParams["code"];
                

                Dispatcher.Invoke(() => Output.Text = url);

                if (!string.IsNullOrEmpty(code) && !isWindowOpened)
                {
                    Dispatcher.Invoke(() => this.Hide());
                    
                    string token = await GetAccessToken(code);
                    if (token != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            API apiWindow = new API();
                            apiWindow.accesstoken = token;
                            apiWindow.Show();
                        });
                    }



                }
            }
        }
        private async Task<string> GetAccessToken(string code)
        {


            using (var httpClient = new HttpClient())
            {
                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", appId),
                    new KeyValuePair<string, string>("client_secret", "eJ13YQPX2OWC8rwmV5EIbxsRdmurOYOoBlTcZG1J"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("redirect_uri", "http://localhost:4000"),
                });
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await httpClient.PostAsync("https://osu.ppy.sh/oauth/token", requestContent))
                {
                    if (response.IsSuccessStatusCode)
                    {

                        var content = await response.Content.ReadAsStringAsync();

                        Dispatcher.Invoke(() => Output.Text = content);
                        var tokenObject = JsonConvert.DeserializeObject<JObject>(content);

                        // Получение значения access_token
                        string accessToken = tokenObject["access_token"].ToString();
                        return accessToken;
                    }
                    return null;
                }
            }
        }
        private void Browser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           
        }
    }
}
