namespace Kjkardum.CloudyBack.Domain.Entities;

public class DockerEnvironment
{
    public List<DockerContainer> Containers { get; set; }
    public List<DockerImage> Images { get; set; }
    public List<DockerNetwork> Networks { get; set; }
    public List<DockerVolume> Volumes { get; set; }
}
public class DockerContainer
{
    public string ContainerId { get; set; }
    public string ContainerName { get; set; }
    public bool StateRunning { get; set; }
    public bool StatePaused { get; set; }
    public bool StateRestarting { get; set; }
    public string StateError { get; set; }
    public DateTime? StateStartedAt { get; set; }
    public DateTime? StateFinishedAt { get; set; }
    public List<string>? NetworkIds { get; set; }
    public List<string>? VolumeIds { get; set; }
}
public class DockerImage
{
    public string ImageId { get; set; }
    public string Tag { get; set; }
    public DateTime CreatedAt { get; set; }
    public long Size { get; set; }
}
public class DockerNetwork
{
    public string NetworkId { get; set; }
    public string Name { get; set; }
    public List<string> ContainerIds { get; set; }
}
public class DockerVolume
{
    public string Name { get; set; }
    public string CreatedAt { get; set; }
}
