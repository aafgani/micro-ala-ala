using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace App.Api.Todo.Configuration;

public class ApplicationInformation
{
    private const string Banner = @"
    
    
 _____         _              ___  ______ _____ 
|_   _|       | |            / _ \ | ___ \_   _|
  | | ___   __| | ___ ______/ /_\ \| |_/ / | |  
  | |/ _ \ / _` |/ _ \______|  _  ||  __/  | |  
  | | (_) | (_| | (_) |     | | | || |    _| |_ 
  \_/\___/ \__,_|\___/      \_| |_/\_|    \___/ 

    ";

    public ApplicationInformation(IHostEnvironment environment)
    {
        var content = new StringBuilder();

        content.Append(Banner);

        var assembly = Assembly.GetExecutingAssembly();

        var date = string.IsNullOrWhiteSpace(assembly.Location)
            ? "-- unknown --"
            : File.GetLastWriteTime(assembly.Location).ToString("u");
        content.AppendLine($"Name: {assembly.GetName().Name} | Version: {assembly.GetName().Version} | Date: {date}");

        if (environment.IsDevelopment())
        {
            content.AppendLine($"Framework: {RuntimeInformation.FrameworkDescription}");
            content.AppendLine($"OS Platform: {RuntimeInformation.OSDescription}");
        }

        content.AppendLine($"Application Started On: {date}");

        Info = content.ToString();
    }

    public string Info { get; }
}
