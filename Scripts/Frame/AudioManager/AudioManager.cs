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
    public const float GRADUALLY_MIN_VOLUME = 0.01f; // �������С���� �������������ͻ��Զ�ֹͣ
    private List<Music> musics = new(); // ���ָ��ŵ���Ϸ����
    private GameObject music_gameObject;
    /// <summary>
    ///  �ڲ��� Ϊ�˾�ȷ�������ֵĲ���״̬
    ///  audio_source�� ��ƵԴ
    ///  coroutine����������Ҫ����ʱ��ʹ��Э�������������ı仯
    ///  callBack: �������ʱ�Ļص�����
    /// </summary>

    public override void Init()
    {
        current_audiodata = JsonMgr.Instance.LoadData<AudioData>("AudioData");
    }
    private void Save()
    {
        JsonMgr.Instance.SaveData(current_audiodata, "AudioData");
    }
    // �����������ֵ�������С ���Զ�����
    public void SetMusicVolume(float volume)
    {
        current_audiodata.music_volume = volume;

        BackMusicManager.Instance.SetVolume(volume);
        MusicManager.Instance.SetVolume(volume);
        Save();
    }
    // ����������Ч��������С ���Զ�����
    public void SetEffectVolume(float volume)
    {
        current_audiodata.effect_volume = volume;
        Save();
    }
    // �����������ֵľ���״̬ ���Զ�����
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

    // ��������ط���
    public void PlayBackMusic(string name, bool isGradually = true) {
        // ������������ڲ��� ��ô��ֻ�Ǽ������� ����ֱ�Ӳ���
        if (MusicManager.Instance.IsAnyMusicPlaying())
            BackMusicManager.Instance.LoadBackMusic(name);
        else
            BackMusicManager.Instance.PlayBackMusic(name, isGradually);
    }
    public void PlayBackMusic(bool isGradually = true) {
        // ���û���������ڲ��� ��ô�Ͳ��� ����ȴ�
        if (!MusicManager.Instance.IsAnyMusicPlaying())
            BackMusicManager.Instance.Play(isGradually);
        else
            Debug.Log("�޷��ָ����� ��Ϊ���������ڲ���");
    }
    public void StopBackMusic(bool isGradually = true)
    {
        BackMusicManager.Instance.Stop(isGradually);
    }
    // ������ط���
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
        // ���������ֳ��� ��Ϊ�����Ͼ�Ҫ�ر���
        for (int i = 0; i < musics.Count; i++) {
            if (musics[i].play_state == E_PlayState.E_PLAYING ||
                musics[i].play_state == E_PlayState.E_UPPER_GRADUALLY)
            { ret = true; break; }

        }
        return ret;
    }
    // ��Ч��ط���
    public void PlaySoundEffect(string path) {
        SoundEffectManager.Instance.CreateSountEffect(path);
    }
    // �������� �ÿձ������������б�
    public void Clear() {
        MusicManager.Instance.Clear();
        BackMusicManager.Instance.Clear();
        SoundEffectManager.Instance.Clear();
    }
}
