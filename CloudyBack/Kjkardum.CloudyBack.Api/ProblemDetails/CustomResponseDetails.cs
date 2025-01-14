using Microsoft.AspNetCore.Mvc;
using Kjkardum.CloudyBack.Api.ProblemDetails;

namespace Kjkardum.CloudyBack.Api.Extensions;

internal class CustomProblemDetailsResponse : Microsoft.AspNetCore.Mvc.ProblemDetails
{
    public ErrorInfoDto[] Errors { get; set; } = null!;
}
