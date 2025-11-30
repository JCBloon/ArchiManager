using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Power;
using Windows.System;

namespace ArchiManagerWinUI.CustomServices
{
    public static class CommonUseService
    {
        public static async Task SearchCadastralReference(string? CadastralReference)
        {
            if (string.IsNullOrEmpty(CadastralReference))
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "No hay nada para buscar en la referencia catastral");
                return;
            }

            string url = $"https://www1.sedecatastro.gob.es/Cartografia/mapa.aspx?refcat={CadastralReference}";

            try
            {
                var uri = new Uri(url);
                await Launcher.LaunchUriAsync(uri);
            }
            catch (Exception ex)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, $"No se pudo abrir el navegador: {ex.Message}");
            }
        }

        private static string? FindFirstMatchingFolder(string directory, string pattern)
        {
            try
            {
                // Buscar coincidencias directas en el directorio actual
                foreach (string folder in Directory.GetDirectories(directory))
                {
                    string name = Path.GetFileName(folder);

                    if (name.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
                        return folder;
                }

                // Recursión en subcarpetas
                foreach (string subdir in Directory.GetDirectories(directory))
                {
                    string? result = FindFirstMatchingFolder(subdir, pattern);
                    if (result != null)
                        return result;
                }
            }
            catch
            {
                // Cualquier excepción (permisos, IO, acceso denegado...) → retornar null
                return null;
            }

            return null;
        }

        public static async Task SearchExpedientNumberFolder(string? pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                await DialogService.SimpleDialog(
                    DialogService.DialogType.Warning,
                    "No se pudo iniciar la búsqueda, no hay un número de expediente");
                return;
            }

            // 1. Validar patrón (DD_DD o DD_DDD)

            var regex = new System.Text.RegularExpressions.Regex(@"^[0-9]{2}_[0-9]{2,3}$");

            if (!regex.IsMatch(pattern))
            {
                await DialogService.SimpleDialog(
                    DialogService.DialogType.Warning,
                    "El número de expediente no cumple el formato esperado (DD_DD o DD_DDD).");
                return;
            }

            // 2. Carpeta raíz (puedes cambiarla si quieres)
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 3. Buscar en segundo plano
            string? foundFolder = await Task.Run(() =>
            {
                return FindFirstMatchingFolder(rootDirectory, pattern);
            });

            // 4. Si no la encuentra, salir
            if (foundFolder == null)
            {
                await DialogService.SimpleDialog(
                    DialogService.DialogType.Error,
                    "No se encontró ninguna carpeta que coincida con ese número de expediente.");
                return;
            }

            // 5. Abrir el explorador con la carpeta encontrada
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{foundFolder}\"",
                UseShellExecute = true
            });
        }
    }
}
