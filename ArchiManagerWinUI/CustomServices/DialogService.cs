using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;

namespace ArchiManagerWinUI.CustomServices
{
    public static class DialogService
    {
        public enum DialogType
        {
            Connection,
            Success,
            Warning,
            Error
        }

        private static async Task ShowSimpleDialog(string title, string message, Uri imageUri)
        {
            ContentDialog dialog = new ContentDialog()
            {
                XamlRoot = App.MainWindowApp!.Content.XamlRoot,
                Title = title,
                Width = 500,
                CloseButtonText = "Ok",
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10,
                    Children =
                        {
                            new Image
                            {
                                Source = new BitmapImage(imageUri),
                                Width= 80,
                                Height = 80
                            },
                            new TextBlock
                            {
                                TextWrapping = TextWrapping.Wrap,
                                Text = message,
                                VerticalAlignment = VerticalAlignment.Center,
                                MaxWidth = 300
                            }
                        }
                }
            };

            await dialog.ShowAsync();
        }

        public static async Task SimpleDialog(DialogType type, string? message = null)
        {
            if (type == DialogType.Connection)
            {
                string conTitle = "Sin conexión";
                string conMessage = "No se pudo conectar, reinténtalo más tarde";
                var conImageUri = new Uri("ms-appx:///Assets/Icons/noConnection.png");

                await ShowSimpleDialog(conTitle, conMessage, conImageUri);
            }
            else if (type == DialogType.Success)
            {
                string succTitle = "Éxito";
                await ShowSimpleDialog(succTitle, message!, new Uri("ms-appx:///Assets/Icons/success.png"));
            }
            else if (type == DialogType.Warning)
            {
                string warTitle = "Advertencia";
                await ShowSimpleDialog(warTitle, message!, new Uri("ms-appx:///Assets/Icons/warning.png"));
            } else if (type == DialogType.Error)
            {
                string errTitle = "Error";
                await ShowSimpleDialog(errTitle, message!, new Uri("ms-appx:///Assets/Icons/error.png"));
            } 


        }


        public static async Task<bool> AnswerableDialog(string question)
        {
            ContentDialog dialog = new ContentDialog()
            {
                XamlRoot = App.MainWindowApp!.Content.XamlRoot,
                Title = "Advertencia",
                Width = 500,
                PrimaryButtonText = "Aceptar",
                SecondaryButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Primary, // Será true si pulsa el primario
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10,
                    Children =
                        {
                            new Image
                            {
                                Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/warning.png")),
                                Width= 80,
                                Height = 80
                            },
                            new TextBlock
                            {
                                TextWrapping = TextWrapping.Wrap,
                                Text = question,
                                VerticalAlignment = VerticalAlignment.Center,
                                MaxWidth = 300
                            }
                        }
                }
            };

            // Es un ContentDialogResult
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
