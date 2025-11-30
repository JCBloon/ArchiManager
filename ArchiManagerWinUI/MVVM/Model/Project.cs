using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArchiManagerWinUI.MVVM.Model
{
    public class Project
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("expedientNumber")]
        public string? ExpedientNumber { get; set; }
        [JsonPropertyName("year")]
        public short? Year { get; set; }
        [JsonPropertyName("cadastralReference")]
        public string? CadastralReference { get; set; }
        [JsonPropertyName("archiveNumber")]
        public short? ArchiveNumber { get; set; }
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
        [JsonPropertyName("clientList")]
        public List<Client>? ClientList { get; set; }
        // Para la imágen se necesita hacer una consulta auxiliar al Id

        public Project(long? id, string? title, string? expedientNumber, short? year, string? cadastralReference, short? archiveNumber, string? comment)
        {
            Id = id;
            Title = title;
            ExpedientNumber = expedientNumber;
            Year = year;
            CadastralReference = cadastralReference;
            ArchiveNumber = archiveNumber;
            Comment = comment;
        }

    }
}
