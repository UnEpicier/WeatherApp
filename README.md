<h1 align="center">Weather App</h1>

# Sommaire

- [Prérequis](#prérequis)

# Prérequis

Afin de pouvoir lancer l'application, il faut dans un premier temps plusieurs choses:

- [Dotnet 6](https://dotnet.microsoft.com/en-us/download)
- Gtk
  Pour installer Gtk, on fait ces commandes :
  ```sh
  $ dotnet new --install GtkSharp.Template.CSharp
  # Si vous avez des erreurs:
  $ dotnet restore
  ```
- Newtonsoft
  Une librairie qui permet de manipuler des objets JSON en C#. Pour l'installer faites :
  ```sh
  $ dotnet add package Newtonsoft.Json
  ```

Lancez une première fois l'application est refermez-la, deux fichers ce sont créés, ouvrez le fichier `config.json`.
Mettez-le de côté et créez-vous une clé API sur [OpenWeatherMap](https://openweathermap.org/api).
Et enfin, mettez cette clé dans le fichier de cette façon :

```json
{
  "API_KEY": "<votre clé>"
}
```
