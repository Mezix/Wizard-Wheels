using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System.Linq;

namespace Modern2D
{

	[ExecuteAlways]
	[ExecuteInEditMode]
	[AddComponentMenu("Light/2D Stylized Lighting")]
	public class LightingSystem : MonoBehaviour
	{
		void Start()
		{
			gradText = new Texture2D(256, 1);
			SetCallbacks();
			OnShadowSettingsChanged();
		}


		#region Singleton
		private void Awake()
		{
			Singleton();
			spritePivotCorrections = pivotCorrections.ToDictionary(t => t.key, t => t.val);
			spritePivotCorrectionsSpriteSheet = pivotCorrectionsSpriteSheet.ToDictionary(t => t.key, t => t.val);
		}

		[SerializeField] public static LightingSystem system;


        void OnEnable() => Singleton();
        void OnDisable()  => Singleton();
        public void Singleton()
		{
			if (system == null || system.gameObject == this.gameObject)
				system = this;
			else if (system.gameObject != this.gameObject)
				if (Application.isPlaying)
					Destroy(this);
		}
		#endregion

		#region Containers

		public List<DictPair<Texture, Vector2>> pivotCorrections = new List<DictPair<Texture, Vector2>>();
		Dictionary<Texture, Vector2> _spritePivotCorrections;

		/// <summary>
		/// used for correcting spriteNewPivots pivot positions by manually cached Vector2 value
		/// </summary>
		public Dictionary<Texture, Vector2> spritePivotCorrections
		{
			get
			{
				if (_spritePivotCorrections == null)
					_spritePivotCorrections = new Dictionary<Texture, Vector2>();
				return _spritePivotCorrections;
			}
			set { _spritePivotCorrections = value; }
		}

		public List<DictPair<Sprite, Vector2>> pivotCorrectionsSpriteSheet = new List<DictPair<Sprite, Vector2>>();
		Dictionary<Sprite, Vector2> _spritePivotCorrectionsSpriteSheet;

		/// <summary>
		/// used for correcting spriteNewPivots pivot positions by manually cached Vector2 value
		/// </summary>
		public Dictionary<Sprite, Vector2> spritePivotCorrectionsSpriteSheet
		{
			get
			{
				if (_spritePivotCorrectionsSpriteSheet == null)
					_spritePivotCorrectionsSpriteSheet = new Dictionary<Sprite, Vector2>();
				return _spritePivotCorrectionsSpriteSheet;
			}
			set { _spritePivotCorrectionsSpriteSheet = value; }
		}

		//used for corrected pivot points
		Dictionary<Texture, Vector2> _spriteNewPivots;

		/// <summary>
		/// used for placing shadows at the bottom of sprites
		/// </summary>
		Dictionary<Texture, Vector2> spriteNewPivots
		{
			get
			{
				if (_spriteNewPivots == null)
					_spriteNewPivots = new Dictionary<Texture, Vector2>();
				return _spriteNewPivots;
			}
			set { _spriteNewPivots = value; }
		}

		//used for corrected pivot points
		Dictionary<Sprite, Vector2> _spriteNewPivotsSpriteSheets;

		/// <summary>
		/// used for placing shadows at the bottom of sprites
		/// </summary>
		Dictionary<Sprite, Vector2> spriteNewPivotsSpriteSheets
		{
			get
			{
				if (_spriteNewPivotsSpriteSheets == null)
					_spriteNewPivotsSpriteSheets = new Dictionary<Sprite, Vector2>();
				return _spriteNewPivotsSpriteSheets;
			}
			set { _spriteNewPivotsSpriteSheets = value; }
		}

		Dictionary<Transform, StylizedShadowCaster> _shadows;
		Dictionary<Transform, StylizedShadowCaster> shadows
		{
			get
			{
				if (_shadows == null)
					_shadows = new Dictionary<Transform, StylizedShadowCaster>();
				return _shadows;
			}
			set { _shadows = value; }
		}

		#endregion

		#region variables

		[Header("Light Source Settings")]

		[Tooltip("decides if light comes from one direction(true), or from a certain source(false)")]
		[HideInInspector] public Cryo<bool> isLightDirectional;
		[Tooltip("if light is Directional, it comes from this angle")]
		[HideInInspector] public Cryo<float> directionalLightAngle;
		[Tooltip("if light is from a source, it comes from this source")]
		public Transform source;

