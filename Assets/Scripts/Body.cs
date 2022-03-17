using Newtonsoft.Json;

[System.Serializable]
public class RequestBody
{
    [System.Serializable]
    public class Argument
    {
        [JsonProperty("language_code")] public string languageCode;
        [JsonProperty("audio")] public string audio;
    }

    [JsonProperty("access_key")] public string accessKey;
    [JsonProperty("argument")] public Argument argument;
}

[System.Serializable]
public class ResponseBody
{
    [System.Serializable]
    public class ReturnObject
    {
        [JsonProperty("recognized")] public string recognized;
    }

    [JsonProperty("request_id")] public string requestId;
    [JsonProperty("result")] public int result;
    [JsonProperty("return_object")] public ReturnObject returnObject;
}
