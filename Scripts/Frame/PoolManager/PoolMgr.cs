using System.Collections.Generic;
using UnityEngine;

public class PoolData {
    private Queue<GameObject> pool;
    private GameObject father;

    public PoolData() {
        pool = new Queue<GameObject>();
    }

    public int Count => pool.Count;
    
    public GameObject GetObj() {
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }
    //仅添加obg
    public void Add(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
    //添加obj并设置父子关系
    public void AddChild(GameObject obj, GameObject grandfather) {
        if (father == null) {
            father = new GameObject();
            father.transform.parent = grandfather.transform;
            father.name = obj.name + "s";
        }
        obj.transform.parent = father.transform;
        Add(obj);
    }
}
public class PoolMgr : Singleton<PoolMgr>
{
    public const string POOL_NAME = "Pool";
    public int max;
    private Dictionary<string, PoolData> pool = new Dictionary<string, PoolData>();
    private GameObject poolObj;

    public bool isLayout = true;

    public override void Init()
    {
        if (isLayout)
        {
            poolObj = new GameObject();
            poolObj.name = POOL_NAME;
        }
    }

    public GameObject GetObj(string path) {
        GameObject obj;
        if (pool.ContainsKey(path) && pool[path].Count > 0)
        {
            obj = pool[path].GetObj();
        }
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(path));
            obj.name = path;
        }
        obj.transform.parent = null;
        return obj;
    }

    public void PushObj(string path, GameObject obj) {

        if (!pool.ContainsKey(path))
        {
            pool.Add(path, new PoolData());
        }

        if (isLayout)
        {
            pool[path].AddChild(obj, poolObj);
        }
        else {
            pool[path].Add(obj);
        }
            
    }
    public void ClearPool()
    {
        pool.Clear();
        GameObject.Destroy(poolObj);
    }
}
