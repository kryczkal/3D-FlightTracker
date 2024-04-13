using OpenTK.Graphics.OpenGL4;

namespace _3D_FlightTracker_App.RenderEngine;

public class Texture
{
    public int Id { get; }

    public Texture(int id)
    {
        Id = id;
    }
}

public class Mesh : IDrawable
{
    // OpenGL buffers
    private int _vbo, _vao, _ebo;

    // Vertex data
    public float[] Vertices;
    public float[] Normals;
    public float[] TexCoords;

    // Index data
    public uint[] Indices;

    // Texture
    public Texture[] Textures;

    public Mesh(float[] vertices, float[] normals, float[] texCoords, uint[] indices, Texture[] textures)
    {
        Vertices = vertices;
        Normals = normals;
        TexCoords = texCoords;
        Indices = indices;
        Textures = textures;
        SetupMesh();
    }

    /// Protected constructor for inheritance, if used, one must call SetupMesh() manually
    protected Mesh()
    {
    }


    public void SetupMesh()
    {
        // Create VAO
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        // Create VBO
        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            (Vertices.Length + Normals.Length + TexCoords.Length) * sizeof(float),
            IntPtr.Zero,
                BufferUsageHint.DynamicDraw);

        // Load data into the VBO
        int vertexOffset = 0;
        int vertexSize = Vertices.Length * sizeof(float);
        GL.BufferSubData(BufferTarget.ArrayBuffer, vertexOffset, vertexSize, Vertices);

        int normalOffset = vertexSize;
        int normalSize = Normals.Length * sizeof(float);
        GL.BufferSubData(BufferTarget.ArrayBuffer, normalOffset, normalSize, Normals);

        int texCoordOffset = vertexSize + normalSize;
        int texCoordSize = TexCoords.Length * sizeof(float);
        GL.BufferSubData(BufferTarget.ArrayBuffer, texCoordOffset, texCoordSize, TexCoords);

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
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.DynamicDraw);

        // Unbind the VAO to avoid accidental modification.
        GL.BindVertexArray(0);
    }

    public void Draw(Shader.Shader shader)
    {
        // Inspired by: https://learnopengl.com/Model-Loading/Mesh

        /*
         * Set the textures
         */
        // TODO: Contrary to this code, the current opengl shader doesn't have multiple texture support
        for (int i = 0; i < Textures.Length; i++)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + i); // Activate proper texture unit before binding
            shader.SetInt($"texture{i}", i);
            GL.BindTexture(TextureTarget.Texture2D, Textures[i].Id);
        }

        /*
         * Draw the mesh
         */
        // Bind the vertex array object
        GL.BindVertexArray(_vao);
        // Draw the mesh
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        // Unbind the VAO to avoid accidental modification.
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_ebo);
    }
}