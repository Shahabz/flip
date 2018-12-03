using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RingManager : MonoBehaviour {

    public Transform[] targetRing;
    public GameObject hole;

    static RingManager instance;
    public static RingManager Instance{
        get { return instance; }
    }
    private void Awake()
    {
        instance = this;
    }

    IEnumerator MoveRing(float time,List<Transform> trans)
    {
        int length = trans.Count;
        while (length > 0)
        {           
            trans[length - 1].DOLocalMoveY(trans[length - 1].localPosition.y - 20, time, false);
            length--;
            yield return new WaitForSeconds(time - 0.05f);
        }
    }

    public void GenerateRings(Vector3 pos)
    {

        List<Transform> rings = new List<Transform>();
        foreach (Transform trans in targetRing)
        {
            GameObject go = Instantiate(trans.gameObject, transform);
            go.transform.position = pos + new Vector3(0, 20, 0);
            rings.Add(go.transform);
        }
        //PlayerController.Instance.currentRing = rings[0];

        StartCoroutine(MoveRing(0.4f, rings));

    }

	public Transform GenerateHole(Vector3 pos)
    {
		return Instantiate(hole.transform, pos, Quaternion.identity);
        //hole.transform.position = pos;
        //hole.transform.DOLocalMoveY(hole.transform.localPosition.y - 20, 0.3f, false);
    }
    
}
