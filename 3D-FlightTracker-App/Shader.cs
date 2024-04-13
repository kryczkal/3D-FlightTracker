
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3D_FlightTracker_App;

public class Shader : IDisposable
{
    private int Handle;
    private bool IsDisposed = false;

    public Shader(string vertexPath, string fragmentPath)
    {
        /*
         * Compile the vertex and fragment shaders
         */

        int VertexShader;
        int FragmentShader;

        // Load the source code of the shaders
        string VertexShaderSource = File.ReadAllText(vertexPath);
        string FragmentShaderSource = File.ReadAllText(fragmentPath);

        VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(VertexShader, VertexShaderSource);

        FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);

        // Compile the shaders
        GL.CompileShader(VertexShader);

        GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int statusCode);
        if (statusCode == 0)
        {
            throw new Exception(GL.GetShaderInfoLog(VertexShader));
        }

        GL.CompileShader(FragmentShader);

        GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out statusCode);
        if (statusCode == 0)
        {
            throw new Exception(GL.GetShaderInfoLog(FragmentShader));
        }

        // Create the shader program
        Handle = GL.CreateProgram();

        // Attach the shaders to the program
        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);

        // Link the program
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out statusCode);
        if (statusCode == 0)
        {
            throw new Exception(GL.GetProgramInfoLog(Handle));
        }

        // Clean up the shaders
        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(VertexShader);
        GL.DeleteShader(FragmentShader);
    }

    public void SetMatrix4(string uniformName, ref Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(Handle, uniformName);
        if (location == -1)
            throw new Exception($"Could not find uniform {uniformName} in shader.");

        GL.UniformMatrix4(location, false, ref matrix);
    }

    public void SetVector3(string uniformName, ref Vector3 vector)
    {
        int location = GL.GetUniformLocation(Handle, uniformName);
        if (location == -1)
            throw new Exception($"Could not find uniform {uniformName} in shader.");

        GL.Uniform3(location, vector);
    }
    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location != -1)
            GL.Uniform1(location, value);
    }

    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            GL.DeleteProgram(Handle);
            IsDisposed = true;
        }
    }

    ~Shader()
    {
        if (!IsDisposed)
        {
            throw new Exception("Shader was not disposed properly.");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}