using _3D_FlightTracker_App.RenderEngine;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3D_FlightTracker_App.App;

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
            float.DegreesToRadians(Settings.Camera.Fov),
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
        base.OnUpdateFrame(args);

        /*
         * Rotation
         */

        float deltaX = 0;
        float deltaY = 0;
        float deltaZ = 0;

        // Keyboard input
        if (KeyboardState.IsKeyDown(Keys.A)) deltaY += Settings.Controls.KeyboardRotationSpeed;
        if (KeyboardState.IsKeyDown(Keys.D)) deltaY += -Settings.Controls.KeyboardRotationSpeed;
        if (KeyboardState.IsKeyDown(Keys.W)) deltaX += Settings.Controls.KeyboardRotationSpeed;
        if (KeyboardState.IsKeyDown(Keys.S)) deltaX += -Settings.Controls.KeyboardRotationSpeed;
        if (KeyboardState.IsKeyDown(Keys.Q)) deltaZ += -Settings.Controls.KeyboardRotationSpeed;
        if (KeyboardState.IsKeyDown(Keys.E)) deltaZ += Settings.Controls.KeyboardRotationSpeed;

        // Mouse input
        if (MouseState.IsButtonDown(MouseButton.Left))
        {
            deltaY += (MouseState.Position.X - MouseState.PreviousPosition.X) * Settings.Controls.MouseRotationSpeed;
            deltaX += (MouseState.Position.Y - MouseState.PreviousPosition.Y)* Settings.Controls.MouseRotationSpeed;
        }

        // Update the rotation
        if (deltaX != 0 || deltaY != 0 || deltaZ != 0)
            App.Controls.Rotation.UpdateRotation(deltaX, deltaY, deltaZ, ref _model);

        /*
         * Zoom
         */

        // Keyboard input
        float zoomDelta = 0;
        if (KeyboardState.IsKeyDown(Keys.Up)) zoomDelta = Settings.Controls.KeyboardZoomSpeed;
        if (KeyboardState.IsKeyDown(Keys.Down)) zoomDelta = -Settings.Controls.KeyboardZoomSpeed;

        // Mouse input
        zoomDelta += MouseState.ScrollDelta.Y;

        // Update the zoom
        if (zoomDelta != 0)
            App.Controls.Zoom.UpdateZoom(zoomDelta, ref _view);

        /*
         * Other
         */

        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
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
