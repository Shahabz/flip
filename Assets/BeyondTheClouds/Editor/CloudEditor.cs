using UnityEngine;
using UnityEditor;

namespace Clouds
{
	[CustomEditor(typeof(Clouds))]
	public class CloudEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			Clouds clouds_target = target as Clouds;
			CloudDatas clouds_datas = clouds_target.CurCloudSkyDatas;

			DrawDefaultInspector();

			clouds_target.CurCloudSkyDatas = EditorGUILayout.ObjectField("Active Datas", clouds_target.CurCloudSkyDatas, typeof(CloudDatas),false) as CloudDatas;

			if(clouds_datas!=null)
			{
				EditorGUI.BeginChangeCheck();
				clouds_datas._OptimizeDatas.x = EditorGUILayout.Slider("Step", clouds_datas._OptimizeDatas.x, 0, 200);
				//clouds_datas._OptimizeDatas.z = EditorGUILayout.Slider("Density", clouds_datas._OptimizeDatas.z, 0.0f, 10.0f);
				clouds_datas._OptimizeDatas.w = EditorGUILayout.Slider("Max Distace", clouds_datas._OptimizeDatas.w, 5.0f, 100.0f);

				clouds_datas._DensityParams.x = EditorGUILayout.Slider("Density Function(Base)", clouds_datas._DensityParams.x, -5.0f, 5.0f);
				clouds_datas._DensityParams.y = EditorGUILayout.Slider("Density Function(Height)", clouds_datas._DensityParams.y, -5.0f, 5.0f);
				clouds_datas._DensityParams.z = EditorGUILayout.Slider("Density Function(Density)", clouds_datas._DensityParams.z, -5.0f, 5.0f);
				clouds_datas._DensityParams.w = EditorGUILayout.Slider("Attenuation", clouds_datas._DensityParams.w, 0, 0.1f);

				clouds_datas._HeightDatas.x = EditorGUILayout.Slider("Base Height", clouds_datas._HeightDatas.x, -5.0f, 5.0f);
				clouds_datas._HeightDatas.y = EditorGUILayout.Slider("Thickness", clouds_datas._HeightDatas.y, 0.0f, 5.0f);

				clouds_datas._WindSpeed = EditorGUILayout.Vector3Field("Wind Speed", clouds_datas._WindSpeed);

				clouds_datas._AmbientColor = EditorGUILayout.ColorField("Ambient Color", clouds_datas._AmbientColor);
				clouds_datas._AmbientParam = EditorGUILayout.Slider("Ambient Param", clouds_datas._AmbientParam,0.1f,2.0f);
				clouds_datas._DiffuseColor = EditorGUILayout.ColorField("Diffuse Color", clouds_datas._DiffuseColor);

				clouds_datas._LowDensityColor = EditorGUILayout.ColorField("Low Density Color", clouds_datas._LowDensityColor);
				clouds_datas._HighDensityColor = EditorGUILayout.ColorField("High Density Color", clouds_datas._HighDensityColor);

				clouds_datas._SkyColor = EditorGUILayout.ColorField("Sky Color", clouds_datas._SkyColor);
				clouds_datas._GroundColor = EditorGUILayout.ColorField("Ground Color", clouds_datas._GroundColor);
				clouds_datas._SkyBase = EditorGUILayout.Slider("Sky Base", clouds_datas._SkyBase, 0.0f, 0.2f);

				clouds_datas._SunRotation = EditorGUILayout.Vector3Field("Sun Rotation", clouds_datas._SunRotation);

				clouds_datas._SunColor = EditorGUILayout.ColorField("Sun Color", clouds_datas._SunColor);
				clouds_datas._SunPower = EditorGUILayout.Slider("Sun Power", clouds_datas._SunPower, 0.0f, 256.0f);
				clouds_datas._SunIntensity = EditorGUILayout.Slider("Sun Intensity", clouds_datas._SunIntensity, 0.0f, 1.0f);

				clouds_datas._SunGlowColor = EditorGUILayout.ColorField("Sun Glow Color", clouds_datas._SunGlowColor);
				clouds_datas._SunGlowPower = EditorGUILayout.Slider("Sun Glow Power", clouds_datas._SunGlowPower, 0.0f, 16.0f);
				clouds_datas._SunGlowIntensity = EditorGUILayout.Slider("Sun Glow Intensity", clouds_datas._SunGlowIntensity, 0.0f, 1.0f);

				clouds_datas._TransitionTime = EditorGUILayout.Slider("Transition Time", clouds_datas._TransitionTime, 0, 10);

				if(EditorGUI.EndChangeCheck())
				{
					//SceneView.RepaintAll();
					//UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
				}				

				if(GUILayout.Button("Save"))
				{
					string strPath = EditorUtility.SaveFilePanelInProject("Save Clouds Data", "NewCloudsData.asset", "asset", "Save Clouds Data To");
					if(null!=strPath && strPath.Length>0)
					{
						var tmp_datas = clouds_datas.Clone();
						AssetDatabase.CreateAsset(tmp_datas, strPath);
						AssetDatabase.Refresh();
					}
				}
			}
		}
	}
}