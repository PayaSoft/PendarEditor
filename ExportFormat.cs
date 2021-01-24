namespace Paya
{
    /// <summary>
    /// Specifies the <see cref="ExportFormat"/> enumeration.
    /// </summary>
    [System.Serializable]
    public enum ExportFormat : byte
    {
        Docx,
        Pdf,
        Rtf,
        Xps,
        Tiff,
        Jpeg,
        Png,
        Html,
        Mhtml,
        Svg,
        Text,
        Excel,
        TelerikXaml,
        Gif,
        Bmp,
        Wmf,

        Unknown = byte.MaxValue,
    }
}
