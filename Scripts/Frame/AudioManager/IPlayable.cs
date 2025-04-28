public interface IPlayable
{
    void Play(); // 播放
    void Stop(); // 停止
    void Pause(); // 暂停
    void GraduallyUpper(); // 渐进 音量或颜色越来越强 FadeIn
    void GraduallyLower(); // 渐出 音量或颜色越来越淡 FadeOut
}
