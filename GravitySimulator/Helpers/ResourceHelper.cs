using System.Diagnostics;
using System.Reflection;

namespace Universe.Helpers;

internal static class ResourceHelper
{
  public static string ReadResourceAsText(string name)
  {
    var assembly = Assembly.GetExecutingAssembly();
    var resourceName = $"{nameof(Universe)}.{name}";

    try
    {
      using Stream stream = assembly.GetManifestResourceStream(resourceName);
      using StreamReader reader = new(stream);
      return reader.ReadToEnd();
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex.Message);
      throw;
    }
  }
}
