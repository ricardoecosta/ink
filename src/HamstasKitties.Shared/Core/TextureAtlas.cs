using Microsoft.Xna.Framework.Graphics;

namespace HamstasKitties.Core;

/// <summary>
/// Texture atlas for reducing texture state changes.
/// Multiple sprites packed into a single texture.
/// </summary>
public class TextureAtlas
{
    private readonly Texture2D _texture;
    private readonly Dictionary<string, SpriteRegion> _regions;

    public TextureAtlas(Texture2D texture)
    {
        _texture = texture;
        _regions = new Dictionary<string, SpriteRegion>();
    }

    public Texture2D Texture => _texture;
    public int RegionCount => _regions.Count;

    /// <summary>
    /// Add a sprite region to the atlas.
    /// </summary>
    public void AddRegion(string name, int x, int y, int width, int height)
    {
        _regions[name] = new SpriteRegion(x, y, width, height);
    }

    /// <summary>
    /// Get a sprite region by name.
    /// </summary>
    public SpriteRegion? GetRegion(string name)
    {
        return _regions.TryGetValue(name, out var region) ? region : null;
    }

    /// <summary>
    /// Check if a region exists.
    /// </summary>
    public bool HasRegion(string name) => _regions.ContainsKey(name);

    /// <summary>
    /// Load regions from a sprite sheet definition file.
    /// Format: Name = X Y Width Height
    /// </summary>
    public static TextureAtlas FromDefinition(Texture2D texture, string definition)
    {
        var atlas = new TextureAtlas(texture);

        foreach (var line in definition.Split('\n'))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            var parts = trimmed.Split('=');
            if (parts.Length != 2) continue;

            var name = parts[0].Trim();
            var coords = parts[1].Trim().Split(' ');

            if (coords.Length >= 4 &&
                int.TryParse(coords[0], out int x) &&
                int.TryParse(coords[1], out int y) &&
                int.TryParse(coords[2], out int width) &&
                int.TryParse(coords[3], out int height))
            {
                atlas.AddRegion(name, x, y, width, height);
            }
        }

        return atlas;
    }
}

/// <summary>
/// Represents a region within a texture atlas.
/// </summary>
public readonly struct SpriteRegion
{
    public readonly int X;
    public readonly int Y;
    public readonly int Width;
    public readonly int Height;

    public SpriteRegion(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Microsoft.Xna.Framework.Rectangle ToRectangle() =>
        new Microsoft.Xna.Framework.Rectangle(X, Y, Width, Height);
}
