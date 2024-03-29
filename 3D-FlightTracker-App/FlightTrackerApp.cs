using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace _3D_FlightTracker_App;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class FlightTrackerApp : GameWindow
{
    private float[] vertices =
    { //    x,     y,     z,
        -0.5f,  0.5f,  0.0f, // top left
         0.5f,  0.5f,  0.0f, // top right
        -0.5f, -0.5f,  0.0f, // bottom left
         0.5f, -0.5f,  0.0f  // bottom right
    };

    private uint[] indicies =
    {
        0, 1, 3,
        3, 2, 0,
    };

    int vertexBufferObject;
    private int elementBufferObject;
    int vertexArrayObject;

    Shader shader;

    public FlightTrackerApp(int width, int height) : base(GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            Size = (width, height),
            Title = "3D Flight Tracker"
        })
    { }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Load the shaders
        shader = new Shader("shader.vert", "shader.frag");

        // Set the clean buffer color
        GL.ClearColor(0.3f, 0.1f, 0.6f, 1.0f);

        // Generate a vertex buffer object
        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);
        GL.VertexAttribPointer(shader.GetAttribLocation("vec3Position"), 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indicies.Length * sizeof(uint), indicies, BufferUsageHint.StaticDraw);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        // Drawing code goes here

        // Clean the screen with the color set in OnLoad
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(0);
        shader.Use();
        GL.BindVertexArray(vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, indicies.Length, DrawElementsType.UnsignedInt, 0);

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
        RotateVertices(vertices, 0.0001f);
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);

    }

    protected override void OnUnload()
    {
        base.OnUnload();
        shader.Dispose();
    }

    private void RotateVertices(float[] vertices, float degree)
    {
        float x, y;
        for (int i = 0; i < vertices.Length; i+= 3)
        {
            x = vertices[i];
            y = vertices[i + 1];
            vertices[i] = x * MathF.Cos(degree) - y * MathF.Sin(degree);
            vertices[i + 1] = x * MathF.Sin(degree) + y * MathF.Cos(degree);
        }
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
    }
}
