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

namespace ArchiManagerWinUI.MVVM.View;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ManageProjectsView : Page
{
    public ManageProjectsViewModel ViewModel { get; private set; }
    public ManageProjectsView()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var project = e.Parameter as Project;
        ViewModel = new ManageProjectsViewModel(project);
        DataContext = ViewModel;

        if (project != null)
            await ViewModel.InitializeAsync();
    }

    // DataGrid no funciona correctamente en WinUI3, necesitamos llamar al VM desde el code-behind
    private void UnassignButton_Click(object sender, RoutedEventArgs e)
    {
        // Obtenemos el Project de la fila
        if (sender is Button button && button.DataContext is Client client)
        {
            // Llamamos al comando del ViewModel
            if (ViewModel.UnassignClientCommand.CanExecute(client))
            {
                ViewModel.UnassignClientCommand.Execute(client);
            }
        }
    }
}
