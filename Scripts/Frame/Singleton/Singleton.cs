public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T instance;

    protected readonly static object lockObj = new object();
    public static T Instance {
        get
        {
            if (instance != null)
                return instance;
            lock (lockObj) {
                if (instance == null)
                    instance = new T();
                instance.Init();
            }
            return instance;
        }
    }

    public abstract void Init();
}
