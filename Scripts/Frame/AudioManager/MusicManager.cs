using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 处理音乐播放逻辑 如追击音乐等
/// 可以返回一个IPlayable供其他类控制音乐播放
/// </summary>
public class MusicManager : Singleton<MusicManager>
{
    // 声明一个音乐列表 用来控制现在正在播放的音乐
    private List<Music> musics;
    private GameObject gameObj;

    public const string MUSIC_OBJ_NAME = "Music"; // 音乐默认附着的GameObject的名字
    public const string MUSIC_AB_NAME = "music"; // AB包的名字 所有的音乐请放到这个包中

    private bool isPuase; // 渐出任务完成后 暂停or停止 用于函数回调判断
    public override void Init()
    {
        musics = new();
    }
    // 同步创建 默认附着物为gameObj
    public IPlayable CreateMusic(string name, GameObject gobj = null) {
        // 新建一个实例
        Music music = CreateNew(name, gobj);
        music.LoadClip(MUSIC_AB_NAME);
        return music as IPlayable;
    }
    // 异步创建 用委托返回IPlayerable接口
    public void CreateMusicAsyc(string name, Action<IPlayable> callback, GameObject gobj = null)
    {
        // 新建一个实例
        Music music = CreateNew(name, gobj);
        music.loadClipCallBack += callback;
        music.LoadClipAsync(MUSIC_AB_NAME);
    }
    // 提供删除音乐的方法
    public void RemoveMusic(IPlayable playable) {
        for (int i = 0;i<musics.Count;i++) {
            if (musics[i] == playable) {
                musics[i].Clear();
                musics.RemoveAt(i);
                return;
            }
        }
    }

    private Music CreateNew(string name, GameObject gobj = null) {
        // 新建一个实例
        Music music = new Music();
        // 如果传入的附着物为空 使用默认附着物
        if (gobj == null)
        {
            if (gameObj == null)
                gameObj = new GameObject(MUSIC_OBJ_NAME);
            music.gameObject = gameObj;
        }
        else
            music.gameObject = gobj; // 传入的gobj不为空 使用传入的gobj

        music.audio_source = music.gameObject.AddComponent<AudioSource>();

        // 绑定回调
        music.upperCallBack = UpperCallBack;
        music.lowerCallBack = LowerCallBack;
        music.loadClipCallBack = LoadCallBack;

        music.play_state = E_PlayState.E_NONE; // 初始状态
        music.audio_type = E_AudioType.E_MUSIC; // 音乐类型
        music.name = name; // 音乐名字
        music.audio_source.loop = true; // 循环播放

        musics.Add(music); // 加入音乐列表
        return music;
    }

    private void LoadCallBack(IPlayable playable)
    {
        // 没有默认播放的说法 都是由申请者自己控制
    }
    private void UpperCallBack(Music music)
    {
        // 暂时不需要做什么
    }
    private void LowerCallBack(Music music)
    {
        if (isPuase)
            music.Pause();
        else
            music.Stop();
    }
    public bool IsAnyMusicPlaying() {
        for (int i = 0; i < musics.Count; i++) {
            if (musics[i].play_state == E_PlayState.E_UPPER_GRADUALLY ||
                musics[i].play_state == E_PlayState.E_PLAYING) {
                return true;
            }
        }
        return false;
    }

    // 提供更改音量和静音的方法
    public void SetVolume(float volume) {
        // 遍历音乐列表修改音量
        for (int i = 0; i < musics.Count; i++) {
            musics[i].SetVolume(volume);
        }
    }
    public void SetMute(bool mute) {
        // 遍历音乐列表修改静音状态
        for (int i = 0; i < musics.Count; i++)
        {
            musics[i].SetMute(mute);
        }
    }
    public void Clear() {
        // 从后往前遍历 边遍历边删除
        for (int i = musics.Count -1;i>=0;i--) {
            musics[i].Clear();
            musics.RemoveAt(i);
        }
    }
}
