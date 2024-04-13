using _3D_FlightTracker_App.RenderEngine;
using BruTile;
using BruTile.Predefined;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace _3D_FlightTracker_App;

public class Earth : IDrawable
{
    /*
     * Private fields
     */
    private SphereMesh _sphereMesh;
    private TextureAtlas _mapTiles;

    /*
     * Public methods
     */
    public Earth(uint sectorCount, uint stackCount, float radius, int textureWidth = 1024)
    {
        _mapTiles = new TextureAtlas(textureWidth, textureWidth);
        InitMapTiles(ref _mapTiles);
        _sphereMesh = new SphereMesh(sectorCount, stackCount, radius, [new(_mapTiles.TextureAtlasId)]);
    }

    public void Draw(Shader.Shader shader)
    {
        _sphereMesh.Draw(shader);
    }

    public void Dispose()
    {
        _sphereMesh.Dispose();
    }

    /*
     * Static methods
     */

    /// <summary>
    /// This method initializes the textureAtlas with map tiles by downloading them from the internet using BruTile
    /// </summary>
    /// <param name="textureAtlas">The textureAtlas to which the tiles will be stored. It's width and height should be the same and a multiplication of 2</param>
    public static void InitMapTiles(ref TextureAtlas textureAtlas)
    {
        var tileSource = KnownTileSources.Create();

        // the extent of the visible map changes but lets start with the whole world
        var extent = new Extent(-20037508, -20037508, 20037508, 20037508);
        var screenWidthInPixels = textureAtlas.AtlasWidth; // The width of the map on screen in pixels
        var resolution = extent.Width / screenWidthInPixels;
        var tileInfos = tileSource.Schema.GetTileInfos(extent, resolution);

        // TODO: Use parallel for loop
        foreach (var tileInfo in tileInfos)
        {
            // Get the tile data synchronously (as this is an initialization method and we can't begin with an empty texture atlas)
            byte[] tile = Task.Run(async Task<byte[]> () => { return await tileSource.GetTileAsync(tileInfo); }).GetAwaiter().GetResult();
            Task.WaitAll();

            // Load the image with ImageSharp
            // This part has to be done since the tiles are in PNG format
            // and we need to convert them to a format that OpenGL can understand
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

                textureAtlas.UpdateTextureRegion(pixelData, width * tileInfo.Index.Col, height * tileInfo.Index.Row,
                    width, height);
            }
        }
    }
}