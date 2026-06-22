
using Sango.Core;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sango.Mod
{
    public class ModManager : Singleton<ModManager>
    {
        public static string EditModName { get; set; }
        public static string MOD_ROOT_DIR = "Mods";
        public static string[] DEFAULT_MODS = { };
        //public static string[] DEFAULT_MODS = { };

        public List<Mod> mModList;
        public Dictionary<string, Mod> mModMap;

        public Mod[] GetEnabledMods()
        {
            return mModList.ToArray();
        }

        public void Init()
        {
            string path = $"{Path.ContentRootPath}/Package/{PlatformUtility.GetPlatformName()}";
            Directory.EnumFiles(path, "*.pkg", SearchOption.AllDirectories, (file) =>
            {
                Sango.Log.Info($"LoadPackage: {file}");
                string packageName = System.IO.Path.GetFileNameWithoutExtension(file).Split('_')[0];
                PackageManager.Instance.AddPackage(packageName, file, true);
            });

            MOD_ROOT_DIR = Path.ModRootPath;

            mModList = new List<Mod>();
            mModMap = new Dictionary<string, Mod>();

            string[] dirs = Directory.GetDirectories(MOD_ROOT_DIR, "*", System.IO.SearchOption.TopDirectoryOnly);
            if (dirs != null)
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    string mod_dir = dirs[i];
                    Mod mod = LoadMod(mod_dir);
                    if (mod != null)
                    {
                        if (mModMap.TryAdd(mod.Id, mod))
                        {
                            mod.ModDir = mod_dir;
                        }
                    }
                }
            }
        }

        public Mod LoadMod(string path)
        {
            string info_file = $"{path}/mod.info";
            if (File.Exists(info_file))
            {
                Mod mod = new Mod();
                mod.ModDirName = System.IO.Path.GetFileName(path);
                string[] lines = File.ReadAllLines(info_file);
                foreach (string s in lines)
                {
                    string[] c_v = s.Split('=');
                    if (c_v.Length > 1)
                    {
                        switch (c_v[0])
                        {
                            case "id":
                                mod.Id = c_v[1];
                                break;
                            case "name":
                                mod.Name = c_v[1];
                                break;
                            case "description":
                                mod.Description = c_v[1];
                                break;
                            case "version":
                                mod.Version = c_v[1];
                                break;
                            case "depends":
                                mod.Depends = c_v[1];
                                break;
                            case "poster":
                                mod.Poster = c_v[1];
                                break;
                            case "assembly":
                                mod.EntryAssembly = c_v[1];
                                break;
                        }
                    }
                }
                return mod;
            }
            return null;
        }

        public string[] GetAllPath(string dirName)
        {
            List<string> path = new List<string>();
            for (int i = 0; i < mModList.Count; i++)
            {
                Mod mod = mModList[i];
                path.Add(mod.GetFullPath(dirName));
            }
            return path.ToArray();
        }

        public void LoadFile(string filename, System.Action<string> mergeAction)
        {
            string baseFile = Path.FindFile(filename);
            if (!string.IsNullOrEmpty(baseFile))
            {
                mergeAction(baseFile);
                return;
            }
            for (int i = 0; i < mModList.Count; i++)
            {
                Mod mod = mModList[i];
                string destFile = mod.GetFullPath(filename);
                if (File.Exists(destFile))
                {
                    mergeAction(destFile);
                    return;
                }
            }
        }


        public string[] LoadModList()
        {
            string list_path = $"{MOD_ROOT_DIR}/modList.txt";
            if (!File.Exists(list_path))
                return null;

            string[] mod_list = File.ReadAllLines(list_path);
            List<string> list = new List<string>(DEFAULT_MODS);
            list.AddRange(mod_list);
            return list.ToArray();
        }

        public void SaveModList(string[] mod_list)
        {
            string list_path = $"{MOD_ROOT_DIR}/modList.txt";
            if (File.Exists(list_path))
                File.Delete(list_path);
            List<string> list = new List<string>(mod_list);
            foreach (string s in DEFAULT_MODS)
                list.Remove(s);
            File.WriteAllText(list_path, string.Join("\n", list));
        }

        public void SaveModList(Mod[] mod_list)
        {
            string list_path = $"{MOD_ROOT_DIR}/modList.txt";
            if (File.Exists(list_path))
                File.Delete(list_path);
            List<string> list = new List<string>();
            for (int i = 0; i < mod_list.Length; i++)
                list.Add(mod_list[i].Id);
            foreach (string s in DEFAULT_MODS)
                list.Remove(s);
            File.WriteAllText(list_path, string.Join("\n", list));
        }

        public void InitMods()
        {
            InitMods(null);
        }

        public void InitMods(string[] modNames)
        {
            if (modNames == null)
                modNames = LoadModList();


           
            Scenario.OnModInitStart();

            mModList.Clear();

            for (int i = 0; i < modNames.Length; i++)
            {
                Mod mod;
                if (mModMap.TryGetValue(modNames[i], out mod))
                {
                    mModList.Add(mod);
                }
            }

            for (int i = 0; i < mModList.Count; i++)
            {
                Mod mod = mModList[i];
                Path.AddSearchPath($"{mod.ModDir}", true); 
                mod.LoadLanguage();
                mod.LoadScenario();
                mod.LoadUI();
                mod.LoadPackage();
                mod.LoadData();
                mod.LoadAssembly();
            }


            //最终可以通过MOD/Lua/名字去查代码
            //for (int i = 0; i < mModList.Count; i++)
            //    mModList[i].LoadLanguage();

            ////Task.Run(() =>
            ////{
            ////    try
            ////    {
            ////        for (int i = 0; i < mModList.Count; i++)
            ////            mModList[i].LoadScenario();
            ////    }
            ////    catch (System.Exception e)
            ////    {
            ////        Sango.Log.Error(e + e.StackTrace);
            ////    }
            ////}
            ////);

            //for (int i = 0; i < mModList.Count; i++)
            //    mModList[i].LoadScenario();

            //for (int i = 0; i < mModList.Count; i++)
            //    mModList[i].LoadUI();
            //for (int i = 0; i < mModList.Count; i++)
            //    mModList[i].LoadPackage();
            //for (int i = 0; i < mModList.Count; i++)
            //    mModList[i].LoadData();
            //for (int i = 0; i < mModList.Count; i++)
            //    Path.AddSearchPath($"{mModList[i].ModDir}", true);
            //// 加载dll
            //for (int i = 0; i < mModList.Count; i++)
            //    mModList[i].LoadAssembly();

            Scenario.OnModInitEnd();
        }

        /// <summary>
        /// 遍历文件,找到Mod下所有这个路径的文件,一般用来合并json文件
        /// </summary>
        /// <param name="path">Assets/AA/BB/cc.dd</param>
        /// <param name="action"></param>
        public void EnumFiles(string path, System.Action<string> action)
        {
            for (int i = 0; i < mModList.Count; i++)
            {
                Mod mod = mModList[i];
                string targetFile = mod.GetFullPath(path);
                if(File.Exists(targetFile))
                    action(targetFile);
            }
        }

        /// <summary>
        /// 遍历文件夹,找到Mod下所有这个路径下的文件,一般用来合并指定类型的json文件
        /// </summary>
        /// <param name="path">Assets/AA/BB/cc.dd</param>
        /// <param name="action"></param>
        public void EnumFiles(string path, string searchPattern, SearchOption searchOption, System.Action<string> action)
        {
            for (int i = 0; i < mModList.Count; i++)
            {
                Mod mod = mModList[i];
                string targetDir = mod.GetFullPath(path);
                Directory.EnumFiles(targetDir, searchPattern, searchOption, action);
            }
        }

        /// <summary>
        /// 遍历文件,找到Mod下所有这个路径的文件,一般用来合并json文件
        /// </summary>
        /// <param name="path">Assets/AA/BB/cc.dd</param>
        /// <param name="action"></param>
        public void EnumDirectory(string path, System.Action<string> action)
        {
            for (int i = 0; i < mModList.Count; i++)
            {
                Mod mod = mModList[i];
                string targetFile = mod.GetFullPath(path);
                if (Directory.Exists(targetFile))
                    action(targetFile);
            }
        }
    }
}
