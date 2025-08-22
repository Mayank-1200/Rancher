using PdfSharpCore.Fonts;
using System;
using System.Collections.Generic;
using System.IO;

public class FontResolver : IFontResolver
{
    private readonly Dictionary<string, byte[]> _fonts = new();

    public FontResolver()
    {
        // Load the font from the Fonts folder
        string fontPath = Path.Combine(AppContext.BaseDirectory, "Fonts", "arial.ttf");
        _fonts["Arial#"] = File.ReadAllBytes(fontPath);
    }

    public string DefaultFontName => "Arial";

    public byte[] GetFont(string faceName)
    {
        if (_fonts.TryGetValue(faceName, out var data))
            return data;

        throw new InvalidOperationException($"Font not found: {faceName}");
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
        {
            return new FontResolverInfo("Arial#");
        }

        // Fallback to Arial
        return new FontResolverInfo("Arial#");
    }
}
