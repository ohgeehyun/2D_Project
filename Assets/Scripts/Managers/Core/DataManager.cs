using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;


// json으로 작성된 data파일을 읽는 매니저 
public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}


public class DataManager
{
    public void Init()
    {
        //StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
    }

    Loader LoadJson<Loader,Key,Value>(string path) where Loader : ILoader<Key,Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
