using ArchiManagerWinUI.CustomServices;
using ArchiManagerWinUI.MVVM.Model;
using ArchiManagerWinUI.MVVM.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ArchiManagerWinUI.MVVM.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageClientsView : Page
    {
        public ManageClientsViewModel ViewModel { get; private set; }
        public ManageClientsView()
        {
            InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var client = e.Parameter as Client;
            ViewModel = new ManageClientsViewModel(client);
            DataContext = ViewModel;

            if (client != null)
                await ViewModel.InitializeAsync();
        }

        // DataGrid no funciona correctamente en WinUI3, necesitamos llamar al VM desde el code-behind
        private async void UnassignButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtenemos el Project de la fila
            if (sender is Button button && button.DataContext is Project project)
            {
                // Llamamos al comando del ViewModel
                if (project == null)
                {
                    await DialogService.SimpleDialog(DialogService.DialogType.Error, "Error al intentar desasignar el proyecto");
                    return;
                }

                if (ViewModel.UnassignProjectCommand.CanExecute(project))
                {
                    ViewModel.UnassignProjectCommand.Execute(project);
                }
            }
        }

    }
}
