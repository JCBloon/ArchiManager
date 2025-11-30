using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArchiManagerWinUI.CustomServices.Navigation;

namespace ArchiManagerWinUI.MVVM.ViewModel
{
    public class DataMenuViewModel : ObservableObject
    {
        public ICommand GoToManageClientsCommand {  get; }
        public ICommand GoToManageProjectsCommand { get; }
        public ICommand GoToClientListCommand { get; }
        public ICommand GoToProjectListCommand { get; }
        public DataMenuViewModel() 
        {
            GoToManageClientsCommand = new RelayCommand(GoToManageClients);
            GoToManageProjectsCommand = new RelayCommand(GoToManageProjects);
            GoToClientListCommand = new RelayCommand(GoToClientList);
            GoToProjectListCommand = new RelayCommand(GoToProjectList);
        }

        public void GoToManageClients()
        {
            NavigationService.Navigate(AppPage.ManageClientsView);
        }

        public void GoToManageProjects()
        {
            NavigationService.Navigate(AppPage.ManageProjectsView);
        }

        public void GoToClientList()
        {
            NavigationService.Navigate(AppPage.ClientListView);
        }

        public void GoToProjectList()
        {
            NavigationService.Navigate(AppPage.ProjectListView);
        }
    }
}
