
using Xunit;
using AutoMapper;
using ApiJuros.Application.Mappings;

namespace ApiJuros.Test.Mappings
{
    public class MappingProfileTests
    {
        [Fact]
        public void Should_Have_Valid_Configuration()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
