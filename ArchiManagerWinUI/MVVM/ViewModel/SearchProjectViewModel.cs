using ArchiManagerWinUI.CustomServices;
using ArchiManagerWinUI.CustomServices.DataAccess;
using ArchiManagerWinUI.CustomServices.Navigation;
using ArchiManagerWinUI.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Protection.PlayReady;
using Windows.System;

namespace ArchiManagerWinUI.MVVM.ViewModel
{
    public partial class SearchProjectViewModel : ObservableObject
    {
        // TwoWay
        [ObservableProperty] 
        private string? title;
        [ObservableProperty] 
        private string? expedientNumber;
        [ObservableProperty] 
        private string? year;
        [ObservableProperty] 
        private string? cadastralReference;

        // Proyecto
        [ObservableProperty] 
        private Project? projectFound;

        // Imagen
        [ObservableProperty]
        private BitmapImage? imageBind;

        // Colección de clientes de este proyecto
        public ObservableCollection<Client> DataGridClientsCollection { get; } = new();
        

        public ICommand FindProjectCommand { get; }
        public ICommand SearchCatRefCommand { get; }
        public ICommand SearchFolderCommand { get; }
        public ICommand GoToSearchClientCommand { get; }

        public SearchProjectViewModel(Project? project = null)
        {
            FindProjectCommand = new AsyncRelayCommand(FindProject);
            SearchCatRefCommand = new AsyncRelayCommand(SearchCatRef);
            SearchFolderCommand = new AsyncRelayCommand(SearchFolder);
            GoToSearchClientCommand = new RelayCommand<Client>(GoToSearchClient);
            // Si metemos un proyecto como argumento, venimos de ListProjects y queremos ver
            // la info, por lo que rellenamos los campos de búsqueda con los del argumento y
            // desde el code-behind, cuando se inicializa la view, se llamará a
            // InitializeAsync para hacer la llamada y que aparezca el proyecto buscado
            if (project != null)
            {
                title = project.Title;
                expedientNumber = project.ExpedientNumber;
                year = project.Year.ToString();
                cadastralReference = project.CadastralReference;
            }
        }

        public async Task InitializeAsync()
        {
            await FindProject();
        }

        public async Task FindProject()
        {
            // Parsear año
            short? yearValue = null;
            if (!string.IsNullOrWhiteSpace(Year) && short.TryParse(Year, out short parsedYear))
            {
                yearValue = parsedYear;
            }

            //Petición
            ArchiHttpResult<Project> result = await ArchiHttpRequest.FindProject(Title, ExpedientNumber, yearValue, CadastralReference);

            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                Project project = result.Value!;
                ProjectFound = project;

                // Rellenar DataGrid con clientes asociados
                DataGridClientsCollection.Clear();
                if (project.ClientList?.Any() == true)
                {
                    foreach (var client in project.ClientList)
                        DataGridClientsCollection.Add(client);
                }

                // Probar si tiene imagen asociada y ponerla
                ArchiHttpResult<byte[]> imageResult = await ArchiHttpRequest.GetImage(project.Id ?? -1);
                if (imageResult.IsSuccess == true)
                {
                    using var ms = new MemoryStream(imageResult.Value!);
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(ms.AsRandomAccessStream());
                    ImageBind = bitmap;
                }
            }
        }

        public async Task SearchCatRef()
        {
            if (ProjectFound == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Necesitas buscar antes el proyecto para ver su referencia catastral");
                return;
            }

            await CommonUseService.SearchCadastralReference(ProjectFound?.CadastralReference);
        }

        public async Task SearchFolder()
        {
            if (ProjectFound == null)
            {
                await DialogService.SimpleDialog(
                    DialogService.DialogType.Warning,
                    "Necesitas buscar antes el proyecto para ver su referencia catastral");
                return;
            }

            await CommonUseService.SearchExpedientNumberFolder(ProjectFound.ExpedientNumber);
        }

        public void GoToSearchClient(Client? client)
        {
            if (client == null)
                return;

            NavigationService.Navigate(AppPage.SearchClientWithArgs, client);
        }

    }
}
