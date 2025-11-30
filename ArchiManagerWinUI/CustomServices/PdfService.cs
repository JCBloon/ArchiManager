using ArchiManagerWinUI.MVVM.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArchiManagerWinUI.CustomServices
{
    public static class PdfService
    {
        static PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        private static string Clean(string? s) => string.IsNullOrWhiteSpace(s) ? "<>" : s!;

        public static byte[] ExportClientsPdf(List<Client> clients)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Header con título y fecha
                    page.Header()
                        .Height(60)
                        .Column(column =>
                        {
                            column.Item()
                                .AlignCenter()
                                .Text("Listado de clientes")
                                .FontSize(16)
                                .Bold();

                            column.Item()
                                .AlignCenter()
                                .Text(DateTime.Now.ToString("dd/MM/yyyy - HH:mm"))
                                .FontSize(10)
                                .Italic()
                                .FontColor(Colors.Grey.Darken2);
                        });

                    // Contenido principal
                    page.Content().Column(col =>
                    {
                        col.Spacing(5);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                // Eliminamos ID y redistribuimos el espacio
                                columns.ConstantColumn(85);   // DNI
                                columns.RelativeColumn(2);    // Nombre
                                columns.RelativeColumn(2);    // Apellido1
                                columns.RelativeColumn(2);    // Apellido2
                                columns.RelativeColumn(2);    // Teléfono
                                columns.RelativeColumn(3);    // Dirección
                            });

                            // Encabezados
                            table.Header(header =>
                            {
                                header.Cell().Element(cell => cell.Padding(3).Background(Colors.Grey.Lighten3))
                                    .Text("DNI").SemiBold().FontSize(9);
                                header.Cell().Element(cell => cell.Padding(3).Background(Colors.Grey.Lighten3))
                                    .Text("Nombre").SemiBold().FontSize(9);
                                header.Cell().Element(cell => cell.Padding(3).Background(Colors.Grey.Lighten3))
                                    .Text("Apellido 1").SemiBold().FontSize(9);
                                header.Cell().Element(cell => cell.Padding(3).Background(Colors.Grey.Lighten3))
                                    .Text("Apellido 2").SemiBold().FontSize(9);
                                header.Cell().Element(cell => cell.Padding(3).Background(Colors.Grey.Lighten3))
                                    .Text("Teléfono").SemiBold().FontSize(9);
                                header.Cell().Element(cell => cell.Padding(3).Background(Colors.Grey.Lighten3))
                                    .Text("Dirección").SemiBold().FontSize(9);
                            });

                            // Filas
                            for (int i = 0; i < clients.Count; i++)
                            {
                                var c = clients[i];
                                bool alternate = (i % 2) == 0;
                                var bg = alternate ? Colors.White : Colors.Grey.Lighten4;

                                table.Cell().Element(cell => cell.Padding(3).Background(bg))
                                    .Text(Clean(c.Dni)).FontSize(9);
                                table.Cell().Element(cell => cell.Padding(3).Background(bg))
                                    .Text(Clean(c.Name)).FontSize(9);
                                table.Cell().Element(cell => cell.Padding(3).Background(bg))
                                    .Text(Clean(c.Surname1)).FontSize(9);
                                table.Cell().Element(cell => cell.Padding(3).Background(bg))
                                    .Text(Clean(c.Surname2)).FontSize(9);
                                table.Cell().Element(cell => cell.Padding(3).Background(bg))
                                    .Text(Clean(c.Phone)).FontSize(9);
                                table.Cell().Element(cell => cell.Padding(3).Background(bg))
                                    .Text(Clean(c.Address)).FontSize(9);
                            }
                        });
                    });

                    // Footer con numeración
                    page.Footer()
                        .AlignRight()
                        .Text(x =>
                        {
                            x.Span("Página ").FontSize(9);
                            x.CurrentPageNumber();
                            x.Span(" / ").FontSize(9);
                            x.TotalPages();
                        });
                });
            });

            // Genera el PDF como byte[]
            return document.GeneratePdf();
        }
    }
}
