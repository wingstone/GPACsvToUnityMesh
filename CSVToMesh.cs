using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedScarf.EasyCSV;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CSVToMesh : MonoBehaviour
{
    public TextAsset csv_v;
    public TextAsset csv_i;
    public int indexStart;
    public int indexCount;
    public bool readColor;
    public bool readUv0;
    public bool readUv1;
}

[CustomEditor(typeof(CSVToMesh))]
public class CSVToMeshEditor : Editor
{
    CSVToMesh csvToMesh;
    void OnEnable()
    {
        csvToMesh = target as CSVToMesh;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TextAsset csv_v = csvToMesh.csv_v;
        TextAsset csv_i = csvToMesh.csv_i;

        if (GUILayout.Button("Convert to milk mesh"))
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<Vector2> uv0s = new List<Vector2>();
            List<Vector2> uv1s = new List<Vector2>();
            List<int> indices = new List<int>();

            CsvHelper.Init();
            CsvTable table_v;
            table_v = CsvHelper.Create(csv_v.name, csv_v.text);
            int row_v = table_v.RowCount;

            int step = csvToMesh.readColor ? 4 : 3;

            for (int i = 1; i < row_v; i += step)
            {
                float x = float.Parse(table_v.Read(i, 1));
                float y = float.Parse(table_v.Read(i + 1, 1));
                float z = float.Parse(table_v.Read(i + 2, 1));

                vertices.Add(new Vector3(x, y, z));
            }

            if (csvToMesh.readColor)
            {
                for (int i = 1; i < row_v; i += step)
                {
                    float r = float.Parse(table_v.Read(i, 2));
                    float g = float.Parse(table_v.Read(i + 1, 2));
                    float b = float.Parse(table_v.Read(i + 2, 2));
                    float a = float.Parse(table_v.Read(i + 3, 2));

                    colors.Add(new Color(u, v));
                }
            }

            if (csvToMesh.readUv0)
            {
                int column = csvToMesh.readColor ? 3 : 2;
                for (int i = 1; i < row_v; i += step)
                {
                    float u = float.Parse(table_v.Read(i, column));
                    float v = float.Parse(table_v.Read(i + 1, column));

                    uv0s.Add(new Vector2(u, v));
                }
            }
            if (csvToMesh.readUv1)
            {
                int column = csvToMesh.readColor ? 4 : 3;
                for (int i = 1; i < row_v; i += step)
                {
                    float u = float.Parse(table_v.Read(i, column));
                    float v = float.Parse(table_v.Read(i + 1, column));

                    uv1s.Add(new Vector2(u, v));
                }
            }

            CsvTable table_i;
            table_i = CsvHelper.Create(csv_i.name, csv_i.text);
            int row_i = table_i.RowCount;

            int indexEnd = csvToMesh.indexStart + csvToMesh.indexCount + 1;
            indexEnd = indexEnd <= row_i ? indexEnd : row_i;
            for (int i = 1 + csvToMesh.indexStart; i < indexEnd; i++)
            {
                int id = int.Parse(table_i.Read(i, 1));

                indices.Add(id);
            }
            mesh.SetVertices(vertices);
            mesh.SetColors(colors);
            mesh.SetUVs(0, uv0s);
            mesh.SetUVs(1, uv1s);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

            AssetDatabase.CreateAsset(mesh, "Assets/mesh.asset");
        }
    }
}