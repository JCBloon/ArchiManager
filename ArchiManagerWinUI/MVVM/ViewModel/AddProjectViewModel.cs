using ArchiManagerWinUI.CustomServices;
using ArchiManagerWinUI.CustomServices.DataAccess;
using ArchiManagerWinUI.CustomServices.Navigation;
using ArchiManagerWinUI.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

namespace ArchiManagerWinUI.MVVM.ViewModel
{
    public partial class AddProjectViewModel : ObservableObject
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
        [ObservableProperty]
        private string? archiveNumber;
        [ObservableProperty]
        private string? comment;

        // Image
        [ObservableProperty]
        public string? imageName;
        [ObservableProperty]
        public byte[]? imageData;
        [ObservableProperty]
        private BitmapImage? imageBind;

        // TwoWay - AssignClient
        [ObservableProperty]
        public string? name;
        [ObservableProperty]
        private string? surname1;
        [ObservableProperty]
        private string? surname2;
        [ObservableProperty]
        private string? dni;
        [ObservableProperty]
        private string? phone;
        [ObservableProperty]
        private string? address;

        public ICommand AddProjectCommand { get; }
        public ICommand SearchCatRefCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand SearchFolderCommand { get; }

        public AddProjectViewModel()
        {
            AddProjectCommand = new AsyncRelayCommand(AddProject);
            SearchCatRefCommand = new AsyncRelayCommand(SearchCatRef);
            AddImageCommand = new AsyncRelayCommand(AddImage);
            RemoveImageCommand = new RelayCommand(RemoveImage);
            SearchFolderCommand = new AsyncRelayCommand(SearchFolder);
        }

        public async Task AddProject()
        {
            // FASE 1: Validación
            Client client = null!;
            bool newClient = true;

            // Si no se introduce nada, seguira ejecutándose pero como los 3 parámetros
            // necesarios para un cliente no están, dará un error, por lo que no es necesario
            // verificar por 2º vez si todo es null
            ArchiHttpResult<Client> findResult = await ArchiHttpRequest.FindClient(Name, Surname1, Surname2, Dni);
            if (findResult.IsSuccess == false)
            {
                // El cliente es nuevo (Verificamos únicamente que haya conexión)
                if (findResult.NoConnection == true)
                {
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, findResult.Error);
                    return;
                }
                // Continuamos...
            } else
            {
                // Preguntamos si quiere usar el cliente encontrado
                client = findResult.Value!;
                bool confirmation = await DialogService.AnswerableDialog("Existe un cliente que coincide con los datos introducidos. ¿Quiere asociar el proyecto a este cliente?\n" 
                    + client.Surname1 + " " + client.Surname2 + ", " + client.Name + " - " + client.Dni);
                if (!confirmation)
                {
                    return;
                } else
                    newClient = false;
            }
                
