using System;
using System.Diagnostics;

namespace Launcher
{
    class Program
    {
        private const string path = @"..\..\..\..\CarsWebApp\bin\Release\netcoreapp3.1\publish\CarsWebApp.exe";

        static void Main(string[] args)
        {
            Process.Start(path);
            Process.Start("explorer", "https://localhost:5001/");
        }
    }
}
