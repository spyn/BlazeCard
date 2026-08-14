namespace BlazeCard.Models;

public enum AppMode { Form, Code, Final }

public enum PassType
{
    /// <summary>v1 supports Generic only. Other values are reserved / not offered in UI.</summary>
    Generic = 0
}

public enum BarcodeFormat
{
    None,
    QR,
    PDF417,
    Aztec,
    Code128
}

public enum TextAlignment { Left, Center, Right, Natural }

/// <summary>Google Wallet image slots (HTTPS in snippets; data URIs in preview).</summary>
public enum GoogleImageSlot
{
    Hero,
    WideLogo,
    ImageModule
}

/// <summary>Google Wallet GenericObject.genericType values.</summary>
public static class GoogleGenericTypes
{
    public static readonly string[] All =
    [
        "GENERIC_TYPE_UNSPECIFIED",
        "GENERIC_SEASON_PASS",
        "GENERIC_UTILITY_BILLS",
        "GENERIC_PARKING_PASS",
        "GENERIC_VOUCHER",
        "GENERIC_GYM_MEMBERSHIP",
        "GENERIC_LIBRARY_MEMBERSHIP",
        "GENERIC_RESERVATIONS",
        "GENERIC_AUTO_INSURANCE",
        "GENERIC_HOME_INSURANCE",
        "GENERIC_ENTRY_TICKET",
        "GENERIC_RECEIPT",
        "GENERIC_LOYALTY_CARD",
        "GENERIC_OTHER"
    ];
}

public enum FieldGroup
{
    Header, Primary, Secondary, Auxiliary, Back
}

public enum Skin
{
    Blaze
}

public enum AppearanceMode
{
    Light,
    Dark
}

public enum PreviewFocus
{
    Dual,
    Apple,
    Google
}
