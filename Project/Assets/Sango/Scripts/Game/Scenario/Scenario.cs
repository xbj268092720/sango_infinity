using TKNewtonsoft.Json;
using Sango.Game.Render;
using Sango.Render;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Sango.Game
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Scenario : SangoObject
    {
        public override SangoObjectType ObjectType { get { return SangoObjectType.Scenario; } }

        #region Data
        [JsonProperty(Order = -97)] public ScenarioInfo Info { get; internal set; }
        [JsonProperty(Order = -96)] public ScenarioCommonData CommonData { internal set; get; }
        [JsonProperty(Order = -95)] public ScenarioVariables Variables { internal set; get; }
        [JsonProperty(Order = -94)] public Map Map { internal set; get; }

        [JsonConverter(typeof(SangoObjectSetConverter<Force>))]
        [JsonProperty] public SangoObjectSet<Force> forceSet = new SangoObjectSet<Force>();

        [JsonConverter(typeof(SangoObjectSetConverter<Corps>))]
        [JsonProperty] public SangoObjectSet<Corps> corpsSet = new SangoObjectSet<Corps>();

        // 这个特殊点,会判断Variables的关卡和港口索引来创建不同的类型
        [JsonConverter(typeof(SangoObjectSetCityConverter))]
        [JsonProperty] public SangoObjectSet<City> citySet = new SangoObjectSet<City>();

        [JsonConverter(typeof(SangoObjectSetConverter<Person>))]
        [JsonProperty] public SangoObjectSet<Person> personSet = new SangoObjectSet<Person>();

        [JsonConverter(typeof(SangoObjectSetConverter<Troop>))]
        [JsonProperty] public SangoObjectSet<Troop> troopsSet = new SangoObjectSet<Troop>();

        [JsonConverter(typeof(SangoObjectSetConverter<Building>))]
        [JsonProperty] public SangoObjectSet<Building> buildingSet = new SangoObjectSet<Building>();

        [JsonConverter(typeof(SangoObjectSetConverter<Fire>))]
        [JsonProperty] public SangoObjectSet<Fire> fireSet = new SangoObjectSet<Fire>();

        /// 结盟信息
        /// </summary>
        [JsonConverter(typeof(SangoObjectSetConverter<Alliance>))]
        [JsonProperty] public SangoObjectSet<Alliance> allianceSet = new SangoObjectSet<Alliance>();

        /// <summary>
        /// 关系信息
        /// </summary>
        [JsonProperty] public int[][] RelationMap { get; set; }

        public Force Add(Force force) { forceSet.Add(force); return force; }
        public Corps Add(Corps corps) { corpsSet.Add(corps); return corps; }
        public City Add(City city) { citySet.Add(city); return city; }
        public Person Add(Person person) { personSet.Add(person); return person; }
        public Troop Add(Troop troop)
        {
            troopsSet.Add(troop);
            GameEvent.OnTroopCreated?.Invoke(troop, this);
            return troop;
        }
        public Building Add(Building building) { buildingSet.Add(building); return building; }
        public Fire Add(Fire fire) { fireSet.Add(fire); return fire; }
        public Alliance Add(Alliance alliance) { allianceSet.Add(alliance); return alliance; }
        public Force Remove(Force force) {
            forceSet.Remove(force); return force; }
        public Corps Remove(Corps corps) { corpsSet.Remove(corps); return corps; }
        public City Remove(City city) { citySet.Remove(city); return city; }
        public Person Remove(Person person) { personSet.Remove(person); return person; }
        public Troop Remove(Troop troop)
        {
            troopsSet.Remove(troop);
            GameEvent.OnTroopDestroyed?.Invoke(troop, this);
            return troop;
        }
        public Building Remove(Building building) { buildingSet.Remove(building); return building; }
        public Fire Remove(Fire fire) { fireSet.Remove(fire); return fire; }
        public Alliance Remove(Alliance alliance) { allianceSet.Remove(alliance); return alliance; }

        public Dictionary<string, List<City>> cityPathMap;

        #endregion Data
        public static Scenario Cur { get; private set; }
        public static List<Scenario> all_scenario_list = new List<Scenario>();
        public static Scenario CurSelected { get; set; }

        public string FilePath { internal set; get; }
        private Queue<Force> runForces = new Queue<Force>();
        public Force CurRunForce { get; private set; }
        public SeasonType CurSeason { get { return GameDefine.SeasonInMonth[Info.month - 1]; } }

        List<int> startPlayerList;

        /// <summary>
        /// 调试用
        /// </summary>
        internal int PauseTrunCount = -1;
        public int TurnCount => Info.turnCount;

        public bool useThreadRun = false;
        Task task;

        public Scenario(string filePath)
        {
            this.FilePath = filePath;
            LoadInfo();
        }

        List<IDatabase> prepareList = new List<IDatabase>();
        List<IDatabase> eventReciveList = new List<IDatabase>();

        public Scenario()
        {
        }

        public Database<T> GetDatabase<T>() where T : SangoObject, new()
        {
            Type tType = typeof(T);
            if (tType == typeof(Person))
            {
                return personSet as Database<T>;
            }
            else if (tType == typeof(Force))
            {
                return forceSet as Database<T>;
            }
            else if (tType == typeof(Troop))
            {
                return troopsSet as Database<T>;
            }
            else if (tType == typeof(City))
            {
                return citySet as Database<T>;
            }
            else if (tType == typeof(Port))
            {
                return citySet as Database<T>;
            }
            else if (tType == typeof(Gate))
            {
                return citySet as Database<T>;
            }
            else if (tType == typeof(Building))
            {
                return buildingSet as Database<T>;
            }
            else if (tType == typeof(Corps))
            {
                return corpsSet as Database<T>;
            }
            else if (tType == typeof(Fire))
            {
                return fireSet as Database<T>;
            }
            else if (tType == typeof(Alliance))
            {
                return allianceSet as Database<T>;
            }
            else if (tType == typeof(TerrainType))
            {
                return CommonData.TerrainTypes as Database<T>;
            }
            else if (tType == typeof(BuildingType))
            {
                return CommonData.BuildingTypes as Database<T>;
            }
            else if (tType == typeof(Feature))
            {
                return CommonData.Features as Database<T>;
            }
            else if (tType == typeof(TroopType))
            {
                return CommonData.TroopTypes as Database<T>;
            }
            else if (tType == typeof(TroopAnimation))
            {
                return CommonData.TroopAnimations as Database<T>;
            }
            else if (tType == typeof(AttributeChangeType))
            {
                return CommonData.AttributeChangeTypes as Database<T>;
            }
            else if (tType == typeof(PersonAttributeType))
            {
                return CommonData.PersonAttributeTypes as Database<T>;
            }
            else if (tType == typeof(CityLevelType))
            {
                return CommonData.CityLevelTypes as Database<T>;
            }
            else if (tType == typeof(Flag))
            {
                return CommonData.Flags as Database<T>;
            }
            else if (tType == typeof(State))
            {
                return CommonData.States as Database<T>;
            }
            else if (tType == typeof(CityLevelType))
            {
                return CommonData.CityLevelTypes as Database<T>;
            }
            else if (tType == typeof(Official))
            {
                return CommonData.Officials as Database<T>;
            }
            else if (tType == typeof(Skill))
            {
                return CommonData.Skills as Database<T>;
            }
            else if (tType == typeof(PersonLevel))
            {
                return CommonData.PersonLevels as Database<T>;
            }
            else if (tType == typeof(ItemType))
            {
                return CommonData.ItemTypes as Database<T>;
            }
            else if (tType == typeof(JobType))
            {
                return CommonData.JobTypes as Database<T>;
            }
            else if (tType == typeof(Buff))
            {
                return CommonData.Buffs as Database<T>;
            }
            else if (tType == typeof(Technique))
            {
                return CommonData.Techniques as Database<T>;
            }
            else if (tType == typeof(Personality))
            {
                return CommonData.Personalities as Database<T>;
            }
            else if (tType == typeof(Argumentation))
            {
                return CommonData.Argumentations as Database<T>;
            }
            return null;
        }

        public T GetObject<T>(int id) where T : SangoObject, new()
        {
            return GetObject(id, typeof(T)) as T;
        }

        public object GetObject(int id, Type tType)
        {
            if (tType == typeof(Person))
            {
                return personSet.Get(id);
            }
            else if (tType == typeof(Force))
            {
                return forceSet.Get(id);
            }
            else if (tType == typeof(Troop))
            {
                return troopsSet.Get(id);
            }
            else if (tType == typeof(City))
            {
                return citySet.Get(id);
            }
            else if (tType == typeof(Building))
            {
                return buildingSet.Get(id);
            }
            else if (tType == typeof(Corps))
            {
                return corpsSet.Get(id);
            }
            else if (tType == typeof(Fire))
            {
                return fireSet.Get(id);
            }
            else if (tType == typeof(Alliance))
            {
                return allianceSet.Get(id);
            }
            else if (tType == typeof(TerrainType))
            {
                return CommonData.TerrainTypes.Get(id);
            }
            else if (tType == typeof(BuildingType))
            {
                return CommonData.BuildingTypes.Get(id);
            }
            else if (tType == typeof(Feature))
            {
                return CommonData.Features.Get(id);
            }
            else if (tType == typeof(TroopType))
            {
                return CommonData.TroopTypes.Get(id);
            }
            else if (tType == typeof(TroopAnimation))
            {
                return CommonData.TroopAnimations.Get(id);
            }
            else if (tType == typeof(AttributeChangeType))
            {
                return CommonData.AttributeChangeTypes.Get(id);
            }
            else if (tType == typeof(PersonAttributeType))
            {
                return CommonData.PersonAttributeTypes.Get(id);
            }
            else if (tType == typeof(CityLevelType))
            {
                return CommonData.CityLevelTypes.Get(id);
            }
            else if (tType == typeof(Flag))
            {
                return CommonData.Flags.Get(id);
            }
            else if (tType == typeof(State))
            {
                return CommonData.States.Get(id);
            }
            else if (tType == typeof(CityLevelType))
            {
                return CommonData.CityLevelTypes.Get(id);
            }
            else if (tType == typeof(Official))
            {
                return CommonData.Officials.Get(id);
            }
            else if (tType == typeof(Skill))
            {
                return CommonData.Skills.Get(id);
            }
            else if (tType == typeof(PersonLevel))
            {
                return CommonData.PersonLevels.Get(id);
            }
            else if (tType == typeof(ItemType))
            {
                return CommonData.ItemTypes.Get(id);
            }
            else if (tType == typeof(JobType))
            {
                return CommonData.JobTypes.Get(id);
            }
            else if (tType == typeof(Buff))
            {
                return CommonData.Buffs.Get(id);
            }
            else if (tType == typeof(Technique))
            {
                return CommonData.Techniques.Get(id);
            }
            else if (tType == typeof(Personality))
            {
                return CommonData.Personalities.Get(id);
            }
            else if (tType == typeof(Argumentation))
            {
                return CommonData.Argumentations.Get(id);
            }
            //else if (tType == typeof(CityLevelType))
            //{
            //    return CommonData.CityLevelTypes.Get(id);
            //}
            //else if (tType == typeof(CityLevelType))
            //{
            //    return CommonData.CityLevelTypes.Get(id);
            //}
            return null;
        }
        public static void Add(string path)
        {
            if (!File.Exists(path))
                return;

            Scenario scenario = new Scenario(path);
            all_scenario_list.Add(scenario);
        }
        public void LoadInfo()
        {
            LoadInfo(FilePath);
        }
        public void LoadInfo(string path)
        {
            FilePath = path;

            using (StreamReader file = System.IO.File.OpenText(FilePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                while (reader.Read()) // Advances to the next token in the JSON stream.
                {
                    if (reader.TokenType == JsonToken.StartObject) // Check for start of an object in the JSON stream.
                    {
                        if (!string.IsNullOrEmpty(reader.Path) && reader.Path == "Info")
                        {
                            Info = JsonSerializer.CreateDefault().Deserialize<ScenarioInfo>(reader); // Deserialize the object.
                            Name = Info.name;
                            return;
                        }
                    }
                }
            }
        }

        public void LoadVariables()
        {
            using (StreamReader file = System.IO.File.OpenText(FilePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                while (reader.Read()) // Advances to the next token in the JSON stream.
                {
                    if (reader.TokenType == JsonToken.StartObject) // Check for start of an object in the JSON stream.
                    {
                        if (!string.IsNullOrEmpty(reader.Path) && reader.Path == "Variables")
                        {
                            Variables = JsonSerializer.CreateDefault().Deserialize<ScenarioVariables>(reader); // Deserialize the object.
                            Name = Info.name;
                            return;
                        }
                    }
                }
            }

            if (Variables == null)
                Variables = new ScenarioVariables();
        }

        public void LoadContent()
        {
            LoadContent(FilePath);
        }

        public void LoadBaseContent()
        {
            Cur = this;
            IsAlive = false;

            // 由此来判断是否加载过base了
            if (Map != null && Map.CellSet != null)
                return;

            if (!Info.isSave)
            {
                if (CommonData == null)
                    CommonData = GameData.Instance.LoadNewCommonData();

                if (Variables == null)
                    Variables = new ScenarioVariables();

                if (Map == null)
                    Map = new Map();
            }

            JsonConvert.PopulateObject(File.ReadAllText(FilePath), this);


            Map.Load(this);

            GameEvent.OnScenarioPrepare?.Invoke(this);
        }

        public void LoadContent(string path)
        {
            LoadBaseContent();

            if (startPlayerList != null)
            {
                Info.playerForceList = startPlayerList.ToArray();
                startPlayerList = null;
            }

            // 玩家确定
            if (Info.playerForceList != null && Info.playerForceList.Length > 0)
            {
                forceSet.ForEach(x =>
                {
                    for (int k = 0; k < Info.playerForceList.Length; k++)
                    {
                        if (Info.playerForceList[k] == x.Id)
                        {
                            x.IsPlayer = true;
                            return;
                        }
                    }
                });
            }




            prepareList.Add(forceSet);
            prepareList.Add(corpsSet);
            prepareList.Add(citySet);
            prepareList.Add(personSet);
            prepareList.Add(buildingSet);
            prepareList.Add(troopsSet);

            eventReciveList.Add(personSet);
            eventReciveList.Add(buildingSet);
            eventReciveList.Add(citySet);
            eventReciveList.Add(troopsSet);
            eventReciveList.Add(forceSet);
        }

        public void LoadWorld()
        {
            MapRender.Instance.Init();
            MapRender.Instance.OnMapLoaded += OnWorldLoaded;
            MapRender.Instance.LoadMap(Map.FileName);
        }

        public void OnWorldLoaded()
        {
            GameEvent.OnWorldLoadEnd?.Invoke(this);
            this.Map.Init(this);
            this.Prepare();
            this.Init(this);
            GameController.Instance.Reset();
            this.Start();
            MapRender.Instance.OnMapLoaded -= OnWorldLoaded;
        }

        //public override void Load(BinaryReader reader)
        //{
        //    Info.Load(reader);

        //}

        //public bool Save(string path)
        //{
        //    XmlDocument xmlDocument = new XmlDocument();
        //    xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));//xml文件头
        //    XmlLoader.Save(this, xmlDocument, "Scenario");
        //    using (Stream stream = System.IO.File.Open(path, FileMode.Create, FileAccess.Write))
        //    {
        //        using (XmlTextWriter writer = new XmlTextWriter(stream, new UTF8Encoding(false)))
        //        {
        //            writer.Formatting = Formatting.Indented;
        //            xmlDocument.Save(writer);
        //        }
        //    }
        //    return true;
        //}

        public static void OnModInitStart()
        {
            all_scenario_list.Clear();
            string path = $"{Path.ContentRootPath}/Scenario";
            Directory.EnumFiles(path, "*.json", SearchOption.AllDirectories, (file) =>
            {
                Sango.Log.Print($"Find Scenario: {file}");
                Add(file);
                ShortScenario.Add(file);
            });
        }

        public static void OnModInitEnd()
        {
            all_scenario_list.Sort((a, b) =>
            {
                if (a.Info.priority == b.Info.priority)
                {
                    return a.Info.id.CompareTo(b.Info.id);
                }
                else
                {
                    return a.Info.priority.CompareTo(b.Info.priority);
                }
            });

            ShortScenario.all_scenario_info_list.Sort((a, b) =>
            {
                if (a.Info.priority == b.Info.priority)
                {
                    return a.Info.id.CompareTo(b.Info.id);
                }
                else
                {
                    return a.Info.priority.CompareTo(b.Info.priority);
                }
            });
        }

        public static void StartScenario(Scenario scenario, List<int> playerList)
        {
            scenario.startPlayerList = playerList;
            StartScenario(scenario);
        }

        public static void StartScenario(Scenario scenario)
        {
            GameRandom.Init();
            Cur = scenario;
            scenario.IsAlive = false;
            GameEvent.OnScenarioLoadStart?.Invoke(scenario);
            Cur.LoadContent();
            GameEvent.OnScenarioLoadEnd?.Invoke(Cur);
            GameEvent.OnWorldLoadStart?.Invoke(Cur);
            Cur.LoadWorld();
            //Cur.OnWorldLoaded();
            //Event.OnScenarioEnd?.Invoke(Cur);
            //Cur = null;
        }

        public override void Clear()
        {
            GameEvent.OnGameShutdown -= OnGameShutdown;
            GameEvent.OnGamePause -= OnGamePause;
            GameEvent.OnGameResume -= OnGameResume;
            MapRender.Instance.OnMapLoaded -= OnWorldLoaded;
            IsAlive = false;
            base.Clear();
            CommonData = null;
            Variables = null;
            if (Map != null)
            {
                Map.Clear();
                Map = null;
            }
            forceSet.Clear();
            corpsSet.Clear();
            citySet.Clear();
            personSet.Clear();
            troopsSet.Clear();
            buildingSet.Clear();
            fireSet.Clear();
            allianceSet.Clear();
            RelationMap = null;
            prepareList.Clear();
            eventReciveList.Clear();
        }

        public void OnGameShutdown()
        {
            End();
            Clear();
            Cur = null;
        }
        public void OnGamePause()
        {
            isThreadPause = true;
        }
        public void OnGameResume()
        {
            isThreadPause = false;
        }
        


        // 在Prepare之后
        public override void Init(Scenario scenario)
        {
            CommonData.Init();

            GameEvent.OnGameShutdown += OnGameShutdown;
            GameEvent.OnGamePause += OnGamePause;
            GameEvent.OnGameResume += OnGameResume;

            SeasonType cur_season = GameDefine.SeasonInMonth[Info.month - 1];
            MapRender.Instance.ChangeSeason((int)cur_season);

            // 初始化路径缓存
            cityPathMap = new Dictionary<string, List<City>>();

            for (int i = 0; i < prepareList.Count; ++i)
            {
                prepareList[i].ForEach(o => { o.Init(this); });
            }

            if (RelationMap == null)
            {
                int forceCount = forceSet.Count;
                RelationMap = new int[forceCount][];
                for (int i = 0; i < forceCount; ++i)
                {
                    RelationMap[i] = new int[forceCount];
                }

                for (int i = 0; i < forceCount; ++i)
                {
                    for (int j = i + 1; j < forceCount; ++j)
                    {
                        RelationMap[i][j] = 0;
                        RelationMap[j][i] = 0;
                    }
                }
            }

            GameEvent.OnScenarioInit?.Invoke(this);
        }

        /// <summary>
        /// 在Init之前
        /// </summary>
        public void Prepare()
        {
            GameEvent.OnScenarioPrepare?.Invoke(this);

            for (int i = 0; i < prepareList.Count; ++i)
            {
                prepareList[i].ForEach(o => { o.OnScenarioPrepare(this); });
            }
            //MapRender.Instance.Update();
        }


        bool isThreadPause = false;
        public void Start()
        {
            MapRender.Instance.SetCamera(Info.cameraPosition, Info.cameraRotation, Info.cameraDistance);

            GameEvent.OnScenarioStart?.Invoke(this);

            Window.Instance.Close("window_start");
            Window.Instance.Close("window_loading");
            Window.Instance.Open("window_game");
#if SANGO_DEBUG_AI
            GameAIDebug.Instance.Init();
#endif
            MakeForceQuene();

            // 恢复游戏
            if (Info.curForceId > 0)
            {
                Force force = runForces.Dequeue();
                while (force != null)
                {
                    if (force.Id == Info.curForceId)
                    {
                        CurRunForce = force;
                        HasTurnStarted = true;
                        break;
                    }
                    force = runForces.Dequeue();
                }
            }

            GameController.Instance.Enabled = true;

            Run();

            IsAlive = true;
            if (useThreadRun)
            {
                task = Task.Run(() =>
                {
                    while (IsAlive)
                    {
                        if (!isThreadPause)
                        {
                            Run();
                            Thread.Sleep(1);
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                });
            }
        }

        public void End()
        {
            GameEvent.OnScenarioEnd?.Invoke(this);
        }

        public void MakeForceQuene()
        {
            // 玩家排到最前面
            if (Info.playerForceList != null)
            {
                for (int k = 0; k < Info.playerForceList.Length; k++)
                {
                    Force force = forceSet.Get(Info.playerForceList[k]);
                    if (force != null && force.IsAlive)
                    {
                        force.ActionOver = false;
                        runForces.Enqueue(force);
                    }
                }

            }

            forceSet.ForEach(force =>
            {
                if (force.IsAlive)
                {
                    if (Info.playerForceList == null || !Info.playerForceList.Contains(force.Id))
                    {
                        force.ActionOver = false;
                        runForces.Enqueue(force);
                    }
                }
            });
        }

        public bool TurnStart()
        {
            if (HasTurnStarted) return true;
            for (int i = 1; i < personSet.Count; i++)
            {
                Person person = personSet[i];
                if (person != null && person.IsAlive)
                    person.OnTurnStart(this);
            }

            for (int i = 1; i < allianceSet.Count; i++)
            {
                Alliance a = allianceSet[i];
                if (a != null && a.IsAlive)
                    a.OnTurnStart(this);
            }

            for (int i = 1; i < fireSet.Count; i++)
            {
                Fire a = fireSet[i];
                if (a != null && a.IsAlive)
                    a.OnTurnStart(this);
            }

            // 检查并触发外交事件
            CheckDiplomacyEvents();

            allianceSet.RemoveAll(a => !a.IsAlive);

            HasTurnStarted = true;
            return true;
        }

        /// <summary>
        /// 检查并触发外交事件
        /// </summary>
        private void CheckDiplomacyEvents()
        {
            // 遍历所有势力对，检查外交事件
            int forceCount = forceSet.Count;
            for (int i = 0; i < forceCount; i++)
            {
                Force forceA = forceSet[i];
                if (forceA == null || !forceA.IsAlive)
                    continue;

                for (int j = i + 1; j < forceCount; j++)
                {
                    Force forceB = forceSet[j];
                    if (forceB == null || !forceB.IsAlive)
                        continue;

                    // 检查并触发外交事件
                    DiplomacyEventManager.Instance.CheckAndTriggerEvents(forceA, forceB);
                }
            }
        }
        public bool RunForces()
        {
            // 处理当前势力的逻辑
            if (CurRunForce != null && CurRunForce.IsAlive)
            {
                if (!CurRunForce.Run(this))
                    return false;
                else
                {
                    CurRunForce.OnForceTurnEnd(this);
                    GameEvent.OnForceTurnEnd?.Invoke(CurRunForce, this);
                    CurRunForce = null;
                }
            }
            // 完成一轮
            if (runForces.Count <= 0)
            {
                return true;
            }
            CurRunForce = runForces.Dequeue();
            while(!CurRunForce.IsAlive && runForces.Count > 0)
                CurRunForce = runForces.Dequeue();

            if (CurRunForce != null && CurRunForce.IsAlive)
            {
                Info.curForceId = CurRunForce.Id;
                CurRunForce.OnForceTurnStart(this);
                GameEvent.OnForceTurnStart?.Invoke(CurRunForce, this);
            }
            else
            {
                CurRunForce = null;
            }
            return false;
        }

        public bool TurnEnd()
        {
            if (HasTurnEnded) return true;
            for (int i = 1; i < fireSet.Count; i++)
            {
                Fire a = fireSet[i];
                if (a != null && a.IsAlive)
                    a.OnTurnEnd(this);
            }

            HasTurnEnded = true;
            Info.turnCount++;
            return true;
        }

        public bool IncreaseDate()
        {
            SeasonType last_season = GameDefine.SeasonInMonth[Info.month - 1];
            Info.day += 10;

            bool hasYear = false;
            bool hasMonth = false;
            if (Info.day > 30)
            {
                Info.day -= 30;
                Info.month += 1;
                hasMonth = true;
                if (Info.month > 12)
                {
                    Info.month -= 12;
                    Info.year += 1;
                    hasYear = true;

                }
            }
            if (hasYear)
            {
                OnYearStart(this);
                GameEvent.OnYearUpdate?.Invoke(this);
            }
            if (hasMonth)
            {
                OnMonthStart(this);
                GameEvent.OnMonthStart?.Invoke(this);
                GameEvent.OnMonthUpdate?.Invoke(this);
            }
            OnDayStart(this);
            GameEvent.OnDayUpdate?.Invoke(this);
            OnDayEnd(this);
            if (hasMonth)
            {
                OnMonthEnd(this);
                GameEvent.OnMonthEnd?.Invoke(this);
            }
            if (hasYear)
            {
                OnYearEnd(this);
            }
            SeasonType cur_season = GameDefine.SeasonInMonth[Info.month - 1];
            if (cur_season != last_season)
            {
                OnSeasonStart(this);
                MapRender.Instance.ChangeSeason((int)cur_season);
                GameEvent.OnSeasonUpdate?.Invoke(this);
                OnSeasonEnd(this);
            }

            //if (Info.year == 500)
            //    IsAlive = false;

            return true;
        }
        float waitTime = 5;

        internal bool HasTurnStarted = false;
        internal bool HasTurnEnded = false;

        public void Run()
        {
            // 事件处理
            if (!RenderEvent.Instance.Update(this, Time.deltaTime))
                return;

            if (!IsAlive)
                return;
            //#if SANGO_DEBUG
            if (PauseTrunCount == Info.turnCount)
                return;
            //#endif
            if (!TurnStart())
                return;

            if (!RunForces())
                return;

            if (!TurnEnd())
                return;

            if (!IncreaseDate())
                return;

#if SANGO_DEBUG
            Sango.Log.Warning($"{GetDateStr()}  第{Info.turnCount}回");
#endif
            MakeForceQuene();

            HasTurnEnded = false;
            HasTurnStarted = false;

            waitTime = 1;
        }


        public override bool OnDayStart(Scenario scenario)
        {
            for (int i = 0; i < eventReciveList.Count; ++i)
            {
                eventReciveList[i].ForEach(o => { if (o.IsAlive) o.OnDayStart(this); });
            }
            return base.OnDayStart(scenario);
        }

        public override bool OnDayEnd(Scenario scenario)
        {
            for (int i = 0; i < eventReciveList.Count; ++i)
            {
                eventReciveList[i].ForEach(o => { if (o.IsAlive) o.OnDayEnd(this); });
            }
            return base.OnDayEnd(scenario);
        }

        public override bool OnMonthStart(Scenario scenario)
        {
            for (int i = 0; i < eventReciveList.Count; ++i)
            {
                eventReciveList[i].ForEach(o => { if (o.IsAlive) o.OnMonthStart(this); });
            }
            return base.OnMonthStart(scenario);
        }
        public override bool OnMonthEnd(Scenario scenario)
        {
            for (int i = 0; i < eventReciveList.Count; ++i)
            {
                eventReciveList[i].ForEach(o => { if (o.IsAlive) o.OnMonthEnd(this); });
            }
            return base.OnMonthEnd(scenario);
        }
        public override bool OnYearStart(Scenario scenario)
        {
            for (int i = 0; i < eventReciveList.Count; ++i)
            {
                eventReciveList[i].ForEach(o => { if (o.IsAlive) o.OnYearStart(this); });
            }
            return base.OnYearStart(scenario);
        }
        public override bool OnYearEnd(Scenario scenario)
        {
            for (int i = 0; i < eventReciveList.Count; ++i)
            {
                eventReciveList[i].ForEach(o => { if (o.IsAlive) o.OnYearEnd(this); });
            }
            return base.OnYearEnd(scenario);
        }
        public override bool OnSeasonStart(Scenario scenario)
        {
            for (int i = 0; i < eventReciveList.Count; ++i)
            {
                eventReciveList[i].ForEach(o => { if (o.IsAlive) o.OnSeasonStart(this); });
            }
            return base.OnSeasonStart(scenario);
        }
        public override bool OnSeasonEnd(Scenario scenario)
        {
            for (int i = 0; i < eventReciveList.Count; ++i)
            {
                eventReciveList[i].ForEach(o => { if (o.IsAlive) o.OnSeasonEnd(this); });
            }
            return base.OnSeasonEnd(scenario);
        }

        /// <summary>
        /// 获取城市之间的相隔距离
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int GetCityDistance(City a, City b)
        {
            // 检查参数
            if (a == null || b == null)
                return -1;

            // 检查是否是同一个城市
            if (a == b)
                return 0;

            // 使用寻路方法获取距离
            List<City> path = FindShortestPath(a, b);
            return path != null ? path.Count - 1 : -1;
        }

        /// <summary>
        /// 寻找两个城市之间的最短路径
        /// </summary>
        /// <param name="startCity">起始城市</param>
        /// <param name="endCity">目标城市</param>
        /// <returns>最短路径的城市列表</returns>
        public List<City> FindShortestPath(City startCity, City endCity)
        {
            // 检查参数
            if (startCity == null || endCity == null)
                return null;

            // 检查是否是同一个城市
            if (startCity == endCity)
            {
                return new List<City> { startCity };
            }

            // 检查缓存中是否已有路径
            string key = $"{startCity.Id}_{endCity.Id}";
            if (cityPathMap.ContainsKey(key))
            {
                return cityPathMap[key];
            }

            // 如果缓存中没有，使用BFS算法重新计算路径
            Dictionary<City, City> parentMap = new Dictionary<City, City>();
            Queue<City> queue = new Queue<City>();
            HashSet<City> visited = new HashSet<City>();

            queue.Enqueue(startCity);
            visited.Add(startCity);
            parentMap[startCity] = null;

            bool found = false;
            while (queue.Count > 0)
            {
                City current = queue.Dequeue();

                foreach (City neighbor in current.NeighborList)
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                        parentMap[neighbor] = current;

                        if (neighbor == endCity)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                    break;
            }

            // 如果找到路径，构建路径并缓存
            if (found)
            {
                List<City> path = new List<City>();
                City current = endCity;
                while (current != null)
                {
                    path.Add(current);
                    current = parentMap[current];
                }
                path.Reverse();
                cityPathMap[key] = path;
                return path;
            }

            // 如果没有找到路径，返回null
            return null;
        }

        public static void Pause()
        {
            Scenario.Cur.IsAlive = false;
        }
        public static void Resume()
        {
            Scenario.Cur.IsAlive = true;
            //Scenario.Cur.CurRunForce.IsPlayerControlled = false;
            Scenario.Cur.PauseTrunCount = -1;
        }

        public static void NextForce()
        {
            Scenario.Cur.IsAlive = true;
            //Scenario.Cur.CurRunForce.IsPlayerControlled = false;
            Force nextForce = Scenario.Cur.runForces.Peek();
            if (nextForce != null)
            {
                //nextForce.IsPlayerControlled = true;
            }
            else
            {
                Scenario.Cur.IsAlive = false;
            }
        }

        public static void NextTurn()
        {
            //Scenario.Cur.CurRunForce.IsPlayerControlled = false;
            Scenario.Cur.PauseTrunCount = Scenario.Cur.Info.turnCount + 1;
            Scenario.Cur.IsAlive = true;
        }

        public int GetRelation(Force forceA, Force forceB)
        {
            if (forceA == null || forceB == null || forceA == forceB)
                return 0;

            // 确保索引有效
            if (forceA.Id < 0 || forceA.Id >= RelationMap.Length || forceB.Id < 0 || forceB.Id >= RelationMap[0].Length)
                return 0;

            return RelationMap[forceA.Id][forceB.Id];
        }

        public void AddRelation(Force forceA, Force forceB, int v)
        {
            if (forceA == null || forceB == null || forceA == forceB)
                return;

            // 确保索引有效
            if (forceA.Id < 0 || forceA.Id >= RelationMap.Length || forceB.Id < 0 || forceB.Id >= RelationMap[0].Length)
                return;

            int r = RelationMap[forceA.Id][forceB.Id] + v;

            if (r < -5000) r = -5000;
            else if (r > 5000) r = 5000;

            RelationMap[forceA.Id][forceB.Id] = r;
            RelationMap[forceB.Id][forceA.Id] = r;
        }

        public string GetDateStr()
        {
            return $"{Info.year}年 {Info.month}月 {Info.day}日";
        }

        public Troop CreateTroop()
        {
            return new Troop();
        }

        public void Save(string path)
        {

            Info.isSave = true;
            Info.cameraPosition = MapRender.Instance.mapCamera.position;
            Info.cameraRotation = MapRender.Instance.mapCamera.lookRotate;
            Info.cameraDistance = MapRender.Instance.mapCamera.distance;
            Info.dateTime = DateTime.Now.ToFileTime();
            Sango.Directory.Create(path, false);
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = TKNewtonsoft.Json.Formatting.Indented;
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; // 忽略循环引用
            JsonSerializer serializer = JsonSerializer.CreateDefault(jsonSerializerSettings);
            using (StreamWriter writer = System.IO.File.CreateText(path))
            {
                serializer.Serialize(writer, this);
            }
        }
    }
}
