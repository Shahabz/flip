using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Clouds
{
	//[ExecuteInEditMode]
	public class Clouds : PostEffectsBase
	{
		public Shader _CloudsShader = null;
		public Texture2D _NoiseTexture = null;
		public Material _Sky_Material = null;

		//public int _Down_Sampling = 1;

		CloudDatas _Pre_Cloud_Sky_Datas = null;
		CloudDatas _Next_Cloud_Sky_Datas = null;
		CloudDatas _Transition_Cloud_Sky_Datas = null;

		Material _CloudsMaterial = null;
		float _TransitionTime = 0;
		bool  _In_Transition = false;

		Light _Dir_Light = null;

		private Camera mainCamera
		{
			get
			{
				var cam = GetComponent<Camera>();
				return cam;
			}
		}

		public override bool CheckResources()
		{
			CheckSupport(false);

			_CloudsMaterial = CheckShaderAndCreateMaterial(_CloudsShader, _CloudsMaterial);

			if (!isSupported)
				ReportAutoDisable();
			return isSupported;
		}

		public void OnEnable()
		{
			mainCamera.depthTextureMode = DepthTextureMode.Depth;

			if(_Transition_Cloud_Sky_Datas==null)
			{
				_Transition_Cloud_Sky_Datas = ScriptableObject.CreateInstance<CloudDatas>();
			}

			if (_Sky_Material!=null)
			{
				RenderSettings.skybox = _Sky_Material;
			}

			var lights = FindObjectsOfType<Light>();
			//fix me...
			//just find the first direction light
			_Dir_Light = null;
			for(int i=0;i<lights.Length;i++)
			{
				var light = lights[i];
				if(light.type==LightType.Directional)
				{
					_Dir_Light = light;
					break;
				}
			}
		}

		public void OnDisable()
		{
			if (_CloudsMaterial)
				DestroyImmediate(_CloudsMaterial);
		}

		void Update()
		{
			if(_In_Transition)
			{
				_TransitionTime += Time.deltaTime;
				float trans_percent = _TransitionTime / _Next_Cloud_Sky_Datas._TransitionTime;
				trans_percent = Mathf.Clamp01(trans_percent);
				CloudDatas.Transition(_Pre_Cloud_Sky_Datas, _Next_Cloud_Sky_Datas, _Transition_Cloud_Sky_Datas, trans_percent);
				if(trans_percent>=1)
				{
					_In_Transition = false;
					_TransitionTime = 0;
					_Pre_Cloud_Sky_Datas = null;
				}
			}

			if (null != _Dir_Light)
			{
				_Dir_Light.transform.eulerAngles = CurCloudSkyDatas._SunRotation;
			}

			if (null != _Sky_Material)
			{
				_Sky_Material.SetColor("_SkyColor", CurCloudSkyDatas._SkyColor);
				_Sky_Material.SetColor("_GroundColor", CurCloudSkyDatas._GroundColor);
				_Sky_Material.SetColor("_SunColor", CurCloudSkyDatas._SunColor);

				_Sky_Material.SetFloat("_Base", CurCloudSkyDatas._SkyBase);
				_Sky_Material.SetFloat("_SunPower", CurCloudSkyDatas._SunPower);
				_Sky_Material.SetFloat("_SunIntensity", CurCloudSkyDatas._SunIntensity);
			}
		}

       
        public CloudDatas coludDatas;

		public CloudDatas CurCloudSkyDatas
		{
			get
			{
                //if(_In_Transition)
                //{
                //	return _Transition_Cloud_Sky_Datas;
                //}
                //else
                //{
                //	CloudDatas retData = null;
                //	if(_Next_Cloud_Sky_Datas!=null)
                //	{
                //		retData = _Next_Cloud_Sky_Datas;
                //	}
                //	else if(_Transition_Cloud_Sky_Datas!=null)
                //	{
                //		retData = _Transition_Cloud_Sky_Datas;
                //	}
                //	return retData;
                //}
                return coludDatas;
			}
			set
			{
				if(CurCloudSkyDatas!=value)
				{
					if (_In_Transition)
					{
						_Pre_Cloud_Sky_Datas = _Transition_Cloud_Sky_Datas.Clone();
#if UNITY_EDITOR
						_Next_Cloud_Sky_Datas = value.Clone();  //prevent editing the cloud datas
#else
						_Next_Cloud_Sky_Datas = value;

#endif
						_TransitionTime = 0;
					}
					else
					{
						CloudDatas retData = null;
						if (_Next_Cloud_Sky_Datas != null)
						{
							retData = _Next_Cloud_Sky_Datas;
						}
						else if (_Transition_Cloud_Sky_Datas != null)
						{
							retData = _Transition_Cloud_Sky_Datas.Clone();
						}
						_Pre_Cloud_Sky_Datas = retData;
#if UNITY_EDITOR
						_Next_Cloud_Sky_Datas = value.Clone();  //prevent editing the cloud datas
#else
						_Next_Cloud_Sky_Datas = value;

#endif
						_TransitionTime = 0;
						_In_Transition = true;
					}
				}
			}
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (CheckResources() == false)
			{
				Graphics.Blit(source, destination);
				return;
			}

			if (UnityEngine.XR.XRSettings.enabled)
			{
				_CloudsMaterial.SetMatrix("_ClipToWorld", mainCamera.cameraToWorldMatrix);
			}
			else
			{
				_CloudsMaterial.SetMatrix("_ClipToWorld", mainCamera.cameraToWorldMatrix * mainCamera.projectionMatrix.inverse);
			}

			Vector4 processed_opti_datas = CurCloudSkyDatas._OptimizeDatas;

			CurCloudSkyDatas._OptimizeDatas.z = Mathf.Clamp(CurCloudSkyDatas._OptimizeDatas.z, 0.01f, 100.0f);

			processed_opti_datas.x = Mathf.Max(0, processed_opti_datas.x);
			processed_opti_datas.x = 1.0f / (1.0f + processed_opti_datas.x);
			processed_opti_datas.y = Mathf.Max(0, processed_opti_datas.y);
			processed_opti_datas.y = 1.0f / (1.0f + processed_opti_datas.y);
			_CloudsMaterial.SetVector("_OptimizeDatas", processed_opti_datas);
			_CloudsMaterial.SetVector("_DensityParams", CurCloudSkyDatas._DensityParams);
			_CloudsMaterial.SetVector("_HeightDatas", CurCloudSkyDatas._HeightDatas);
			_CloudsMaterial.SetVector("_QScales", CurCloudSkyDatas._QScales);

			_CloudsMaterial.SetVector("_WindSpeed", CurCloudSkyDatas._WindSpeed);
			if(_NoiseTexture!=null)
			{
				_CloudsMaterial.SetTexture("_NoiseTex", _NoiseTexture);
			}

			_CloudsMaterial.SetColor("_AmbientColor", CurCloudSkyDatas._AmbientColor);
			_CloudsMaterial.SetFloat("_AmbientParam", CurCloudSkyDatas._AmbientParam);
			_CloudsMaterial.SetColor("_DiffuseColor", CurCloudSkyDatas._DiffuseColor);
			_CloudsMaterial.SetColor("_HighDensityColor", CurCloudSkyDatas._HighDensityColor);
			_CloudsMaterial.SetColor("_LowDensityColor", CurCloudSkyDatas._LowDensityColor);

			_CloudsMaterial.SetColor("_SunGlowColor", CurCloudSkyDatas._SunGlowColor);
			_CloudsMaterial.SetFloat("_SunGlowPower", CurCloudSkyDatas._SunGlowPower);
			_CloudsMaterial.SetFloat("_SunGlowIntensity", CurCloudSkyDatas._SunGlowIntensity);

			source.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, destination, _CloudsMaterial, 0);
		}
	}
}