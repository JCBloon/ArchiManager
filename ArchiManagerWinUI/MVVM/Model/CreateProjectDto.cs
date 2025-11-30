using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArchiManagerWinUI.MVVM.Model
{
    public class CreateProjectDto
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
        [JsonPropertyName("clientId")]
        public long? ClientId { get; set; }
        // Por defecto, el backend SpringBoot descarta los atributos que no coincidan con los que solicita. En este proyecto, client y
        // clientParamsDto tienen los mismos atributos, por lo que se puede usar la propia clase client para hacer la petición. Por el contrario,
        // Project de normal tiene una lista de clientes asociados y no un único cliente, pero para crear el proyecto tenemos que mandar 1 solo
        // cliente existente. Por eso, para no mandar una lista con 1 cliente y que el backend tenga que desempaquetarlo, se crea este Dto en el
        // frontend para hacer la petición. (Solo en create, ya que en update no actualizamos los clientes directamente, se usan los métodos
        // de assign y unassign)

        public CreateProjectDto(long? id, string? title, string? expedientNumber, short? year, string? cadastralReference, short? archiveNumber, string? comment, long? clientId)
        {
            Id = id;
            Title = title;
            ExpedientNumber = expedientNumber;
            Year = year;
            CadastralReference = cadastralReference;
            ArchiveNumber = archiveNumber;
            Comment = comment;
            ClientId = clientId;
        }

        public CreateProjectDto(Project project, long clientId)
        {
            Id = project.Id;
            Title = project.Title;
            ExpedientNumber = project.ExpedientNumber;
            Year = project.Year;
            CadastralReference = project.CadastralReference;
            ArchiveNumber = project.ArchiveNumber;
            Comment = project.Comment;
            ClientId = clientId;
        }
    }
}
