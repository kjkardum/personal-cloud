using FluentValidation;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.Create;

public class CreateKafkaClusterCommandValidator: AbstractValidator<CreateKafkaClusterCommand>
{
    public CreateKafkaClusterCommandValidator()
    {
        RuleFor(x => x.ResourceGroupId).NotEmpty();
        RuleFor(x => x.ServerName)
            .Must(x => !x.Contains(' '))
            .WithMessage("Name cannot contain spaces")
            .Must(x => !"123456789".Contains(x[0]))
            .WithMessage("Name cannot start with a number")
            .MaximumLength(100);
    }
}
