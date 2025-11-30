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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Protection.PlayReady;

namespace ArchiManagerWinUI.MVVM.ViewModel
{
    public partial class SearchClientViewModel : ObservableObject
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

        [ObservableProperty]
        private Client? foundClient;

        public ICommand FindClientCommand { get; }
        public ICommand GoToSearchProjectCommand { get; }

        public ObservableCollection<Project> DataGridProjectsCollection { get; } = new ObservableCollection<Project>();
        
        public SearchClientViewModel(Client? client = null)
        {
            FindClientCommand = new AsyncRelayCommand(FindClient);
            GoToSearchProjectCommand = new RelayCommand<Project>(GoToSearchProject);
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
            } else
            {
                DataGridProjectsCollection.Clear();

                Client client = result.Value!;
                FoundClient = client;

                List<Project> projects = client.ProjectList!;
                if (projects.Any())
                {
                    foreach(var proj in projects)
                        DataGridProjectsCollection.Add(proj);
                }
            }
        }

        public void GoToSearchProject(Project? project)
        {
            if (project == null)
                return;

            NavigationService.Navigate(AppPage.SearchProjectWithArgs, project);
        }
    }
}
