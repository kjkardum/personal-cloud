using FluentValidation;

namespace Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Queries.PreCheckDns;

public class PreCheckDnsQueryValidator: AbstractValidator<PreCheckDnsQuery>
{
    public PreCheckDnsQueryValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("Url is required.");

        RuleFor(x => x.AdminUrl)
            .NotEmpty()
            .WithMessage("Your AdminUrl is required.");
    }
}
