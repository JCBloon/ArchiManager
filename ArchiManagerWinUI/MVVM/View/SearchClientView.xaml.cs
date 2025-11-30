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
    public sealed partial class SearchClientView : Page
    {
        public SearchClientViewModel ViewModel { get; private set; } = null!;

        public SearchClientView()
        {
            InitializeComponent();
            // Como necesitamos lógica, usamos el OnNavigated para inicializar y no generar dos instancias
        }

        private void ProjectInfo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Project project)
            {
                if (ViewModel.GoToSearchProjectCommand.CanExecute(project))
                {
                    ViewModel.GoToSearchProjectCommand.Execute(project);
                }
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var client = e.Parameter as Client;
            ViewModel = new SearchClientViewModel(client);
            DataContext = ViewModel;

            if (client != null)
                await ViewModel.InitializeAsync();
        }

    }
}
