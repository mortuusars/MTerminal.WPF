using System.Reflection;

namespace MTerminal.WPF.Utils;

internal class Version
{
    public static System.Version? GetVersion()
    {
        return Assembly.GetAssembly(typeof(Version))?.GetName().Version;
    }
}
