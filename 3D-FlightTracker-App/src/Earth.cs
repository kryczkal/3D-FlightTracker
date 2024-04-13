using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using _3D_FlightTracker_App.Sphere;
using OpenTK.Mathematics;

namespace _3D_FlightTracker_App;

public class Earth : IDrawable
{
    private SphereMesh _sphereMesh;
    public Earth(uint sectorCount, uint stackCount, float radius, Texture texture)
    {
        _sphereMesh = new SphereMesh(sectorCount, stackCount, radius, []);
        _sphereMesh.Textures = [texture];
        //CreatePlaceholderTexture(_sphereMesh);
    }

    public void CreatePlaceholderTexture(Mesh mesh)
    {
        int width = 2, height = 2;
        byte[] pixels = new byte[width * height * 3]; // RGB format

        //// Simple checkerboard pattern
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offset = (y * width + x) * 3;
                bool isWhite = (x % 2 == y % 2); // Checkerboard pattern
                pixels[offset] = isWhite ? (byte)255 : (byte)0; // Red
                pixels[offset + 1] = isWhite ? (byte)255 : (byte)0; // Green
                pixels[offset + 2] = isWhite ? (byte)255 : (byte)0; // Blue
            }
        }

        // Generate and bind the texture
        int genTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, genTexture);

        // Set texture parameters
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // Upload the texture data
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);

        // Unbind the texture
        GL.BindTexture(TextureTarget.Texture2D, 0);

        mesh.Textures = [new(genTexture)];
    }

    public void Draw(Shader shader)
    {
        _sphereMesh.Draw(shader);
    }

    public void Dispose()
    {
        _sphereMesh.Dispose();
    }
}