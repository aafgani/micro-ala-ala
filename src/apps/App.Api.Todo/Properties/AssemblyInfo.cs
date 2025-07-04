using System.Reflection;

[assembly: AssemblyVersion(ThisAssembly.Version)]
[assembly: AssemblyFileVersion(ThisAssembly.Version)]
[assembly: AssemblyInformationalVersion(ThisAssembly.Version)]

internal static class ThisAssembly
{
    public const string Version = "1.0.0"; // ← will be replaced by MSBuild
}
