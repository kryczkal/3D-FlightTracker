using _3D_FlightTracker_App.RenderEngine;

namespace _3D_FlightTracker_App.Debug;

/// <summary>
///  A simple 2D map for debugging purposes, showing the 2D map of the world
/// </summary>
public class Debug2DMap : IDrawable
{
    public Mesh Mesh;
    public TextureAtlas TextureAtlas;

    public Debug2DMap()
    {
        // Create a simple quad
        float[] vertices =
        [
            -3.0f - Settings.Earth.EarthRadius, -1.0f, 0.0f,
            -1.0f - Settings.Earth.EarthRadius, -1.0f, 0.0f,
            -1.0f - Settings.Earth.EarthRadius, 1.0f, 0.0f,
            -3.0f - Settings.Earth.EarthRadius, 1.0f, 0.0f
        ];

        float[] normals =
        [
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f
        ];

        float[] texCoords =
        [
            0.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f,
            0.0f, 1.0f
        ];

        uint[] indices =
        [
            0, 1, 2,
            2, 3, 0
        ];

        TextureAtlas = new TextureAtlas(Settings.Earth.InitialResolution, Settings.Earth.InitialResolution);
        Earth.InitMapTiles(ref TextureAtlas);
        Mesh = new Mesh(vertices, normals, texCoords, indices,
            [new(TextureAtlas.TextureAtlasId)]);
        }

    public void Draw(Shader.Shader shader)
    {
        Mesh.Draw(shader);
    }
}