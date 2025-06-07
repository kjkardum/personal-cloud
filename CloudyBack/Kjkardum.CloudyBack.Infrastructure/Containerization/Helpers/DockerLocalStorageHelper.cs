using System.Reflection;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;

public static class DockerLocalStorageHelper
{
    public static string FileCopyLocation(string filePath)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
            throw new InvalidOperationException("Could not determine the current directory.");
        var persistFolder = Path.Combine(
            currentDirectory,
            "persist");
        if (filePath.Contains(currentDirectory) && !filePath.Contains(persistFolder))
        {
            var path = filePath.Replace(currentDirectory, persistFolder);
            Console.WriteLine($"Transforming {filePath} to {persistFolder}");
            return path;
        }
        return !filePath.Contains(currentDirectory) ? Path.Combine(persistFolder, filePath) : filePath;
    }
}
