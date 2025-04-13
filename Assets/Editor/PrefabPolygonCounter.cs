using UnityEngine;
using UnityEditor;
using System.Linq;

public class PrefabPolygonCounter : EditorWindow
{
    [MenuItem("Tools/Count Polygons in Selected Prefab")]
    static void CountPolygons()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("何も選択されていません");
            return;
        }

        int totalTriangles = 0;

        // 選択中のオブジェクトとその子オブジェクトをすべて取得
        var meshes = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>()
            .Select(mf => mf.sharedMesh)
            .Where(mesh => mesh != null);

        totalTriangles += meshes.Sum(mesh => mesh.triangles.Length / 3);

        var skinnedMeshes = Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>()
            .Select(smr => smr.sharedMesh)
            .Where(mesh => mesh != null);

        totalTriangles += skinnedMeshes.Sum(mesh => mesh.triangles.Length / 3);

        Debug.Log($"選択中のPrefabのポリゴン数: {totalTriangles} 三角形");
    }
}
