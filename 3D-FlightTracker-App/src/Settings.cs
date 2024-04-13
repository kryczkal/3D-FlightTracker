using OpenTK.Mathematics;

namespace _3D_FlightTracker_App;

public static class Settings
{
    public static class App
    {
        public static Color4 BackgroundColor = new(0.1f, 0.1f, 0.1f, 1.0f);
    }
    public static class Camera {
        // General settings
        public static float FOV = 45.0f;
        public static float NearPlane = 0.1f;
        public static float FarPlane = 100.0f;

        // Position settings
        public static float InitialZoomLevel = 3.0f;
        public static Vector3 CameraPosition = new(0, 0, z: InitialZoomLevel);
        public static float CameraMinDistance = Earth.EarthRadius;
        public static float CameraMaxDistance = Background.BackgroundRadius;

        // Rotation settings
        public static float KeyboardRotationSpeed = 0.01f;
        public static float MouseRotationSpeed = 0.005f;

        // Zoom settings

        public static float KeyboardZoomSpeed = 0.01f;
        public static float ScrollZoomSpeed = 0.05f;
    }

    public static class Earth
    {
        // Object settings
        public static float EarthRadius = 1.0f;
        public static uint EarthSectorCount = 70;
        public static uint EarthStackCount = 70;

        // Tile settings
        public static int InitialResolution = 1024;
    }

    public static class Background
    {
        public static float BackgroundRadius = 100.0f;
        public static string BackgroundImagePath = "assets/background.jpg";
    }

    public static class Shader
    {
        public static string VertexShaderPath = "src/Shader/shader.vert";
        public static string FragmentShaderPath = "src/Shader/shader.frag";
    }
}