        [HideInInspector] public Cryo<bool> enableBlur = new Cryo<bool>(false);

        [HideInInspector] public Cryo<int> blurSampleSize = new Cryo<int>(4);

        [HideInInspector] public Cryo<float> blurStrength = new Cryo<float>(1);

        [HideInInspector] public Cryo<Vector2> blurDirection = new Cryo<Vector2>(new Vector2(1,1));

		[HideInInspector] public Cryo<string> defaultShadowLayer;

        [Header("Global Shadow Settings")]

		[Tooltip("Change it to disable and enable shadows")]
		[HideInInspector] public static Cryo<bool> showShadows;

		[Tooltip("Color that's applied to shadow color calculation and other shaders")]
		[HideInInspector] public Cryo<Color> _shadowColor;

		[Tooltip("special abstract property of the shadow that's responsible for the illusion of shadow reflecting shadowcaster")]
		[HideInInspector] public Cryo<float> _shadowReflectiveness;

		[Tooltip("Alpha of shadow color that's applied to shadow color calculation and other shaders")]
		[HideInInspector] public Cryo<float> _shadowAlpha;

		[Tooltip("Angle of the drop shadow in shadowcasters")]
		[HideInInspector] public Cryo<float> _shadowAngle;

		[Tooltip("Shadow Length of the drop shadow in shadowcasters")]
		[HideInInspector] public Cryo<float> _shadowLength;

		[Tooltip("Shadow Narrowing of the drop shadow in shadowcasters")]
		[HideInInspector] public Cryo<float> _shadowNarrowing;

		[Tooltip("Shadow Falloff of the drop shadow in shadowcasters")]
		[HideInInspector] public Cryo<float> _shadowFalloff;

		[Header("DEPENDENCIES")]

		[Tooltip("Material that determines how the shadows are rendered")]
		[SerializeField] public Material _shadowsMaterial;

		[SerializeField] public Material dropShadowDefaultMaterial;

		[Tooltip("Treshold for omitting transparent elements on sprites")]
		[SerializeField] int pivotDetectionAlphaTreshold = 122;

		[SerializeField] public BoxCollider2D enterCollider;
		[SerializeField] public BoxCollider2D exitCollider;

		[HideInInspector] public Cryo<Vector2> distMinMax;
		[HideInInspector] public Cryo<Vector2> shadowLengthMinMax;
		[HideInInspector] public Transform followPlayer;
		[HideInInspector] public bool extendedUpdateThisFrame = false;
		[Tooltip("Changes the default shadow-pivot source from auto-generated to source-sprite pivot")]
		[HideInInspector] public Cryo<bool> _useSpritePivotForShadowPivot;

		[Tooltip("Flips your shadow sprite depending on its direction (90-270degrees -> flip-x true , 270-90 degrees -> flip-x-false) ")]
		[HideInInspector] [SerializeField] public static bool shadowSprFlip = false;
		[Tooltip("Deafult shadow sprite x orientation")]
		[HideInInspector] [SerializeField] public static bool defaultShadowSprflipx = false;

		#endregion

		#region Callbacks

		/// <summary>
		/// sets the callbacks for realtime editor editing
		/// </summary>
		public void SetCallbacks()
		{
			directionalLightAngle.onValueChanged = OnShadowSettingsChanged;
			_shadowColor.onValueChanged = OnShadowSettingsChanged;
			_shadowReflectiveness.onValueChanged = OnShadowSettingsChanged;
			_shadowAlpha.onValueChanged = OnShadowSettingsChanged;
			_shadowAngle.onValueChanged = OnShadowSettingsChanged;
			_shadowLength.onValueChanged = OnShadowSettingsChanged;
			_shadowNarrowing.onValueChanged = OnShadowSettingsChanged;
			_shadowFalloff.onValueChanged = OnShadowSettingsChanged;
            enableBlur.onValueChanged = OnShadowSettingsChanged;
            blurStrength.onValueChanged = OnShadowSettingsChanged;
            blurSampleSize.onValueChanged = OnShadowSettingsChanged;
            blurDirection.onValueChanged = OnShadowSettingsChanged;
			_useSpritePivotForShadowPivot.onValueChanged = OnPivotSettingsChanged;
		}

