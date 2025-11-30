using ArchiManagerWinUI.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArchiManagerWinUI.MVVM.Model
{
    public class Client
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("dni")]
        public string? Dni { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("surname1")]
        public string? Surname1 { get; set; }

        [JsonPropertyName("surname2")]
        public string? Surname2 { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }
        [JsonPropertyName("projectList")]
        public List<Project>? ProjectList { get; set; }

        public Client(long? id, string? dni, string? name, string? surname1, string? surname2, string? phone, string? address)
        {
            Id = id;
            Dni = dni;
            Name = name;
            Surname1 = surname1;
            Surname2 = surname2;
            Phone = phone;
            Address = address;
        }
    }
}
