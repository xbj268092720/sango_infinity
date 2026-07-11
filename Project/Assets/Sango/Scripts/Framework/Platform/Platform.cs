/*
'*******************************************************************
'Tank Framework
'*******************************************************************
*/
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Sango
{
    static public class Platform
    {
        public const string VersionFile = "version.txt";
        public const string ContentZipFile = "Content.zip";
        public const string ModZipFile = "Mods.zip";

        public enum PlatformName
        {
            Android,
            Ios,
            Window,
            Mac,
            Webgl,
            Webgl_wechat,
            Webgl_tiktok,
        }

        /// <summary>
        /// 当前平台
        /// </summary>
        public static PlatformName targetPlatform;

        /// <summary>
        /// Java类名
        /// </summary>
        static public string JaveClassName = "cn.com.XFramework.XAndroidSDK";

        /// <summary>
        /// Java工具类名
        /// </summary>
        static public string JaveUtilityClassName = "cn.com.XFramework.XAndroidUtility";

        /// <summary>
        /// 资源版本号
        /// </summary>
        static public string ResourceVersion = "0.0.1";

        /// <summary>
        /// 是否使用JIT
        /// </summary>
        public static bool useJit = false;

        /// <summary>
        /// 谁否为编辑器模式
        /// </summary>
        public static bool isEditorMode
        {
            get { return UnityEngine.Application.isEditor; }
        }

        /// <summary>
        /// 平台相关初始化
        /// </summary>
        public static void Init()
        {
            PlatformListener.Init();
        }

        /// <summary>
        /// 获取平台名字
        /// </summary>
        /// <returns></returns>
        static public string GetPlatformName()
        {
            return PlatformUtility.GetPlatformName();
        }

        static public bool CheckAppVersion()
        {
            string versionFilePath = System.IO.Path.Combine(Path.SaveRootPath, VersionFile);
            if (File.Exists(versionFilePath))
            {
                string version = File.ReadAllText(versionFilePath);
                if (version.Equals(PlatformUtility.GetApplicationVersion()))
                    return true;
            }
            return false;
        }

        static public void SaveAppVersion()
        {
            string versionFilePath = System.IO.Path.Combine(Path.SaveRootPath, VersionFile);
            if (File.Exists(versionFilePath))
            {
                string version = File.ReadAllText(versionFilePath);
                if (version.Equals(PlatformUtility.GetApplicationVersion()))
                    return;
            }

            File.WriteAllText(versionFilePath, PlatformUtility.GetApplicationVersion());
        }

        static public void ExtractZipFile(string filePath, string savePath, System.Action<float> progress)
        {
            progress?.Invoke(0);
            using ZipArchive source = ZipFile.Open(filePath, ZipArchiveMode.Read, null);
            DirectoryInfo directoryInfo = System.IO.Directory.CreateDirectory(savePath);
            string text = directoryInfo.FullName;
            int length = text.Length;
            if (length != 0 && text[length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                text += System.IO.Path.DirectorySeparatorChar;
            }
            int count = source.Entries.Count;
            int cur_count = 0;
            if (count > 0)
            {
                foreach (ZipArchiveEntry entry in source.Entries)
                {
                    string fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(text, entry.FullName));
                    if (System.IO.Path.GetFileName(fullPath).Length == 0)
                    {
                        System.IO.Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));
                        entry.ExtractToFile(fullPath, overwrite: false);
                    }
                    cur_count++;
                    progress?.Invoke(0.2f + (float)cur_count / count * 0.4f);
                }
            }
            progress?.Invoke(1);
        }

        static public void CopyContentAndModZipFile(System.Action<float> progress)
        {
            string path = System.IO.Path.Combine(Sango.Path.StreamingAssetsPath, ContentZipFile);
            string uri = new System.Uri(path).AbsoluteUri;
            UnityWebRequest request = UnityWebRequest.Get(uri);
            UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = request.SendWebRequest();
            while (!unityWebRequestAsyncOperation.isDone)
            {
                progress?.Invoke(unityWebRequestAsyncOperation.progress * 0.5f);
                Thread.Sleep(10);
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string zipSavePath = System.IO.Path.Combine(Sango.Path.PersistentDataPathPath, ContentZipFile);
                if (File.Exists(zipSavePath))
                    File.Delete(zipSavePath);

                File.WriteAllBytes(zipSavePath, request.downloadHandler.data);
            }

            path = System.IO.Path.Combine(Sango.Path.StreamingAssetsPath, ModZipFile);
            uri = new System.Uri(path).AbsoluteUri;
            request = UnityWebRequest.Get(uri);
            unityWebRequestAsyncOperation = request.SendWebRequest();
            while (!unityWebRequestAsyncOperation.isDone)
            {
                progress?.Invoke(0.5f + unityWebRequestAsyncOperation.progress * 0.5f);
                Thread.Sleep(10);
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string zipSavePath = System.IO.Path.Combine(Sango.Path.PersistentDataPathPath, ModZipFile);
                if (File.Exists(zipSavePath))
                    File.Delete(zipSavePath);

                File.WriteAllBytes(zipSavePath, request.downloadHandler.data);
            }
        }


        static public void ExtractContentAndModZipFile(System.Action<float> progress)
        {
            progress?.Invoke(0);
            string zipSavePath = System.IO.Path.Combine(Sango.Path.PersistentDataPathPath, ContentZipFile);
            ZipArchive source = ZipFile.Open(zipSavePath, ZipArchiveMode.Read, null);
            DirectoryInfo directoryInfo = System.IO.Directory.CreateDirectory(Sango.Path.PersistentDataPathPath);
            string text = directoryInfo.FullName;
            int length = text.Length;
            if (length != 0 && text[length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                text += System.IO.Path.DirectorySeparatorChar;
            }
            int count = source.Entries.Count;
            int cur_count = 0;
            if (count > 0)
            {
                foreach (ZipArchiveEntry entry in source.Entries)
                {
                    string fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(text, entry.FullName));
                    if (System.IO.Path.GetFileName(fullPath).Length == 0)
                    {
                        if(!System.IO.Directory.Exists(fullPath))
                            System.IO.Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        string dirName = System.IO.Path.GetDirectoryName(fullPath);
                        if (!System.IO.Directory.Exists(dirName))
                            System.IO.Directory.CreateDirectory(dirName);
                        entry.ExtractToFile(fullPath, overwrite: true);
                    }
                    cur_count++;
                    progress?.Invoke((float)cur_count / count * 0.7f);
                }
            }

            zipSavePath = System.IO.Path.Combine(Sango.Path.PersistentDataPathPath, ModZipFile);
            source = ZipFile.Open(zipSavePath, ZipArchiveMode.Read, null);
            directoryInfo = System.IO.Directory.CreateDirectory(Sango.Path.PersistentDataPathPath);
            text = directoryInfo.FullName;
            length = text.Length;
            if (length != 0 && text[length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                text += System.IO.Path.DirectorySeparatorChar;
            }
            count = source.Entries.Count;
            cur_count = 0;
            if (count > 0)
            {
                foreach (ZipArchiveEntry entry in source.Entries)
                {
                    string fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(text, entry.FullName));
                    if (System.IO.Path.GetFileName(fullPath).Length == 0)
                    {
                        if (!System.IO.Directory.Exists(fullPath))
                            System.IO.Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        string dirName = System.IO.Path.GetDirectoryName(fullPath);
                        if (!System.IO.Directory.Exists(dirName))
                            System.IO.Directory.CreateDirectory(dirName);
                        entry.ExtractToFile(fullPath, overwrite: true);
                    }
                    cur_count++;
                    progress?.Invoke(0.7f + (float)cur_count / count * 0.3f);
                }
            }
            progress?.Invoke(1);
        }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("Sango/打包/压缩资源包至StreamingAssets", false, 0)]
        public static void ZipContentToStreamingAssets()
        {
            Sango.Path.Init();
            Sango.Directory.Create(Application.streamingAssetsPath);
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, Sango.Platform.ContentZipFile);
            if (Sango.File.Exists(path))
            {
                Sango.File.Delete(path);
            }
            ZipFile.CreateFromDirectory(Sango.Path.ContentRootPath, path, System.IO.Compression.CompressionLevel.Optimal, true);
            path = System.IO.Path.Combine(Application.streamingAssetsPath, Sango.Platform.ModZipFile);
            if (Sango.File.Exists(path))
            {
                Sango.File.Delete(path);
            }
            ZipFile.CreateFromDirectory(Sango.Path.ModRootPath, path, System.IO.Compression.CompressionLevel.Optimal, true);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif

    }
}
