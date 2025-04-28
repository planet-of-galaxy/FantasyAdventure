
using System.Collections.Generic;
using System.Linq;
using UnityEngine; 

public class SoundEffectManager : SingletonMono<SoundEffectManager>
{
    private List<AudioSource> soundEffects = new();

    private const string PATH = "SoundEffect/";
    private const string SOUND_EFFECT_OBJ_NAME = "SoundEffect";

    private GameObject soundEffectObj; // 音效默认附着物

    // 创建一个音效 默认附着物为空
    public void CreateSountEffect(string path,GameObject gameObj = null) {
        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(PATH + path));

        AudioSource audioSource = obj.GetComponent<AudioSource>();
        if (audioSource == null) {
            Debug.LogError("该预制体中没有AudioSource!!");
            return;
        }

        audioSource.loop = false;
        audioSource.volume = AudioManager.Instance.GetCurrentData().effect_volume;
        audioSource.mute = AudioManager.Instance.GetCurrentData().effect_isMute;

        if (gameObj == null) {
            if (soundEffectObj == null)
                soundEffectObj = new GameObject(SOUND_EFFECT_OBJ_NAME);

            obj.transform.SetParent(soundEffectObj.transform); // 实例化到父物体
            soundEffects.Add(audioSource); // 添加至列表方便后续管理
        }
        else
        {
            audioSource.maxDistance = 15; // 设置音效最远传播距离
            obj.transform.SetParent(gameObj.transform); // 实例化到父物体
            soundEffects.Add(audioSource); // 添加至列表方便后续管理
        }
    }

    private void Update() {
        if (soundEffects.Count == 0)
            return;

        //从后往前遍历列表 找出已经播放完成的音效并销毁它
        for (int i = soundEffects.Count - 1; i >= 0; i--) {
            if (!soundEffects[i].isPlaying) {
                GameObject.Destroy(soundEffects[i].gameObject);
                soundEffects.RemoveAt(i);
            }
        }
    }

    public void Clear() {
        // 从后往前遍历 边遍历边删除
        for (int i = soundEffects.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(soundEffects[i].gameObject);
            soundEffects.RemoveAt(i);
        }
    }
}