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
using System.Threading.Tasks;
using System.Windows.Input;

namespace ArchiManagerWinUI.MVVM.ViewModel
{
    public partial class ProjectListViewModel : ObservableObject
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

        // Cache
        private string? _cacheTitle;
        private string? _cacheExpedientNumber;
        private string? _cacheYear;
        private string? _cacheCadastralReference;

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
        public ICommand SearchProjectCommand { get; }
        public ICommand SearchNextPageCommand { get; }
        public ICommand SearchPrevPageCommand { get; }
        public ICommand GoToSearchProjectCommand { get; }
        public ICommand GoToEditProjectCommand { get; }
        public ObservableCollection<Project> DataGridProjectsCollection { get; } = new ObservableCollection<Project>();
        public ProjectListViewModel() 
        {
            SearchProjectCommand = new AsyncRelayCommand(SearchProject);
            SearchNextPageCommand = new AsyncRelayCommand(SearchNextPage);
            SearchPrevPageCommand = new AsyncRelayCommand(SearchPrevPage);
            _currentPage = 0;
            GoToSearchProjectCommand = new RelayCommand<Project>(GoToSearchProject);
            GoToEditProjectCommand = new RelayCommand<Project>(GoToEditProject);
        }

        public async Task Search()
        {
            // Filtros
            string? column = null;
            switch (SelectedColumn)
            {
                case "Título":
                    column = "title";
                    break;
                case "NºExp.":
                    column = "expedientNumber";
                    break;
                case "Año":
                    column = "year";
                    break;
                case "Ref.Cat.":
                    column = "cadastralReference";
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

            // Parse
            short? yearValue = null;
            if (!string.IsNullOrWhiteSpace(_cacheYear) && short.TryParse(_cacheYear, out short parsedYear))
            {
                yearValue = parsedYear;
            }

            // Petición
            ArchiHttpResult<List<Project>> result = await ArchiHttpRequest.SearchProject(_cacheTitle, _cacheExpedientNumber, yearValue, _cacheCadastralReference, _currentPage.ToString(), column, order);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                List<Project> projects = result.Value!;
                DataGridProjectsCollection.Clear();
                if (projects.Any())
                {
                    foreach (var proj in projects)
                        DataGridProjectsCollection.Add(proj);
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

                if (_currentPage != 0)
                {
                    CanGoBack = true;
                }
                else
                {
                    CanGoBack = false;
                }
            }
        }

        public async Task SearchProject()
        {
            // Cache
            _cacheTitle = Title;
            _cacheExpedientNumber = ExpedientNumber;
            _cacheYear = Year;
            _cacheCadastralReference = CadastralReference;
            _currentPage = 0;

            await Search();
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

        public void GoToSearchProject(Project? project)
        {
            if (project == null)
                return;

            ShouldRefresh = true;
            NavigationService.Navigate(AppPage.SearchProjectWithArgs, project);
        }

        public void GoToEditProject(Project? project)
        {
            if (project == null)
                return;

            ShouldRefresh = true;
            NavigationService.Navigate(AppPage.ManageProjectsWithArgs, project);
        }
    }
}
