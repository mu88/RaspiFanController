namespace RaspiFanController.Components;

/// <summary>
/// Code-behind for App.razor. This partial class ensures the App type is resolvable
/// even when Razor source generators are not available (e.g. during Stryker mutation testing).
/// </summary>
#pragma warning disable MA0206 // "class App;" would trigger StyleCop SA1106 (empty statement) - braces are required here.
public partial class App : Microsoft.AspNetCore.Components.ComponentBase { }
#pragma warning restore MA0206
