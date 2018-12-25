using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMerge : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MergeMesh ();
	}

	private void MergeMesh()
	{		
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter> ();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		Material mat = meshFilters [1].GetComponent<MeshRenderer> ().sharedMaterial;
		//Material[] mats = new Material[meshFilters.Length];
		Matrix4x4 matrix = transform.worldToLocalMatrix;
		for (int i = 0; i < meshFilters.Length; i++) {
			MeshFilter mf = meshFilters [i];
			MeshRenderer mr = meshFilters [i].GetComponent<MeshRenderer> ();
			if (mr == null) {
				continue;
			}
			combine[i].mesh = mf.sharedMesh;
			combine[i].transform = matrix * mf.transform.localToWorldMatrix;
			mr.enabled = false;
			//mats [i] = mr.sharedMaterial;
		}
		MeshFilter thisMeshFilter = GetComponent<MeshFilter> ();
		Mesh mesh = new Mesh ();
		mesh.name = "Combined";
		thisMeshFilter.mesh = mesh;
		mesh.CombineMeshes (combine, true);
		MeshRenderer thisMeshRenderer = GetComponent<MeshRenderer> ();
		//thisMeshRenderer.sharedMaterials = mats;
		thisMeshRenderer.material = mat;
		thisMeshRenderer.enabled = true;

		MeshCollider thisMeshCollider = GetComponent<MeshCollider> ();
		if (thisMeshCollider != null) {
			thisMeshCollider.sharedMesh = mesh;
		}
	}

}
