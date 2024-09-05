using System.Collections.Generic;
using UnityEngine;


public static class MeshExt 
{
	public static Mesh GetCombinedMesh(GameObject gameObject)
	{
		return GetMergedMesh(gameObject.GetComponentsInChildren<MeshFilter>(), gameObject.transform);
	}


	public static Mesh GetMergedMesh(MeshFilter[] meshFilters, Transform root)
	{
		Vector3 pivot = root.position;
		
		List<Vector3> vertices  = new List<Vector3>();
		List<Vector3> normals   = new List<Vector3>();
		List<int>     triangles = new List<int>();
		List<Color32> colors    = new List<Color32>();

		for (int i = 0; i < meshFilters.Length; i++)
		{
			Mesh mesh = meshFilters[i].sharedMesh;
			Transform transform = meshFilters[i].transform;

			Vector3[] verts = mesh.vertices;
			Vector3[] norms = mesh.normals;
			int[]     tris  = mesh.triangles;
			Color32[] col = GetOrCreateMeshColors(mesh, false);
			
			int offset = vertices.Count;
			
			for (int v = 0; v < verts.Length; v++)
			{
				vertices.Add(root.InverseTransformPoint(transform.TransformPoint(verts[v]) - pivot));
				normals.Add(root.InverseTransformDirection(transform.TransformDirection(norms[v])));
			}

			
			for (int t = 0; t < tris.Length; t++)
				triangles.Add(offset + tris[t]);


			Material mat = transform.GetComponent<MeshRenderer>().sharedMaterial;
			if (mat.name == "MainMat")
				for (int c = 0; c < col.Length; c++)
					colors.Add(col[c]);
		}
		
		
		Mesh combinedMesh = new Mesh();
		     combinedMesh.SetVertices(vertices);
		     combinedMesh.SetNormals(normals);
		     combinedMesh.SetTriangles(triangles, 0);
		
		if(colors.Count > 0)
			combinedMesh.SetColors(colors);
		
		combinedMesh.RecalculateBounds();

		
		return combinedMesh;
	}


	private static Color32[] GetOrCreateMeshColors(Mesh mesh, bool importedMesh)
	{
		Color32[] colors = mesh.colors32;
		if (colors.Length > 0)
		{
			if(importedMesh)
				for (int i = 0; i < colors.Length; i++)
					colors[i] = new Color32(0, 0, 0, colors[i].r);
		}
		else
		{
			colors = new Color32[mesh.vertices.Length];
			for (int i = 0; i < colors.Length; i++)
				colors[i] = new Color32(0, 0, 0, 255);
		}
		
		return colors;
	}


	public static uint[] GetRenderArgs(this Mesh mesh)
	{
		uint[] renderArgs = new uint[5];
		renderArgs[0] = mesh.GetIndexCount(0);
		renderArgs[2] = mesh.GetIndexStart(0);
		renderArgs[3] = mesh.GetBaseVertex(0);
		return renderArgs;
	}
	
	
	public static void AddRenderArgs(this Mesh mesh, uint[] renderArgs, int offset)
	{
		renderArgs[offset * 5]     = mesh.GetIndexCount(0);
		renderArgs[offset * 5 + 2] = mesh.GetIndexStart(0);
		renderArgs[offset * 5 + 3] = mesh.GetBaseVertex(0);
	}
}
