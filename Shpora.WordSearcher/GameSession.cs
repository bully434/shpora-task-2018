using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace Shpora.WordSearcher
{
    public class GameSession: IDisposable
    {
        private readonly string URI;
        private readonly string Token;
        private DateTime SessionCloseTime;
        public SlidingWindow CurrentWindow;
        
        public GameSession(string uri, string token)
        {
            Token = token;
            URI = uri;
        }

        private IRestResponse SendRequest(Method method, string resourse, string jsonBody = null)
        {
            var client = new RestClient(URI);
            var request = new RestRequest(resourse, method);
            request.AddHeader("Authorization", Token);
            if (jsonBody != null)
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            var response = client.Execute(request);
            return response.StatusCode != HttpStatusCode.OK
                ? throw new WebException(response.StatusDescription) 
                : response;

        }

        public void InitGameSession()
        {
            var response = SendRequest(Method.POST, "/task/game/start");

            var expires = FetchHeaderValue(response, "Expires");
            var lastModified = FetchHeaderValue(response, "Last-Modified");

            var sessionDurability = double.Parse(expires);
            var lastModificationTime = DateTime.Parse(lastModified);
            var reservedTime = Math.Min(2, sessionDurability * 0.2);
            SessionCloseTime = lastModificationTime.AddSeconds(sessionDurability - reservedTime);
            Console.WriteLine(DateTime.Now);
            var content = JsonConvert.DeserializeObject<string>(response.Content);
            CurrentWindow = new SlidingWindow(content);
        }

        public string FinishGameSession()
        {
            var response = SendRequest(Method.POST, "/task/game/finish");
            Console.WriteLine(response.Content);
            return response.Content;
        }

        public string GetGameStatistics()
        {
            CheckSessionDuration();
            var response = SendRequest(Method.GET, "/task/game/stats");
            return response.Content;
        }

        public void Move(Directions direction, bool rewrite = false)
        {
            CheckSessionDuration();
            var directionName = Enum.GetName(typeof(Directions), direction)?.ToLower();
            var response = SendRequest(Method.POST, $"/task/move/{directionName}");
            var content = JsonConvert.DeserializeObject<string>(response.Content);           
            if (rewrite)
                CurrentWindow = new SlidingWindow(content);
            else CurrentWindow.UpdateWindow(content, direction);
        }

        public string SendAnswer()
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var sortedWords = WordSearcher.RecognizedWords.OrderBy(x => x.Length).ToList();
            Console.WriteLine($"{sortedWords.Count} words were send!");
            var jsonBody = jsonSerialiser.Serialize(sortedWords);
            Console.WriteLine(jsonBody);
            var response = SendRequest(Method.POST, "task/words/", jsonBody);
            return response.Content;
        }

        private void CheckSessionDuration()
        {
            if (DateTime.Now > SessionCloseTime)
                throw new WebException("Session TimeOut");
        }

        private string FetchHeaderValue(IRestResponse response, string headerName)
        {
            return response.Headers
                                .Where(x => x.Name == headerName)
                                .Select(x => x.Value).First()
                                .ToString();
        }


        private void CloseSession()
        {
            SendAnswer();
            Console.WriteLine(DateTime.Now);
            FinishGameSession();
        }

        private void ReleaseUnmanagedResouces() => CloseSession();

        public void Dispose()
        {
            ReleaseUnmanagedResouces();
            GC.SuppressFinalize(this);
        }

        ~GameSession()
        {
            ReleaseUnmanagedResouces();
        }
    }
}
