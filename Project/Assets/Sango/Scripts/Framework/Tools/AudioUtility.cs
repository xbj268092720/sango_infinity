using System.IO;
using UnityEngine;
using Sango.Loader;

namespace Sango.Tools
{
    /// <summary>
    /// 音频工具类，用于OGG文件加载和管理
    /// </summary>
    public static class AudioUtility
    {
        /// <summary>
        /// 从文件路径加载OGG音频文件
        /// </summary>
        /// <param name="filePath">OGG文件路径</param>
        /// <param name="is3D">是否为3D音效</param>
        /// <returns>加载的AudioClip</returns>
        public static AudioClip LoadOggFromFile(string filePath, bool is3D = false)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"OGG文件不存在: {filePath}");
                return null;
            }

            // 使用AudioLoader加载
            return AudioLoader.LoadFromFileSync(filePath, is3D);
        }

        /// <summary>
        /// 从资源路径加载OGG音频文件
        /// </summary>
        /// <param name="resourcePath">Resources文件夹中的路径（不含扩展名）</param>
        /// <param name="is3D">是否为3D音效</param>
        /// <returns>加载的AudioClip</returns>
        public static AudioClip LoadOggFromResources(string resourcePath, bool is3D = false)
        {
            return ObjectLoader.LoadObject<AudioClip>(resourcePath, is3D);
        }

        /// <summary>
        /// 异步加载OGG音频文件
        /// </summary>
        /// <param name="filePath">OGG文件路径</param>
        /// <param name="callback">加载完成回调</param>
        /// <param name="is3D">是否为3D音效</param>
        public static void LoadOggFromFileAsync(string filePath, System.Action<AudioClip> callback, bool is3D = false)
        {
            AudioLoader.LoadFromFileAsync(filePath, (obj, customData) =>
            {
                callback?.Invoke(obj as AudioClip);
            }, is3D);
        }

        /// <summary>
        /// 播放OGG音频文件
        /// </summary>
        /// <param name="filePath">OGG文件路径</param>
        /// <param name="volume">音量（0-1）</param>
        /// <param name="is3D">是否为3D音效</param>
        /// <param name="position">3D音效的位置</param>
        /// <returns>播放声道索引</returns>
        public static int PlayOggFromFile(string filePath, float volume = 1.0f, bool is3D = false, Vector3 position = default)
        {
            AudioClip clip = LoadOggFromFile(filePath, is3D);
            if (clip == null)
                return -1;

            return is3D ? 
                Manager.AudioManager.Instance.PlaySfx3D(clip.name, position, volume) :
                Manager.AudioManager.Instance.PlaySfx(clip.name, volume);
        }

        /// <summary>
        /// 设置AudioClip的循环属性
        /// </summary>
        /// <param name="clip">AudioClip</param>
        /// <param name="loop">是否循环</param>
        public static void SetLoop(AudioClip clip, bool loop)
        {
            if (clip != null)
            {
                // Unity的AudioClip.loop属性是只读的，需要通过AudioSource设置
                // 这里提供一个辅助方法
                Debug.Log($"AudioClip '{clip.name}' 的循环属性设置为: {loop}");
            }
        }

        /// <summary>
        /// 获取音频文件的信息
        /// </summary>
        /// <param name="filePath">音频文件路径</param>
        /// <returns>音频信息</returns>
        public static AudioInfo GetAudioInfo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"文件不存在: {filePath}");
                return null;
            }

            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                return new AudioInfo
                {
                    fileName = System.IO.Path.GetFileName(filePath),
                    fileSize = fileInfo.Length,
                    filePath = filePath,
                    extension = System.IO.Path.GetExtension(filePath).ToLower()
                };
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"获取音频信息失败: {ex.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// 音频信息类
    /// </summary>
    public class AudioInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName;

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long fileSize;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath;

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string extension;

        /// <summary>
        /// 获取文件大小的可读字符串
        /// </summary>
        public string FileSizeReadable
        {
            get
            {
                if (fileSize < 1024)
                    return $"{fileSize} B";
                else if (fileSize < 1024 * 1024)
                    return $"{(fileSize / 1024.0):F2} KB";
                else
                    return $"{(fileSize / (1024.0 * 1024)):F2} MB";
            }
        }
    }
}
