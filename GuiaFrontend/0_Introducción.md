[Volver](Apuntes%20WinUI.md)
## Tecnologías consideradas:

- WinUI: Uso único en WIN10 y 11, solo windows nativo. Usa la tienda de microsoft.
- .NET MUAI (Multiplataforma como iOs, Windows, Apps, etc)
- UNO PLATFORM (Multiplataforma, open source), 
-  AVALONIA UI (Similar a UNO, usa xaml pero no deja definir con solo C#, soporte principal para visionOS de iOs)

WinUI elegida. Más información [Youtube - .NET: eligiendo framework gratuito para apps de escritorio](https://www.youtube.com/watch?v=fmcadSwjKKA)


## Historia de .NET - Proceso para llegar a WinUI

Se empezó usando Windows Forms (**WinForms**, 2002) junto con la primera versión de **.NET Framework**. WinForms no usaba XAML y estaba basado en tecnología nativa de Windows. En 2006, con **.NET Framework 3.0**, apareció **WPF** (Windows Presentation Foundation), que sí usaba XAML y un motor gráfico basado en DirectX. Se lanzaron versiones de .NET Framework hasta llegar a la 4.8 (2019), que continúa en mantenimiento. También se puede mencionar **ASP.NET**, tecnología que usaba este framework enfocado en desarrollo web.

En 2015 se creó **UWP** (Universal Windows Platform), una API pensada para aplicaciones del ecosistema Windows 10 (PC, Xbox, HoloLens…), usando un subconjunto restringido de .NET y compilación nativa. Nunca fue pensado para otros SO, pero su evolución serviría de base para lo que más tarde sería WinUI.

Mientras tanto, Microsoft dio el salto al open source y creó **.NET Core** (2016), multiplataforma y orientado a aplicaciones de consola, backend y web. A partir de la versión 3.0/3.1 (2019) también soportó WPF y WinForms en Windows.

En 2020, para evitar confusiones entre la versión 4.x de .NET Framework y la de .NET Core, se unificó la plataforma bajo el nombre **.NET**, empezando desde la versión 5. Desde entonces se pueden crear aplicaciones de distintos ámbitos, existiendo a día de hoy (2025):
- **WinUI 3**: para apps nativas de Windows 10/11, sucesor de UWP, disponible a través de Windows App SDK.
- **MAUI**: sucesor de Xamarin.Forms para aplicaciones multiplataforma (móvil y escritorio).
- **WPF y Windows Forms**: con soporte continuo en Windows, pero no son su principal enfoque.
## Crear un proyecto WinUI

[Herramientas necesarias](https://learn.microsoft.com/es-es/windows/apps/windows-app-sdk/set-up-your-development-environment?tabs=cs-vs-community%2Ccpp-vs-community%2Cvs-2022-17-10%2Cvs-2022-17-1-b)
[Ver documentación](https://learn.microsoft.com/es-es/windows/apps/winui/winui3/create-your-first-winui3-app)

Por rapidez, se puede ejecutar el proyecto con **Package**, que usa UWP y simula un entorno aislado, que sería lo que se vería en otro equipo.

Notas importantes:
- En el constructor de **App**, verá que se llama al método **InitializeComponent**. Básicamente, ese método analiza el contenido de **App.xaml**, que es el marcado XAML. Y eso es importante porque **App.xaml** contiene recursos combinados que deben resolverse y cargarse en un diccionario para que la aplicación en ejecución la use.

## Patrón de la App

Considerando los siguientes:

- `MVC (Model-View-Controller)`: La Vista muestra datos y gestiona eventos, con mínima lógica de presentación. El Controller responde a eventos, actualiza el Model y la Vista si es necesario. El Model puede comunicarse con la View.
- `MVP (Model-View-Presenter)`: La Vista es pasiva y solo notifica eventos al Presenter. El Presenter maneja la lógica de presentación, actualiza la Vista y el Model. El Model contiene datos y lógica de negocio, no se comunica con la View.
- `MVVM (Model-View-ViewModel)`: La Vista es la más pasiva y se enlaza al ViewModel mediante binding; tiene mínima lógica de presentación. El ViewModel expone datos y comandos, maneja la lógica de presentación y actualiza el Model. El Model contiene datos y lógica de negocio, no se comunica con la View.

Usaremos MVVM, ya que además, está facilitado para esta tecnología.
## WinUI Gallery

Es una aplicación oficial de la tienda de Microsoft Store donde se puede consultar ejemplos para aplicaciones obre diferentes controles. Es un buen soporte para descubrir esta herramienta.

## MVVM Toolkit Sample App

Al igual que WinUI Gallery, se puede descagar desde Microsoft Sotre. Está enfocado en la librería Mvvm mencionada a continuación.

## Librerías NuGet Package Manager usadas

Es el gestor de paquetes de WinUI (Entre otros). Para acceder al gestor de paquetes NuGet iremos a `Tools`->`NuGet Package Manager`->`Manage NuGet Packages for Solution...`

Destacaremos:
- `CommunityToolkit.Mvvm`: Facilita mucho el uso del patrón MVVM
- `CommunityToolkit.WinUI.UI.Controls`: Permite el uso de controles procedentes de UWP/WPF
- [WinUiEx](https://dotmorten.github.io/WinUIEx/concepts/WindowExtensions.html): En WPF/UWP existen métodos para ajustar la ventana, y como en WinUi3 no existe, este paquete NuGet facilita la implementación
- `QuestPDF - Modern PDF library for C# developers`: Permite crear pdfs customizados para guardarlos en el dispositivo. Usa la licencia "Community"