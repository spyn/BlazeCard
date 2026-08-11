using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using BlazeCard.Models;
using Microsoft.Extensions.Options;
using Passbook.Generator;
using Passbook.Generator.Fields;

namespace BlazeCard.Services;

public class PkPassService : IPkPassService
{
    private readonly PkPassOptions _options;
    private readonly IWebHostEnvironment _env;

    public PkPassService(IOptions<BlazeCardOptions> options, IWebHostEnvironment env)
    {
        _options = options.Value.PkPass;
        _env = env;
    }

    public bool IsConfigured =>
        _options.Enabled && (_options.UsePreviewCertificates || HasSigningCertificates());

    public string? ConfigurationHint
    {
        get
        {
            if (!_options.Enabled)
                return "Set BlazeCard:PkPass:Enabled=true to enable Final packaging.";
            if (_options.UsePreviewCertificates)
                return null;
            var missing = new List<string>();
            if (!File.Exists(ResolvePath(_options.PassCertificatePath)))
                missing.Add($"Pass certificate not found: {_options.PassCertificatePath}");
            if (!File.Exists(ResolvePath(_options.AppleWwdrCertificatePath)))
                missing.Add($"Apple WWDR certificate not found: {_options.AppleWwdrCertificatePath}");
            return missing.Count == 0 ? null : string.Join(" ", missing);
        }
    }

    public PassGeneratorRequestPreview BuildRequestPreview(CardModel model)
    {
        var missing = new List<string>();
        if (!_options.Enabled)
            missing.Add("PkPass packaging is disabled.");
        if (string.IsNullOrWhiteSpace(model.Description))
            missing.Add("Description is required.");
        if (string.IsNullOrWhiteSpace(model.OrganizationName))
            missing.Add("Organization Name is required.");
        if (!HasAnyIcon(model))
            missing.Add("At least one Icon image (icon.png / @2x / @3x) is required for a .pkpass package.");
        if (!_options.UsePreviewCertificates && !HasSigningCertificates())
            missing.Add(ConfigurationHint ?? "Signing certificates are not configured.");

        return new PassGeneratorRequestPreview
        {
            PassTypeIdentifier = _options.PassTypeIdentifier,
            TeamIdentifier = _options.TeamIdentifier,
            Description = model.Description,
            OrganizationName = model.OrganizationName,
            ImageCount = model.PassbookImages.Count,
            HasIcon = HasAnyIcon(model),
            MissingRequirements = missing
        };
    }

    public PkPassGenerateResult Generate(CardModel model)
    {
        var preview = BuildRequestPreview(model);
        if (preview.MissingRequirements.Count > 0)
            return PkPassGenerateResult.Fail(string.Join(" ", preview.MissingRequirements));

        try
        {
            if (_options.UsePreviewCertificates)
                return BuildPreviewPackage(model);

            var request = BuildSignedRequest(model);
            var bytes = new PassGenerator().Generate(request);
            return SuccessResult(model, bytes);
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException?.Message ?? ex.Message;
            return PkPassGenerateResult.Fail($"Pass packaging failed: {detail}");
        }
    }

