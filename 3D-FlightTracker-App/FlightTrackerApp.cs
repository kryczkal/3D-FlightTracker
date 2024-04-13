using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Mathematics;

namespace _3D_FlightTracker_App;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class FlightTrackerApp : GameWindow
{
    Shader shader;
    Earth earth;

    private int _width, _height;

    public FlightTrackerApp(int width, int height) : base(GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            Size = (width, height),
            Title = "3D Flight Tracker",
            Flags = ContextFlags.Default,
            NumberOfSamples = 4,
        })
    {
        _width = width;
        _height = height;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Load the shaders
        shader = new Shader("shader.vert", "shader.frag");

        // Enable depth testing
        GL.Enable(EnableCap.DepthTest);
        // Enable multisampling
        GL.Enable(EnableCap.Multisample);
        GL.DepthFunc(DepthFunction.Lequal);

        // Initialize the Earth
        earth = new Earth(72, 72, 1.0f);

        // Set the clean buffer color
        GL.ClearColor(1.0f, 1.0f, 0.5f, 1.0f);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);


        // Clean the screen with the color set in OnLoad
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        //GL.UseProgram(0);
        shader.Use();

        // Set up transformations
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _width / (float)_height, 0.1f, 100.0f);
        Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY);
        Matrix4 model = Matrix4.CreateRotationY((float)Environment.TickCount / 1000.0f);
        //Matrix4 model = Matrix4.Identity;
        
        shader.SetMatrix4("projection", ref projection);
        shader.SetMatrix4("view", ref view);
        shader.SetMatrix4("model", ref model);

        // Set light and view positions
        Vector3 lightPosition = new Vector3(1.2f, 1.0f, 2.0f);
        Vector3 viewPosition = new Vector3(0, 0, 3); // same as camera position
        shader.SetVector3("lightPos", ref lightPosition);
        shader.SetVector3("viewPos", ref viewPosition);
        shader.SetInt("texture1", 0);

        // Draw the Earth
        earth.Draw();

        // Swap the front and back buffers to present the new frame
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        _width = e.Width;
        _height = e.Height;
        GL.Viewport(0, 0, e.Width, e.Height);

    }

    protected override void OnUnload()
    {
        base.OnUnload();
        earth.Dispose();
        shader.Dispose();
    }
}
