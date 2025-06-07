using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.ReverseProxy.Dtos;

public class CaddyConfigDto
{
    public Apps apps { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Logging? logging { get; set; }
}
public class Logging
{
    public Logs logs { get; set; }
}

public class Logs
{
    public DefaultLog @default { get; set; }
}

public class DefaultLog
{
    public string level { get; set; }
}
public class Apps
{
    public Http http { get; set; }
}

public class Handler
{
    public string handler { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<RouteInHandlerUpstream>? routes { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Upstream>? upstreams { get; set; }
}

public class Http
{
    public Dictionary<string, Srv> servers { get; set; }
}

public class Match
{
    public List<string> host { get; set; }
}

public class RouteMatch
{
    public List<Match> match { get; set; }
    public List<Handler> handle { get; set; }
    public bool terminal { get; set; }
}

public class RouteInHandlerUpstream
{
    public List<Handler> handle { get; set; }
}


public class Srv
{
    public List<string> listen { get; set; }
    public List<RouteMatch> routes { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AutomaticHttps? automatic_https { get; set; }
}

public class AutomaticHttps
{
    public List<string> skip { get; set; }
    public bool? disable { get; set; }
}

public class Upstream
{
    public string dial { get; set; }
}
