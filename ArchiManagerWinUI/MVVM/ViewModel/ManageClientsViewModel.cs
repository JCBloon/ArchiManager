using ArchiManagerWinUI.CustomServices;
using ArchiManagerWinUI.CustomServices.DataAccess;
using ArchiManagerWinUI.CustomServices.Navigation;
using ArchiManagerWinUI.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ArchiManagerWinUI.MVVM.ViewModel
{
    public partial class ManageClientsViewModel : ObservableObject
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

        //OneWay
        [ObservableProperty]
        private Client? foundClient = null;
            // NOTA: Se hace Binding directamente al cliente porque todos los atributos son string

        public ICommand FindClientCommand { get; }
        public ICommand UpdateClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand UnassignProjectCommand { get; }
        public ObservableCollection<Project> DataGridProjectsCollection { get; } = new ObservableCollection<Project>();

        public ManageClientsViewModel(Client? client = null) 
        {
            FindClientCommand = new AsyncRelayCommand(FindClient);
            UpdateClientCommand = new AsyncRelayCommand(UpdateClient);
            DeleteClientCommand = new AsyncRelayCommand(DeleteClient);
            UnassignProjectCommand = new AsyncRelayCommand<Project>(UnassignProject);

            if (client != null)
            {
                name = client.Name;
                surname1 = client.Surname1;
                surname2 = client.Surname2;
                dni = client.Dni;
            }
        }
        public async Task InitializeAsync()
        {
            await FindClient();
        }

        public async Task FindClient()
        {
            ArchiHttpResult<Client> result = await ArchiHttpRequest.FindClient(Name, Surname1, Surname2, Dni);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                Client client = result.Value!;
                FoundClient = client;

                DataGridProjectsCollection.Clear();
                List<Project> projects = client.ProjectList!;
                if (projects.Any())
                {
                    foreach (var proj in projects)
                        DataGridProjectsCollection.Add(proj);
                }
            }
        }

        public async Task UpdateClient()
        {
            // Validación
            if (FoundClient == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Ningún cliente seleccionado");
                return;
            }

            if (string.IsNullOrEmpty(FoundClient.Dni) || string.IsNullOrEmpty(FoundClient.Name) || string.IsNullOrEmpty(FoundClient.Surname1)) {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "El cliente debe tener DNI, Nombre y Primer Apellido");
                return;
            }

            if (FoundClient.Dni.Length != 9)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "El DNI debe contener 9 carácteres");
                return;
            }

            // Petición
            ArchiHttpResult<Client> result = await ArchiHttpRequest.UpdateClient(FoundClient);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                Client client = result.Value!;
                FoundClient = client;

                await DialogService.SimpleDialog(DialogService.DialogType.Success, "Cliente modificado con éxito");
            }
        }

        public async Task DeleteClient()
        {
            if (FoundClient == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Ningún cliente seleccionado");
                return;
            }

            // Confirmación
            bool confirmation = await DialogService.AnswerableDialog("¿Está seguro de eliminar este cliente? Todos los proyectos que queden sin clientes asociados también serán eliminados");
            if (!confirmation)
            {
                return;
            }

            // Petición
                // Conversión de long? Id a long Id
            long id;
            if (FoundClient!.Id.HasValue)
            {
                id = FoundClient.Id.Value;
            }
            else
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Error, "Error de conversión interno del programa");
                return;
            }

            ArchiHttpResult<object> result = await ArchiHttpRequest.DeleteClient(id);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                FoundClient = null;
                DataGridProjectsCollection.Clear();
                await DialogService.SimpleDialog(DialogService.DialogType.Success, "Cliente eliminado con éxito");
            }
        }

        public async Task UnassignProject(Project? project)
        {
            // Validación
            if (project == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Ningún proyecto seleccionado");
            }

            if (FoundClient == null)
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Warning, "Ningún cliente seleccionado");
                return;
            }

            // Confirmación
            bool confirmation = await DialogService.AnswerableDialog("¿Está seguro de desasignar este proyecto? Todos los proyectos que queden sin clientes asociados también serán eliminados");
            if (!confirmation)
            {
                return;
            }

            ArchiHttpResult<object> result = await ArchiHttpRequest.UnassignClient(project!.Id ?? -1, FoundClient.Id ?? -1);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                DataGridProjectsCollection.Remove(project);
            }

        }
    }
}
