[Volver](Apuntes%20WinUI.md)

## XAML Preview

En WPF, se permitía previsualizar el código xaml de una página sin compilarlo. En WinUI, esto ya no es así. En su lugar tenemos que compilarlo, activar la página y en caso de que no aparezcan los cambios, pulsar el botón con forma de bola de fuego `Hot Reload` (Alt + F10).

Puedes ver en una pestaña el programa en ejecución en `Debug -> Windows -> XAML Live Preview`

## Importar controles de NuGet Package Manager

Recordar que para acceder al gestor de paquetes NuGet. `Tools`->`NuGet Package Manager`->`Manage NuGet Packages for Solution...`

Véase los ejemplos para su uso:

- **DataGrid**:
Necesita el paquete `CommunityToolkit.WinUI.UI.Controls`, ya que proviene de UWP.Controls en versiones anteriores
[Ver vídeo](https://www.youtube.com/watch?v=qFwaV8Zm1AQ)

``` xml
<Window
...
xmlns:controlsprimitives="using:CommunityToolkit.WinUI.UI.Controls.Primitives"
>
	<controls:DataGrid ...>
		...
	</controls:DataGrid>
</Window>
```

- **FlexGrid**:
[Ver vídeo](https://www.youtube.com/watch?v=qfn8G7C8jz0)

``` xml
<Window
...
xmlns:c1="using:c1.WinUI.Grid"
>
	<c1:FlexGrid ...>
		...
	</c1:FlexGrid>

<Window/>
```
## Recursos - Application

``` xml
<Application>
	<!-- Recursos globales -->
    <Application.Resources>
        <Color x:Key="PrimaryColor">#0078D4</Color>

        <Style TargetType="Button">
            <Setter Property="Background" Value="LightBlue"/>
        </Style>
    </Application.Resources>
</Application>
```

> `Resources` explica esto con más detalle en la WinUI Gallery
## Recursos organizados

Es una buena práctica, al tener muchos recursos, organizarlos con `ResourceDictionary`

#### ResourceDictionary personalizado

``` xml
<ResourceDictionary 
  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:system="clr-namespace:System;assembly=mscorlib">
  
  <Style TargetType="Button">
	  <Setter Property="Background" Value="Blue"/> 
  </Style>
  
</ResourceDictionary>
```
#### Application con ResourceDictionarys definidos

``` xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Styles/ButtonStyles.xaml"/>
            <ResourceDictionary Source="Colors/ColorPalette.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources> 
```

## Data/ControlTemplate & VisualStateManager & States

`DataTemplate` define cómo mostrar un dato en un control con contenido. Se usa al definir controles como `HeaderTemplates`

`ControlTemplate` define la apariencia para un tipo de control un árbol de elementos, de modo que se pueda acceder y customizar. Suele usarse `Grid` para acceder a cada elemento.

`VisualStateManager` maneja tanto los estados visuales como sus lógicas de transiciones. Da soporte a `VisualStateGroups`, que permiten definir los estados visuales en el XAML para el `ControlTemplate`

Véase ejemplos en la documentación oficial. Contienen el mismo ejemplo:
- [DataTemplate](https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.datatemplate?view=winrt-26100)
- [ControlTemplate Class](https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.controltemplate?view=winrt-26100)
- [VisualStateManager](https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.visualstatemanager?view=winrt-26100)

`VisualStateManager` también es usado al definir estilos accediendo a `Style.VisualStateManager.VisualStateGroups`

Muchos controles cuentan con estados dependiendo de la interacción con el usuario. Estos no se configuran con propiedades simples; dependen del **`VisualState`**:
- **Normal** -> bajo ninguna interacción.
- **PointerOver** -> cuando el mouse está encima.
- **Focused** -> cuando el control tiene foco.
- **ReadOnly** -> cuando es solo lectura.
- **Disabled** -> cuando está deshabilitado.

> En el siguiente apartado se explica cómo configurarlo
## Styles & ThemeResources

Existe una carpeta que contiene un `generic.xaml` donde se encuentran todos los estilos default usados en .NET. En la siguiente página se explica esto y el como usarlo con un ejemplo de un `button`. También se explica cómo cambiar los estados de un control.
[Página](https://nicksnettravels.builttoroam.com/default-winui-styles/)

>Hay otra forma para lograr esto, por ejemplo para un TextBox (Necesitas afectar a TextControl). De esta forma redefines el `generic.xaml`.

``` xml
<SolidColorBrush x:Key="TextControlBackground" Color="LightGray"/>
<SolidColorBrush x:Key="TextControlBackgroundPointerOver" Color="LightYellow"/>
<SolidColorBrush x:Key="TextControlBackgroundFocused" Color="LightBlue"/>
<SolidColorBrush x:Key="TextControlBackgroundReadOnly" Color="Gainsboro"/>
<SolidColorBrush x:Key="TextControlBackgroundDisabled" Color="Blue"/>

<Style>
	<Setter Property="Background" Value="{ThemeResource TextControlBackground}"/>
</Style>
```

## Controles

- `x - Atributos generales útiles`

``` xml
<X
	Margin="0,10,20,30"  // Separación exterior
	Padding="0,10,20,30" // Separación interior 
	// 0 izquierda, 10 arriba, 20 derecha, 30 abajo (px)
	Margin="10"
	Padding="10"
	// 10 en todos lados
	
	
	MinWidth="100"
	Width="200"
	MaxWidth="400"
	
	MinHeight="100"
	Height="200"
	MaxHeight="400"

	HorizontalAligment="Center"
	VerticalAligment="Center"
	
	Background="White"
	Foreground="Red"  // Color de la letra de texto
/>
```

- `Grid`: Permite definir una cuadrícula donde insertar elementos
- `StackPanel`: Sirve para organizar un espacio donde insertar controles

``` xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="1*"/>
        <ColumnDefinition Width="2.5*"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
	<Grid.RowDefinitions>
        <RowDefinition Width="1*"/>
        <RowDefinition Width="2.5*"/>
        <RowDefinition Width="*"/>
    </Grid.RowDefinitions>

	<StackPanel Grid.Column="1" Grid.Row="1">
		<Button Content="Púlsame"/>
	</StackPanel>

</Grid>
```

``` xml
<StackPanel
	Orientation="Horizontal"
/>
```

- `Border`: Permite marcar el borde de un contorno
``` xml
<Border 
BorderBrush="Black" 
BorderThickness="0,0,0,1"
/>
```
- `TextBlock`

``` xml
<TextBlock 
	Text="Contenido de texto"
	TextAlignment="Center"
	IsTextSelectionEnabled="True"  // Permite la selección de texto
	SelectionHighlightColor="Green"  // Color al resaltar texto
	Foreground="Blue" // Color de la letra
	FontWeight="Light" // Light, Normal, Bold...
	FontFamily="Arial" 
	FontStyle="Italic"
	FontSize=20
	CharacterSpacing="200"
	TextWrapping="Wrap" // Wrap, NoWrap
	MaxLines="3"
    TextAlignment="Left" // Left, Center, Right, Justify)
    LineHeight="20"
/>
```

- `Image`

``` xml
<Image 
Source="/Assets/Logo/Imagen.png"
Stretch="Uniform"  
	// Default. Controla el espacio vacío. Uniform/UniformToFill/Fill
/>
```

> Si la imagen no carga y la ruta es correcta, posiblemente necesites darle click derecho, `Properties`, y en el menú activar en `Build Action` la opción `Content` (Es necesario recompilar)

- `TextBox`

``` xml
<TextBox
	Text="Contenido de texto"  // Texto por defecto al cargar
		// Todas las propiedades de estilo al texto solo se aplican
		// al texto que esté dentro sin seleccionar el TBOX
	Margin="0,5,0,5"
	PlaceholderText="Nombre"
	IsReadOnly="True"  // No permite modificar su interior
	ScrollViewer.VerticalScrollBarVisibility="Auto"  // Scroller
	AcceptsReturn="True" // Saltos de línea al pulsar ENTER (\n)
	TextWrapping="Wrap"
	Header="Introduce el nombre:"
/>

// DEFINIR HEADER
// #1 Definiendo una plantilla
<TextBox>
    <TextBox.HeaderTemplate>
        <DataTemplate>
            <TextBlock 
                Text="{Binding}" 
                Foreground="Blue" 
                FontSize="18" 
                FontWeight="Bold"/>
        </DataTemplate>
    </TextBox.HeaderTemplate>
    <TextBox.Header>Nombre</TextBox.Header>
</TextBox>
// #2 Insertando el control directamente
<TextBox>
    <TextBox.Header>
        <TextBlock Text="Nombre" 
                   Foreground="DarkGreen" 
                   FontSize="18" 
                   FontStyle="Italic"/>
    </TextBox.Header>
</TextBox>
// #3 Definiendo una plantilla como estilo
<Style TargetType="TextBox">
    <Setter Property="HeaderTemplate">
        <Setter.Value>
            <DataTemplate>
                <TextBlock Text="{Binding}" Foreground="Red" FontSize="16"/>
                <!-- Es necesario poner Text Binding para sobreescribirlo -->
            </DataTemplate>
        </Setter.Value>
    </Setter>
</Style>

```

> Para saber cómo usar leer y/o modificar el texto del control, ver el [[4_CS_WinUI#Binding Text| apartado 4 sección "Binding Text" ]]
> 
- `DataGrid`:
No es nativo de WinUI, está desarrollado por la Windows Community Toolkit (Proviene de la UWP). Para su importación, [[3_XAML_WinUI#Importar controles & NuGet Package Manager | ver el apartado correspondiente]]

``` xml
<controls:DataGrid
	VerticalAlignment="Stretch"
	// El control se expande verticalmente para ocupar todo el espacio disponible
	HorizontalAlignment="Stretch"
	
	HorizontalScrollBarVisibility="Visible"
	VerticalScrollBarVisibility="Visible"
	AutoGenerateColumns="False"  // Evita que se creen columnas automáticamente
	CanUserReorderColumns="True"
	CanUserResizeColumns="True"
	IsReadOnly="True"
	HeadersVisibilty="Column"  // Muestra encabezados de columna, no filas
	SelectionMode="Extended"  // Permite seleccionar varias celdas
	GridLinesVisibilty="Vertical"  // Líneas divisoras solo en vertical
	ItemsSource="{x:Bind DataGridCustomerCollection}"
	Sorting="DataGrid_Sorting"  // Evento que se dispara al ordenar columnas
>
	<controls:DataGrid.Columns>
	
	<!-- Columna de texto -->
	<controls:DataGridTextColumn Header="Order ID" Binding="{Binding OrderID}" IsReadOnIy="true" Tag="Orders"/>
	
	<!-- Columna para imágenes -->
	<controls:DataGridTemplateColumn Header="Avatar">
	<controls:DataGridTempIateColumn.CellTemplate>
		<DataTemplate>
		<Image Source="{Binding Avatar}" Width="50" Height="50" Stretch="Uniform"/>
		</DataTempIate>
	</controls:DataGridTempIateColumn.CellTemplate>
	</controls:DataGridTemplateColumn>
	
	</controls:DataGrid.Columns>
</controls:DataGrid>
```

- `Button`:
En WinUI3, el estilo por defecto entra con conflicto con controles con los que se combinan. Para imágenes, necesitas establecer el `Background` transparente o del mismo color que el `StackPanel` donde esté.

``` xml
<Button 
Command="{Binding GoToAddClientCommand}" 
Background="LightGray">
	<Image Source="/Assets/Icons/ejemplo.png"/>
</Button>
```