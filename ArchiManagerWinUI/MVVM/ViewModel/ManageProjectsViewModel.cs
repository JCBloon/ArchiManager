using ArchiManagerWinUI.CustomServices;
using ArchiManagerWinUI.CustomServices.DataAccess;
using ArchiManagerWinUI.CustomServices.Navigation;
using ArchiManagerWinUI.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

namespace ArchiManagerWinUI.MVVM.ViewModel
{
    public partial class ManageProjectsViewModel : ObservableObject
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

        // TwoWay - AssignClient
        [ObservableProperty]
        public string? name;
        [ObservableProperty]
        private string? surname1;
        [ObservableProperty]
        private string? surname2;
        [ObservableProperty]
        private string? dni;

        // TwoWay - FoundProject short params
        [ObservableProperty]
        private string? yearParam;
        [ObservableProperty]
        private string? archiveNParam;

        // OneWay
        [ObservableProperty]
        private Project? foundProject = null;

        // Imagen
        [ObservableProperty]
        public string? imageName;
        [ObservableProperty]
        public byte[]? imageData;
        [ObservableProperty]
        public byte[]? imageFound;
        [ObservableProperty]
        private BitmapImage? imageBind;

        public ICommand FindProjectCommand { get; }
        public ICommand UpdateProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand AssignProjectCommand { get; }
        public ICommand UnassignClientCommand { get; }
        public ICommand ChangeImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand SearchCatRefCommand { get; }
        public ICommand SearchFolderCommand { get; }

        public ObservableCollection<Client> DataGridClientsCollection { get; } = new ObservableCollection<Client>();

