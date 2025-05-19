namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

public class ContainerDto
{
    public bool StateRunning { get; set; }
    public bool StatePaused { get; set; }
    public bool StateRestarting { get; set; }
    public string StateError { get; set; }
    public DateTime StateStartedAt { get; set; }
    public DateTime StateFinishedAt { get; set; }

}
