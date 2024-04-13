using _3D_FlightTracker_App.RenderEngine;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3D_FlightTracker_App;

public class FlightTrackerApp : GameWindow
{
    Shader.Shader _shader = null!;
    SphereMesh _background = null!;
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
         * Background
         */
        TextureAtlas textureAtlas = new TextureAtlas(Settings.Background.BackgroundImagePath);
        _background = new SphereMesh(100, 100, Settings.Background.BackgroundRadius, [new Texture(textureAtlas.TextureAtlasId)]);

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
        
        _shader.Use();

        // Set the transformation matrices
        _shader.SetMatrix4("projection", ref _projection);
        _shader.SetMatrix4("view", ref _view);
        _shader.SetMatrix4("model", ref _model);

        // TODO: Experiment with different lighting settings (lower the ambient light, increase the specular light)

        // Draw the Earth
        _earth.Draw(_shader);
        _background.Draw(_shader);

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
            UpdateZoom(MouseState.ScrollDelta.Y);
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }
    
    // Variables to hold the current zoom level
    private float _currentZoomLevel = Settings.Camera.InitialZoomLevel;

    /// Method to update the zoom based on scroll input
    private void UpdateZoom(float scrollDelta)
    {
        // Constants for the sigmoid function
        float k = 1.1f; // steepness of the bell curve
        float zoomMidpoint = 3f;

        // Calculate the sigmoid factor to adjust the zoom speed
        float expFactor = MathF.Exp(-k * MathF.Pow(_currentZoomLevel - zoomMidpoint, 2));

        // Calculate the new zoom level
        float zoomChange = scrollDelta * Settings.Camera.ScrollZoomSpeed * expFactor;
        _currentZoomLevel += zoomChange;

        // Clamp the zoom level between the min and max distances
        _currentZoomLevel = Math.Clamp(_currentZoomLevel, Settings.Camera.CameraMinDistance, Settings.Camera.CameraMaxDistance);

        // Apply the zoom transformation
        _view = Matrix4.LookAt(new Vector3(0,0,_currentZoomLevel), Vector3.Zero, Vector3.UnitY);
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
