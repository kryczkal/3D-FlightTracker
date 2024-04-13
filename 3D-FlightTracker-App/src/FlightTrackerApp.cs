using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Mathematics;

namespace _3D_FlightTracker_App;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class FlightTrackerApp : GameWindow
{
    Shader _shader;
    Earth _earth;

    private int _width, _height;

    // Transformation matrices
    private Matrix4 _projection;
    private Matrix4 _view;
    private Matrix4 _model;

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
        _shader = new Shader(Settings.VertexShaderPath, Settings.FragmentShaderPath);

        // Enable depth testing
        GL.Enable(EnableCap.DepthTest);
        // Enable multisampling
        GL.Enable(EnableCap.Multisample);
        //
        GL.DepthFunc(DepthFunction.Lequal);

        // Initialize the transformation matrices
        _projection = Matrix4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(Settings.FOV), _width / (float)_height, Settings.NearPlane, Settings.FarPlane);
        _view = Matrix4.LookAt(Settings.CameraPosition, Vector3.Zero, Vector3.UnitY);
        _model = Matrix4.Identity;

        // Initialize the Earth
        _earth = new Earth(Settings.EarthSectorCount, Settings.EarthStackCount, Settings.EarthRadius);

        // Set the clean buffer color
        GL.ClearColor(1.0f, 1.0f, 0.5f, 1.0f);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Clean the screen with the color set in OnLoad
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        //GL.UseProgram(0);
        _shader.Use();

        // Set up transformations

        _shader.SetMatrix4("projection", ref _projection);
        _shader.SetMatrix4("view", ref _view);
        _shader.SetMatrix4("model", ref _model);

        // Set light and view positions
        Vector3 lightPosition = new Vector3(1.2f, 1.0f, 2.0f);
        Vector3 viewPosition = new Vector3(0, 0, 3); // same as camera position
        _shader.SetVector3("lightPos", ref lightPosition);
        _shader.SetVector3("viewPos", ref viewPosition);

        // Draw the Earth
        _earth.Draw(_shader);

        // Swap the front and back buffers to present the new frame
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.W))
        {
            _model *= Matrix4.CreateRotationX(Settings.RotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.S))
        {
            _model *= Matrix4.CreateRotationX(-Settings.RotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.A))
        {
            _model *= Matrix4.CreateRotationY(Settings.RotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.D))
        {
            _model *= Matrix4.CreateRotationY(-Settings.RotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.Q))
        {
            _model *= Matrix4.CreateRotationZ(Settings.RotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.E))
        {
            _model *= Matrix4.CreateRotationZ(-Settings.RotationSpeed);
        }

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
        _earth.Dispose();
        _shader.Dispose();
    }
}