		Texture2D gradText;

		/// <summary>
		/// updates global shadow settings after they changed (the floor is made out of floor)
		/// </summary>
		public void OnShadowSettingsChanged()
		{

			spritePivotCorrections = pivotCorrections.ToDictionary(t => t.key, t => t.val);
			spritePivotCorrectionsSpriteSheet = pivotCorrectionsSpriteSheet.ToDictionary(t => t.key, t => t.val);

			_shadowsMaterial.SetColor("_shadowBaseColor", _shadowColor.value);
			_shadowsMaterial.SetFloat("_shadowBaseAlpha", _shadowAlpha.value);
			_shadowsMaterial.SetFloat("_shadowReflectiveness", _shadowReflectiveness.value);
			_shadowsMaterial.SetFloat("_shadowNarrowing", _shadowNarrowing.value);
			_shadowsMaterial.SetFloat("_shadowFalloff", _shadowFalloff.value);

			_shadowsMaterial.SetInt("_enableBlur", enableBlur.value ? 1  : 0 );
			_shadowsMaterial.SetFloat("_blurStrength", !enableBlur ? 0 : blurStrength.value);
			_shadowsMaterial.SetInt("_blurArea", !enableBlur ? 0 : blurSampleSize.value);
			_shadowsMaterial.SetVector("_blurDir", !enableBlur ? Vector2.zero : blurDirection.value);


			_shadowsMaterial.SetFloat("_directional", isLightDirectional.value == true ? 1 : 0);
			_shadowsMaterial.SetVector("_distMinMax", distMinMax.value);

			if (source != null)
				_shadowsMaterial.SetVector("_source", source.transform.position);

			extendedUpdateThisFrame = true;
			UpdateShadows(null);

		}

		/// <summary>
		/// same as OnShadowSettingsChanged but for shadow pivot
		/// </summary>
		public void OnPivotSettingsChanged() 
		{
			foreach (StylizedShadowCaster2D c in GameObject.FindObjectsOfType<StylizedShadowCaster2D>())
            {
				if(c.overrideCustomPivot.value == false)
                {
					c.customPivot = _useSpritePivotForShadowPivot.value == false ? PivotSourceMode.sprite : PivotSourceMode.auto;
					c.RebuildShadow();
				}
            }
			extendedUpdateThisFrame = true;
			UpdateShadows(null);
		}

		#endregion

		#region realtimeUpdates

		string collisionTag = "Shadow";
		public void ColliderEnter(Collider2D collision)
		{
			if (collision.gameObject.tag == collisionTag)
			{
				AddShadow(collision.transform.parent.GetComponent<StylizedShadowCaster2D>().shadowData);
			}
		}

		public void ColliderExit(Collider2D collision)
		{
			if (collision.gameObject.tag == collisionTag)
			{
				shadows.Remove(collision.transform.parent);
			}
		}

		private void Update()
		{
			_shadowsMaterial.SetMatrix("_camProj", Camera.main.projectionMatrix);
			_shadowsMaterial.SetMatrix("_camWorldToCam", Camera.main.worldToCameraMatrix);

			if (followPlayer != null)
				transform.position = followPlayer.position;

			if (source != null)
				_shadowsMaterial.SetVector("_source", source.transform.position);
			UpdateShadows(null);
		}

		//	shadow clean buffer with precomputed size in order to avoid real-time data allocation
		//	(in order to avoid GC fps drops)
		//  if you are considering removing more than 65536 shadows in one frame, 
		//	then you're crazy so don't bother changing the size lol
		//  (lower if memory is important)

		StylizedShadowCaster[] casters = new StylizedShadowCaster[65536];
		Dictionary<Transform, StylizedShadowCaster> data = new Dictionary<Transform, StylizedShadowCaster>();

		public void UpdateShadows(Dictionary<Transform, StylizedShadowCaster> dict)
		{


			Profiler.BeginSample("Update Shadows");
			int i = 0;
			//update shadows
			if (dict == null)
				i = UpdateShadowList(shadows);
			else
			{
				extendedUpdateThisFrame = true;
				i = UpdateShadowList(dict);
			}
			Profiler.BeginSample("Clean Shadows");

			//clean
			for (int k = 0; k < casters.Length && k < i; k++)
				shadows.Remove(casters[k].shadowCaster);

			extendedUpdateThisFrame = false;

			Profiler.EndSample();
			Profiler.EndSample();
		}
		
