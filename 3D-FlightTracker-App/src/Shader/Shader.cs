using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_FlightTracker_App.Shader;

public class Shader : IDisposable
{
    private readonly int _handle;
    private bool _isDisposed = false;

    public Shader(string vertexPath, string fragmentPath)
    {
        /*
         * Compile the vertex and fragment shaders
         */

        // Load the source code of the shaders
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);

        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);

        // Compile the shaders
        GL.CompileShader(vertexShader);

        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int statusCode);
        if (statusCode == 0)
        {
            throw new Exception(GL.GetShaderInfoLog(vertexShader));
        }

        GL.CompileShader(fragmentShader);

        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out statusCode);
        if (statusCode == 0)
        {
            throw new Exception(GL.GetShaderInfoLog(fragmentShader));
        }

        // Create the shader program
        _handle = GL.CreateProgram();

        // Attach the shaders to the program
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);

        // Link the program
        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out statusCode);
        if (statusCode == 0)
        {
            throw new Exception(GL.GetProgramInfoLog(_handle));
        }

        // Clean up the shaders
        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void SetMatrix4(string uniformName, ref Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(_handle, uniformName);
        if (location == -1)
            throw new Exception($"Could not find uniform {uniformName} in shader.");

        GL.UniformMatrix4(location, false, ref matrix);
    }

    public void SetVector3(string uniformName, ref Vector3 vector)
    {
        int location = GL.GetUniformLocation(_handle, uniformName);
        if (location == -1)
            throw new Exception($"Could not find uniform {uniformName} in shader.");

        GL.Uniform3(location, vector);
    }
    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(_handle, name);
        if (location != -1)
            GL.Uniform1(location, value);
    }

    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(_handle, attribName);
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            GL.DeleteProgram(_handle);
            _isDisposed = true;
        }
    }

    ~Shader()
    {
        if (!_isDisposed)
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