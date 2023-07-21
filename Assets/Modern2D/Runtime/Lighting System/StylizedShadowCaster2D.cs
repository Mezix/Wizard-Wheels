using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Modern2D
{

    //  shadow type that is used by stylized lighting 2D system
    //  stylized lighting 2D system needs to be setup beforehand
    //  it also needs an "Shadows" sprite sorting layer and "Shadow" tag
    //  for detailed tutorial please read the whole setup section in my documentation

    public class StylizedShadowCaster2D : MonoBehaviour
    {
        [SerializeField] private ShadowData _shadowData;

        [SerializeField] [HideInInspector] public Cryo<bool> flipShadowX;

        [Tooltip("Color that's applied to shadow color calculation and other shaders")]
        [SerializeField] [HideInInspector] public Cryo<Color> _shadowColor;

        [Tooltip("special abstract property of the shadow that's responsible for the illusion of shadow reflecting shadowcaster")]
        [SerializeField] [HideInInspector] public Cryo<float> _shadowReflectiveness;

        [Tooltip("Alpha of shadow color that's applied to shadow color calculation and other shaders")]
        [SerializeField] [HideInInspector] public Cryo<float> _shadowAlpha;

        [Tooltip("Shadow Narrowing of the drop shadow in shadowcasters")]
        [SerializeField] [HideInInspector] public Cryo<float> _shadowNarrowing;

        [Tooltip("Shadow Falloff of the drop shadow in shadowcasters")]
        [SerializeField] [HideInInspector] public Cryo<float> _shadowFalloff;

        [SerializeField] [HideInInspector] public Cryo<bool> customShadowLayer;

        [SerializeField] [HideInInspector] public Cryo<string> customShadowLayerName;

        [SerializeField] [HideInInspector] public bool extendedProperties;

        [SerializeField] [HideInInspector] private MaterialPropertyBlock _propBlock;

        [SerializeField] [HideInInspector] public Cryo<bool> overrideCustomPivot;

        [SerializeField] [HideInInspector] public PivotSourceMode customPivot;

        [SerializeField] [HideInInspector] public Transform customPivotTransform;

        public void SetCallbacks()
        {
            _shadowColor.onValueChanged = SetPropBlock;
            _shadowReflectiveness.onValueChanged = SetPropBlock;
            _shadowAlpha.onValueChanged = SetPropBlock;
            _shadowNarrowing.onValueChanged = SetPropBlock;
            _shadowFalloff.onValueChanged = SetPropBlock;
            overrideCustomPivot.onValueChanged = PivotOptionsChanged;
            flipShadowX.onValueChanged = PivotOptionsChanged;
        }

        public void PivotOptionsChanged() 
        {
            customPivot = LightingSystem.system._useSpritePivotForShadowPivot.value == true ? PivotSourceMode.sprite : PivotSourceMode.auto;
            RebuildShadow();
        }

        public void Start()
        {
            if (extendedProperties) SetPropBlock();
            CreateShadow();
        }

        private void OnValidate()
        {
            if (extendedProperties) SetPropBlock();
        }

        public void SetPropBlock()
        {
            if (!extendedProperties) return;
            _propBlock = new MaterialPropertyBlock();

            if (shadowData.shadow.shadowSr == null || shadowData.shadow.shadowSr.sprite == null)
            {
                Debug.LogError("Can't change properties : create a shadow first!");
                return;
            }

            shadowData.shadow.shadowSr.GetPropertyBlock(_propBlock);

            _propBlock.SetColor("_shadowBaseColor", _shadowColor.value);
            _propBlock.SetFloat("_shadowBaseAlpha", _shadowAlpha.value);
            _propBlock.SetFloat("_shadowReflectiveness", _shadowReflectiveness.value);
            _propBlock.SetFloat("_shadowNarrowing", _shadowNarrowing.value);
            _propBlock.SetFloat("_shadowFalloff", _shadowFalloff.value);

            shadowData.shadow.shadowSr.SetPropertyBlock(_propBlock);
        }


        /// <summary>
        /// if shadow wasn't created, automatically creates one
        /// </summary>
        public ShadowData shadowData
        {
            get { if (_shadowData == null || _shadowData.shadow.shadowPivot == null) _shadowData = CreateShadowData(); return _shadowData; }
            set { _shadowData = value; }

        }

        /// <summary>
        /// creates shadow or returns existent one
        /// </summary>
        public void CreateShadow()
        {
            if(CanCreateShadow()) LightingSystem.system.AddShadow(shadowData);
        }

        /// <summary>
        /// if shadow exists, it destroys it and creates a new one
        /// </summary>
        public void RebuildShadow()
        {
            if (!Application.isPlaying && shadowData!= null && shadowData.shadow.shadowPivot!=null)
                DestroyImmediate(shadowData.shadow.shadowPivot.gameObject);
            if (CanCreateShadow()) LightingSystem.system.AddShadow(shadowData);
        }

        /// <summary>
        /// checks if object have a shadow, omitting shadowData getter
        /// </summary>
        /// <returns></returns>
        public bool HasShadow() => _shadowData == null;

        public bool CanCreateShadow() 
        {
            if (GetComponent<SpriteRenderer>()==null) return false;
            if (GetComponent<SpriteRenderer>().sprite == null) return false;
            return true;
        }

        //needed if you use a prefarb in the scene, as prefarbs can't save scriptable objects (dirty fix)
        private void CleanOldShadow() 
        {
            foreach(var child in GetComponentsInChildren<Transform>())
            {
                if (child.tag == "Shadow")
                {   
                    if (Application.isPlaying) Destroy(child.gameObject);
                    else DestroyImmediate(child.gameObject);
                }
            }
        }

        private ShadowData CreateShadowData()
        {

            if(!CanCreateShadow()) return null;
            ShadowData data = (ShadowData)ScriptableObject.CreateInstance(typeof(ShadowData));

            StylizedShadowCaster caster = new StylizedShadowCaster(transform, null, null, null, Vector2.zero, customPivot,customPivotTransform, flipShadowX.value, customShadowLayer, customShadowLayerName);

            GameObject parent = caster.shadowCaster.gameObject;
            GameObject shadowGO = new GameObject(parent.name + " : shadow");
            CreatePivot(ref caster);

            caster.shadowPivot.parent = parent.transform;
            caster.shadow = shadowGO.transform;
            caster.shadow.parent = caster.shadowPivot.transform;
            shadowGO.transform.localRotation = Quaternion.identity;
            shadowGO.transform.localPosition = Vector3.zero;

            caster.shadowSr = shadowGO.AddComponent<SpriteRenderer>();
            caster.shadowSr.sortingLayerName = !customShadowLayer.value ?  "Shadows" : customShadowLayerName.value;
            caster.shadowSr.sortingOrder = 1;

            caster.shadowSr.material = LightingSystem.system._shadowsMaterial;
            caster.shadowCasterSr = parent.GetComponent<SpriteRenderer>();
            caster.shadowSr.sprite = caster.shadowCasterSr.sprite;

            if (LightingSystem.shadowSprFlip)
            {
                if (Mathf.Abs(caster.shadowPivot.eulerAngles.z % 360) < 90 || Mathf.Abs(caster.shadowPivot.eulerAngles.z % 360) > 270) caster.shadowSr.flipX = LightingSystem.defaultShadowSprflipx;
                else caster.shadowSr.flipX = !LightingSystem.defaultShadowSprflipx;
            }
            else caster.shadowSr.flipX = LightingSystem.defaultShadowSprflipx;

            caster.shadow.SetGlobalScale(caster.shadowCaster.lossyScale);

            data.shadow = caster;

            return data;
        }

        /// <summary>
        /// creates or reuses already created pivot
        /// </summary>
        /// <param name="caster"></param>
        private void CreatePivot(ref StylizedShadowCaster caster)
        {
            for (int i = 0; i < transform.childCount; i++)
                if (transform.GetChild(i).tag == "Shadow")
                {
                    caster.shadowPivot = transform.GetChild(i);
                    return;
                }

            GameObject shadowPivot = new GameObject(caster.shadowCaster.name + " : shadowPivot");
            caster.shadowPivot = shadowPivot.transform;
            caster.shadowPivot.tag = "Shadow";

            CircleCollider2D c = caster.shadowPivot.gameObject.AddComponent<CircleCollider2D>();
            c.isTrigger = true;
            c.radius = 0.3f;

            caster.shadowPivot.parent = caster.shadowCaster;
        }




    }

    public static class TransformExtensionMethods
    {
        public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
        }
    }



    /// <summary>
    /// Source of the shadow pivot point : 
    /// auto -> auto generation with algorithm (default)
    /// sprite -> takes sprite source pivot as shadow pivot
    /// custom -> you can set the pivot yourself via setting Transform Point in Unity Inspector
    /// </summary>
    public enum PivotSourceMode
    {
        auto,
        sprite,
        custom
    }

}
