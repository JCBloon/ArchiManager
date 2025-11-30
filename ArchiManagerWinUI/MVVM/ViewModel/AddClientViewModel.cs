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
using Windows.Media.Protection.PlayReady;

namespace ArchiManagerWinUI.MVVM.ViewModel
{
    public partial class AddClientViewModel : ObservableObject
    {
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

        public ICommand GoBackCommand { get; }
        public ICommand AddClientCommand { get; }

        public AddClientViewModel()
        {
            GoBackCommand = new RelayCommand(GoBack);
            AddClientCommand = new AsyncRelayCommand(AddClient);
        }

        public void GoBack()
        {
            NavigationService.Navigate(AppPage.GoBack);
        }

        public async Task AddClient()
        {
            // Validación
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

            // Petición
            Client clientParams = new Client(
                null,
                Dni,
                Name,
                Surname1,
                string.IsNullOrWhiteSpace(Surname2) ? null : Surname2,
                string.IsNullOrWhiteSpace(Phone) ? null : Phone,
                string.IsNullOrWhiteSpace(Address) ? null : Address
            );
            ArchiHttpResult<Client> result = await ArchiHttpRequest.CreateClient(clientParams);
            if (result.IsSuccess == false)
            {
                if (result.NoConnection == true)
                    await DialogService.SimpleDialog(DialogService.DialogType.Connection, result.Error);
                else
                    await DialogService.SimpleDialog(DialogService.DialogType.Warning, result.Error);
            }
            else
            {
                await DialogService.SimpleDialog(DialogService.DialogType.Success, "Cliente creado con éxito");
                Name = string.Empty;
                Surname1 = string.Empty;
                Surname2 = string.Empty;
                Dni = string.Empty;
                Phone = string.Empty;
                Address = string.Empty;
            }
        }
    }
}
