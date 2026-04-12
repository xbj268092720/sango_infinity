/*
'*******************************************************************
Tank Framework
'*******************************************************************
*/
using UnityEditor;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Sango;

/// <summary>
/// 简单打包工具类
/// </summary>
public class SimpleBuilder
{
    [MenuItem("Sango/打包/发布游戏", false, 1)]
    public static void BuildGame()
    {
        // 初始化路径
        Sango.Path.Init();
        
        // 获取当前目标平台
        BuildTarget targetPlatform = EditorUserBuildSettings.activeBuildTarget;
        
        // 执行ModPackageBuilder
        ModPackageBuilder.BuildAssetBundls();
        
        if (targetPlatform == BuildTarget.StandaloneWindows || targetPlatform == BuildTarget.StandaloneWindows64)
        {
            // 处理Windows平台打包
            BuildWindows();
        }
        else if (targetPlatform == BuildTarget.Android)
        {
            // 处理Android平台打包
            BuildAndroid();
        }
        else
        {
            Log.Info("当前平台不支持自动打包", Log.LogType.Game);
        }
    }
    
    /// <summary>
    /// 为Windows平台构建游戏
    /// </summary>
    private static void BuildWindows()
    {
        // 发布项目至Sango.Path.SaveRootPath
        string buildPath = Sango.Path.SaveRootPath;
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath + "/Infinity三国志.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        
        BuildPipeline.BuildPlayer(buildOptions);
        
        Log.Info("Windows平台构建完成", Log.LogType.Game);
        
        // 拷贝到D:/无限三国发布
        string publishPath = "D:/无限三国发布";
        if (System.IO.Directory.Exists(publishPath))
        {
            System.IO.Directory.Delete(publishPath, true);
        }
        
        System.IO.Directory.CreateDirectory(publishPath);
        CopyDirectory(buildPath, publishPath);
        
        Log.Info("拷贝到发布目录完成", Log.LogType.Game);
        
        // 删除指定文件和文件夹
        DeleteUnnecessaryFiles(publishPath);
        
        Log.Info("删除不必要文件完成", Log.LogType.Game);
        
        // 压缩发布文件夹
        string zipPath = publishPath + ".zip";
        if (System.IO.File.Exists(zipPath))
        {
            System.IO.File.Delete(zipPath);
        }
        
        System.IO.Compression.ZipFile.CreateFromDirectory(publishPath, zipPath, System.IO.Compression.CompressionLevel.Optimal, true);
        
        Log.Info("压缩发布文件夹完成", Log.LogType.Game);
    }
    
    /// <summary>
    /// 为Android平台构建游戏
    /// </summary>
    private static void BuildAndroid()
    {
        // 执行Platform.ZipContentToStreamingAssets
        Platform.ZipContentToStreamingAssets();
        
        // 删除{Sango.Path.SaveRootPath}/Content/Package/win文件夹
        string winPackagePath = System.IO.Path.Combine(Sango.Path.ContentRootPath, "Package", "win");
        if (System.IO.Directory.Exists(winPackagePath))
        {
            System.IO.Directory.Delete(winPackagePath, true);
        }
        
        // 发布项目至Sango.Path.SaveRootPath，发布apk名字为infinitySango.apk
        string buildPath = Sango.Path.SaveRootPath + "/infinitySango.apk";
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        
        BuildPipeline.BuildPlayer(buildOptions);
        
        Log.Info("Android平台构建完成", Log.LogType.Game);
    }
    
    /// <summary>
    /// 拷贝目录
    /// </summary>
    /// <param name="sourceDir">源目录</param>
    /// <param name="destDir">目标目录</param>
    private static void CopyDirectory(string sourceDir, string destDir)
    {
        System.IO.DirectoryInfo sourceInfo = new System.IO.DirectoryInfo(sourceDir);
        System.IO.DirectoryInfo destInfo = new System.IO.DirectoryInfo(destDir);
        
        if (!destInfo.Exists)
        {
            destInfo.Create();
        }
        
        // 拷贝文件
        foreach (System.IO.FileInfo file in sourceInfo.GetFiles())
        {
            string destFilePath = System.IO.Path.Combine(destDir, file.Name);
            file.CopyTo(destFilePath, true);
        }
        
        // 拷贝子目录
        foreach (System.IO.DirectoryInfo subDir in sourceInfo.GetDirectories())
        {
            string destSubDir = System.IO.Path.Combine(destDir, subDir.Name);
            CopyDirectory(subDir.FullName, destSubDir);
        }
    }
    
    /// <summary>
    /// 删除不必要的文件和文件夹
    /// </summary>
    /// <param name="publishPath">发布目录</param>
    private static void DeleteUnnecessaryFiles(string publishPath)
    {
        // 删除指定文件夹
        string[] directoriesToDelete = {
            System.IO.Path.Combine(publishPath, "Infinity三国志_BackUpThisFolder_ButDontShipItWithYourGame"),
            System.IO.Path.Combine(publishPath, "Infinity三国志_BurstDebugInformation_DoNotShip"),
            System.IO.Path.Combine(publishPath, "Save")
        };
        
        foreach (string dirPath in directoriesToDelete)
        {
            if (System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.Delete(dirPath, true);
            }
        }
        
        // 删除指定文件
        string[] filesToDelete = {
            System.IO.Path.Combine(publishPath, "Infinity三国志_Data", "StreamingAssets", "Content.zip"),
            System.IO.Path.Combine(publishPath, "Infinity三国志_Data", "StreamingAssets", "Mods.zip")
        };
        
        foreach (string filePath in filesToDelete)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}