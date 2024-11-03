using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager 
{
    //아직 서버연동을안해서 오브젝트별 ID를 배치하기엔 무리가있어서 주석처리
    //Dictionary<int,GameObject> objcets = new Dictionary<int, GameObject> ();
    List<GameObject> _objects = new List<GameObject>();

    public void Add(GameObject go)
    {
        _objects.Add(go);
    }

    public void Remove(GameObject go)
    {
        _objects.Remove(go);
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach(GameObject obj in _objects)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (cc.CellPos == cellPos)
                return obj;
        }
        return null;
    }

    public GameObject Find(Func<GameObject,bool> condition)
    {
        foreach (GameObject obj in _objects)
        {
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        _objects.Clear();
    }

}
