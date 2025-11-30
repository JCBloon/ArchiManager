using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiManagerWinUI.CustomServices.Navigation
{
    public static class NavigationService
    {
        private static MainWindow? MAIN_WINDOW { get; set; }

        public static void Initialize(MainWindow window)
        {
            MAIN_WINDOW = window;
        }

        public static void Navigate(AppPage page, object? parameter = null)
        {
            if (MAIN_WINDOW == null)
                throw new InvalidOperationException("NavigationService no inicializado");

            MAIN_WINDOW.NavigateTo(page, parameter);
        }
    }

}