		/// <summary>
		/// updates an array of shadows and returns the last index of deletion buffer
		/// </summary>
		/// <param name="shadows"></param>
		/// <param name=""></param>
		/// <returns></returns>
		int UpdateShadowList(Dictionary<Transform, StylizedShadowCaster> shadows)
		{
			int i = -1;
			foreach (var shadowPair in shadows)
			{
				var shadow = shadowPair.Value;
				if (shadow.shadow == null)
				{
					i++;
					casters[i] = shadow;
				}
				else
				{

					//update shadow sprite
					shadow.shadowSr.sprite = shadow.shadowCasterSr.sprite;
					SetShadowXOrientation(shadow);
					if (!isLightDirectional.value)
					{
						shadow.shadowPivot.rotation = Quaternion.RotateTowards(shadow.shadowPivot.rotation, Quaternion.Euler(_shadowAngle.value, shadow.shadowPivot.transform.rotation.eulerAngles.y, Quaternion.AngleAxis(AngleToLightSource(shadow.shadowPivot.position), Vector3.forward).eulerAngles.z), 1000 * Time.deltaTime);
						shadow.shadow.SetGlobalScale( new Vector3(shadow.shadowCaster.transform.lossyScale.x, SourceShadowLength(shadow.shadowPivot.position) * _shadowLength.value, (shadow.shadowCaster.transform.lossyScale.x)));
						SetShadowPivotPos(shadow);
					}

					//gate to more fps consuming operations
					if (!extendedUpdateThisFrame)
						continue;

					//update shadow rotation
					shadow.shadowPivot.rotation = Quaternion.AngleAxis(AngleToLightSource(shadow.shadowPivot.position), Vector3.forward);

					//update shadow angle
					if (shadow.shadowPivot.transform.rotation.eulerAngles.x != _shadowAngle.value)
						shadow.shadowPivot.transform.rotation = Quaternion.Euler(_shadowAngle.value, shadow.shadowPivot.transform.rotation.eulerAngles.y, shadow.shadowPivot.transform.rotation.eulerAngles.z);
					//update shadow 
					if (shadow.shadow.transform.localScale.y != _shadowLength.value)
					{
						shadow.shadow.SetGlobalScale(new Vector3(shadow.shadowCaster.transform.lossyScale.x, shadow.shadowCaster.transform.lossyScale.y *_shadowLength.value , shadow.shadowCaster.transform.lossyScale.z));
						SetShadowPivotPos(shadow);
					}
				}
			}
			return i;
		}

		#endregion

		#region AddingShadowCasters

		/// <summary>
		/// adds or overwrites cached shadow in shadows dictionary and updates the shadow settings
		/// </summary>
		/// <param name="data"></param>
		public void AddShadow(ShadowData data)
		{
			if (shadows.ContainsKey(data.shadow.shadowCaster))
				shadows[data.shadow.shadowCaster] = data.shadow;
			else
				shadows.Add(data.shadow.shadowCaster, data.shadow);
			UpdateShadowPositions(data.shadow);
		}

		private void UpdateShadowPositions(StylizedShadowCaster shadowCaster)
		{
			shadowCaster.shadowSr.sortingLayerName = !shadowCaster.customShadowLayer ? "Shadows" : shadowCaster.shadowLayer;
			Transform shadow = shadowCaster.shadow;
			SetShadowPosition(shadowCaster, shadow, shadowCaster.shadowPivot, shadowCaster.shadowCaster, shadowCaster.shadowCasterSr.sprite);
		}

		private void SetShadowXOrientation(StylizedShadowCaster shadow)
        {
			if (shadowSprFlip)
			{
				if (Mathf.Abs(shadow.shadowPivot.eulerAngles.z % 360) < 90 || Mathf.Abs(shadow.shadowPivot.eulerAngles.z % 360) > 270) shadow.shadowSr.flipX = (shadow.flipX ? !defaultShadowSprflipx : defaultShadowSprflipx);
				else shadow.shadowSr.flipX = (shadow.flipX ? !defaultShadowSprflipx : defaultShadowSprflipx);
			}
			else shadow.shadowSr.flipX = (shadow.flipX ? !defaultShadowSprflipx : defaultShadowSprflipx);
		}

