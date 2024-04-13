using _3D_FlightTracker_App.Debug;
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
    Debug2DMap _debug2DMap; // TODO: Remove this line

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

        /*
         * Shaders
         */

        // Load the shaders
        _shader = new Shader(Settings.VertexShaderPath, Settings.FragmentShaderPath);

        /*
         * OpenGL settings
         */
        // Enable depth testing
        GL.Enable(EnableCap.DepthTest);
        // Enable multisampling
        GL.Enable(EnableCap.Multisample);
        //
        GL.DepthFunc(DepthFunction.Lequal);

        /*
         * Transformation matrices
         */

        // Initialize the transformation matrices
        _projection = Matrix4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(Settings.FOV), _width / (float)_height, Settings.NearPlane, Settings.FarPlane);
        _view = Matrix4.LookAt(Settings.CameraPosition, Vector3.Zero, Vector3.UnitY);
        _model = Matrix4.Identity;

        /*
         * Earth
         */

        // Initialize the Earth
        _debug2DMap = new Debug2DMap(); // TODO: Remove this line
        _earth = new Earth(Settings.EarthSectorCount, Settings.EarthStackCount, Settings.EarthRadius, new Texture(_debug2DMap._textureAtlas.textureAtlasID));
        /*
         * Clear color
         */

        // Set the clean buffer color
        GL.ClearColor(Settings.BackgroundColor);
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
        _debug2DMap.Draw(_shader); // TODO: Remove this line

        // Swap the front and back buffers to present the new frame
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        /*
         * Keyboard input
         */
        if (KeyboardState.IsKeyDown(Keys.W))
        {
            _model *= Matrix4.CreateRotationX(Settings.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.S))
        {
            _model *= Matrix4.CreateRotationX(-Settings.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.A))
        {
            _model *= Matrix4.CreateRotationY(Settings.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.D))
        {
            _model *= Matrix4.CreateRotationY(-Settings.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.Q))
        {
            _model *= Matrix4.CreateRotationZ(Settings.KeyboardRotationSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.E))
        {
            _model *= Matrix4.CreateRotationZ(-Settings.KeyboardRotationSpeed);
        }

        if (KeyboardState.IsKeyDown(Keys.Up))
        {
            _view *= Matrix4.CreateTranslation(0, 0, Settings.KeyboardZoomSpeed);
        }
        if (KeyboardState.IsKeyDown(Keys.Down))
        {
            _view *= Matrix4.CreateTranslation(0, 0, -Settings.KeyboardZoomSpeed);
        }

        /*
         * Mouse input
         */

        // Rotate by dragging
        if (MouseState.IsButtonDown(MouseButton.Left))
        {
            Vector2 delta = MouseState.Position - MouseState.PreviousPosition;
            _model *= Matrix4.CreateRotationY(delta.X * Settings.MouseRotationSpeed);
            _model *= Matrix4.CreateRotationX(delta.Y * Settings.MouseRotationSpeed);
        }

        // Zoom by scrolling
        if (MouseState.ScrollDelta.Y != 0)
        {
            _view *= Matrix4.CreateTranslation(0, 0, MouseState.ScrollDelta.Y * Settings.ScrollZoomSpeed);
        }

        // Update the debug 2D map
        if (KeyboardState.IsKeyPressed(Keys.Space))
        {
            Random Random = new Random();
            // Generate a random color
            Vector4 color = new Vector4((float)Random.NextDouble(), (float)Random.NextDouble(), (float)Random.NextDouble(), 1.0f);
            // Get a random region
            int xOffset = Random.Next(0, _debug2DMap._textureAtlas.atlasWidth - 100);
            int yOffset = Random.Next(0, _debug2DMap._textureAtlas.atlasHeight - 100);
           _debug2DMap._textureAtlas.MakePartialTextureColored(xOffset, yOffset, 100, 100, color);
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
