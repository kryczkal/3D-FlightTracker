using _3D_FlightTracker_App.RenderEngine;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using BruTile;
using BruTile.Predefined;
using BruTile.Web;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using OpenTK.Windowing.Desktop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Threading.Tasks;
using BruTile;
using BruTile.Predefined;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace _3D_FlightTracker_App.Debug;

/// <summary>
///  A simple 2D map for debugging purposes, showing the 2D map of the world
/// </summary>
public class Debug2DMap : IDrawable
{
    public Mesh _mesh;
    public TextureAtlas _textureAtlas;

    public Debug2DMap()
    {
        // Create a simple quad
        float[] vertices = new float[]
        {
            -3.0f, -1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f,
            -3.0f, 1.0f, 0.0f
        };

        float[] normals = new float[]
        {
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f
        };

        float[] texCoords = new float[]
        {
            0.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f,
            0.0f, 1.0f
        };

        uint[] indices = new uint[]
        {
            0, 1, 2,
            2, 3, 0
        };

        _textureAtlas = new TextureAtlas();

        var tileSource = KnownTileSources.Create();

        // the extent of the visible map changes but lets start with the whole world
        var extent = new Extent(-20037508, -20037508, 20037508, 20037508);
        var screenWidthInPixels = _textureAtlas.atlasWidth; // The width of the map on screen in pixels
        var resolution = extent.Width / screenWidthInPixels;
        var tileInfos = tileSource.Schema.GetTileInfos(extent, resolution);

        foreach (var tileInfo in tileInfos)
        {
            Console.WriteLine("Show tile info");
            byte[] tile = Task.Run(async Task<byte[]> () => { return await tileSource.GetTileAsync(tileInfo); }).GetAwaiter().GetResult();
            Task.WaitAll();
            // Load the image with ImageSharp
            using (Image<Rgba32> image = Image.Load<Rgba32>(tile))
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
                Console.WriteLine($"{pixelData.Length} {tileInfo.Index.Col} {tileInfo.Index.Row}");
                _textureAtlas.UpdateTextureRegion(pixelData, width * tileInfo.Index.Col, height * tileInfo.Index.Row,
                    width, height);
            }
            ///
            _mesh = new Mesh(vertices, normals, texCoords, indices,
                new Texture[] { new(_textureAtlas.textureAtlasID) });
        }
    }


    public void Draw(Shader shader)
    {
        _mesh.Draw(shader);
    }
}