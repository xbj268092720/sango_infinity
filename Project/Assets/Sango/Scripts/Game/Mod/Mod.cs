using HybridCLR;

using Sango.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Sango.Mod
{
    public class Mod
    {
        /// <summary>
        /// 唯一标识符,只能用英文
        /// </summary>
        public string Id { internal set; get; }
        /// <summary>
        /// Mod名字,可以中文
        /// </summary>
        public string Name { internal set; get; }
        /// <summary>
        /// Mod标签
        /// </summary>
        public string Tag { internal set; get; }
        /// <summary>
        /// 海报图片相对路径
        /// </summary>
        public string Version { internal set; get; }
        /// <summary>
        /// 说明文字
        /// </summary>
        public string Description { internal set; get; }
        /// <summary>
        /// 依赖模组,以分号隔开 mod1;mod2
        /// </summary>
        public string Depends { internal set; get; }
        /// <summary>
        /// 海报图片相对路径
        /// </summary>
        public string Poster { internal set; get; }
        /// <summary>
        /// Mod路径
        /// </summary>
        public string ModDir { internal set; get; }
        /// <summary>
        /// Assembly名字
        /// </summary>
        public string EntryAssembly { internal set; get; }

        public string ModDirName { internal set; get; }

        public Assembly Assembly { internal set; get; }

        public void LoadData()
        {
            string path = GetFullPath("Data");
            Directory.EnumFiles(path, "*.json", SearchOption.AllDirectories, (file) =>
            {
#if SANGO_DEBUG
                Sango.Log.Print($"LoadData: {file}");
#endif
            });
            //Directory.EnumFiles(path, "*.txt", SearchOption.AllDirectories, (file) =>
            //{
            //    Sango.Log.Print($"LoadData: {file}");
            //    Game.GameData.Load(file);
            //});

            //Game.GameData.LoadBin(path);
        }

        public void LoadAssembly()
        {
            string path = GetFullPath("Metadata");
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            Directory.EnumFiles(path, "*.dll", SearchOption.AllDirectories, (file) =>
            {
#if SANGO_DEBUG
                Sango.Log.Print($"LoadMetadataAssembly: {file}");
#endif
                byte[] dllBytes = File.ReadAllBytes(file);
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
#if SANGO_DEBUG
                Sango.Log.Print($"LoadMetadataForAOTAssembly:{file}. mode:{mode} ret:{err}");
#endif
            });

            path = GetFullPath($"{EntryAssembly}.dll");
            if (File.Exists(path))
            {
#if !UNITY_EDITOR
                Assembly = Assembly.Load(File.ReadAllBytes(path));
#else
                Assembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == EntryAssembly);
#endif
                // 执行入口函数
                Type entryType = Assembly.GetType("Entry");
                entryType.GetMethod("Start").Invoke(null, null);
            }
        }

        public void LoadUI()
        {
            string path = GetFullPath("UI");
            Directory.EnumFiles(path, "*.bytes", SearchOption.AllDirectories, (file) =>
            {
#if SANGO_DEBUG
                Sango.Log.Print($"LoadUI: {file}");
#endif
                string packageName = System.IO.Path.GetFileNameWithoutExtension(file).Split('_')[0];
                Window.Instance.AddPackage(file, packageName);
            });
        }
        public void LoadPackage()
        {
            string path = GetFullPath($"Package/{PlatformUtility.GetPlatformName()}");
            Directory.EnumFiles(path, "*.pkg", SearchOption.AllDirectories, (file) =>
            {
#if SANGO_DEBUG
                Sango.Log.Print($"LoadPackage: {file}");
#endif
                string packageName = System.IO.Path.GetFileNameWithoutExtension(file).Split('_')[0];
                PackageManager.Instance.AddPackage(packageName, file, true);
            });
        }

        public void LoadLanguage()
        {
            string path = GetFullPath("Language");
            Directory.EnumFiles(path, "*.json", SearchOption.AllDirectories, (file) =>
            {
#if SANGO_DEBUG
                Sango.Log.Print($"Find Language File: {file}");
#endif
                GameLanguage.AddFile(file);
            });
        }
        public void LoadScenario()
        {
            string path = GetFullPath("Scenario");
            Directory.EnumFiles(path, "*.json", SearchOption.AllDirectories, (file) =>
            {
#if SANGO_DEBUG
                Sango.Log.Print($"Find Scenario: {file}");
#endif
                ShortScenario.Add(file);
            });
        }
        public string GetFullPath(string path)
        {
            return $"{ModDir}/{path}";
        }
    }
}
