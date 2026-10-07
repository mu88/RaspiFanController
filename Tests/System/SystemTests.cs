using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentAssertions;
using mu88.Shared.Testing.Docker;
using mu88.Shared.Testing.SystemTests;
using NUnit.Framework;

namespace Tests.System;

[Category("System")]
public class SystemTests : SystemTestsBase
{
    protected override string SubPath => "/cool";

    [Test]
    public async Task AppRunningInDocker_ShouldBeHealthy()
    {
        // Arrange
        var containerImageTag = DockerImageBuilder.GenerateContainerImageTag();
        await BuildDockerImageOfAppAsync(containerImageTag, CancellationToken);
        Container = await StartAppInContainerAsync(containerImageTag, CancellationToken);

        // Act & Assert
        await LogsShouldNotContainWarningsAsync(CancellationToken);
        await HealthCheckShouldSucceedAsync(CancellationToken);
        await AppShouldRunAsync(CancellationToken, "Raspi Fan Controller");
    }

    private static async Task BuildDockerImageOfAppAsync(string containerImageTag, CancellationToken cancellationToken)
    {
        var rootDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent ?? throw new NullReferenceException();
        var projectFile = Path.Join(rootDirectory.FullName, "RaspiFanController", "RaspiFanController.csproj");
        await DockerImageBuilder.BuildAsync(projectFile, containerImageTag, "raspifancontroller", rootDirectory.FullName, cancellationToken);
    }

    private static async Task<IContainer> StartAppInContainerAsync(string containerImageTag, CancellationToken cancellationToken)
    {
        var network = new NetworkBuilder().Build();
        await network.CreateAsync(cancellationToken);

        var container = new ContainerBuilder($"raspifancontroller:{containerImageTag}-chiseled")
            .WithNetwork(network)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development") // this enables the faked temperature and fan controller as we're not on a real Raspi
            .WithPortBinding(8080, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("Content root path: /app",
                    strategy => strategy.WithTimeout(TimeSpan.FromSeconds(30)))) // as it's a chiseled container, waiting for the port does not work
            .Build();
        await container.StartAsync(cancellationToken);
        return container;
    }
}