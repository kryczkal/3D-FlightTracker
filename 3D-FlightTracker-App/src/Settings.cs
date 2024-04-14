using OpenTK.Mathematics;

namespace _3D_FlightTracker_App;

public static class Settings
{
    /*
     * User settings
     */
    public static class App
    {
        public static Color4 BackgroundColor = new(0.1f, 0.1f, 0.1f, 1.0f);
    }
    public static partial class Camera {
        // General settings
        public static readonly float Fov = 45.0f;
        public static readonly float NearPlane = 0.1f;

        // Position settings
        public static readonly float InitialZoomLevel = 2.0f;
        public static readonly Vector3 CameraPosition = new(0, 0, z: InitialZoomLevel);
    }

    public static partial class Controls
    {
        // Rotation settings
        public static readonly float KeyboardRotationSpeed = 0.05f;
        public static readonly float MouseRotationSpeed = 0.05f;

        // Zoom settings
        public static readonly float KeyboardZoomSpeed = 0.5f;
        public static readonly float ZoomSigmoidFactor = 10.0f;
    }
    public static class Earth
    {
        // Object settings
        public static readonly float EarthRadius = 2.0f;
        public static readonly uint EarthSectorCount = 70;
        public static readonly uint EarthStackCount = 70;

        // Tile settings
        public static readonly int InitialResolution = 1024;
    }

    public static class Background
    {
        public static readonly float BackgroundRadius = 100.0f;
        public static readonly string BackgroundImagePath = "assets/background.jpg";
    }

    public static class Shader
    {
        public static readonly string VertexShaderPath = "src/Shader/shader.vert";
        public static readonly string FragmentShaderPath = "src/Shader/shader.frag";
    }

    /*
     * Constants
     */
    public static class Constants
    {
        public static readonly float CameraMinOffset = 0.1f; // Offset to prevent camera clipping
        public static readonly float ZoomCoefficient = 5f;
    }

    /*
     * Auto-generated settings
     */
    public static partial class Camera
    {
        // General settings
        public static readonly float FarPlane = 2 * Background.BackgroundRadius;

        // Zoom settings
        public static readonly float MinDistance = Earth.EarthRadius + Constants.CameraMinOffset;
        public static readonly float MaxDistance =
            Constants.ZoomCoefficient + Constants.ZoomCoefficient * (Earth.EarthRadius) /
            (Background.BackgroundRadius) - Constants.CameraMinOffset;
    }

    public static partial class Controls
    {
        public static readonly float MinZoomLevel = -ZoomSigmoidFactor * 5;
        public static readonly float MaxZoomLevel = ZoomSigmoidFactor * 5;
    }
}