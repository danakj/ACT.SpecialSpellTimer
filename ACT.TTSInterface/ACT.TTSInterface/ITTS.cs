namespace ACT.TTSInterface
{
    /// <summary>
    /// TTSインターフェイス
    /// </summary>
    public interface ITTS
    {
        /// <summary>
        /// Speak
        /// </summary>
        /// <param name="textToSpeak">
        /// 読上げるテキスト</param>
        void Speak(string textToSpeak);
    }
}
