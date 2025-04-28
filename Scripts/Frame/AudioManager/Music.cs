using System;
using UnityEngine;
using UnityEngine.Rendering;
// 音频类型
public enum E_AudioType
{
    E_NONE = -1,
    E_BACK_MUSIC,
    E_MUSIC,
    E_SOUND_EFFECT,
}
// 音频状态
public enum E_PlayState
{
    E_NONE = -1,
    E_PLAYING,
    E_UPPER_GRADUALLY,
    E_LOWER_GRADUALLY,
    E_PAUSE,
    E_STOP,
    E_LOADING,
    E_READY,
}
public class Music : IPlayable
{
    public AudioSource audio_source; // 音源
    public Action<Music> upperCallBack; // 渐进的回调 应该由所属Manager进行处理
    public Action<Music> lowerCallBack; // 渐出的回调 应该由所属Manager进行处理
    public Action<IPlayable> loadClipCallBack; // 异步加载的回调 应该由所属Manager进行处理
    public E_PlayState play_state; // 播放状态
    public E_AudioType audio_type; // 音频类型
    public GameObject gameObject; // 附着的游戏物体
    public string name; // 音频名字
    public bool usePool = false; // 是否使用了对象池 默认不使用 该字段仅用于标识 逻辑需手动实现
    public bool DEBUG_LOG = false;

    private Coroutine coroutine; // 音量渐变的协程

    public void LoadClipAsync(string ABName)
    {
        if (DEBUG_LOG)
            Debug.Log("LoadClipAsync");

        play_state = E_PlayState.E_LOADING;
        ABMgr.Instance.LoadResAsync<AudioClip>(ABName, name, LoadClipCallback);
    }
    public void LoadClip(string ABName)
    {
        if (DEBUG_LOG)
            Debug.Log("LoadClip");

        ABMgr.Instance.LoadRes<AudioClip>(ABName, name);
        play_state = E_PlayState.E_READY;
    }
    public void LoadClipCallback(AudioClip clip)
    {
        if (DEBUG_LOG)
            Debug.Log("LoadClipCallback");

        if (clip == null) {
            Debug.Log("加载失败！！");
            play_state = E_PlayState.E_NONE;
            return;
        }
        play_state = E_PlayState.E_READY;
        audio_source.clip = clip;
        audio_source.clip.LoadAudioData();
        loadClipCallBack?.Invoke(this);
    }
    // 实现播放接口 直接以设置音量播放
    public void Play()
    {
        if (DEBUG_LOG)
            Debug.Log("Play");

        if (play_state == E_PlayState.E_NONE)
            return;

        // 播放音乐时 关闭背景音乐
        if (audio_type == E_AudioType.E_MUSIC)
            AudioManager.Instance.StopBackMusic();

        CancelCoroutine();

        audio_source.volume = AudioManager.Instance.GetCurrentData().music_volume;
        audio_source.Play();


        play_state = E_PlayState.E_PLAYING;
    }
    // 实现暂停接口
    public void Pause()
    {
        if (DEBUG_LOG)
            Debug.Log("Pause");

        if (play_state == E_PlayState.E_NONE)
            return;

        play_state = E_PlayState.E_PAUSE;
        // 暂停音乐时 恢复背景音乐

        CancelCoroutine();
        audio_source.Pause();

        // 最后再改变背景音乐的状态 因为我们没暂停完成的话 背景音乐是不能被恢复的
        if (audio_type == E_AudioType.E_MUSIC)
            AudioManager.Instance.PlayBackMusic();
    }
    // 实现停止接口
    public void Stop()
    {
        if (DEBUG_LOG)
            Debug.Log("Stop");

        if (play_state == E_PlayState.E_NONE)
            return;

        play_state = E_PlayState.E_STOP;
        CancelCoroutine();
        audio_source?.Stop();

        // 最后再改变背景音乐的状态 因为我们没停止完成的话 背景音乐是不能被恢复的
        if (audio_type == E_AudioType.E_MUSIC)
            AudioManager.Instance.PlayBackMusic();
    }
    // 实现渐进接口

    public void GraduallyUpper()
    {
        if (DEBUG_LOG)
            Debug.Log("GraduallyUpper");

        if (play_state == E_PlayState.E_NONE)
            return;

        // 播放音乐时 关闭背景音乐
        if (audio_type == E_AudioType.E_MUSIC)
            AudioManager.Instance.StopBackMusic();

        // 如果正在进行音量渐变 那么取消它
        if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
            CancelCoroutine();

        // 设置当前状态为渐增
        play_state = E_PlayState.E_UPPER_GRADUALLY;
        audio_source.volume = 0;
        audio_source.Play();
        // 开启协程 让音乐渐变到最大
        coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, AudioManager.Instance.GetCurrentData().music_volume, SetUpperVolume);
    }
    // 实现渐出效果
    public void GraduallyLower()
    {
        if (DEBUG_LOG)
            Debug.Log("GraduallyLower");

        if (play_state == E_PlayState.E_NONE)
            return;

        // 如果正在进行音量渐变 那么取消它
        if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
            CancelCoroutine();

        play_state = E_PlayState.E_LOWER_GRADUALLY;
        // 开启协程 让音乐渐变到0 不过实际上音量到达最小音量（AudioManager.GRADUALLY_MIN_VOLUME）时就会自动停止
        coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, 0, SetLowerVolume);
    }
    // 封装设置音量和静音的方法
    public void SetVolume(float volume) {
        CancelCoroutine();
        if (audio_source != null)
            audio_source.volume = volume;
    }
    public void SetMute(bool isMute) {
        CancelCoroutine();
        if (audio_source != null)
            audio_source.mute = isMute;
    }
    public void Clear() {
        if (DEBUG_LOG)
            Debug.Log("Clear");
        Stop(); // 先停止播放并关闭协程
        GameObject.Destroy(audio_source); // 销毁AudioSource
        upperCallBack = null;
        lowerCallBack = null;
        loadClipCallBack = null;
        play_state = E_PlayState.E_NONE;
        audio_type = E_AudioType.E_NONE;
        gameObject = null;
        name = null;
    }

    // 提供方法 取消协程
    private void CancelCoroutine()
    {
        // 防御式编程 如果MonoMgr已被销毁(强制结束游戏时) 那么无需取消coroutine
        if (coroutine != null && MonoMgr.isInstantiated)
        {
           
            MonoMgr.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private void SetUpperVolume(float volume)
    {
        audio_source.volume = volume;

        // 当音量即将达到目标音量时，停止渐变
        if (Mathf.Abs(AudioManager.Instance.GetCurrentData().music_volume - volume) <= AudioManager.GRADUALLY_MIN_VOLUME)
        {
            audio_source.volume = AudioManager.Instance.GetCurrentData().music_volume;
            if (coroutine != null)
            {
                // 当音量达到时 用户正好强制结束游戏 几乎不可能 但还是判断一下
                if (MonoMgr.isInstantiated)
                    MonoMgr.Instance.StopCoroutine(coroutine);
                coroutine = null;
            }
            play_state = E_PlayState.E_PLAYING;
            upperCallBack?.Invoke(this);
        }
    }
    private void SetLowerVolume(float volume)
    {
        audio_source.volume = volume;

        // 当音量即将达到0时，停止渐变
        if (volume <= AudioManager.GRADUALLY_MIN_VOLUME)
        {
            if (coroutine != null && MonoMgr.isInstantiated)
            {
                MonoMgr.Instance.StopCoroutine(coroutine);
                coroutine = null;
            }
            lowerCallBack?.Invoke(this);
        }
    }
}