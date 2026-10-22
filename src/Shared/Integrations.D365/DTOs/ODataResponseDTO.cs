
using Newtonsoft.Json;

public class ODataResponseDTO<T>
    {
        [JsonProperty("value")]
        public List<T> Value { get; set; }
    };