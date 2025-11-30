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
public sealed partial class ClientListView : Page
{
    public ClientListViewModel ViewModel { get; } = new();
    public ClientListView()
    {
        InitializeComponent();
        DataContext = ViewModel;
        this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
    }

    private void ClientInfo_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Client client)
        {
            if (ViewModel.GoToSearchClientCommand.CanExecute(client))
            {
                ViewModel.GoToSearchClientCommand.Execute(client);
            }
        }
    }

    private void ClientEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Client client)
        {
            if (ViewModel.GoToManageClientCommand.CanExecute(client))
            {
                ViewModel.GoToManageClientCommand.Execute(client);
            }
        }
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (ViewModel.ShouldRefresh)
        {
            await ViewModel.Search();
        }

        ViewModel.ShouldRefresh = false;
    }

}
