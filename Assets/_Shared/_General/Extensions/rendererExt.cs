using UnityEngine;
using UnityEngine.Rendering;


public static class rendererExt 
{
	public static void Simplify(this MeshRenderer renderer, bool disableShadows = true)
	{
		if (disableShadows)
		{
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
		}
		
		renderer.lightProbeUsage            =            LightProbeUsage.Off;
		renderer.reflectionProbeUsage       =       ReflectionProbeUsage.Off;
		renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
	}
	
	
	public static void Simplify(this SkinnedMeshRenderer renderer, bool disableShadows = true)
	{
		if (disableShadows)
		{
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
		}
		
		renderer.lightProbeUsage            =            LightProbeUsage.Off;
		renderer.reflectionProbeUsage       =       ReflectionProbeUsage.Off;
		renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		renderer.skinnedMotionVectors       = false;

		renderer.updateWhenOffscreen = true;
	}
}
