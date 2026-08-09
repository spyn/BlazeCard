using BlazeCard;
using BlazeCard.Components;
using BlazeCard.Services;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddScoped<IBlazeCardStateService, BlazeCardStateService>();
builder.Services.AddScoped<IApplePassService, ApplePassService>();
builder.Services.AddScoped<IGoogleWalletService, GoogleWalletService>();
builder.Services.AddScoped<ISessionRestoreService, SessionRestoreService>();
builder.Services.AddScoped<ILocalStorage, JsLocalStorage>();

builder.Services.Configure<BlazeCardOptions>(
    builder.Configuration.GetSection("BlazeCard"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

app.UseAntiforgery();
app.UseStaticFiles();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", app = "BlazeCard" }));

app.Run();
