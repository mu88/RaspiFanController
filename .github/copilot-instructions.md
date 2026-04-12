# RaspiFanController — Repo Context

## Architecture
- Single-project Blazor Server app. Production target is a Raspberry Pi (ARM); fan is controlled via GPIO pin 17.
- Path base: `/cool` — all URL assertions in tests must account for this prefix.

## NSubstitute and Internal Types
- `AppSettings` is `internal`. `InternalsVisibleTo("Tests")` is declared in the production `.csproj` — this allows Castle DynamicProxy (used by NSubstitute) to proxy `IOptions<AppSettings>`.
- Always mock `IRaspiTemperatureController` (the interface), never the concrete class. NSubstitute cannot proxy internal concrete types without an interface.

## Active Analyzer Rules (beyond the project defaults)
- **MA0076**: never write `$"{value:F1}"` or `$"{intVal}"` in interpolated strings for culture-sensitive output — use `string.Create(CultureInfo.CurrentCulture, $"...")` instead.
- **MA0042**: always call `await cts.CancelAsync()` instead of `cts.Cancel()` in both production code and tests.
