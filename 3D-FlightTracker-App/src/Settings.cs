using OpenTK.Mathematics;

namespace _3D_FlightTracker_App;

public static class Settings
{
    /*
     * Application settings
     */
    public static Color4 BackgroundColor = new(0.1f, 0.1f, 0.1f, 1.0f);
    /*
     * Camera settings
     */
    public static float FOV = 45.0f;
    public static float NearPlane = 0.1f;
    public static float FarPlane = 100.0f;

    public static Vector3 CameraPosition = new(0, 0, 3);

    public static float KeyboardRotationSpeed = 0.01f;
    public static float MouseRotationSpeed = 0.005f;

    public static float KeyboardZoomSpeed = 0.01f;
    public static float ScrollZoomSpeed = 0.05f;

    /*
     * Earth settings
     */
    public static float EarthRadius = 1.0f;
    public static uint EarthSectorCount = 70;
    public static uint EarthStackCount = 70;
    
    /*
     * Shader settings
     */
    public static string VertexShaderPath = "src/Shader/shader.vert";
    public static string FragmentShaderPath = "src/Shader/shader.frag";
}