        public ManageProjectsViewModel(Project? project) 
        {
            FindProjectCommand = new AsyncRelayCommand(FindProject);
            UpdateProjectCommand = new AsyncRelayCommand(UpdateProject);
            DeleteProjectCommand = new AsyncRelayCommand(DeleteProject);
            AssignProjectCommand = new AsyncRelayCommand(AssignProject);
            UnassignClientCommand = new AsyncRelayCommand<Client>(UnassignClient);
            ChangeImageCommand = new AsyncRelayCommand(ChangeImage);
            RemoveImageCommand = new RelayCommand(RemoveImage);
            SearchCatRefCommand = new AsyncRelayCommand(SearchCatRef);
            SearchFolderCommand = new AsyncRelayCommand(SearchFolder);

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
                FoundProject = project;
                YearParam = project.Year?.ToString();
                ArchiveNParam = project.ArchiveNumber?.ToString();

                DataGridClientsCollection.Clear();
                if (project.ClientList?.Any() == true)
                {
                    foreach (var client in project.ClientList)
                        DataGridClientsCollection.Add(client);
                }

                // Probar si tiene imagen asociada y ponerla
                ImageName = string.Empty;
                ArchiHttpResult<byte[]> imageResult = await ArchiHttpRequest.GetImage(project.Id ?? -1);
                if (imageResult.IsSuccess == true)
                {
                    var bytes = imageResult.Value!;
                    using var ms = new MemoryStream(bytes);
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(ms.AsRandomAccessStream());

                    ImageBind = bitmap;
                    ImageFound = bytes;
                    ImageData = bytes;
                }
                else
                {
                    ImageFound = null;
                    ImageData = null;
                    ImageBind = null;
                }
            }

        }

        public async Task UpdateProject()
        {
            // Validación
            if (FoundProject == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Ningún proyecto seleccionado");
                return;
            }

            if(string.IsNullOrEmpty(FoundProject.ExpedientNumber) || string.IsNullOrEmpty(FoundProject.CadastralReference))
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "El proyecto debe tener Nº de Expediente y Referencia Catastral");
                return;
            }

            if (FoundProject.ExpedientNumber.Length < 1 || FoundProject.ExpedientNumber.Length > 5)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "El Nº de Expediente debe contener entre 1 y 5 carácteres");
                return;
            }

            // Petición
                // Conversiones
            short? yearUpdated = null;
            if (!string.IsNullOrWhiteSpace(YearParam))
            {
                if (short.TryParse(YearParam, out short y))
                {
                    yearUpdated = y;
                }
                else
                {
                    await DialogService.SimpleDialog(DialogService.DialogType.Error, "Año no válido");
                    return;
                }
            }

            short? archiveNUpdated = null;
            if (!string.IsNullOrWhiteSpace(ArchiveNParam))
            {
                if (short.TryParse(ArchiveNParam, out short a))
                {
                    archiveNUpdated = a;
                }
                else
                {
                    await DialogService.SimpleDialog(DialogService.DialogType.Error, "El Nº Archivo no es válido.");
                    return;
                }
            }
            Project projectUpdated = new Project(FoundProject.Id, FoundProject.Title, FoundProject.ExpedientNumber, yearUpdated,
                FoundProject.CadastralReference, archiveNUpdated, FoundProject.Comment);

            // Peticiones
                // Proyecto
                ArchiHttpResult<Project> result = await ArchiHttpRequest.UpdateProject(projectUpdated);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                FoundProject = result.Value!;

                // -- Imágen --
                FileData? file = null;

                if ((ImageFound == null && ImageData == null) ||
                    (ImageFound != null && ImageData != null && ImageFound.SequenceEqual(ImageData)))
                {
                    // Si la imagen era null y mandas null o si tenia algo y mandas los mismo -> No mandar solicitud
                }
                else if (ImageFound != null && (ImageData == null || ImageData.Length == 0))
                {
                    // Si habia imagen y mandas null -> Se borra la imagen mandando un null como ya estaba
                    var deleteResult = await ArchiHttpRequest.SetImage(FoundProject.Id ?? -1, null);
                    if (deleteResult.IsSuccess == true)
                    {
                        ImageBind = null;
                        ImageFound = null;
                        ImageName = string.Empty;
                    }
                }
                else if (ImageData != null && ImageData.Length > 0 && ImageName != null &&
                         (ImageFound == null || !ImageFound.SequenceEqual(ImageData)))
                {
                    // Si se cambia la imagen -> Enviar imagen y actualizar imageFound = imageData para evitar mas peticiones inutiles si se sigue actualizando
                    file = new FileData(ImageName, ImageData);
                    var updateImgResult = await ArchiHttpRequest.SetImage(FoundProject.Id ?? -1, file);

                    if (updateImgResult.IsSuccess == true)
                    {
                        ImageFound = ImageData;
                        ImageName = string.Empty;
                    }
                }

                await DialogService.SimpleDialog(DialogService.DialogType.Success, "Proyecto modificado con éxito");
            }
        }

        public async Task DeleteProject()
        {
            if (FoundProject == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Ningún proyecto seleccionado");
                return;
            }

            // Confirmación
            bool confirmation = await DialogService.AnswerableDialog("¿Está seguro de eliminar este proyecto? Todos los clientes asociados perderán la referencia al proyecto");
            if (!confirmation)
            {
                return;
            }

            // Petición
                // Conversión de long? Id a long Id
            long id;
            if (FoundProject!.Id.HasValue)
            {
                id = FoundProject.Id.Value;
            }
            else
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Error, "Error de conversión interno del programa");
                return;
            }

            ArchiHttpResult<object> result = await ArchiHttpRequest.DeleteProject(id);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                FoundProject = null;
                YearParam = string.Empty;
                ArchiveNParam = string.Empty;
                DataGridClientsCollection.Clear();

                ImageBind = null;
                ImageData = null;
                ImageName = string.Empty;

                await DialogService.SimpleDialog(DialogService.DialogType.Success, "Proyecto eliminado con éxito");
            }
        }

        public async Task AssignProject()
        {
            if (FoundProject == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Ningún proyecto seleccionado");
                return;
            }

            ArchiHttpResult<Client> result = await ArchiHttpRequest.FindClient(Name, Surname1, Surname2, Dni);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
                return;
            }

            // Validación
            Client clientAssign = result.Value!;
            if (DataGridClientsCollection.Any(c => c.Dni == clientAssign.Dni))
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Cliente ya asignado");
                return;
            }

            // Confirmación
            bool confirmation = await DialogService.AnswerableDialog("¿Está seguro de querer asignar al siguiente cliente?\n" 
                + clientAssign.Surname1 + " " + clientAssign.Surname2 + ", " + clientAssign.Name + " - " + clientAssign.Dni);
            if (!confirmation)
            {
                return;
            }

            // Petición

            ArchiHttpResult<Project> result2 = await ArchiHttpRequest.AssignClient(FoundProject.Id ?? -1, clientAssign.Id ?? -1);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                DataGridClientsCollection.Add(clientAssign);
                await DialogService.SimpleDialog(DialogService.DialogType.Success, "Cliente asociado correctamente");
            }
        }

        public async Task UnassignClient(Client? client)
        {
            // Validación
            if (client == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Ningún cliente seleccionado");
            }

            if (FoundProject == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Ningún proyecto seleccionado");
                return;
            }

            if (DataGridClientsCollection.Count() == 1)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "No puede dejar un proyecto sin clientes asignados");
                return;
            }

            // Confirmación
            bool confirmation = await DialogService.AnswerableDialog("¿Está seguro de desasignar a este cliente?");
            if (!confirmation)
            {
                return;
            }

            ArchiHttpResult<object> result = await ArchiHttpRequest.UnassignClient(FoundProject.Id ?? -1, client!.Id ?? -1);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                DataGridClientsCollection.Remove(client);
            }
        }

        public async Task ChangeImage()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindowApp);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var bytes = await FileIO.ReadBufferAsync(file);
                ImageData = bytes.ToArray();
                ImageName = file.Name;

                using var ms = new MemoryStream(ImageData);
                var bitmap = new BitmapImage();
                bitmap.SetSource(ms.AsRandomAccessStream());
                ImageBind = bitmap;

            }
        }

        public void RemoveImage()
        {
            ImageData = null;
            ImageBind = null;
            ImageName = string.Empty;
        }

        public async Task SearchCatRef()
        {
            if (FoundProject == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Necesitas buscar antes el proyecto para ver su referencia catastral");
                return;
            }

            await CommonUseService.SearchCadastralReference(FoundProject?.CadastralReference);

        }

        public async Task SearchFolder()
        {
            if (FoundProject == null)
            {
                await DialogService.SimpleDialog(
                    DialogService.DialogType.Warning,
                    "Necesitas buscar antes el proyecto para ver su referencia catastral");
                return;
            }

            await CommonUseService.SearchExpedientNumberFolder(FoundProject.ExpedientNumber);
        }
    }
}
