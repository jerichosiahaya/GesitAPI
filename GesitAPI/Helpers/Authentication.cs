using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Helpers
{
    public class Authentication
    {
        public string GesitAuth(string npp, string password)
        {
            JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Include,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
            var client = new RestClient("http://35.219.8.90:90/");
            client.UseNewtonsoftJson(DefaultSettings);
            var request = new RestRequest("api/Authentication?npp=" + npp + "&password=" + password);
            var response = client.Execute(request);
            if (response.Content == null)
            {
                return null;
            }
            else
            {
                JObject obj = JObject.Parse(response.Content);
                string token = (string)obj["token"];
                return token;
            }
        }
    }
}
