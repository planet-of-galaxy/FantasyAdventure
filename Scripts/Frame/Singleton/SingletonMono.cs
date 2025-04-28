using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 尝试在场景中找到现有实例
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    // 创建新的 GameObject 并添加组件
                    GameObject go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go); // 确保跨场景不被销毁
                }
            }
            return instance;
        }
    }

    public static bool isInstantiated { get => instance != null;}

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 防止多个实例
        }
    }
}