		private void SetShadowPivotPos(StylizedShadowCaster shadowCaster)
        {
			//pivot pos
			switch (shadowCaster.pivotMode)
			{
				case PivotSourceMode.auto:
					shadowCaster.shadowPivot.position = (Vector2)shadowCaster.shadowCaster.position - (GetSpritePivot(shadowCaster.shadowCasterSr.sprite) * new Vector2(1, _shadowLength.value)) + GetSpritePivotCorrection(shadowCaster.shadowCasterSr.sprite);
					break;
				case PivotSourceMode.sprite:
					shadowCaster.shadowPivot.position = (Vector2)shadowCaster.shadowCaster.position;
					break;
				case PivotSourceMode.custom:
					shadowCaster.shadowPivot.position = (Vector2)shadowCaster.pivotObject.position;
					break;
			}
		}
		
		private void SetShadowPos(StylizedShadowCaster shadowCaster)
		{
			//pivot pos
			switch (shadowCaster.pivotMode)
			{
				case PivotSourceMode.auto:
					shadowCaster.shadow.localPosition = GetSpritePivot(shadowCaster.shadowCasterSr.sprite) + GetSpritePivotCorrection(shadowCaster.shadowCasterSr.sprite);
					break;
				case PivotSourceMode.sprite:
					shadowCaster.shadow.localPosition = Vector2.zero;
					break;
				case PivotSourceMode.custom:
					shadowCaster.shadow.localPosition = (Vector2)shadowCaster.shadowCaster.position - (Vector2)shadowCaster.pivotObject.position;
					break;
			}
		}

		private void SetShadowPosition(StylizedShadowCaster shadowCaster, Transform shadow, Transform pivotT, Transform originalObject, Sprite t)
		{
			Sprite sprite = originalObject.GetComponent<SpriteRenderer>().sprite;
			SetShadowPivotPos(shadowCaster);
			pivotT.rotation = Quaternion.Euler(_shadowAngle.value, 0, 0);
			SetShadowPos(shadowCaster);
			shadow.SetGlobalScale(new Vector3(originalObject.lossyScale.x, originalObject.lossyScale.y*_shadowLength.value, originalObject.lossyScale.z));
			pivotT.rotation = Quaternion.AngleAxis(AngleToLightSource(pivotT.position), Vector3.forward);

			//update shadow angle
			if (pivotT.transform.rotation.eulerAngles.x != _shadowAngle.value)
				pivotT.transform.rotation = Quaternion.Euler(_shadowAngle.value, pivotT.transform.rotation.eulerAngles.y, pivotT.transform.rotation.eulerAngles.z);
        }

        Vector2 GetSpritePivotCorrection(Sprite t) 
		{
			if (spritePivotCorrections.ContainsKey(t.texture) && !spritePivotCorrectionsSpriteSheet.ContainsKey(t)) return spritePivotCorrections[t.texture]; 
			if (spritePivotCorrectionsSpriteSheet.ContainsKey(t)) return spritePivotCorrectionsSpriteSheet[t];
			return Vector2.zero;
		}

		private float AngleToLightSource(Vector3 point)
		{
			if (isLightDirectional.value || (!isLightDirectional.value && source == null))
				return directionalLightAngle.value;

			var direction = (source.position - point).normalized;
			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
			return (angle + 360) % 360;
		}

		private float DistanceToLightSourceClamped(Vector3 point)
		{
			return Mathf.Clamp(Vector2.Distance(point, source.position), distMinMax.value.x, distMinMax.value.y);
		}

		private float SourceShadowLength(Vector3 point)
		{
			float distance = DistanceToLightSourceClamped(point);
			return Mathf.SmoothStep(shadowLengthMinMax.value.x, shadowLengthMinMax.value.y, distance / distMinMax.value.y);
		}


		#endregion

		private bool IsSpriteFromSpriteSheet(Sprite s) 
		{
			if (s.rect.width >= s.texture.width && s.rect.height >= s.texture.height)
				return false;
			return true;
		
		}

		#region pivot

