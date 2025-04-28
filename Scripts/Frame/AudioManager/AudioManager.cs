using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioData
{
    public float music_volume = 0.2f;
    public float effect_volume = 0.2f;
    public bool music_isMute = false;
    public bool effect_isMute = false;

    public void Deconstruct(out bool music_isMute, out float music_volume, out bool effect_isMute, out float effect_volume) =>
        (music_isMute, music_volume, effect_isMute, effect_volume) = (this.music_isMute, this.music_volume, this.effect_isMute, this.effect_volume);
}

public class AudioManager : Singleton<AudioManager>
{
    public AudioData current_audiodata;
    public const float GRADUALLY_MIN_VOLUME = 0.01f; // 渐变的最小音量 到达这个音量后就会自动停止
    private List<Music> musics = new(); // 音乐附着的游戏物体
    private GameObject music_gameObject;
    /// <summary>
    ///  内部类 为了精确控制音乐的播放状态
    ///  audio_source： 音频源
    ///  coroutine：当音乐需要渐变时，使用协程来控制音量的变化
    ///  callBack: 渐变完成时的回调函数
    /// </summary>

    public override void Init()
    {
        current_audiodata = JsonMgr.Instance.LoadData<AudioData>("AudioData");
    }
    private void Save()
    {
        JsonMgr.Instance.SaveData(current_audiodata, "AudioData");
    }
    // 调节整体音乐的音量大小 并自动保存
    public void SetMusicVolume(float volume)
    {
        current_audiodata.music_volume = volume;

        BackMusicManager.Instance.SetVolume(volume);
        MusicManager.Instance.SetVolume(volume);
        Save();
    }
    // 调节整体音效的音量大小 并自动保存
    public void SetEffectVolume(float volume)
    {
        current_audiodata.effect_volume = volume;
        Save();
    }
    // 调节整体音乐的静音状态 并自动保存
    public void SetMusicMute(bool isMute)
    {
        current_audiodata.music_isMute = isMute;

        BackMusicManager.Instance.SetMute(isMute);
        MusicManager.Instance.SetMute(isMute);
        Save();
    }

    public void SetEffectMute(bool isMute)
    {
        current_audiodata.effect_isMute = isMute;
        Save();
    }

    public AudioData GetCurrentData()
    {
        return current_audiodata;
    }

    // 背景音相关方法
    public void PlayBackMusic(string name, bool isGradually = true) {
        // 如果有音乐正在播放 那么就只是加载音乐 否则直接播放
        if (MusicManager.Instance.IsAnyMusicPlaying())
            BackMusicManager.Instance.LoadBackMusic(name);
        else
            BackMusicManager.Instance.PlayBackMusic(name, isGradually);
    }
    public void PlayBackMusic(bool isGradually = true) {
        // 如果没有音乐正在播放 那么就播放 否则等待
        if (!MusicManager.Instance.IsAnyMusicPlaying())
            BackMusicManager.Instance.Play(isGradually);
        else
            Debug.Log("无法恢复背景 因为有音乐正在播放");
    }
    public void StopBackMusic(bool isGradually = true)
    {
        BackMusicManager.Instance.Stop(isGradually);
    }
    // 音乐相关方法
    public IPlayable CreateMusic(string name, GameObject gameObj = null) {
        return MusicManager.Instance.CreateMusic(name, gameObj);
    }
    public void CreateMusicAsync(string name,Action<IPlayable> callback, GameObject gameObj = null) {
        MusicManager.Instance.CreateMusicAsyc(name,callback, gameObj);
    }
    public void RemoveMusic(IPlayable playable) {
        MusicManager.Instance.RemoveMusic(playable);
    }
    public bool IsAnyMusicPlaying() {
        bool ret = false;
        // 渐减的音乐除外 因为它马上就要关闭了
        for (int i = 0; i < musics.Count; i++) {
            if (musics[i].play_state == E_PlayState.E_PLAYING ||
                musics[i].play_state == E_PlayState.E_UPPER_GRADUALLY)
            { ret = true; break; }

        }
        return ret;
    }
    // 音效相关方法
    public void PlaySoundEffect(string path) {
        SoundEffectManager.Instance.CreateSountEffect(path);
    }
    // 过场景用 置空背景音和音乐列表
    public void Clear() {
        MusicManager.Instance.Clear();
        BackMusicManager.Instance.Clear();
        SoundEffectManager.Instance.Clear();
    }
}
