using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Image = SixLabors.ImageSharp.Image;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace _3D_FlightTracker_App.RenderEngine;

public class TextureAtlas
{
    public readonly int TextureAtlasId;
    public readonly int AtlasWidth;
    public readonly int AtlasHeight;

    public TextureAtlas(int width = 2048, int height = 2048)
    {
        AtlasWidth = width;
        AtlasHeight = height;
        TextureAtlasId = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, TextureAtlasId);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
            AtlasWidth, AtlasHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

        InitBlackTexture();
    }

    public void UpdateTextureRegion(byte[] imageData, int xOffset, int yOffset, int regionWidth, int regionHeight)
    {
        GL.BindTexture(TextureTarget.Texture2D, TextureAtlasId);
        GL.TexSubImage2D(TextureTarget.Texture2D, 0, xOffset, yOffset, regionWidth, regionHeight, PixelFormat.Rgba,
            PixelType.UnsignedByte, imageData);
    }

    private void InitBlackTexture()
    {
        byte[] blackTexture = new byte[AtlasWidth * AtlasHeight * 4];
        for (int i = 0; i < blackTexture.Length; i++)
        {
            blackTexture[i] = 0;
        }

        UpdateTextureRegion(blackTexture, 0, 0, AtlasWidth, AtlasHeight);
    }

    public void MakePartialTextureColored(int xOffset, int yOffset, int regionWidth, int regionHeight, Vector4 color)
    {
        byte[] coloredTexture = new byte[regionWidth * regionHeight * 4];
        for (int i = 0; i < coloredTexture.Length; i += 4)
        {
            coloredTexture[i] = (byte)(color.X * 255);
            coloredTexture[i + 1] = (byte)(color.Y * 255);
            coloredTexture[i + 2] = (byte)(color.Z * 255);
            coloredTexture[i + 3] = (byte)(color.W * 255);
        }

        UpdateTextureRegion(coloredTexture, xOffset, yOffset, regionWidth, regionHeight);
    }

    public void LoadImageToTexture(string filePath, int xOffset, int yOffset)
    {
        // Load the image with ImageSharp
        using (Image<Rgba32> image = Image.Load<Rgba32>(filePath))
        {
            int width = image.Width;
            int height = image.Height;

            // Prepare the pixel array
            byte[] pixelData = new byte[width * height * 4];

            // Copy the pixel data to the array, pixel by pixel
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                    for (int x = 0; x < width; x++)
                    {
                        int index = (y * width + x) * 4;
                        pixelData[index] = pixelRow[x].R;
                        pixelData[index + 1] = pixelRow[x].G;
                        pixelData[index + 2] = pixelRow[x].B;
                        pixelData[index + 3] = pixelRow[x].A;
                    }
                }
            });

            // Update the texture region in the atlas
            UpdateTextureRegion(pixelData, xOffset, yOffset, width, height);
        }
    }
}