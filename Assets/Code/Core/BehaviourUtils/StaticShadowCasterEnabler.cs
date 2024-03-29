using UnityEngine;

namespace Code.Core.BehaviourUtils
{
public class StaticShadowCasterEnabler : MonoBehaviour
{
    [field: SerializeField]
    private bool _enable;
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (_enable)
        {
            var componentsInChildren = GetComponentsInChildren<MeshRenderer>();
            EnableStaticMotionAndShadow(componentsInChildren);
            _enable = false;
        }
    }

    private void EnableStaticMotionAndShadow(MeshRenderer[] meshRenderers)
    {
        foreach (var meshRenderer in meshRenderers)
        {
            if (!meshRenderer.gameObject.isStatic)
            {
                continue;
            }
            meshRenderer.staticShadowCaster = true;
            meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        }
    }
    #endif
}
}
