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
        var primary = model.PrimaryFields.FirstOrDefault();
        var headerValue = FirstNonEmpty(primary?.Value, model.LogoText, model.Description, "Generic pass");

        var dto = new GenericObjectDto
        {
            Id = "{ISSUER_ID}.object.sample",
            ClassId = "{ISSUER_ID}.class.sample",
            GenericType = string.IsNullOrWhiteSpace(model.GoogleGenericType)
                ? "GENERIC_TYPE_UNSPECIFIED"
                : model.GoogleGenericType,
            State = "ACTIVE",
            HexBackgroundColor = model.BackgroundColor,
            CardTitle = Localized(lang, model.OrganizationName),
            Header = Localized(lang, headerValue)
        };

        if (!string.IsNullOrWhiteSpace(primary?.Label))
            dto.Subheader = Localized(lang, primary.Label);

        var linkFields = model.BackFields.Where(f => IsLink(f.Value)).ToList();
        var textFields = model.SecondaryFields
            .Concat(model.AuxiliaryFields)
            .Concat(model.BackFields.Where(f => !IsLink(f.Value)))
            .ToList();

        var modules = textFields
            .Select(f => new TextModuleDataDto
            {
                Id = string.IsNullOrWhiteSpace(f.Key) ? Guid.NewGuid().ToString("N") : f.Key,
                Header = f.Label,
                Body = f.Value
            })
            .ToList();
        if (modules.Count > 0)
            dto.TextModulesData = modules;

        if (linkFields.Count > 0)
        {
            dto.LinksModuleData = new LinksModuleDataDto
            {
                Uris = linkFields.Select(f => new UriDto
                {
                    Id = string.IsNullOrWhiteSpace(f.Key) ? Guid.NewGuid().ToString("N") : f.Key,
                    Description = string.IsNullOrWhiteSpace(f.Label) ? f.Value : f.Label,
                    Uri = f.Value.Trim()
                }).ToList()
            };
        }

        if (model.GetPassbookPreview(Passbook.Generator.PassbookImage.Logo3X, Passbook.Generator.PassbookImage.Logo2X, Passbook.Generator.PassbookImage.Logo) is not null)
            dto.Logo = PlaceholderImage("https://example.com/images/logo.png");
        if (!string.IsNullOrEmpty(model.WideLogoImage))
            dto.WideLogo = PlaceholderImage("https://example.com/images/wide-logo.png");
        if (!string.IsNullOrEmpty(model.HeroImage))
            dto.HeroImage = PlaceholderImage("https://example.com/images/hero.png");
        if (!string.IsNullOrEmpty(model.ImageModuleImage))
        {
            dto.ImageModulesData =
            [
                new ImageModuleDataDto
                {
                    Id = "IMAGE_MODULE_ID",
                    MainImage = PlaceholderImage("https://example.com/images/module.png")
                }
            ];
        }

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

        var objectMarker = code.IndexOf("walletobjects/v1/genericObject", StringComparison.Ordinal);
        var searchFrom = objectMarker >= 0 ? objectMarker : 0;
        var jsonStart = code.IndexOf("\n{", searchFrom, StringComparison.Ordinal);
        if (jsonStart >= 0)
            jsonStart += 1;
        else
            jsonStart = code.IndexOf('{', searchFrom);
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

            if (root.TryGetProperty("genericType", out var gt) && gt.ValueKind == JsonValueKind.String)
            {
                var type = gt.GetString();
                if (!string.IsNullOrWhiteSpace(type))
                    target.GoogleGenericType = type;
            }

            if (root.TryGetProperty("header", out var header))
            {
                var value = ReadLocalized(header);
                var label = root.TryGetProperty("subheader", out var sub) ? ReadLocalized(sub) : null;
                if (value is not null || label is not null)
                {
                    if (target.PrimaryFields.Count == 0)
                        target.PrimaryFields.Add(new PassField { Key = "primary" });
                    if (label is not null) target.PrimaryFields[0].Label = label;
                    if (value is not null) target.PrimaryFields[0].Value = value;
                }
            }

            if (root.TryGetProperty("barcode", out var barcode) && barcode.ValueKind == JsonValueKind.Object)
            {
                if (barcode.TryGetProperty("type", out var bt) && bt.ValueKind == JsonValueKind.String)
                    target.BarcodeFormat = ParseBarcode(bt.GetString());
                if (barcode.TryGetProperty("value", out var bv) && bv.ValueKind == JsonValueKind.String)
                    target.BarcodeMessage = bv.GetString() ?? target.BarcodeMessage;
                if (barcode.TryGetProperty("alternateText", out var ba) && ba.ValueKind == JsonValueKind.String)
                    target.BarcodeAltText = ba.GetString() ?? target.BarcodeAltText;
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

    private static BarcodeFormat ParseBarcode(string? type) => type switch
    {
        "QR_CODE" => BarcodeFormat.QR,
        "PDF_417" => BarcodeFormat.PDF417,
        "AZTEC" => BarcodeFormat.Aztec,
        "CODE_128" => BarcodeFormat.Code128,
        _ => BarcodeFormat.QR
    };

    private static string? ReadLocalized(JsonElement element)
    {
        if (element.TryGetProperty("defaultValue", out var dv)
            && dv.TryGetProperty("value", out var v)
            && v.ValueKind == JsonValueKind.String)
            return v.GetString();
        return null;
    }

    private static string FirstNonEmpty(params string?[] values) =>
        values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)) ?? string.Empty;

    private static bool IsLink(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var v = value.Trim();
        return v.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || v.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || v.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)
            || v.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase);
    }
}
