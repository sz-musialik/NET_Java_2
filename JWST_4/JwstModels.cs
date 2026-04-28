using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AplikacjaBazodanowa
{
    public class JwstResponse
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("body")]
        public List<JwstObservation> Body { get; set; }

    }

    public class JwstObservation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("program")]
        public int Program { get; set; }

        [JsonPropertyName("file_type")]
        public string FileType { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        public override string ToString()
        {
            return $"ID Obserwacji: {Id}\n" +
                   $"Nazwa programu: {Program} | Typ pliku: {FileType}\n" +
                   $"Adres pliku: {Location}\n";
        }
    }
}