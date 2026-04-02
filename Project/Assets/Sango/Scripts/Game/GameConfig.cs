/*
 * 文件名：GameConfig.cs
 * 描述：游戏配置类，存储游戏的基本配置信息
 * 创建日期：2026-03-27
 * 最后修改：2026-03-27
 */

using Sango.Mod;
using Sango.Tools;
using System.Collections;
using UnityEngine;

namespace Sango.Core
{
    /// <summary>
    /// 游戏配置类，存储游戏的基本配置信息
    /// </summary>
    public static class GameConfig
    {
        /// <summary>
        /// 窗口宽度
        /// </summary>
        public static int WindowWidth;

        /// <summary>
        /// 窗口高度
        /// </summary>
        public static int WindowHeight;

        /// <summary>
        /// 语言设置
        /// </summary>
        public static int Language;

        /// <summary>
        /// 保存路径
        /// </summary>
        public static string SavePath;
        
        /// <summary>
        /// 初始化游戏配置
        /// </summary>
        public static void Init()
        {
            SavePath = Path.ContentRootPath + "/Save";
        }
    }
}
