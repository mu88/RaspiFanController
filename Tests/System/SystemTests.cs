using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.System;

[Category("System")]
public class SystemTests
{
    [Test]
    public async Task AppRunningInDocker_ShouldBeHealthy()
    {
        // Arrange
        var containerImageTag = GenerateContainerImageTag();
        var cancellationToken = CreateCancellationToken(TimeSpan.FromMinutes(1));
        await BuildDockerImageOfAppAsync(containerImageTag, cancellationToken);
        var container = await StartAppInContainersAsync(containerImageTag, cancellationToken);
        var httpClient = new HttpClient { BaseAddress = GetAppBaseAddress(container) };

        // Act
        var healthCheckResponse = await httpClient.GetAsync("healthz", cancellationToken);
        var appResponse = await httpClient.GetAsync("/", cancellationToken);
        var healthCheckToolResult = await container.ExecAsync(["dotnet", "/app/mu88.HealthCheck.dll", "http://localhost:8080/cool/healthz"], cancellationToken);

        // Assert
        await LogsShouldNotContainWarningsAsync(container, cancellationToken);
        await HealthCheckShouldBeHealthyAsync(healthCheckResponse, cancellationToken);
        await AppShouldRunAsync(appResponse, cancellationToken);
        healthCheckToolResult.ExitCode.Should().Be(0);
    }

    private static CancellationToken CreateCancellationToken(TimeSpan timeout)
    {
        var timeoutCts = new CancellationTokenSource();
        timeoutCts.CancelAfter(timeout);
        var cancellationToken = timeoutCts.Token;

        return cancellationToken;
    }

    private static async Task BuildDockerImageOfAppAsync(string containerImageTag, CancellationToken cancellationToken)
    {
        var rootDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent ?? throw new NullReferenceException();
        var projectFile = Path.Join(rootDirectory.FullName, "RaspiFanController", "RaspiFanController.csproj");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments =
                    $"publish {projectFile} --os linux --arch amd64 " +
                    $"/t:PublishContainersForMultipleFamilies " +
                    $"-p:ReleaseVersion={containerImageTag} " +
                    "-p:IsRelease=false " +
                    "-p:ContainerRegistry=\"\" " + // image shall not be pushed
                    "-p:ContainerRepository=\"me/raspifancontroller\" ",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        while (await process.StandardOutput.ReadLineAsync(cancellationToken) is { } line)
        {
            Console.WriteLine(line);
        }

        await process.WaitForExitAsync(cancellationToken);
        process.ExitCode.Should().Be(0);
    }

    private static async Task<IContainer> StartAppInContainersAsync(string containerImageTag, CancellationToken cancellationToken)
    {
        Console.WriteLine("Building and starting network");
        var network = new NetworkBuilder().Build();
        await network.CreateAsync(cancellationToken);
        Console.WriteLine("Network started");

        Console.WriteLine("Building and starting app container");
        var container = BuildAppContainer(network, containerImageTag);
        await container.StartAsync(cancellationToken);
        Console.WriteLine("App container started");

        return container;
    }

    private static IContainer BuildAppContainer(INetwork network, string containerImageTag) =>
        new ContainerBuilder()
            .WithImage($"me/raspifancontroller:{containerImageTag}-chiseled")
            .WithNetwork(network)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development") // this enables the faked temperature and fan controller as we're not on a real Raspi
            .WithPortBinding(8080, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                                  .UntilMessageIsLogged("Content root path: /app",
                                      strategy => strategy.WithTimeout(TimeSpan.FromSeconds(30)))) // as it's a chiseled container, waiting for the port does not work
            .Build();

    private static Uri GetAppBaseAddress(IContainer container) => new($"http://{container.Hostname}:{container.GetMappedPublicPort(8080)}/cool");

    private static async Task AppShouldRunAsync(HttpResponseMessage appResponse, CancellationToken cancellationToken)
    {
        appResponse.Should().Be200Ok();
        (await appResponse.Content.ReadAsStringAsync(cancellationToken)).Should().Contain("<title>Raspi Fan Controller</title>");
    }

    private static async Task HealthCheckShouldBeHealthyAsync(HttpResponseMessage healthCheckResponse, CancellationToken cancellationToken)
    {
        healthCheckResponse.Should().Be200Ok();
        (await healthCheckResponse.Content.ReadAsStringAsync(cancellationToken)).Should().Be("Healthy");
    }

    private static async Task LogsShouldNotContainWarningsAsync(IContainer container, CancellationToken cancellationToken)
    {
        (string Stdout, string Stderr) logValues = await container.GetLogsAsync(ct: cancellationToken);
        Console.WriteLine($"Stderr:{Environment.NewLine}{logValues.Stderr}");
        Console.WriteLine($"Stdout:{Environment.NewLine}{logValues.Stdout}");
        logValues.Stdout.Should().NotContain("warn:");
    }

    [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "Okay for me")]
    private static string GenerateContainerImageTag() => $"system-test-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
}