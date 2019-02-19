using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GroupMeAnalysis {
    static class GroupMeApi {
        private const string ApiEndpoint = "https://api.groupme.com/v3/";

        private static string GenerateRequestUrl(string request) {
            return ApiEndpoint + request + "?token=" + Secret.Token;
        }

        public static Task<List<Group>> GetGroupListAsync() {
            var task = new Task<List<Group>>(() => {
                HttpClient client = new HttpClient();

                var groupsResponseTask = client.GetAsync(GroupMeApi.GenerateRequestUrl("groups"));
                var groupsResponseContentTask = groupsResponseTask.Result.Content.ReadAsStringAsync();

                var deserialized = JsonConvert.DeserializeObject<ApiResponse<List<Group>>>(groupsResponseContentTask.Result);

                return deserialized.Response;
            });
            task.Start();
            return task;
        }
    }

    class ApiResponse<T> {
        [JsonProperty(PropertyName = "response")]
        public T Response {get; set;}
    }

    public class UnixSecondsToDateTimeConverter : DateTimeConverterBase {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var timestamp = long.Parse(reader.Value.ToString());
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
        }
    }
}
