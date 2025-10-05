# Raspberry Pi Fan Controller

![Combined CI / Release](https://github.com/mu88/RaspiFanController/actions/workflows/CI_CD.yml/badge.svg)
![Mutation testing](https://github.com/mu88/RaspiFanController/actions/workflows/Mutation%20Testing.yml/badge.svg)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=mu88_RaspiFanController&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=mu88_RaspiFanController)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=mu88_RaspiFanController&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=mu88_RaspiFanController)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=mu88_RaspiFanController&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=mu88_RaspiFanController)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=mu88_RaspiFanController&metric=bugs)](https://sonarcloud.io/summary/new_code?id=mu88_RaspiFanController)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=mu88_RaspiFanController&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=mu88_RaspiFanController)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=mu88_RaspiFanController&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=mu88_RaspiFanController)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=mu88_RaspiFanController&metric=coverage)](https://sonarcloud.io/summary/new_code?id=mu88_RaspiFanController)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Fmu88%2FRaspiFanController%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/mu88/RaspiFanController/main)

This repo contains an application to control the fan of a Raspberry Pi in order to avoid overheating. It is based on ASP.NET Core Blazor Server and uses the [.NET Core IoT Library](https://github.com/dotnet/iot) to access the GPIO pins of the Raspberry Pi.

<img src="https://mu88.github.io/public/post_assets/2020-04-24-Raspi-Fan-Controller/Image1.jpg" width="350" />

I wrote the following blog posts describing the complete ceremony a bit more in detail:
*   [Is .NET Core cool enough to cool a Raspberry Pi? - Part 1](https://mu88.github.io/2020/04/24/Raspi-Fan-Controller_p1)
*   [Is .NET Core cool enough to cool a Raspberry Pi? - Part 2](https://mu88.github.io/2020/04/24/Raspi-Fan-Controller_p1)

## Local development
I've integrated the two classes `Logic\DevFanController.cs` and `Logic\DevTemperatureProvider.cs` for development purposes: they're simulating the temperature measurement and fan controlling when running the app in development mode.

## Deployment
The app is deployed both as a [self-contained executable](https://docs.microsoft.com/en-us/dotnet/core/deploying/#publish-self-contained) and the Docker image [`mu88/raspifancontroller`](https://github.com/mu88/RaspiFanController/pkgs/container/raspifancontroller).

### Self-contained executable
Use the following command to generate the app:
```shell
dotnet publish -r linux-arm64 /p:PublishSingleFile=true --self-contained
```

The following command copies the build results:
```shell
scp -r E:\Development\GitHub\RaspiFanController\RaspiFanController\bin\Release\net*.0\linux-arm64\publish user@yourRaspi:/tmp/RaspiFanController/
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
docker run -p 8080:8080 -d -v /sys/class/thermal/thermal_zone0:/sys/class/thermal/thermal_zone0:ro --device /dev/gpiomem --restart always --name raspifancontroller mu88/raspifancontroller:latest
```
This will do the following:
*   Mount the necessary directory so that the file containing the current temperature can be accessed by the .NET IoT library. 
*   Mount the necessary device so that the Raspberry's GPIO pins can be controlled from within the container.

## App configuration
Within `appsettings.json`, the following app parameters can be controlled:

*   `RefreshMilliseconds` → The polling interval of the temperature controller. The shorter, the more often the current temperature will be retrieved from the OS.
*   `UpperTemperatureThreshold` → When this temperature is exceeded, the fan will be turned on.
*   `LowerTemperatureThreshold` → When this temperature is undershot, the fan will be turned off.
*   `GpioPin` → The GPIO pin that will control the transistor and therefore turning the fan on/off.
*   `AppPathBase` → This path will be used as the app's path base, see [`UsePathBase()`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.usepathbaseextensions.usepathbase?f1url=https%3A%2F%2Fmsdn.microsoft.com%2Fquery%2Fdev16.query%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(Microsoft.AspNetCore.Builder.UsePathBaseExtensions.UsePathBase);k(DevLang-csharp)%26rd%3Dtrue%26f%3D255%26MSPPError%3D-2147217396&view=aspnetcore-3.1).

These parameters are read on app startup. When the app is running, they can be overridden via http://localhost:8080/cool, but they won't be written to `appsettings.json`.

## Supported Platforms
The app is running on my Raspberry Pi 4 Model B using Raspberry Pi OS x64.

Theoretically, the code can be used on any IoT device and OS that provides its own temperature. For this, the `ITemperatureProvider` interface has to be implemented and registered within the DI container.

If you're interested in adding support for another device and/or OS, please file an issue. I'm curious whether it will work on other devices as well!