            // Comprobar proyecto
            if (string.IsNullOrEmpty(ExpedientNumber) || string.IsNullOrEmpty(CadastralReference))
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "El proyecto debe tener Nº de Expediente y Referencia Catastral");
                return;
            }

            if (ExpedientNumber.Length < 1 || ExpedientNumber.Length > 5)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "El Nº de Expediente debe contener entre 1 y 5 carácteres");
                return;
            }
            // Comprobar cliente (Si es nuevo)
            if (newClient)
            {
                if (string.IsNullOrEmpty(Dni) || string.IsNullOrEmpty(Name))
                {
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, "El cliente debe tener DNI, Nombre y Primer Apellido");
                    return;
                }

                if (Dni.Length != 9)
                {
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, "El DNI debe contener 9 carácteres");
                    return;
                }
            }


            // FASE 2: Conversiones
            // Conversiones proyecto
            short? yearParam = null;
            if (!string.IsNullOrWhiteSpace(Year))
            {
                if (short.TryParse(Year, out short y))
                {
                    yearParam = y;
                }
                else
                {
                    await DialogService.SimpleDialog(DialogService.DialogType.Error, "Año no válido");
                    return;
                }
            }

            short? archiveNParam = null;
            if (!string.IsNullOrWhiteSpace(ArchiveNumber))
            {
                if (short.TryParse(ArchiveNumber, out short a))
                {
                    archiveNParam = a;
                }
                else
                {
                    await DialogService.SimpleDialog(DialogService.DialogType.Error, "El Nº Archivo no es válido.");
                    return;
                }
            }
            Project newProj = new Project(
                null,
                string.IsNullOrWhiteSpace(Title) ? null : Title,
                ExpedientNumber,
                yearParam,
                CadastralReference,
                archiveNParam,
                string.IsNullOrWhiteSpace(Comment) ? null : Comment
            );


            //Conversiones cliente
            if (newClient)
            {
                client = new Client(
                    null,
                    Dni,
                    Name,
                    Surname1,
                    string.IsNullOrWhiteSpace(Surname2) ? null : Surname2,
                    string.IsNullOrWhiteSpace(Phone) ? null : Phone,
                    string.IsNullOrWhiteSpace(Address) ? null : Address
                );
            }

            // FASE 3: Peticiones
            // Primero, creamos al cliente en caso de que sea nuevo
            if (newClient)
            {
                ArchiHttpResult<Client> clientResult = await ArchiHttpRequest.CreateClient(client);
                if (clientResult.IsSuccess == false)
                {
                    if (clientResult.NoConnection == true)
                        await DialogService.SimpleDialog(DialogService.DialogType.Connection, clientResult.Error);
                    else
                        await DialogService.SimpleDialog(DialogService.DialogType.Warning, clientResult.Error);
                    return;
                }
                else
                {
                    
                    client = clientResult.Value!;
                }
            }

            // Creamos el proyecto
            ArchiHttpResult<Project> projectResult = await ArchiHttpRequest.CreateProject(newProj, client.Id?? -1);
            if (projectResult.IsSuccess == false)
            {
                if (projectResult.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, projectResult.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, projectResult.Error);
                // Si algo salió mal, borramos el cliente si no existía
                if(newClient)
                    await ArchiHttpRequest.DeleteClient(client.Id ?? -1);
                return;
            }
            else
            {
                
                newProj = projectResult.Value!;
            }

            // Asociamos la imágen
            FileData? image;
            if (ImageData != null && ImageData.Length > 0 && ImageName != null)
            {
                image = new FileData(ImageName, ImageData);
                var imageResult = await ArchiHttpRequest.SetImage(newProj.Id ?? -1, image);
                if (imageResult.IsSuccess == false)
                {
                    // Si algo salió mal, borramos el cliente si no existía y el proyecto creado
                    if(newClient)
                        await ArchiHttpRequest.DeleteClient(client.Id ?? -1);
                    await ArchiHttpRequest.DeleteProject(newProj.Id ?? -1);
                    return;
                }
            }

            // FASE4: Limpieza
            // Cliente
            Name = string.Empty;
            Surname1 = string.Empty;
            Surname2 = string.Empty;
            Dni = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;

            // Proyecto
            Title = string.Empty;
            ExpedientNumber = string.Empty;
            ArchiveNumber = string.Empty;
            Year = string.Empty;
            CadastralReference = string.Empty;
            Comment = string.Empty;

            // Imágen
            ImageName = null;
            ImageData = null;
            ImageBind = null;

            if (newClient)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Success, "Cliente y proyecto creados con éxito");
            } else
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Success, "Proyecto creado con éxito");
            }
        }

        public async Task SearchCatRef()
        {
            await CommonUseService.SearchCadastralReference(CadastralReference);
        }

        public async Task AddImage()
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
                using var stream = await file.OpenStreamForReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                ImageData = ms.ToArray();
                ImageName = file.Name;

                using var stream2 = await file.OpenReadAsync();
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream2);
                ImageBind = bitmap;
            }

        }
        public void RemoveImage()
        {
            ImageData = null;
            ImageBind = null;
            ImageName = string.Empty;
        }

        public async Task SearchFolder()
        {
            await CommonUseService.SearchExpedientNumberFolder(ExpedientNumber);
        }
    }
}
