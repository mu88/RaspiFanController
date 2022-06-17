# Raspberry Pi Fan Controller
![CI](https://github.com/mu88/RaspiFanController/workflows/CI/badge.svg)
![Release](https://github.com/mu88/RaspiFanController/workflows/Release/badge.svg)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/b8f48ccf13bb4b2089c379bf90a11a40)](https://www.codacy.com/gh/mu88/RaspiFanController/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=mu88/RaspiFanController&amp;utm_campaign=Badge_Grade)
[![Codacy Badge](https://app.codacy.com/project/badge/Coverage/b8f48ccf13bb4b2089c379bf90a11a40)](https://www.codacy.com/gh/mu88/RaspiFanController/dashboard?utm_source=github.com&utm_medium=referral&utm_content=mu88/RaspiFanController&utm_campaign=Badge_Coverage)

This repo contains an application to control the fan of a Raspberry Pi in order to avoid overheating. It is based on ASP.NET Core Blazor Server and uses the [.NET Core IoT Library](https://github.com/dotnet/iot) to access the GPIO pins of the Raspberry Pi.

<img src="https://mu88.github.io/public/post_assets/200424_Raspi_Fan_Controller/Image1.jpg" width="350" />

I wrote the following blog posts describing the complete ceremony a bit more in detail:
*   [Is .NET Core cool enough to cool a Raspberry Pi? - Part 1](https://mu88.github.io/2020/04/24/Raspi-Fan-Controller_p1)
*   [Is .NET Core cool enough to cool a Raspberry Pi? - Part 2](https://mu88.github.io/2020/04/24/Raspi-Fan-Controller_p1)

## Local development
The app targets .NET 6. I've integrated the two classes `Logic\DevFanController.cs` and `Logic\DevTemperatureProvider.cs` for development purposes: they're simulating the temperature measurement and fan controlling when running the app in development mode.

## Deployment
The app is deployed both as a [self-contained executable](https://docs.microsoft.com/en-us/dotnet/core/deploying/#publish-self-contained) and the Docker image [`mu88/raspifancontroller`](https://hub.docker.com/repository/docker/mu88/raspifancontroller).

### Self-contained executable
Use the following command to generate the app:
```shell
dotnet publish -r linux-arm64 -c Release /p:PublishSingleFile=true --self-contained
```

The following command copies the build results:
```shell
scp -r E:\Development\GitHub\RaspiFanController\RaspiFanController\bin\Release\net6.0\linux-arm64\publish user@yourRaspi:/tmp/RaspiFanController/
```

On the Raspberry, we have to allow the app to be executed:
```shell
chmod 777 /tmp/RaspiFanController/RaspiFanController
```

And finally, start the app using `sudo`. This is important because otherwise, reading the temperature doesn't work.
```shell
sudo /tmp/RaspiFanController/RaspiFanController
```

### Docker
You can either grab the prepared [`docker-compose.yml`](/RaspiFanController/docker-compose.yml) or start a new container with the following command:
```shell
docker run -p 5000:80 -d -v /lib:/lib -v /usr/bin/vcgencmd:/usr/bin/vcgencmd -e LD_LIBRARY_PATH=/opt/vc/lib --device /dev/vchiq --device /dev/gpiomem --restart always --name raspifancontroller mu88/raspifancontroller:latest
```
This will do the following:
- Mount the necessary directories and executables into the container so that `vcgencmd` works properly.
- Set the environment variable `LD_LIBRARY_PATH` so that `vcgencmd` works properly.
- Mount the necessary devices so that `vcgencmd` works properly and the Raspberry's GPIO pins can be controlles from within the container.

## App configuration
Within `appsettings.json`, the following app parameters can be controlled:

*   `RefreshMilliseconds` → The polling interval of the temperature controller. The shorter, the more often the current temperature will be retrieved from the OS.
*   `UpperTemperatureThreshold` → When this temperature is exceeded, the fan will be turned on.
*   `LowerTemperatureThreshold` → When this temperature is undershot, the fan will be turned off.
*   `GpioPin` → The GPIO pin that will control the transistor and therefore turning the fan on/off.
*   `AppPathBase` → This path will be used as the app's path base, see [`UsePathBase()`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.usepathbaseextensions.usepathbase?f1url=https%3A%2F%2Fmsdn.microsoft.com%2Fquery%2Fdev16.query%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(Microsoft.AspNetCore.Builder.UsePathBaseExtensions.UsePathBase);k(DevLang-csharp)%26rd%3Dtrue%26f%3D255%26MSPPError%3D-2147217396&view=aspnetcore-3.1).

These parameters are read on app startup. When the app is running, they can be overridden via http://localhost:5000/cool, but they won't be written to `appsettings.json`.

## Supported Platforms
The app is running on my Raspberry Pi 4 Model B using Raspberry Pi OS x64.

Theoretically, the code can be used on any IoT device and OS that provides its own temperature. For this, the `ITemperatureProvider` interface has to be implemented and registered within the DI container.

If you're interested in adding support for another device and/or OS, please file an issue. I'm curious whether it will work on other devices as well!