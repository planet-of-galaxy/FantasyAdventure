
using UnityEngine;

public class BackMusicManager : Singleton<BackMusicManager>
{
    private Music backMusic;
    private GameObject gameObj;
    
    private bool isPuase; // 渐出任务完成后 暂停or停止 用于函数回调判断
    private bool isAutoPlay;// 加载结束后 是否自动播放 用于函数回调判断
    private bool isGradually; // 加载结束后 是否渐进 用于函数回调判断 只有isAutoPlay为true的话 这个字段才有效

    public const string BACK_MUSIC_OBJ_NAME = "BackMusic"; // 背景音附着的GameObject的名字
    public const string BACK_MUSIC_AB_NAME = "music"; // AB包的名字 所有的背景音乐请放到这个包中
    public override void Init()
    {
        // 创建一个音乐类
        backMusic = new Music();
        // 绑定回调函数
        backMusic.upperCallBack = UpperCallBack;
        backMusic.lowerCallBack = LowerCallBack;
        backMusic.loadClipCallBack = LoadCallBack;
        // 设置状态和类型
        backMusic.play_state = E_PlayState.E_NONE; // 默认无播放状态
        backMusic.audio_type = E_AudioType.E_BACK_MUSIC;  // 设置音频类型
        backMusic.DEBUG_LOG = true;
        // 实例完成 音频附着物gameObject 音乐源AudioSource 音乐名nam 音乐切片clip等到第一次播放时再添加
    }

    // 指定音乐名字播放背景音 默认渐进播放
    public void PlayBackMusic(string name, bool isGradually = true)
    {
        // 如果音乐不为空 且请求的音乐名字与现有的音乐名字相同 那么直接播放并返回
        if (backMusic.play_state != E_PlayState.E_NONE && backMusic.name == name) {
            Play(isGradually);
            return;
        }
        // 如果音乐不为空 但是请求的音乐名字与现有的音乐名不同 那么先重置 交给下面重新加载
        if (backMusic.play_state != E_PlayState.E_NONE && backMusic.name != name)
            Reset();

        // 如果背景音乐为空或已被重置 那么创建一个新的音乐切片
        if (backMusic.play_state == E_PlayState.E_NONE) {
            if (backMusic.gameObject == null) {
                if (gameObj == null)
                    gameObj = new GameObject(BACK_MUSIC_OBJ_NAME); // 添加一个背景音的附着物

                backMusic.gameObject = gameObj;
            }
            if (backMusic.audio_source == null) {
                backMusic.audio_source = gameObj.AddComponent<AudioSource>(); // 添加AudioSource
                backMusic.audio_source.loop = true; // 循环播放
            }

            isAutoPlay = true; // 加载完成后 自动播放
            this.isGradually = isGradually; // 设置是否渐进播放
            backMusic.name = name; // 先设置clip名字
            backMusic.LoadClipAsync(BACK_MUSIC_AB_NAME);
        }
    }
    // 只加载不播放
    public void LoadBackMusic(string name) {
        Reset(); // 先重置现有的音乐

        isAutoPlay = false; // 加载完成后 不自动播放
        backMusic.name = name;
        backMusic.LoadClipAsync(BACK_MUSIC_AB_NAME);
        backMusic.name = name;
    }

    // 播放背景音 默认渐进播放
    public void Play(bool isGradually = true) {
        // 音乐文件会自己判断 如果为空就不播
        //if (backMusic.audio_type == E_AudioType.E_NONE)
        //    return;
        if (isGradually)
            backMusic.GraduallyUpper();
        else
            backMusic.Play();
    }
    // 暂停封装 默认渐出后暂停
    public void Pause(bool isGradually = true) {
        if (isGradually) {
            backMusic.GraduallyLower();
            isPuase = true;
        }
        else
            backMusic.Pause(); ;
    }
    // 停止播放封装 默认渐出后停止
    public void Stop(bool isGradually = true) {
        if (isGradually)
        {
            backMusic.GraduallyLower();
            isPuase = false;
        }
        else
            backMusic.Pause(); ;
    }

    // 加载完成自动播放或渐进播放 或者 什么也不做
    // 暂时不考虑 加载完成时其他如追击音乐正在播放
    // 如果有 直接重叠播放 以后再去追加判断逻辑
    private void LoadCallBack(IPlayable playable)
    {
        if (isAutoPlay && isGradually)
            playable.GraduallyUpper();
        else if (isAutoPlay)
            playable.Play();
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
    /// <summary>
    /// 对于背景音乐来说
    /// 它的各个回调函数 附着物体 以及音频类型是不会变的所以只需重置音源的clip
    /// </summary>
    public void Reset() {
        if (backMusic.play_state == E_PlayState.E_NONE)
            return;
        // 先停止音乐和协程
        backMusic.Stop();
        // 重置播放状态
        backMusic.play_state = E_PlayState.E_NONE;
        // 重置音频切片
        backMusic.audio_source.clip = null;
        // 在此处可以卸载AB包的资源
        // 重置名字
        backMusic.name = null;
    }

    // 清空 重置且删除附着物 下次播放时重新添加
    public void Clear() {
        Reset();

        GameObject.Destroy(gameObj);
        backMusic.gameObject = null;
        backMusic.audio_source = null;
    }

    // 提供更改音量和静音的方法
    public void SetVolume(float volume) =>
        backMusic.SetVolume(volume);
    public void SetMute(bool mute) =>
        backMusic.SetMute(mute);
}
