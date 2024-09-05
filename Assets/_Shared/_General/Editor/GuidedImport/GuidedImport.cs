using UnityEditor;


public class GuidedImport : AssetPostprocessor
{
	private const string Guide = "ImportGuide_";


	private static bool GetGuide(string assetPath, out string guidePath)
	{
		if (!assetPath.Contains(Guide))
		{
			string[] parts = assetPath.Split('/');
			string folder = "";
			for (int i = 0; i < parts.Length - 1; i++)
				folder += parts[i] + "/";
		
			string[] matches = Assets.FindMatchingAssets(new []{ folder, Guide});
			if (matches.Length > 0)
			{
				guidePath = matches[0];
				return true;
			}
		}
		
		
		guidePath = "";
		return false;
	}
	
	private void OnPreprocessModel()
	{
		ModelImporter importer = assetImporter as ModelImporter;
		
		if(!GetGuide(importer.assetPath, out string guidePath))
			return;

		ModelImporter guide = (ModelImporter)AssetImporter.GetAtPath(guidePath);

		importer.globalScale          = guide.globalScale;
		importer.isReadable           = guide.isReadable;
		importer.animationType        = guide.animationType;
		importer.importTangents       = guide.importTangents;
		importer.optimizeMeshPolygons = guide.optimizeMeshPolygons;
		importer.optimizeMeshVertices = guide.optimizeMeshVertices;
		importer.materialImportMode   = guide.materialImportMode;
		importer.importLights         = guide.importLights;
		importer.importCameras        = guide.importCameras;
		importer.importBlendShapes    = guide.importBlendShapes;
		importer.importVisibility     = guide.importVisibility;
		importer.weldVertices         = guide.weldVertices;
		importer.sortHierarchyByName  = guide.sortHierarchyByName;
		importer.meshCompression      = guide.meshCompression;
		importer.importNormals        = guide.importNormals;
		importer.materialImportMode   = guide.materialImportMode;
	}


	/*private void OnPreprocessTexture()
	{
		TextureImporter importer = assetImporter as TextureImporter;
		
		if(!GetGuide(importer.assetPath, out string guidePath))
			return;
		
		TextureImporter guide = (TextureImporter)AssetImporter.GetAtPath(guidePath);
		
		importer.wrapMode   = guide.wrapMode;
		importer.anisoLevel = guide.anisoLevel;
		importer.filterMode = guide.filterMode;
		importer.isReadable = guide.isReadable;
	}*/
}