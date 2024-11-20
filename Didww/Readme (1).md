## MiniProfiler for .NET (and .NET Core)

[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects)

Welcome to MiniProfiler for .NET, ASP.NET, ASP.NET Core, ASP.NET MVC and generally all the combinations of those words. Documentation for MiniProfiler for .NET is in `/docs`, accessible via GitHub pages at: [miniprofiler.com/dotnet](https://miniprofiler.com/dotnet/). General information for MiniProfiler across platforms can be found at [miniprofiler.com](https://miniprofiler.com/). It is part of the [.NET Foundation](https://www.dotnetfoundation.org/), and operates under their [code of conduct](https://www.dotnetfoundation.org/code-of-conduct).

[![AppVeyor Build Status](https://ci.appveyor.com/api/projects/status/sieyhfuhjww5ur5i/branch/main?svg=true)](https://ci.appveyor.com/project/StackExchange/dotnet/branch/main)
![Actions Build](https://github.com/MiniProfiler/dotnet/workflows/Main%20Build/badge.svg)


The current major version of MiniProfiler is v4.

#### Handy Links

* Documentation
  * [Getting started for ASP.NET (*not* .NET Core)](https://miniprofiler.com/dotnet/AspDotNet)
  * [Getting started for ASP.NET Core](https://miniprofiler.com/dotnet/AspDotNetCore)
  * [How-To Profile Code](https://miniprofiler.com/dotnet/HowTo/ProfileCode)
  * [NuGet Packages](https://miniprofiler.com/dotnet/NuGet)
  * [How-To Upgrade From MiniProfiler V3](https://miniprofiler.com/dotnet/HowTo/UpgradeFromV3)
* Samples
  * [ASP.NET Core Sample App](https://github.com/MiniProfiler/dotnet/tree/main/samples/Samples.AspNetCore3)
  * [ASP.NET CORE MVC 6 Sample App](https://github.com/MiniProfiler/dotnet/tree/main/samples/Samples.Mvc5)
  * [Console Application](https://github.com/MiniProfiler/dotnet/tree/main/samples/Samples.Console)

#### Building
To build the DIDww Api solution in Visual Studio, you'll need:
- Visual Studio 2022 17+ (or the .NET Core 6.x SDK)
- The [Web Compiler](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebCompiler) extension
  - Note: no extension is needed if building via `build.cmd` or `build.ps1` in the repository root. They pull it in via a package.

After a clone, running `build.cmd`. To create packages, use `build.cmd -CreatePackages $true` and it'll output them in the `.nukpgs\` folder.
If you get any issue in running project check out he package installation.
Below is the list of some packages which is used in the project.
#### Package Status

MyGet Pre-release feed: https://www.myget.org/gallery/miniprofiler
[Session](https://www.nuget.org/packages/Microsoft.AspNetCore.Session) 
[Session]

Packages which you need :

dotnet add package Swashbuckle.AspNetCore --version 5.6.3
dotnet add package Swashbuckle.AspNetCore.Annotations --version 5.6.3
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 3.1.0

#### Running
After startin Project you need to write the right url to access the api.
Fisrt of all API-URL : https://didww.talknsave.net/api/.
This is the basic url after this you need to write the 
	1. Controller name 
	2. Request name 
	3. Input Parameters (if any)
For Example :
             

Here 
1. https://didww.talknsave.net/api/ is basic URL
2. auth is controller name 
3. login is Request name/Function
4. username and password are parameters
1. 

After you get login you will get a token, This is the required step and can be missed.
As if you donot logged in then you don't have any access to any of the api. 
                                                                                                       