using UnityEngine;
using Sango.Tools;
using System.IO;

namespace Sango.Example
{
    /// <summary>
    /// OGG音频加载示例
    /// </summary>
    public class AudioExample : MonoBehaviour
    {
        [Header("OGG文件路径")]
        public string oggFilePath = "Assets/Audio/sound.ogg";
        
        [Header("播放设置")]
        public bool is3DSound = false;
        public float volume = 1.0f;
        public Vector3 soundPosition = Vector3.zero;
        
        private AudioClip loadedClip;
        private int currentChannel = -1;

        void Start()
        {
            // 确保路径正确
            if (!Path.IsPathRooted(oggFilePath))
            {
                oggFilePath = System.IO.Path.Combine(Application.dataPath, oggFilePath);
            }
            
            Debug.Log($"准备加载OGG文件: {oggFilePath}");
        }

        /// <summary>
        /// 同步加载OGG文件
        /// </summary>
        public void LoadOggSync()
        {
            loadedClip = AudioUtility.LoadOggFromFile(oggFilePath, is3DSound);
            if (loadedClip != null)
            {
                Debug.Log($"成功加载OGG文件: {loadedClip.name}");
                Debug.Log($"音频信息: {loadedClip.channels}声道, {loadedClip.frequency}Hz, {loadedClip.length:F2}秒");
            }
            else
            {
                Debug.LogError("加载OGG文件失败");
            }
        }

        /// <summary>
        /// 异步加载OGG文件
        /// </summary>
        public void LoadOggAsync()
        {
            AudioUtility.LoadOggFromFileAsync(oggFilePath, (clip) =>
            {
                loadedClip = clip;
                if (clip != null)
                {
                    Debug.Log($"异步加载成功: {clip.name}");
                }
                else
                {
                    Debug.LogError("异步加载失败");
                }
            }, is3DSound);
        }

        /// <summary>
        /// 播放加载的音频
        /// </summary>
        public void PlayAudio()
        {
            if (loadedClip != null)
            {
                if (is3DSound)
                {
                    currentChannel = Manager.AudioManager.Instance.PlaySfx3D(loadedClip.name, soundPosition, volume);
                }
                else
                {
                    currentChannel = Manager.AudioManager.Instance.PlaySfx(loadedClip.name, volume);
                }
                Debug.Log($"正在播放音频，声道索引: {currentChannel}");
            }
            else
            {
                Debug.LogError("没有加载音频文件");
            }
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void StopAudio()
        {
            if (currentChannel >= 0)
            {
                Manager.AudioManager.Instance.StopSfx(currentChannel);
                currentChannel = -1;
                Debug.Log("音频播放已停止");
            }
        }

        /// <summary>
        /// 获取音频文件信息
        /// </summary>
        public void GetAudioInfo()
        {
            AudioInfo info = AudioUtility.GetAudioInfo(oggFilePath);
            if (info != null)
            {
                Debug.Log($"音频文件信息:");
                Debug.Log($"文件名: {info.fileName}");
                Debug.Log($"文件大小: {info.FileSizeReadable}");
                Debug.Log($"文件路径: {info.filePath}");
                Debug.Log($"扩展名: {info.extension}");
            }
        }

        /// <summary>
        /// 直接播放OGG文件
        /// </summary>
        public void PlayOggDirectly()
        {
            currentChannel = AudioUtility.PlayOggFromFile(oggFilePath, volume, is3DSound, soundPosition);
            if (currentChannel >= 0)
            {
                Debug.Log($"直接播放OGG文件成功，声道索引: {currentChannel}");
            }
            else
            {
                Debug.LogError("直接播放OGG文件失败");
            }
        }
    }
}
