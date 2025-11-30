Este programa está pensado para facilitar la portabilidad. Se consideró varias formas de integrar las diferentes capas para evitar tener que ejecutarlas una por una:
1. Usar [nssm](https://nssm.cc/download) para crear los servicios en windows, de modo que Backend y BD están esperando a que el usuario use la aplicación Frontend

```
nssm // Imprime instrucciones
nssm install <servicio> <Instrucción de ejecución> // Crea el servicio
nssm start <servicio> // Inicia el servicio
nssm stop // Detener servicio
nssm remove <servicio> // Borrar servicio, necesitamos detenerlo antes
```

2. Crear un programa de consola con C# que inicie cada parte por orden

A continuación se detallará como conseguir los ejecutables / servicios portables, pero no se detallará cómo crear estos complementos para su integración

## MySQL
Versión usada: MySql-8.4.6
Se recomienda ver [este foro](https://stackoverflow.com/questions/42045494/running-starting-mysql-without-installation-on-windows) para consultar cómo hacerlo portable

```
MySql-8.4.6\bin\mysqld --console --port=10200
```
## Backend - Java
Versión usada: 17.0.15

El proyecto cuenta con maven wrapper para poder exportarlo y ejecutarlo sin necesidad de tener maven

```
.\mvnw -v
.\mvnw clean package
```

Para ejecutar el backend, en caso de querer evitar que el dispositivo tenga que tener la versión de Java necesaria instalada, es necesario descargar el [Java SE Development Kit 17, 17.0.15](https://www.oracle.com/java/technologies/javase/17-0-15-relnotes.html#R17_0_15) y usarlo tal que:

```
ms-17.0.15\bin\java.exe -jar amproject-0.0.1-SNAPSHOT.jar 
```

## Frontend - WinUI3
Para obtener el ejecutable de el programa que actúa de frontend, es necesario tener Visual y seguir estos pasos:
1. En el proyecto, pulsar click derecho -> "Publish"
2. Crear un perfil con las siguientes características
	1. Target - Folder
	2. Specific Target - Folder
	3. Ubicación - Libre configuración
	4. Terminar
3. Configurar perfil (Algunas dependerán de las preferencias que queramos)
	1. Darle en el perfil a "Show all Settings"
	2. Configuration - Release x64
	3. Target framework - .NET 8
	4. Deployment mode - Self contained
	5. Target runtime - win-x64 
	6. Target Location - ///
	7. File publish options
		1. Produce Single File - Sí
		2. Enable ReadyToRun compilation - /// (Precompila una parte del programa para una ejecución rápida, pero hace que el programa sea más pesado)
		3. Trim unused code - /// (Elimina librerías, métodos y tipos que la aplicación no usa para reducir el peso final del ejecutable, no recomendable porque puede llegar a romper el programa)
Al terminar, dirá donde está el ejecutable, y ya estará listo para usar


#### AVISO SOBRE EL USO DE NSSM
Este programa guarda imágenes en el dispositivo en una carpeta fija del "Home" del usuario, pero el users-home por defecto está en System32:
```
C:\Windows\System32\config\systemprofile\ArchiManager
```

Para cambiarlo al usuario hay que usar powershell para obtener con `whoami` el usuario (El usuario necesita contraseña, y la necesitará configurar), y con
```
nssm edit AM_Backend
```
En "Log On", poner los datos para conectarse al usuario (Necesita privilegios de admin, o no irá). También se puede usar la consola para hacer:

```
nssm set AM_Backend ObjectName "jaimepc\usuario" "contraseña"  
```

> PROBAR QUE PUEDES CONECTARTE AL USUARIO-CONTRASEÑA

```
runas /user:usuario "cmd.exe"
```
