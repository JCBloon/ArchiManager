using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using ArchiManagerWinUI.MVVM.View;
using Windows.ApplicationModel.Background;
using ArchiManagerWinUI.CustomServices.Navigation;
using WinUIEx;
using ArchiManagerWinUI.MVVM.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ArchiManagerWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            InitializeComponent();

            // Ajustar pantalla
            this.MinWidth = 1200;
            this.MinHeight = 780;
            this.CenterOnScreen();
            this.Maximize();

            contentFrame.Navigate(typeof(HomePage));

            // Inicializar NavigationService
            NavigationService.Initialize(this);
            // Inicializar el botón
            NVMenu.IsBackEnabled = contentFrame.CanGoBack;
            // Suscribir la navegación al método donde actualizamos el estado de IsBackEnabled
            contentFrame.Navigated += ContentFrame_Navigated;
        }

        public void NavigateTo(AppPage page, object? parameter = null)
        {
            Type pageType;
            switch (page)
            {
                case (AppPage.ManageClientsView):
                    pageType = typeof(ManageClientsView);
                    break;
                case (AppPage.AddClientView):
                    pageType = typeof(AddClientView);
                    break;
                case (AppPage.ManageProjectsView):
                    pageType = typeof(ManageProjectsView);
                    break;
                case (AppPage.AddProjectView):
                    pageType = typeof(AddProjectView);
                    break;
                case (AppPage.ClientListView):
                    pageType = typeof(ClientListView);
                    break;
                case (AppPage.ProjectListView):
                    pageType = typeof(ProjectListView);
                    break;
                case (AppPage.GoBack):
                    if (contentFrame.CanGoBack)
                    {
                        contentFrame.GoBack();
                    }
                    return;
                case (AppPage.SearchClientWithArgs):
                    pageType = typeof(SearchClientView);
                    break;
                case (AppPage.SearchProjectWithArgs):
                    pageType = typeof(SearchProjectView);
                    break;
                case (AppPage.ManageClientsWithArgs):
                    pageType = typeof(ManageClientsView);
                    break;
                case (AppPage.ManageProjectsWithArgs):
                    pageType = typeof(ManageProjectsView);
                    break;
                default:
                    return;
            }
                

            _ = contentFrame.Navigate(pageType, parameter, new SuppressNavigationTransitionInfo());
        }

        public void NV_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            /*FrameNavigationOptions navOptions = new FrameNavigationOptions(); // Inicia las opciones de navegación del frame nuevo
            
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo; // Ajusta la animación a la interacción del usuario, es puramente estético
            
             * Esto afecta al historial de paginación. Como está ahora, indica que si viene de un panel lateral la interacción de cambio,
             * no se va a guardar en el historial y no se podrá volver atrás. En un futuro, si se necesita un tab con opciones laterales,
             * esta configuración establece que la última página visitada no va a ser uno de los tab
            if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Left)
            {
                navOptions.IsNavigationStackEnabled = false;
            }*/

            Type pageType;
            var selectedItem = (NavigationViewItem) args.SelectedItem;

            switch (selectedItem.Tag?.ToString())
            {
                case "SearchProjectView":
                    pageType = typeof(SearchProjectView);
                    break;
                case "SearchClientView":
                    pageType = typeof(SearchClientView);
                    break;
                case "DataMenuView":
                    pageType = typeof(DataMenuView);
                    break;
                case "AddClientView":
                    pageType = typeof(AddClientView);
                    break;
                case "AddProjectView":
                    pageType = typeof(AddProjectView);
                    break;
                default:
                    return;
            }
            
            if (contentFrame.CurrentSourcePageType == pageType)
                return;

            // No queremos guardar el historial de otros NVItems, por lo que navegamos a home, borramos el historial, y al ir al nuevo Item cargará a la pila la página
            // actual (Home) y la nueva del item al que viajamos, de modo que al darle a atrás volveremos a Home
            _ = contentFrame.Navigate(typeof(HomePage), null, new SuppressNavigationTransitionInfo());
            contentFrame.BackStack.Clear();
            _ = contentFrame.Navigate(pageType, null, new SuppressNavigationTransitionInfo()); // También se podría crear en el navOptions, en el constructor este objeto y pasarlo

            /*
            var navOptions = new FrameNavigationOptions
            {
                TransitionInfoOverride = new SuppressNavigationTransitionInfo()
            }; 
             */

            // _ = contentFrame.Navigate(pageType); // Transición por defecto
            // Navigate devuelve un bool, para evitar warnings, se puede usar esto para indicar que lo ignoras a propósito
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NVMenu.IsBackEnabled = contentFrame.CanGoBack;
        }

        private void NV_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
            }

            if(!contentFrame.CanGoBack)
            {
                NVMenu.SelectedItem = HiddenItem;
            }

        }

    }
}
