using ArchiManagerWinUI.CustomServices;
using ArchiManagerWinUI.CustomServices.DataAccess;
using ArchiManagerWinUI.CustomServices.Navigation;
using ArchiManagerWinUI.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace ArchiManagerWinUI.MVVM.ViewModel
{
    public partial class ClientListViewModel : ObservableObject
    {
        // TwoWay
        [ObservableProperty]
        public string? name;
        [ObservableProperty]
        private string? surname1;
        [ObservableProperty]
        private string? surname2;
        [ObservableProperty]
        private string? dni;

        // Cache
        private string? _cacheName;
        private string? _cacheSurname1;
        private string? _cacheSurname2;
        private string? _cacheDni;

        // Botones
        [ObservableProperty]
        public bool canGoBack = false;
        [ObservableProperty]
        public bool canGoForw = false;
        private short _currentPage;

        // Filtros
        [ObservableProperty]
        private string? selectedColumn;
        [ObservableProperty]
        private string? selectedOrder;

        // Cuando vuelves a la página después de ir a una opción de INFO o EDITAR, te
        // interesa que vuelva a aparecer el resultado actualizado
        public bool ShouldRefresh { get; set; } = false;

        public ICommand SearchClientCommand { get; }
        public ICommand ExportClientsCommand { get; }
        public ICommand SearchNextPageCommand { get; }
        public ICommand SearchPrevPageCommand { get; }
        public ICommand GoToSearchClientCommand { get; }
        public ICommand GoToManageClientCommand { get; }
        
        public ObservableCollection<Client> DataGridClientsCollection { get; } = new ObservableCollection<Client>();
        public ClientListViewModel()
        {
            SearchClientCommand = new AsyncRelayCommand(SearchClient);
            ExportClientsCommand = new AsyncRelayCommand(ExportClients);
            SearchNextPageCommand = new AsyncRelayCommand(SearchNextPage);
            SearchPrevPageCommand = new AsyncRelayCommand(SearchPrevPage);
            _currentPage = 0;
            GoToSearchClientCommand = new RelayCommand<Client>(GoToSearchClient);
            GoToManageClientCommand = new RelayCommand<Client>(GoToManageClient);
        }

        public async Task Search()
        {
            // Aplicar filtro
            string? column = null;
            switch (SelectedColumn)
            {
                case "Nombre":
                    column = "name";
                    break;
                case "Apellido 1":
                    column = "surname1";
                    break;
                case "Apellido 2":
                    column = "surname2";
                    break;
                case "DNI":
                    column = "dni";
                    break;
                default:
                    break;
            }
            string? order = null;
            switch (SelectedOrder)
            {
                case "Ascendente":
                    order = "ASC";
                    break;
                case "Descendente":
                    order = "DESC";
                    break;
                default:
                    break;
            }

            // Petición
            ArchiHttpResult<List<Client>> result = await ArchiHttpRequest.SearchClient(_cacheName, _cacheSurname1, _cacheSurname2, _cacheDni, _currentPage.ToString(), column, order);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                List<Client> clients = result.Value!;
                DataGridClientsCollection.Clear();
                if (clients.Any())
                {
                    foreach (var cl in clients)
                        DataGridClientsCollection.Add(cl);
                }
                // Ver si hay más paginación
                if (result.HasNext!.Value)
                {
                    CanGoForw = true;
                }
                else
                {
                    CanGoForw = false;
                }

                if(_currentPage != 0)
                {
                    CanGoBack = true;
                }
                else
                {
                    CanGoBack = false;
                }
            }
        }

        public async Task SearchClient()
        {
            // Guardar en caché la búsqueda por si el usuario cambia algún parámetro al cambiar de página
            _cacheName = Name;
            _cacheSurname1 = Surname1;
            _cacheSurname2 = Surname2;
            _cacheDni = Dni;
            _currentPage = 0;

            await Search();
        }

        public async Task ExportClients()
        {
            // Confirmación
            bool confirmation = await DialogService.AnswerableDialog("¿Está seguro de exportar todos los clientes con los parámetros introducidos?\n\n Esta operación podrá tardar un rato y recogerá a todos los clientes de todas las páginas");
            if (!confirmation)
            {
                return;
            }

            // Datos
            string? column = null;
            switch (SelectedColumn)
            {
                case "Nombre":
                    column = "name";
                    break;
                case "Apellido 1":
                    column = "surname1";
                    break;
                case "Apellido 2":
                    column = "surname2";
                    break;
                case "DNI":
                    column = "dni";
                    break;
                default:
                    break;
            }
            string? order = null;
            switch (SelectedOrder)
            {
                case "Ascendente":
                    order = "ASC";
                    break;
                case "Descendente":
                    order = "DESC";
                    break;
                default:
                    break;
            }
            short page = 0;
            bool hasNext = true;
            List<Client> allClients = new();

            while(hasNext)
            {
                ArchiHttpResult<List<Client>> result = await ArchiHttpRequest.SearchClient(_cacheName, _cacheSurname1, _cacheSurname2, _cacheDni, page.ToString(), column, order);
                if (result.IsSuccess == false)
                {
                    if (result.NoConnection == true)
                        await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                    else
                        await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
                    return;
                } else
                {
                    if (result.Value != null && result.Value.Any())
                    {
                        allClients.AddRange(result.Value);
                    }
                    hasNext = result.HasNext ?? false;
                    page++;

                    // Para no saturar la BD
                    await Task.Delay(100);
                }
            }

            if (!allClients.Any())
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "No hay clientes con esas características que exportar.");
                return;
            }

            // PARTE CREAR PDF
            try
            {
                byte[] pdfData = PdfService.ExportClientsPdf(allClients);

                // Crear archivo automáticamente en Documentos
                var folder = KnownFolders.DocumentsLibrary;
                var file = await folder.CreateFileAsync(
                    $"Clientes_{DateTime.Now:yyyy_MM_dd-HH_mm}.pdf",
                    CreationCollisionOption.GenerateUniqueName);

                // Guardar el PDF
                await FileIO.WriteBytesAsync(file, pdfData);

                await DialogService.SimpleDialog(
                    DialogService.DialogType.Success,
                    $"Clientes exportados correctamente en:\n{file.Path}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generando o guardando PDF: {ex}");
                await DialogService.SimpleDialog(
                    DialogService.DialogType.Warning,
                    $"Error generando o guardando PDF: {ex.Message}");
            }


        }

        public async Task SearchNextPage()
        {
            _currentPage++;
            await Search();
        }

        public async Task SearchPrevPage()
        {
            if (_currentPage > 0)
                _currentPage--;
            await Search();
        }

        public void GoToSearchClient(Client? client)
        {
            if (client == null)
                return;

            ShouldRefresh = true;
            NavigationService.Navigate(AppPage.SearchClientWithArgs, client);
        }

        public void GoToManageClient(Client? client)
        {
            if (client == null)
                return;

            ShouldRefresh = true;
            NavigationService.Navigate(AppPage.ManageClientsWithArgs, client);
        }

    }
}
