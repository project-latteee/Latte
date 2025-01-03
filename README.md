# Latte

Experimental, work-in-progress Wii U emulator written in C# for researching.
Don't expect this to work. We don't even know if it even will run a homebrew app.

## Running and downloading the emulator

You can download the latest version of the emulator from the [Releases](https://github.com/project-latteee/Latte/releases) page, or you can build it yourself. *currently there is no release, so you need to build it yourself*

To run the emulator, you need to have the [.NET Desktop Runtime 8.0.x](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.

After installing the runtime, you can run the emulator by running `Latte.exe` in the root directory of the repository.

## Building

Currently, the project can be built only on Windows. For building, you need to have:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/)
- [Git](https://git-scm.com/downloads)

After installing the dependencies, you can build the project by running `dotnet build` in the root directory of the repository or by opening the `Latte.sln` file in Visual Studio 2022.

## Issues & Bugs

If you find any issues or bugs, please report them in the [Issues](https://github.com/project-latteee/Latte/issues) page.

## Contributing

If you want to contribute, please read the [CONTRIBUTING.md](docs/CONTRIBUTING.md) file.

## License

This project is licensed under the GPLv3 or later license - see the [LICENSE](LICENSE.txt) file for details.
