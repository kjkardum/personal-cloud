using System.Reflection;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;

public static class DockerLocalStorageHelper
{
    public static string? SavedOuterPersist { get; set; } = string.Empty;
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
            var pathWithoutFileName = Path.GetDirectoryName(path)
                ?? throw new InvalidOperationException("Could not determine the directory name.");
            if (!Directory.Exists(pathWithoutFileName))
            {
                Directory.CreateDirectory(pathWithoutFileName);
            }
            Console.WriteLine($"Transforming {filePath} to {path}");
            return path;
        }

        if (!filePath.Contains(currentDirectory))
        {
            var path = Path.Combine(persistFolder, filePath);
            var pathWithoutFileName = Path.GetDirectoryName(path)
                ?? throw new InvalidOperationException("Could not determine the directory name.");
            if (!Directory.Exists(pathWithoutFileName))
            {
                Directory.CreateDirectory(pathWithoutFileName);
            }
            return path;
        }
        else
        {
            return filePath;
        }
    }

    public static string CopyAndResolvePersistedPath(string filePath)
    {
        var copiedPath = FileCopyLocation(filePath);
        File.Copy(filePath, copiedPath, true);
        return ResolveOuterPersistedPath(copiedPath);
    }
    public static string ResolveOuterPersistedPath(string persistFilePath)
    {
        if (string.IsNullOrEmpty(SavedOuterPersist))
        {
            var outerPersist = Environment.GetEnvironmentVariable("CLOUDYBACK_OUTER_PERSIST_FOLDER");
            if (!string.IsNullOrEmpty(outerPersist))
            {
                SavedOuterPersist = outerPersist;
            }
        }
        if (string.IsNullOrEmpty(SavedOuterPersist))
        {
            return persistFilePath;
        }
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                               throw new InvalidOperationException("Could not determine the current directory.");
        var innerPersistFolder = Path.Combine(
            currentDirectory,
            "persist");
        return persistFilePath
            .Replace(innerPersistFolder, SavedOuterPersist);
    }
}
