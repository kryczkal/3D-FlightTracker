using OpenTK.Mathematics;

namespace _3D_FlightTracker_App;

public static class Settings
{
    /*
     * Camera settings
     */
    public static float FOV = 45.0f;
    public static float NearPlane = 0.1f;
    public static float FarPlane = 100.0f;

    public static Vector3 CameraPosition = new Vector3(0, 0, 3);

    public static float RotationSpeed = 0.01f;

    /*
     * Earth settings
     */
    public static float EarthRadius = 1.0f;
    public static uint EarthSectorCount = 72;
    public static uint EarthStackCount = 72;
    
    /*
     * Shader settings
     */
    public static string VertexShaderPath = "src/Shader/shader.vert";
    public static string FragmentShaderPath = "src/Shader/shader.frag";

}