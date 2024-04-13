using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3D_FlightTracker_App;

public class FlightTrackerApp : GameWindow
{
    Shader.Shader _shader = null!;
    Earth _earth = null!;

    private int _width, _height;

    // Transformation matrices
    private Matrix4 _projection;
    private Matrix4 _view;
    private Matrix4 _model;

    public FlightTrackerApp(int width, int height) : base(GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            ClientSize = (width, height),
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

        /*
         * Shaders
         */

        // Load the shaders
        _shader = new Shader.Shader(Settings.Shader.VertexShaderPath, Settings.Shader.FragmentShaderPath);

        /*
         * OpenGL settings
         */
        // Enable depth testing
        GL.Enable(EnableCap.DepthTest);
        // Enable multisampling
        GL.Enable(EnableCap.Multisample);
        GL.DepthFunc(DepthFunction.Lequal);

        /*
         * Transformation matrices
         */

        // Initialize the transformation matrices
        _projection = Matrix4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(Settings.Camera.FOV),
            _width / (float)_height,
            Settings.Camera.NearPlane,
            Settings.Camera.FarPlane);
        _view = Matrix4.LookAt(Settings.Camera.CameraPosition, Vector3.Zero, Vector3.UnitY);
        _model = Matrix4.Identity;

        /*
         * Earth
         */

        // Initialize the Earth
        _earth = new Earth(
            Settings.Earth.EarthSectorCount,
            Settings.Earth.EarthStackCount,
            Settings.Earth.EarthRadius,
            Settings.Earth.InitialResolution
            );
        /*
         * Clear color
         */

        // Set the clean buffer color
        GL.ClearColor(Settings.App.BackgroundColor);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Clean the screen with the color set in OnLoad
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        //GL.UseProgram(0);
        _shader.Use();

        // Set the transformation matrices
        _shader.SetMatrix4("projection", ref _projection);
        _shader.SetMatrix4("view", ref _view);
        _shader.SetMatrix4("model", ref _model);

        // Draw the Earth
        _earth.Draw(_shader);

        // Swap the front and back buffers to present the new frame
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        // TODO: This is a temporary solution to get the mouse and keyboard input
        // It should be replaced with a proper input handling system

        base.OnUpdateFrame(args);

        /*
         * Keyboard input
         */
        if (KeyboardState.IsKeyDown(Keys.W))
        {
            _model *= Matrix4.CreateRotationX(Settings.Camera.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.S))
        {
            _model *= Matrix4.CreateRotationX(-Settings.Camera.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.A))
        {
            _model *= Matrix4.CreateRotationY(Settings.Camera.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.D))
        {
            _model *= Matrix4.CreateRotationY(-Settings.Camera.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.Q))
        {
            _model *= Matrix4.CreateRotationZ(Settings.Camera.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.E))
        {
            _model *= Matrix4.CreateRotationZ(-Settings.Camera.KeyboardRotationSpeed);
        }

        if (KeyboardState.IsKeyDown(Keys.Up))
        {
            _view *= Matrix4.CreateTranslation(0, 0, Settings.Camera.KeyboardZoomSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.Down))
        {
            _view *= Matrix4.CreateTranslation(0, 0, -Settings.Camera.KeyboardZoomSpeed);
        }

        /*
         * Mouse input
         */

        // Rotate by dragging
        if (MouseState.IsButtonDown(MouseButton.Left))
        {
            Vector2 delta = MouseState.Position - MouseState.PreviousPosition;
            _model *= Matrix4.CreateRotationY(delta.X * Settings.Camera.MouseRotationSpeed);
            _model *= Matrix4.CreateRotationX(delta.Y * Settings.Camera.MouseRotationSpeed);
        }

        // Zoom by scrolling
        if (MouseState.ScrollDelta.Y != 0)
        {
            _view *= Matrix4.CreateTranslation(0, 0, MouseState.ScrollDelta.Y * Settings.Camera.ScrollZoomSpeed);
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
