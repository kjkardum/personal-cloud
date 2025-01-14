using AutoMapper;
using Xunit;

namespace Kjkardum.CloudyBack.Application.Tests.Mappings;

public class ProfileTests
{
    [Fact]
    public void Mapper_configuration_is_valid()
    {
        //Arrange
        var mapperConfiguration = new MapperConfiguration(
            config => config.AddMaps(typeof(IExplicitAssemblyReference).Assembly));

        // Act + Assert
        mapperConfiguration.AssertConfigurationIsValid();
    }
}
