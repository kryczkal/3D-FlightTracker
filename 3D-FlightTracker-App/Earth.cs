using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace _3D_FlightTracker_App;

public class Earth : IDrawable
{
    // OpenGL buffers
    private int _vbo, _vao, _ebo;

    // Vertex data
    private float[] _vertices;
    private float[] _normals;
    private float[] _texCoords;

    // Index data
    private uint[] _indices;
    private uint[] _lineIndicies;

    // Texture
    private int _texture;

    public Earth(uint sectorCount, uint stackCount, float radius)
    {
        InitializeGeometry(sectorCount, stackCount, radius);
        CreatePlaceholderTexture();
        SetupBuffers();
    }

    private void InitializeGeometry(uint sectorCount, uint stackCount, float radius)
    {
        // Sphere generation algorithm from Song Ho Ahn:
        // https://www.songho.ca/opengl/gl_sphere.html

        float x, y, z, xy;                           // vertex position
        float nx, ny, nz, lengthInv = 1.0f / radius; // vertex normal
        float s, t;                                  // vertex texCoord

        float sectorStep = 2 * MathF.PI / sectorCount;
        float stackStep = MathF.PI / stackCount;
        float sectorAngle, stackAngle;

        List<float> vertices = new List<float>();
        List<float> normals = new List<float>();
        List<float> texCoords = new List<float>();

        for (int i = 0; i <= stackCount; i++)
        {
            stackAngle = MathF.PI / 2 - i * stackStep;        // starting from pi/2 to -pi/2
            xy = radius * MathF.Cos(stackAngle);             // r * cos(u)
            z = radius * MathF.Sin(stackAngle);              // r * sin(u)

            // add (sectorCount+1) vertices per stack
            // the first and last vertices have same position and normal, but different tex coords
            for (int j = 0; j <= sectorCount; j++)
            {
                sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                // vertex position (x, y, z)
                x = xy * MathF.Cos(sectorAngle);             // r * cos(u) * cos(v)
                y = xy * MathF.Sin(sectorAngle);             // r * cos(u) * sin(v)
                vertices.Add(x);
                vertices.Add(y);
                vertices.Add(z);

                // normalized vertex normal (nx, ny, nz)
                nx = x * lengthInv;
                ny = y * lengthInv;
                nz = z * lengthInv;
                normals.Add(nx);
                normals.Add(ny);
                normals.Add(nz);

                // vertex tex coord (s, t) range between [0, 1]
                s = (float)j / sectorCount;
                t = (float)i / stackCount;
                texCoords.Add(s);
                texCoords.Add(t);
            }
        }

        // generate CCW index list of sphere triangles
        List<uint> indicies = new List<uint>();
        List<uint> lineIndicies = new List<uint>();
        uint k1, k2;
        for (int i = 0; i < stackCount; i++)
        {
            k1 = (uint)(i * (sectorCount + 1));     // beginning of current stack
            k2 = k1 + sectorCount + 1;      // beginning of next stack

            for (int j = 0; j < sectorCount; j++, k1++, k2++)
            {
                // 2 triangles per sector excluding first and last stacks
                // k1 => k2 => k1+1
                if (i != 0)
                {
                    indicies.Add(k1);
                    indicies.Add(k2);
                    indicies.Add(k1 + 1);
                }

                // k1+1 => k2 => k2+1
                if (i != (stackCount - 1))
                {
                    indicies.Add(k1 + 1);
                    indicies.Add(k2);
                    indicies.Add(k2 + 1);
                }

                // vertical lines for all stacks
                lineIndicies.Add(k1);
                lineIndicies.Add(k2);
                // horizontal lines
                lineIndicies.Add(k1);
                lineIndicies.Add(k1 + 1);
            }
        }

        // Convert the result to arrays
        _vertices = vertices.ToArray();
        _normals = normals.ToArray();
        _texCoords = texCoords.ToArray();

        _indices = indicies.ToArray();
        _lineIndicies = lineIndicies.ToArray();
    }

    private void SetupBuffers()
    {
        // Create VAO
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        // Create VBO
        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            (_vertices.Length + _normals.Length + _texCoords.Length) * sizeof(float),
            IntPtr.Zero,
                BufferUsageHint.DynamicDraw);

        // Load data into the VBO
        int vertexOffset = 0;
        int vertexSize = _vertices.Length * sizeof(float);
        GL.BufferSubData(BufferTarget.ArrayBuffer, vertexOffset, vertexSize, _vertices);

        int normalOffset = vertexSize;
        int normalSize = _normals.Length * sizeof(float);
        GL.BufferSubData(BufferTarget.ArrayBuffer, normalOffset, normalSize, _normals);

        int texCoordOffset = vertexSize + normalSize;
        int texCoordSize = _texCoords.Length * sizeof(float);
        GL.BufferSubData(BufferTarget.ArrayBuffer, texCoordOffset, texCoordSize, _texCoords);

        // Set the vertex attribute pointers
        GL.VertexAttribPointer(0, 3,
            VertexAttribPointerType.Float, false, 3 * sizeof(float), (IntPtr)vertexOffset);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3,
            VertexAttribPointerType.Float, false, 3 * sizeof(float), (IntPtr)normalOffset);
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(2, 2,
            VertexAttribPointerType.Float, false, 2 * sizeof(float), (IntPtr)texCoordOffset);
        GL.EnableVertexAttribArray(2);

        // Create EBO
        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.DynamicDraw);
        
        // Unbind the VAO to avoid accidental modification.
        GL.BindVertexArray(0);
    }
    
    public void CreatePlaceholderTexture()
    {
        int width = 2, height = 2;
        byte[] pixels = new byte[width * height * 3]; // RGB format

        //// Simple checkerboard pattern
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offset = (y * width + x) * 3;
                bool isWhite = (x % 2 == y % 2); // Checkerboard pattern
                pixels[offset] = isWhite ? (byte)255 : (byte)0; // Red
                pixels[offset + 1] = isWhite ? (byte)255 : (byte)0; // Green
                pixels[offset + 2] = isWhite ? (byte)255 : (byte)0; // Blue
            }
        }

        // Generate and bind the texture
        _texture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _texture);

        // Set texture parameters
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // Upload the texture data
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);

        // Unbind the texture
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _texture);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_ebo);
    }
}