using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
class BaseItem { }
class Item<T> : BaseItem where T : class
{
    public T myObject;
    public Action<T> callback;

    public Item (Action<T> callback) {
        this.callback += callback;
    }
}
public class ResMgr : Singleton<ResMgr>
{
    private Dictionary<string, BaseItem> assets = new Dictionary<string, BaseItem>();
    public void LoadAsync<T>(string path,Action<T> callback) where T : UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        if (assets.ContainsKey(resName) && (assets[resName] as Item<T>).myObject != null)
        {
            callback.Invoke((assets[resName] as Item<T>).myObject);
        }
        else if (assets.ContainsKey(resName))
        {
            (assets[resName] as Item<T>).callback += callback;
        }
        else {
            Item<T> item = new Item<T>(callback);
            assets.Add(resName, item);
            MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
    }

    private IEnumerator ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
    {
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        yield return rq;

        string resName = path + "_" + typeof(T).Name;
        T ret = rq.asset as T;
        Item<T> item = assets[resName] as Item<T>;
        item.myObject = ret;
        item.callback?.Invoke(ret);
        item.callback = null;
    }

    public void UnLoadUnusedAssetsAsync(Action callback = null)
    {
        assets.Clear();
        MonoMgr.Instance.StartCoroutine(ReallyUnLoadUnusedAssetsAsync(callback));
    }

    private IEnumerator ReallyUnLoadUnusedAssetsAsync(Action callback)
    {
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callback?.Invoke();
    }

    public override void Init()
    {

    }
}
