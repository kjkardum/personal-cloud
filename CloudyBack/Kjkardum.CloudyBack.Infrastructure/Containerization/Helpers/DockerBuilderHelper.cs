using ICSharpCode.SharpZipLib.Tar;
using System.Reflection;
using System.Text;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;

public static class DockerBuilderHelper
{
    public static Stream GetDockerfileContext(string DockerfilePath)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
            throw new InvalidOperationException("Could not determine the current directory.");
        var dockerFile = Path.Combine(
            currentDirectory,
            "Containerization/Clients",
            DockerfilePath);

        var tarball = new MemoryStream();
        using var archive = new TarOutputStream(tarball, Encoding.UTF8)
        {
            IsStreamOwner = false
        };
        var entry = TarEntry.CreateTarEntry(dockerFile);
        using var fileStream = File.OpenRead(dockerFile);
        entry.Size = fileStream.Length;
        entry.Name = Path.GetFileName(dockerFile);
        archive.PutNextEntry(entry);
        var localBuffer = new byte[32 * 1024];
        while (true)
        {
            var numRead = fileStream.Read(localBuffer, 0, localBuffer.Length);
            if (numRead <= 0)
            {
                break;
            }

            archive.Write(localBuffer, 0, numRead);
        }
        archive.CloseEntry();
        archive.Close();
        tarball.Position = 0;
        return tarball;
    }
}
