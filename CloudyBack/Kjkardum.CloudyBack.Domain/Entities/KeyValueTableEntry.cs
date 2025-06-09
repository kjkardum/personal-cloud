namespace Kjkardum.CloudyBack.Domain.Entities;

public class KeyValueTableEntry
{
    public const string GrafanaPublicConnection = "GrafanaPublicConnection";
    public const string PublicHttpsAccessConnection = "PublicHttpsAccessConnection";
    public string Key { get; set; }
    public string Value { get; set; }
}
