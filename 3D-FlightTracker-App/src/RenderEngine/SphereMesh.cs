namespace _3D_FlightTracker_App.Sphere;

public class SphereMesh :  Mesh
{
    public SphereMesh(uint sectorCount, uint stackCount, float radius, Texture[] textures) : base()
    {
        InitializeGeometry(sectorCount, stackCount, radius);
        SetupMesh();
    }
    private void InitializeGeometry(uint sectorCount, uint stackCount, float radius)
    {
        // Sphere generation algorithm by Song Ho Ahn:
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

        // Set the mesh data except the Texture
        Vertices = vertices.ToArray();
        Normals = normals.ToArray();
        TexCoords = texCoords.ToArray();
        Indices = indicies.ToArray();
    }
}