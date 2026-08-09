using System.Text.Json;
using BlazeCard.Models;
using BlazeCard.Models.Google;

namespace BlazeCard.Services;

public class GoogleWalletService : IGoogleWalletService
{
    public string GenerateCodeSnippet(CardModel model) =>
        GoogleCodeTemplate.Wrap(BuildGenericObject(model));

    public GenericObjectDto BuildGenericObject(CardModel model)
    {
        var lang = string.IsNullOrWhiteSpace(model.DefaultLanguage) ? "en-AU" : model.DefaultLanguage;
        var dto = new GenericObjectDto
        {
            Id = "{ISSUER_ID}.object.sample",
            ClassId = "{ISSUER_ID}.class.sample",
            GenericType = "GENERIC_TYPE_UNSPECIFIED",
            HexBackgroundColor = model.BackgroundColor,
            CardTitle = Localized(lang, model.OrganizationName)
        };

        var primary = model.PrimaryFields.FirstOrDefault();
        if (primary is not null)
        {
            dto.Subheader = Localized(lang, primary.Label);
            dto.Header = Localized(lang, primary.Value);
        }

        var modules = model.SecondaryFields
            .Concat(model.AuxiliaryFields)
            .Concat(model.BackFields)
            .Select(f => new TextModuleDataDto
            {
                Id = string.IsNullOrWhiteSpace(f.Key) ? Guid.NewGuid().ToString("N") : f.Key,
                Header = f.Label,
                Body = f.Value
            })
            .ToList();
        if (modules.Count > 0)
            dto.TextModulesData = modules;

        if (!string.IsNullOrEmpty(model.LogoImage))
            dto.Logo = PlaceholderImage("https://example.com/images/logo.png");
        if (!string.IsNullOrEmpty(model.HeroImage))
            dto.HeroImage = PlaceholderImage("https://example.com/images/hero.png");

        if (model.BarcodeFormat != BarcodeFormat.None)
        {
            dto.Barcode = new BarcodeDto
            {
                Type = MapBarcode(model.BarcodeFormat),
                Value = model.BarcodeMessage,
                AlternateText = string.IsNullOrEmpty(model.BarcodeAltText) ? null : model.BarcodeAltText
            };
        }

        return dto;
    }

    public bool TryParseCodeSnippet(string code, CardModel target, out string? warning)
    {
        warning = null;
        if (string.IsNullOrWhiteSpace(code))
        {
            warning = "Google code snippet is empty.";
            return false;
        }

        var jsonStart = code.IndexOf('{');
        var jsonEnd = code.LastIndexOf('}');
        if (jsonStart < 0 || jsonEnd <= jsonStart)
        {
            warning = "Could not find JSON body in Google snippet.";
            return false;
        }

        var json = code[jsonStart..(jsonEnd + 1)];
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("cardTitle", out var cardTitle))
                target.OrganizationName = ReadLocalized(cardTitle) ?? target.OrganizationName;

            if (root.TryGetProperty("hexBackgroundColor", out var bg) && bg.ValueKind == JsonValueKind.String)
                target.BackgroundColor = bg.GetString() ?? target.BackgroundColor;

            if (root.TryGetProperty("header", out var header) && root.TryGetProperty("subheader", out var sub))
            {
                var value = ReadLocalized(header);
                var label = ReadLocalized(sub);
                if (value is not null || label is not null)
                {
                    if (target.PrimaryFields.Count == 0)
                        target.PrimaryFields.Add(new PassField { Key = "primary" });
                    if (label is not null) target.PrimaryFields[0].Label = label;
                    if (value is not null) target.PrimaryFields[0].Value = value;
                }
            }

            return true;
        }
        catch (JsonException)
        {
            warning = "Could not parse Google JSON body.";
            return false;
        }
    }

    private static LocalizedStringDto Localized(string language, string value) => new()
    {
        DefaultValue = new TranslatedStringDto { Language = language, Value = value ?? string.Empty }
    };

    private static ImageDto PlaceholderImage(string uri) => new()
    {
        SourceUri = new ImageUriDto { Uri = uri }
    };

    private static string MapBarcode(BarcodeFormat format) => format switch
    {
        BarcodeFormat.QR => "QR_CODE",
        BarcodeFormat.PDF417 => "PDF_417",
        BarcodeFormat.Aztec => "AZTEC",
        BarcodeFormat.Code128 => "CODE_128",
        _ => "QR_CODE"
    };

    private static string? ReadLocalized(JsonElement element)
    {
        if (element.TryGetProperty("defaultValue", out var dv)
            && dv.TryGetProperty("value", out var v)
            && v.ValueKind == JsonValueKind.String)
            return v.GetString();
        return null;
    }
}
