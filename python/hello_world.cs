using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CableGenerator : MonoBehaviour
{
    [Header("Cable Points (ordered)")]
    public List<Transform> cablePoints = new List<Transform>();

    [Header("Ground")]
    public Terrain terrain;
    public LayerMask groundMask = -1;   // what counts as ground for raycast

    [Header("Cable Shape")]
    public float cableRadius = 0.05f;
    public int radialSegments = 10;
    public int resolutionPerSegment = 10;
    public float groundOffset = 0.02f;

    [Header("Material")]
    public Material cableMaterial;

    [HideInInspector]
    public List<Vector3> worldPath = new List<Vector3>();   // used by other scripts

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    // -------------------------------------------------------
    // MAIN GENERATION ENTRY
    // -------------------------------------------------------
    public void GenerateCable()
    {
        if (cablePoints.Count < 2)
        {
            Debug.LogWarning("Cable needs at least 2 points.");
            return;
        }

        EnsureComponents();

        worldPath.Clear();

        List<Vector3> localPath = BuildGroundPath();
        Mesh mesh = BuildTubeMesh(localPath);

        meshFilter.sharedMesh = mesh;

        if (cableMaterial != null)
            meshRenderer.sharedMaterial = cableMaterial;
    }

    // ENSURE MESH COMPONENTS EXIST
    
    void EnsureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter>();
        if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    // BUILD PATH FOLLOWING REAL GROUND

    List<Vector3> BuildGroundPath()
    {
        List<Vector3> path = new List<Vector3>();

        for (int i = 0; i < cablePoints.Count - 1; i++)
        {
            Vector3 start = cablePoints[i].position;
            Vector3 end = cablePoints[i + 1].position;

            for (int r = 0; r <= resolutionPerSegment; r++)
            {
                float t = r / (float)resolutionPerSegment;
                Vector3 worldPos = Vector3.Lerp(start, end, t);

                float groundY = GetGroundHeight(worldPos);
                worldPos.y = groundY + groundOffset;

                worldPath.Add(worldPos);
                path.Add(transform.InverseTransformPoint(worldPos));
            }
        }

        return path;
    }

    // GET TRUE GROUND HEIGHT
    // Uses BOTH terrain sampling and physics raycast
    
    float GetGroundHeight(Vector3 worldPos)
    {
        float best = float.MinValue;

        // TERRAIN HEIGHT
        if (terrain != null)
        {
            Vector3 tpos = terrain.transform.position;

            float localX = worldPos.x - tpos.x;
            float localZ = worldPos.z - tpos.z;

            if (localX >= 0 &&
                localZ >= 0 &&
                localX <= terrain.terrainData.size.x &&
                localZ <= terrain.terrainData.size.z)
            {
                float h = terrain.SampleHeight(worldPos) + tpos.y;
                best = h;
            }
        }

        // RAYCAST HEIGHT
        RaycastHit hit;
        if (Physics.Raycast(worldPos + Vector3.up * 500f,
                            Vector3.down,
                            out hit,
                            1000f,
                            groundMask))
        {
            if (best == float.MinValue)
                return hit.point.y;

            return Mathf.Max(best, hit.point.y);
        }

        if (best != float.MinValue)
            return best;

        return worldPos.y;
    }

    // BUILD TUBE MESH

    Mesh BuildTubeMesh(List<Vector3> path)
    {
        Mesh mesh = new Mesh();
        mesh.name = "CableMesh";

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int ringCount = path.Count;

        for (int i = 0; i < ringCount; i++)
        {
            Vector3 forward;

            if (i < ringCount - 1)
                forward = (path[i + 1] - path[i]).normalized;
            else
                forward = (path[i] - path[i - 1]).normalized;

            if (forward == Vector3.zero)
                forward = Vector3.forward;

            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            if (right == Vector3.zero) right = Vector3.right;

            Vector3 up = Vector3.Cross(forward, right);

            for (int j = 0; j < radialSegments; j++)
            {
                float angle = (j / (float)radialSegments) * Mathf.PI * 2f;

                Vector3 offset =
                    Mathf.Cos(angle) * right * cableRadius +
                    Mathf.Sin(angle) * up * cableRadius;

                verts.Add(path[i] + offset);
                uvs.Add(new Vector2(j / (float)radialSegments,
                                    i / (float)ringCount));
            }
        }

        for (int i = 0; i < ringCount - 1; i++)
        {
            int ringStart = i * radialSegments;
            int nextRingStart = (i + 1) * radialSegments;

            for (int j = 0; j < radialSegments; j++)
            {
                int current = ringStart + j;
                int next = ringStart + (j + 1) % radialSegments;
                int currentNextRing = nextRingStart + j;
                int nextNextRing = nextRingStart + (j + 1) % radialSegments;

                tris.Add(current);
                tris.Add(next);
                tris.Add(currentNextRing);

                tris.Add(next);
                tris.Add(nextNextRing);
                tris.Add(currentNextRing);
            }
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
