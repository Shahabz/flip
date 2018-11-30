using UnityEngine;

namespace Clouds
{
	public class CloudDatas : ScriptableObject
	{
		#region Clouds
		public Vector4 _OptimizeDatas = new Vector4(120, 1, 1, 10);                 //Step,Distance Step,Density,Max Distance
		public Vector4 _DensityParams = new Vector4(-0.5f, -0.9f, 1.75f,0.003f);    //Base,Height,Density,Attenuation
		public Vector2 _HeightDatas = new Vector2(0, 3);                            //Base Height,Thickness
		public Vector3 _WindSpeed = new Vector3(0, -0.3f, -1);
		public Vector4 _QScales = new Vector4(2.02f, 2.03f, 2.01f, 2.02f);         //Q Scales

		public Color _AmbientColor = new Color(0.65f, 0.7f, 0.75f);
		public float _AmbientParam = 1.4f;
		public Color _DiffuseColor = new Color(1.0f, 0.6f, 0.3f);
		
		public Color _HighDensityColor = new Color(0.25f, 0.3f, 0.35f);
		public Color _LowDensityColor = new Color(1.0f, 0.95f, 0.8f);
		#endregion

		#region Sky
		public Color _SkyColor = new Color(0.47f, 0.71f, 0.78f);
		public Color _GroundColor = new Color(0.48f, 0.60f, 0.68f);
		public float _SkyBase = 0.075f;

		public Color _SunColor = new Color(1.0f, 0.6f, 0.1f);
		public float _SunPower = 58;
		public float _SunIntensity = 0.35f;

		public float _SunGlowPower = 3.0f;
		public float _SunGlowIntensity = 0.2f;
		public Color _SunGlowColor = new Color(1.0f, 0.4f, 0.2f);

		public Vector3 _SunRotation = new Vector3(178.3f,-395.91f,27.8f);
		#endregion

		public float _TransitionTime = 1;

		public CloudDatas Clone()
		{
			CloudDatas ret = CreateInstance<CloudDatas>();
			ret._OptimizeDatas = _OptimizeDatas;
			ret._DensityParams = _DensityParams;
			ret._HeightDatas = _HeightDatas;
			ret._WindSpeed = _WindSpeed;
			ret._QScales = _QScales;

			ret._AmbientColor = _AmbientColor;
			ret._DiffuseColor = _DiffuseColor;
			ret._HighDensityColor = _HighDensityColor;
			ret._LowDensityColor = _LowDensityColor;

			ret._SkyColor = _SkyColor;
			ret._GroundColor = _GroundColor;
			ret._SunColor = _SunColor;

			ret._SkyBase = _SkyBase;
			ret._SunPower = _SunPower;
			ret._SunIntensity = _SunIntensity;

			ret._SunGlowPower = _SunGlowPower;
			ret._SunGlowIntensity = _SunGlowIntensity;
			ret._SunGlowColor = _SunGlowColor;

			ret._SunRotation = _SunRotation;

			ret._TransitionTime = _TransitionTime;

			return ret;
		}

		public static void Transition(CloudDatas preData,CloudDatas nextData,CloudDatas transData,float percent)
		{
			transData._OptimizeDatas = Vector4.Lerp(preData._OptimizeDatas, nextData._OptimizeDatas, percent);
			transData._DensityParams = Vector4.Lerp(preData._DensityParams, nextData._DensityParams, percent);
			transData._HeightDatas = Vector2.Lerp(preData._HeightDatas, nextData._HeightDatas, percent);
			transData._WindSpeed = Vector3.Lerp(preData._WindSpeed, nextData._WindSpeed, percent);

			transData._QScales = Vector4.Lerp(preData._QScales, nextData._QScales, percent);

			transData._AmbientColor = Color.Lerp(preData._AmbientColor, nextData._AmbientColor, percent);
			transData._DiffuseColor = Color.Lerp(preData._DiffuseColor, nextData._DiffuseColor, percent);
			transData._HighDensityColor = Color.Lerp(preData._HighDensityColor, nextData._HighDensityColor, percent);
			transData._LowDensityColor = Color.Lerp(preData._LowDensityColor, nextData._LowDensityColor, percent);

			transData._SkyColor = Color.Lerp(preData._SkyColor, nextData._SkyColor, percent);
			transData._GroundColor = Color.Lerp(preData._GroundColor, nextData._GroundColor, percent);
			transData._SunColor = Color.Lerp(preData._SunColor, nextData._SunColor, percent);

			transData._SkyBase = Mathf.Lerp(preData._SkyBase, nextData._SkyBase, percent);
			transData._SunPower = Mathf.Lerp(preData._SunPower, nextData._SunPower, percent);
			transData._SunIntensity = Mathf.Lerp(preData._SunIntensity, nextData._SunIntensity, percent);

			transData._SunGlowPower = Mathf.Lerp(preData._SunGlowPower, nextData._SunGlowPower, percent);
			transData._SunGlowIntensity = Mathf.Lerp(preData._SunGlowIntensity, nextData._SunGlowIntensity, percent);
			transData._SunGlowColor = Color.Lerp(preData._SunGlowColor, nextData._SunGlowColor, percent);

			transData._SunRotation = Vector3.Lerp(preData._SunRotation, nextData._SunRotation, percent);
		}
	}
}