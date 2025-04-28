#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 注意：只有在开发时能使用该管理器加载资源 用于开发功能
// 发布后 是无法使用该管理器的 因为它需要用到编辑器相关功能
public class EditorResMgr : Singleton<EditorResMgr>
{
    private string rootPath = "Assets/Editor/ArtRes/";

    public override void Init()
    {

    }

    // 1. 加载单个资源
    public T LoadEditorRes<T>(string path) where T : Object
    {
        string suffixName = "";
        // 预设体、纹理（图片）、材质球、音效等等
        if (typeof(T) == typeof(GameObject))
            suffixName = ".prefab";


        T res = AssetDatabase.LoadAssetAtPath<T>(rootPath + path + suffixName);
        return res;
    }

    // 2. 加载图集相关资源的
    public Sprite LoadSprite(string path, string spriteName)
    {
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        foreach (var item in sprites)
        {
            if (spriteName == item.name)
                return item as Sprite;
        }
        return null;
    }

    // 3. 加载图集中所有子图片并返回给外部
    public Dictionary<string, Sprite> LoadSprites(string path)
    {
        Dictionary<string, Sprite> sprite_dic = new Dictionary<string, Sprite>();
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        foreach (var item in sprites)
        {
            sprite_dic.Add(item.name, item as Sprite);
        }
        return sprite_dic;
    }
}
#endif