    /// <summary>
    /// Builds a .pkpass-shaped zip (pass.json, images, manifest, preview signature marker)
    /// without Apple CMS signing — for Final-tab inspection only.
    /// </summary>
    private PkPassGenerateResult BuildPreviewPackage(CardModel model)
    {
        var request = BuildRequestCore(model);
        var passJson = WritePassJson(request);
        var images = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, dataUri) in model.PassbookImages)
        {
            var bytes = DecodeDataUri(dataUri);
            if (bytes is { Length: > 0 })
                images[key.ToFilename()] = bytes;
        }

        var manifest = BuildManifestJson(passJson, images);
        var signature = Encoding.UTF8.GetBytes(
            "BlazeCard preview package — not signed for Apple Wallet. Set UsePreviewCertificates=false and supply real certs to sign.");

        using var zipMs = new MemoryStream();
        using (var archive = new ZipArchive(zipMs, ZipArchiveMode.Create, true))
        {
            WriteZipEntry(archive, "pass.json", passJson);
            WriteZipEntry(archive, "manifest.json", manifest);
            WriteZipEntry(archive, "signature", signature);
            foreach (var (name, bytes) in images)
                WriteZipEntry(archive, name, bytes);
        }

        return SuccessResult(model, zipMs.ToArray());
    }

    private PassGeneratorRequest BuildSignedRequest(CardModel model)
    {
        var request = BuildRequestCore(model);
        request.PassbookCertificate = LoadPassCertificate();
        request.AppleWWDRCACertificate = LoadWwdrCertificate();
        return request;
    }

    internal PassGeneratorRequest BuildRequestCore(CardModel model)
    {
        var request = new PassGeneratorRequest
        {
            PassTypeIdentifier = _options.PassTypeIdentifier,
            TeamIdentifier = _options.TeamIdentifier,
            SerialNumber = Guid.NewGuid().ToString("N"),
            Description = model.Description,
            OrganizationName = model.OrganizationName,
            LogoText = model.LogoText,
            Style = PassStyle.Generic,
            BackgroundColor = model.BackgroundColor,
            LabelColor = model.LabelColor,
            ForegroundColor = model.ForegroundColor
        };

        foreach (var f in model.HeaderFields)
            request.AddHeaderField(new StandardField(f.Key, f.Label, f.Value));
        foreach (var f in model.PrimaryFields)
            request.AddPrimaryField(new StandardField(f.Key, f.Label, f.Value));
        foreach (var f in model.SecondaryFields)
            request.AddSecondaryField(new StandardField(f.Key, f.Label, f.Value));
        foreach (var f in model.AuxiliaryFields)
            request.AddAuxiliaryField(new StandardField(f.Key, f.Label, f.Value));
        foreach (var f in model.BackFields)
            request.AddBackField(new StandardField(f.Key, f.Label, f.Value));

        if (model.BarcodeFormat != BarcodeFormat.None && !string.IsNullOrWhiteSpace(model.BarcodeMessage))
        {
            request.AddBarcode(
                MapBarcode(model.BarcodeFormat),
                model.BarcodeMessage,
                "ISO-8859-1",
                string.IsNullOrWhiteSpace(model.BarcodeAltText) ? model.BarcodeMessage : model.BarcodeAltText);
        }

        foreach (var (key, dataUri) in model.PassbookImages)
        {
            var bytes = DecodeDataUri(dataUri);
            if (bytes is { Length: > 0 })
                request.Images[key] = bytes;
        }

        return request;
    }

    private static PkPassGenerateResult SuccessResult(CardModel model, byte[] bytes) => new()
    {
        Success = true,
        Bytes = bytes,
        Entries = Unpack(bytes),
        FileName = SanitizeFileName(model.LogoText) + ".pkpass"
    };

    private bool HasSigningCertificates() =>
        File.Exists(ResolvePath(_options.PassCertificatePath))
        && File.Exists(ResolvePath(_options.AppleWwdrCertificatePath));

    private X509Certificate2 LoadPassCertificate()
    {
        var path = ResolvePath(_options.PassCertificatePath);
        var flags = X509KeyStorageFlags.Exportable;
        if (OperatingSystem.IsWindows())
            flags |= X509KeyStorageFlags.UserKeySet;
        else
            flags |= X509KeyStorageFlags.EphemeralKeySet;

        return X509CertificateLoader.LoadPkcs12FromFile(path, _options.PassCertificatePassword, flags);
    }

    private X509Certificate2 LoadWwdrCertificate() =>
        X509CertificateLoader.LoadCertificateFromFile(ResolvePath(_options.AppleWwdrCertificatePath));

    private string ResolvePath(string path) =>
        Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(_env.ContentRootPath, path));

    private static bool HasAnyIcon(CardModel model) =>
        model.PassbookImages.ContainsKey(PassbookImage.Icon)
        || model.PassbookImages.ContainsKey(PassbookImage.Icon2X)
        || model.PassbookImages.ContainsKey(PassbookImage.Icon3X);

    private static BarcodeType MapBarcode(BarcodeFormat format) => format switch
    {
        BarcodeFormat.QR => BarcodeType.PKBarcodeFormatQR,
        BarcodeFormat.PDF417 => BarcodeType.PKBarcodeFormatPDF417,
        BarcodeFormat.Aztec => BarcodeType.PKBarcodeFormatAztec,
        BarcodeFormat.Code128 => BarcodeType.PKBarcodeFormatCode128,
        _ => BarcodeType.PKBarcodeFormatQR
    };

    private static byte[] WritePassJson(PassGeneratorRequest request)
    {
        using var ms = new MemoryStream();
        using var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true });
        request.Write(writer);
        writer.Flush();
        return ms.ToArray();
    }

    private static byte[] BuildManifestJson(byte[] passJson, Dictionary<string, byte[]> images)
    {
        using var ms = new MemoryStream();
        using var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true });
        writer.WriteStartObject();
        writer.WriteString("pass.json", Sha1Hex(passJson));
        foreach (var (name, bytes) in images.OrderBy(kv => kv.Key, StringComparer.Ordinal))
            writer.WriteString(name, Sha1Hex(bytes));
        writer.WriteEndObject();
        writer.Flush();
        return ms.ToArray();
    }

    private static string Sha1Hex(byte[] bytes)
    {
        var hash = SHA1.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static void WriteZipEntry(ZipArchive archive, string name, byte[] bytes)
    {
        var entry = archive.CreateEntry(name, CompressionLevel.Optimal);
        using var stream = entry.Open();
        stream.Write(bytes);
    }

    private static byte[]? DecodeDataUri(string? dataUri)
    {
        if (string.IsNullOrWhiteSpace(dataUri)) return null;
        var comma = dataUri.IndexOf(',');
        if (comma < 0) return null;
        try
        {
            return Convert.FromBase64String(dataUri[(comma + 1)..]);
        }
        catch
        {
            return null;
        }
    }

    private static IReadOnlyList<PkPassPackageEntry> Unpack(byte[] pkpass)
    {
        var entries = new List<PkPassPackageEntry>();
        using var ms = new MemoryStream(pkpass);
        using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
        foreach (var entry in zip.Entries.OrderBy(e => e.FullName, StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrEmpty(entry.Name)) continue;
            using var stream = entry.Open();
            using var outMs = new MemoryStream();
            stream.CopyTo(outMs);
            var bytes = outMs.ToArray();
            var name = entry.FullName.Replace('\\', '/');
            var isImage = name.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                          || name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                          || name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase);
            string? dataUri = null;
            string? text = null;
            if (isImage)
            {
                var mime = name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg";
                dataUri = $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
            }
            else if (name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                     || name.Equals("signature", StringComparison.OrdinalIgnoreCase))
            {
                text = Encoding.UTF8.GetString(bytes);
            }

            entries.Add(new PkPassPackageEntry
            {
                Name = name,
                SizeBytes = bytes.Length,
                IsImage = isImage,
                DataUri = dataUri,
                Text = text
            });
        }

        return entries;
    }

    private static string SanitizeFileName(string? logoText)
    {
        var name = string.IsNullOrWhiteSpace(logoText) ? "blazecard" : logoText.Trim();
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '-');
        return name.Length == 0 ? "blazecard" : name;
    }
}
