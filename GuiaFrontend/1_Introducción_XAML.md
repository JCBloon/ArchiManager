[Volver](Apuntes%20WinUI.md)

## Enlaces

[Introducción a XAML con WPF](https://learn.microsoft.com/es-es/dotnet/desktop/wpf/xaml/)
[XAML en WPF](https://learn.microsoft.com/es-es/dotnet/desktop/wpf/advanced/xaml-syntax-in-detail)
## LO BÁSICO

 Son archivos XML que generalmente tienen la extensión `.xaml` 
  
``` xml
 <StackPanel>
    <Button Content="Click Me"/>
</StackPanel>
```

Admiten varios atributos de elementos del objeto

``` xml
<Button Background="Blue" Foreground="Red" Content="This is a button"/>
```  

Para algunas propiedades de un elemento de objeto, no es posible la sintaxis de atributo, ya que el objeto o la información necesarios para proporcionar el valor de la propiedad no se pueden expresar adecuadamente.
La sintaxis de la etiqueta de inicio del elemento de propiedad es `<TypeName.PropertyName>`. Siempre es posible, para un marcado más compacto su uso, es cuestión de estilo. Siguiendo el ejemplo anterior:

``` xml
<Button>
    <Button.Background>
        <SolidColorBrush Color="Blue"/>
    </Button.Background>
    <Button.Foreground>
        <SolidColorBrush Color="Red"/>
    </Button.Foreground>
    <Button.Content>
        This is a button
    </Button.Content>
</Button>
```

Como regla del lenguaje XAML, el valor de una propiedad de contenido XAML debe proporcionarse completamente antes o completamente después de cualquier otro elemento de propiedad en ese elemento de objeto. Por ejemplo, el siguiente código no se compila.

``` xml
<Button>I am a
  <Button.Background>Blue</Button.Background>
  blue button</Button>
```

En general, XAML distingue mayúsculas de minúsculas. Para resolver tipos de respaldo, XAML de WPF distingue mayúsculas de minúsculas por las mismas reglas que CLR distingue entre mayúsculas y minúsculas. Dependerá del comportamiento del convertidor de tipos asociado con la propiedad a la que se aplica el valor, o del tipo del valor de la propiedad. Por ejemplo, las propiedades que toman el tipo **Boolean** pueden tomar `true` o `True` como valores equivalentes, pero solo porque la conversión nativa del analizador XAML de WPF de cadena a **Boolean** ya permite estos valores como equivalentes. Los procesadores y serializadores (conversión de objeto o datos complejos a un formato que pueda ser almacenado/transmitido) XAML de WPF omitirán o quitarán todos los espacios en blanco no asignados y normalizarán cualquier espacio en blanco significativo.

## XAML y CLR

XAML es un lenguaje de marcado que se usa para definir interfaces gráficas, como ventanas y botones. CLR (Common Language Runtime) es el motor que ejecuta aplicaciones .NET, como las hechas en C#.

XAML no se ejecuta directamente en el CLR, pero sí se traduce en objetos que sí entiende el CLR.  
Esto funciona porque WPF (el sistema gráfico de Windows) convierte los elementos XAML en objetos reales de C# cuando se ejecuta el programa.

Por eso, aunque XAML tiene su propio estilo, al final todo se conecta con el sistema de tipos de C# y .NET (CLR).  Y aunque en teoría XAML podría usarse con otros lenguajes que no sean .NET, eso requeriría un sistema completamente diferente.
## Extensiones de marcado

Las extensiones de marcado son un concepto de lenguaje XAML. Cuando se usa para proporcionar el valor de una sintaxis de atributo, las llaves (`{` y `}`) indican un uso de extensión de marcado. Este uso orienta el procesamiento XAML para evitar el tratamiento general de los valores de los atributos como cadenas literales o valores convertibles a cadenas. Normalmente se aplicará a referencias de objetos disponibles solo en tiempo de ejecución.

Se suele usar en WPF  extensiones como el `Binding`, que se emplean para las expresiones de enlace de datos, y las referencias de recursos `StaticResource` y `DynamicResource`

``` xml
<Window x:Class="index.Window1"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Window1" Height="100" Width="300">
    <Window.Resources>
        <SolidColorBrush x:Key="MyBrush" Color="Gold"/>
        <Style TargetType="Border" x:Key="PageBackground">
            <Setter Property="BorderBrush" Value="Blue"/>
            <Setter Property="BorderThickness" Value="5" />
        </Style>
    </Window.Resources>
    <Border Style="{StaticResource PageBackground}">
        <StackPanel>
            <TextBlock Text="Hello" />
        </StackPanel>
    </Border>
</Window>
```

Donde en el estilo definimos el tipo de objeto afectado (Border) y el id para aplicarlo (x:Key)

``` xml
<Style TargetType="Border" x:Key="PageBackground">
	...
</Style>
```

Y se aplica llamando al recurso estático

``` xml
<Border Style="{StaticResource PageBackground}">
        ...
</Border>
```

## Convertidores de tipo

 El control nativo básico de cómo se convierten las cadenas en otros tipos de objeto o valores primitivos se basa en el tipo **String** en sí mismo. Por ejemplo, la estructura *Thickness*, indica las medidas de un rectángulo y se usa como valor para propiedades como **Margin**. Los siguientes ejemplos son equivalentes:

``` xml
<Button Margin="10,20,10,30" Content="Click me"/>

<Button Content="Click me">
    <Button.Margin>
        <Thickness Left="10" Top="20" Right="10" Bottom="30"/>
    </Button.Margin>
</Button>
```

## Elementos raíz

Un archivo XAML solo debe tener un elemento raíz, para que sea un archivo XML bien formado y un archivo XAML válido. Para escenarios típicos de WPF, se usa un elemento raíz que tiene un significado destacado en el modelo de aplicación de WPF (por ejemplo, [Window](https://learn.microsoft.com/es-es/dotnet/api/system.windows.window) o [Page](https://learn.microsoft.com/es-es/dotnet/api/system.windows.controls.page) para una página, [ResourceDictionary](https://learn.microsoft.com/es-es/dotnet/api/system.windows.resourcedictionary) para un diccionario externo o [Application](https://learn.microsoft.com/es-es/dotnet/api/system.windows.application) para la definición de la aplicación). Vease el ejemplo:

``` xml
<Page x:Class="index.Page1"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      Title="Page1">
</Page>
```
+ `x:Class`: Nombre de la página
+ `xmlns & xmlns:x`: Indica al procesador XAML cuáles espacios de nombres XAML contienen las definiciones de los tipos de elementos. Como si importáramos una librería en C# o JAVA
+ `xmlns`: Este es el espacio de nombre por defecto, que se usa para los elementos sin prefijo (`<Window>`, `<Grid>`, `<Button>`, etc.).
+ `xmlns:x`: Este asigna el prefijo `x:` al espacio de nombre XAML básico, como `x:Class` o  `x:Key`

## Prefijos
### Prefijos x más comunes
+ `x:Key`: Establece una clave única para cada recurso dentro de un **ResourceDictionary**
`<SolidColorBrush x:Key="ColorPrimario" Color="Blue" />`

+ `x:Class`: Define el namespace CLR y el nombre de clase del archivo de código asociado
`<Window x:Class="MiApp.MainWindow" ... />`

> **CODE BEHIND / Código subyacente**
> El **code-behind** es un archivo de código C# (o VB.NET) que **acompaña a un archivo XAML**.  
> Contiene la **lógica de programación** que controla la interfaz definida en el XAML.
> `MainWindow.xaml` - define la UI (con XAML)
> `MainWindow.xaml.cs` - contiene el code-behind (con C#)

+ `x:Name`: Establece un nombre en tiempo de ejecución para acceder a la instancia desde el código
``` xml
<Button x:Name="BotonEnviar" Content="Enviar" />
```

Se puede usar `Name` si el controlador cuenta con la propiedad. El nombre debe ser único en todo el ámbito XAML

``` xml
<StackPanel Name="buttonContainer">
    <Button Click="RemoveThis_Click">Click to remove this button</Button>
</StackPanel>
```

``` csharp
private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
{
    MessageBox.Show("¡StackPanel recibió el clic del TextBlock!");
}
```


+ `x:Static`: Referencia que retorna un valor estático que no es una propiedad compatible con XAML
`<TextBlock Text="{x:Static sys:Math.PI}" />`

+ `x:Type`: construye una **Type** referencia basada en un nombre de tipo CLR
`<Style TargetType="{x:Type Button}" />`

### Prefijos personalizados

Se usan para definir ensamblados personalizados o para ensamblados fuera del núcleo WPF. Véase el ejemplo:

```csharp
namespace MiApp.Controles
{
    public class MiBotonEspecial : Button { }
}
```

``` xml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:local="clr-namespace:MiApp.Controles">

    <Grid>
        <local:MiBotonEspecial Content="Soy especial" />
    </Grid>
</Window>
```

+ `xmlns:local="..."`: crea un prefijo personalizado (`local:`) para tu espacio de nombres.
- `<local:MiBotonEspecial />`: usa tu clase personalizada como si fuera un control normal.

## Código subyacente y eventos

La mayoría de las aplicaciones de WPF constan de marcado XAML y código subyacente. Dentro de un proyecto, el XAML se escribe como un `.xaml` archivo y un lenguaje CLR como Microsoft Visual Basic (Mayormente obsoleto) o C# se usa para escribir un archivo de código subyacente. Como ya vimos, el código XAML se identifica especificando un espacio de nombres y una clase como atributo x:Class del elemento raíz de XAML. Este proceso es implícito (Es decir, no necesitamos configurar nada). Ejemplo:

``` xml
<Page x:Class="ExampleNamespace.ExamplePage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel>
        <Button Click="Button_Click">Click me</Button> 
        <!-- Llama a la función con el mismo nombre -->
    </StackPanel>
</Page>
```

``` csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExampleNamespace;

public partial class ExamplePage : Page
{
    public ExamplePage() =>
        InitializeComponent();

	//Ejecuta la función cada vez que reciba un evento
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var buttonControl = (Button)e.Source;
        buttonControl.Foreground = Brushes.Red;
    }
}
```

Se pueden enrutar eventos por la jerarquía de elementos visuales. Esto sirve para manejar eventos a nivel de grupo o contenedor sin agregar código a cada control individual o para interceptar o cancelar eventos antes de que lleguen al destino. Hay 3 tipos de rutas:

1. Bubbling (ascendente)

> **NOTA:** La etiqueta MouseDown de StackPanel actúa en forma de burbuja, mientras que PreviewMouseDown en túnel, véase en otro apartado las etiquetas de cada controlador con más detalle

``` xml
<StackPanel MouseDown="StackPanel_MouseDown">
    <TextBlock Text="Haz clic aquí" />
</StackPanel>
```

``` csharp
private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
{
    MessageBox.Show("¡StackPanel recibió el clic del TextBlock!");
}
```

2. Tunneling (descendente)

``` xml
<StackPanel PreviewMouseDown="StackPanel_PreviewMouseDown">
    <TextBlock Text="Haz clic aquí" />
</StackPanel>
```

``` csharp
private void StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
{
    MessageBox.Show("StackPanel interceptó el clic ANTES de que llegue al TextBlock.");
}
```

3. Directo 
`Usado en el ejemplo del apartado`

## Propiedades adjuntas y eventos adjuntos

En XAML, puedes aplicar ciertas propiedades o eventos a elementos, aunque no pertenezcan directamente a ese tipo.

Las **propiedades adjuntas** se usan con la sintaxis `TipoPropietario.Propiedad` en elementos como paneles de diseño como `DockPanel`, que necesitan que los hijos definan cómo deben comportarse dentro del contenedor.

``` xml
<DockPanel>
    <Button DockPanel.Dock="Left">Izquierda</Button>
    <Button DockPanel.Dock="Right">Derecha</Button>
</DockPanel>
```

Los **eventos adjuntos** usan la sintaxis `TipoPropietario.Evento`. La mayoría de eventos tunelados en WPF son eventos adjuntos

``` xml
<StackPanel UIElement.PreviewMouseDown="OnPreviewMouseDown">
    <TextBlock Text="Haz clic aquí" />
</StackPanel>
```

## Tipos base

En WPF, los elementos XAML representan clases del sistema CLR. No todas las clases pueden usarse directamente en XAML (por ejemplo, las clases abstractas como `ButtonBase` solo sirven como base para otras).

Sin embargo, estas clases base son importantes porque los elementos concretos heredan de ellas y así comparten propiedades y eventos comunes. Por ejemplo:

- **`FrameworkElement`** es la clase base más usada para controles visuales en la interfaz.  
- **`FrameworkContentElement`** es similar, pero pensada para contenido fluido como texto.
