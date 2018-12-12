using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;  

public class Perspective : MonoBehaviour {  

	// 所有障碍物的Renderer数组  
	private List<MeshRenderer> _ObstacleCollider;  

	// 不能遮挡的目标 
	public GameObject _target; 


	// 临时接收，用于存储  
	private MeshRenderer _tempRenderer;  

	[SerializeField]
	Material atlas_alphaMat;
	[SerializeField]
	Material atlasMat;
	[SerializeField]
	Material car_alphaMat;
	[SerializeField]
	Material carMat;
	[SerializeField]
	Material[] brige;
	[SerializeField]
	Material mkMat;
	[SerializeField]
	Material mk_alphaMat;

	static Perspective instance;
	public static Perspective Instance{
		get { return instance; }
	}   
	private void Awake()
	{
		instance = this;
	}

	void Start()  
	{  
		_ObstacleCollider = new List<MeshRenderer>();  
		StartCoroutine (CheckObstacle (_target));
	}  

	void Update()  
	{  
		// 调试使用：红色射线，仅Scene场景可见     
		#if UNITY_EDITOR  
		//Debug.DrawLine(_target.transform.position+new Vector3(0,1,0), transform.position, Color.red);  
		//Debug.DrawLine(_targetRing.transform.position+new Vector3(0,1,0), transform.position, Color.red);  
		#endif  
	}  

	//根据名字设置材质
	void SetMaterial(string name,MeshRenderer renderer,bool atlas){
		if (name.StartsWith("Atlas1")&&atlas) {
			renderer.material = atlas_alphaMat;
		}
		if (name.StartsWith("atlas_Alpha")&&!atlas) {
			renderer.material = atlasMat;
		}
		if (name.StartsWith("cars")&&atlas) {
			renderer.material = car_alphaMat;
		}
		if (name.StartsWith("cars_Alpha")&&!atlas) {
			renderer.material = carMat;
		}
		if (name.StartsWith("mk4_logo")&&atlas) {
			renderer.material = mk_alphaMat;
		}
		if (name.StartsWith("mk4_Alpha")&&!atlas) {
			renderer.material = mkMat;
		}
	}
		

	//通过射线检测摄像机与目标之间是否有遮挡，有则赋予透明材质，无则复原材质
	public IEnumerator CheckObstacle(GameObject target){
		while (true) {
			RaycastHit[] hit;  
			Vector3 transPos = transform.position;

			if (target == null) {
				yield break;
			}

			Vector3 targetPos = target.transform.position + new Vector3 (0, 1f, 0);
			hit = Physics.RaycastAll (transPos, (targetPos - transPos), Vector3.Distance (transPos, targetPos));  

			if (hit.Length > 0) {   
				for (int i = 0; i < hit.Length; i++) {  
					_tempRenderer = hit [i].collider.gameObject.GetComponent<MeshRenderer> (); 
					if (!_tempRenderer) {
						Transform collParent = hit [i].collider.transform.parent;
						if (collParent)
							_tempRenderer = collParent.GetComponent<MeshRenderer> (); 
					}
					if (_tempRenderer) {
						_ObstacleCollider.Add (_tempRenderer);	
						SetMaterial (_tempRenderer.material.name, _tempRenderer, true);

						int length = _tempRenderer.materials.Length;
						if (length > 1) {
							Material[] targetMaterial = new Material[length];
							for (int index = 0; index < length; index++) {
								targetMaterial [index] = atlas_alphaMat;
							}
							_tempRenderer.materials = targetMaterial;
						}
						//Debug.Log (hit [i].collider.name);
					}
				}  					
			} else {  
				for (int i = 0; i < _ObstacleCollider.Count; i++) {  
					_tempRenderer = _ObstacleCollider [i];  
					if (_tempRenderer) {
						SetMaterial (_tempRenderer.material.name, _tempRenderer, false);
					
						int length = _tempRenderer.materials.Length;
						if (length > 1) {
							Material[] targetMaterial = new Material[length];
							for (int index = 0; index < length; index++) {
								targetMaterial [index] = atlasMat;
							}
							_tempRenderer.materials = targetMaterial;
						}
						if (_tempRenderer.name == "Street_el1") {
							_tempRenderer.materials = brige;
						}
					}
				}  
			}
			
			yield return new WaitForSeconds(0.05f);
			
		}
	}
}  