		public Vector2 GetSpritePivotSpriteSheet(Sprite org) 
		{
			
			//sprite sheets
			if (!spriteNewPivotsSpriteSheets.ContainsKey(org))
			{
				//delete spritesheet texture from pivots if it was placed by accident, as it will overshadow our pivots.
				if (spriteNewPivots.ContainsKey(org.texture))
					spriteNewPivots.Remove(org.texture);

				Vector2 pos = new Vector2(0, 0.5f);
				var col = org.texture.GetPixelData<Color32>(0);
				Color32[] colors = new Color32[(int)org.rect.width * ((int)org.rect.height+1)];

				int wX = (int)org.rect.xMax - (int)org.rect.xMin;

				for (int y = (int)org.rect.yMin; y < (int)org.rect.yMax; y++)
					for (int x = (int)org.rect.xMin; x < (int)org.rect.xMax; x++)
					{
						colors[(x - (int)org.rect.xMin) + ((y - (int)org.rect.yMin) * wX)] = col[x + (y * (int)org.texture.width)];
					}
				int width = (int)org.rect.width;

				//get the pivot by iterating sprite texture and getting the average of the last row of pixels
				for (int i = 0; i < colors.Length; i++)
					if (colors[i].a > pivotDetectionAlphaTreshold)
					{
						//get whole row average
						int sum = 0;
						int m = 0;
						for (int k = i; k < (width - (i % width)) + i; k++)
							if (colors[k].a > pivotDetectionAlphaTreshold)
							{
								m += 1;
								sum += k;
							}
						int medianIdx = sum / m;

						pos = new Vector2(medianIdx % width, medianIdx / width);
						break;
					}

				pos = new Vector2(Mathf.Lerp(0, 1, pos.x / (float)org.rect.width), Mathf.Lerp(0, 1, pos.y / (float)org.rect.height));

				Vector2 pivot = new Vector2(org.pivot.x / org.rect.width, org.pivot.y / org.rect.height);
				Vector2 offsetVec = pivot - pos;

				//multiply the pivot displacement vector by world sprite size
				Vector3 spriteNewPivotOffset = new Vector3(org.bounds.size.x * offsetVec.x, org.bounds.size.y * offsetVec.y);
				spriteNewPivotsSpriteSheets[org] = spriteNewPivotOffset;
			}

			return spriteNewPivotsSpriteSheets[org];
		}

		/// <summary>
		/// automatically calculates a new pivot on the bottom center of the sprite, so you don't need to do this manually
		/// </summary>
		/// <param name="org"></param>
		/// <returns></returns>
		public Vector2 GetSpritePivot(Sprite org)
		{

			if (IsSpriteFromSpriteSheet(org))
				return GetSpritePivotSpriteSheet(org);

			if (!spriteNewPivots.ContainsKey(org.texture))
			{
				Vector2 pos = new Vector2(0, 0.5f);
				Color32[] colors = org.texture.GetPixels32();
				int width = org.texture.width;

				//get the pivot by iterating sprite texture and getting the average of the last row of pixels
				for (int i = 0; i < colors.Length; i++)
					if (colors[i].a > pivotDetectionAlphaTreshold)
					{
						//get whole row average
						int sum = 0;
						int m = 0;
						for (int k = i; k < (width - (i % width)) + i; k++)
							if (colors[k].a > pivotDetectionAlphaTreshold)
							{
								m += 1;
								sum += k;
							}
						int medianIdx = sum / m;

						pos = new Vector2(medianIdx % width, medianIdx / width);
						break;
					}

				pos = new Vector2(Mathf.Lerp(0, 1, pos.x / (float)org.rect.width), Mathf.Lerp(0, 1, pos.y / (float)org.rect.height));

				Vector2 pivot = new Vector2(org.pivot.x / org.texture.width, org.pivot.y / org.texture.height);
				Vector2 offsetVec = pivot - pos;

				//multiply the pivot displacement vector by world sprite size
				Vector3 spriteNewPivotOffset = new Vector3(org.bounds.size.x * offsetVec.x, org.bounds.size.y * offsetVec.y);
				spriteNewPivots[org.texture] = spriteNewPivotOffset;
			}

			return spriteNewPivots[org.texture];
		}

        #endregion

    }

}
