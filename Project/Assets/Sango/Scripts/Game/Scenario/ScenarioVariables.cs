using System.IO;
using TKNewtonsoft.Json;
using UnityEngine;


namespace Sango.Game
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ScenarioVariables : SangoObject
    {
        /// <summary>
        /// 最大可存储的行动力上限
        /// </summary>
        [JsonProperty] public int ActionPointLimit = 255;

        /// <summary>
        /// 行动力获取倍率
        /// </summary>
        [JsonProperty] public float ActionPointFactor = 1;

        /// <summary>
        /// 真实年龄开关
        /// </summary>
        [JsonProperty] public bool AgeEnabled = true;

        /// <summary>
        /// 能力随年龄变化
        /// </summary>
        [JsonProperty] public bool EnableAgeAbilityFactor = true;

        /// <summary>
        /// 能力每级经验
        /// </summary>
        [JsonProperty] public ushort AbilityExpLevelNeed = 1000;

        /// <summary>
        /// 最高能力等级
        /// </summary>
        [JsonProperty] public byte MaxAbilityLevel = 10;

        /// <summary>
        /// 属性每点经验
        /// </summary>
        [JsonProperty] public ushort AttributeExpLevelNeed = 250;

        /// <summary>
        /// 属性成长不超过这个点数
        /// </summary>
        [JsonProperty] public byte MaxAttributeGet = 30;

        /// <summary>
        /// 基础伤害
        /// </summary>
        [JsonProperty] public float fight_base_damage = 64;

        /// <summary>
        /// 基准兵力(攻守兵力差)
        /// </summary>
        [JsonProperty] public float fight_base_troops_need = 2000;

        /// <summary>
        /// 每多基准兵力,获得一次兵力系数增益
        /// </summary>
        [JsonProperty] public float fight_base_troop_count = 200;

        /// <summary>
        /// 兵力系数增益
        /// </summary>
        [JsonProperty] public double fight_damage_magic_number = 0.000476190455;

        /// <summary>
        /// 伤害难度系数
        /// </summary>
        [JsonProperty] public float[] fight_damage_difficulty_factor = new float[] { 1.3f, 1, 0.7f };

        /// <summary>
        /// 难度
        /// </summary>
        [JsonProperty] public int difficulty = 1;

        /// <summary>
        /// 可招募的忠诚度,高于或等于此数值不可招募
        /// </summary>
        [JsonProperty] public float recruitableLine = 95;

        /// <summary>
        /// 每一点农业带来的粮食收入
        /// </summary>
        [JsonProperty] public int agriculture_add_food = 10;

        /// <summary>
        /// 每一点商业点带来的金币收入
        /// </summary>
        [JsonProperty] public int commerce_add_gold = 1;

        /// <summary>
        /// 部队攻击武力影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_attack_strength_factor = 7000;
        /// <summary>
        /// 部队攻击智力影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_attack_intelligence_factor = 1000;
        /// <summary>
        /// 部队攻击统率影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_attack_command_factor = 2000;
        /// <summary>
        /// 部队攻击政治影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_attack_politics_factor = 0;
        /// <summary>
        /// 部队攻击魅力影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_attack_glamour_factor = 0;

        /// <summary>
        /// 部队防御武力影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_defence_strength_factor = 1000;
        /// <summary>
        /// 部队防御智力影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_defence_intelligence_factor = 2000;
        /// <summary>
        /// 部队防御统率影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_defence_command_factor = 7000;
        /// <summary>
        /// 部队防御政治影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_defence_politics_factor = 0;
        /// <summary>
        /// 部队防御魅力影响比例(万分比)
        /// </summary>
        [JsonProperty] public int fight_troop_defence_glamour_factor = 0;

        /// <summary>
        /// 适应能力加成(百分比)
        /// </summary>
        [JsonProperty]
        public int[] troops_adaptation_level_boost = new int[]
         // C    B        A       S        SS
           {80,   90,    100,   110,    120, };

        /// <summary>
        /// 兵种克制(小数)
        /// </summary>
        [JsonProperty]
        public float[][] troops_type_restraint = new float[][]{

        //////////   0,   1杂     2枪    3戟    4弩      5骑         6运     7水    8冲      9井
        new float[] {0,   0,     0,       0,      0,       0,        0,      0,      0,       0  },      //0
        new float[] {0,   1,     1,       1,      1,       1,        1,      1,      1,       1  },      //1杂
        new float[] {0,   1,     1,       1,      1,       1.15f,    1,      1,      1,       1  },      //2枪
        new float[] {0,   1,     1,       1,      1.15f,   1,        1,      1,      1,       1  },      //3戟
        new float[] {0,   1,     1.15f,   1,      1,       1,        1,      1,      1,       1  },      //4弩
        new float[] {0,   1,     1,       1.15f,  1,       1,        1,      1,      1,       1  },      //5骑
        new float[] {0,   1,     1,       1,      1,       1,        1,      1,      1,       1  },      //6运
        new float[] {0,   1,     1,       1,      1,       1,        1,      1,      1,       1  },      //7水
        new float[] {0,   1,     1,       1,      1,       1,        1,      1,      1,       1  },      //8冲
        new float[] {0,   1,     1,       1,      1,       1,        1,      1,      1,       1  },      //9井
        };
        /// <summary>
        /// 人口系统开关
        /// </summary>
        [JsonProperty] public bool populationEnable = false;

        /// <summary>
        /// 基础人口增长率
        /// </summary>
        [JsonProperty] public float populationIncreaseBaseFactor = 0.0113f;

        /// <summary>
        /// 队伍粮食基础消耗率 1粮养10兵每回合
        /// </summary>
        [JsonProperty] public float baseFoodCostInTroop = 0.1f;

        /// <summary>
        /// 城池中粮食基础消耗率(每回合) 1粮养40兵每回合
        /// </summary>
        [JsonProperty] public float baseFoodCostInCity = 0.025f; 

        /// <summary>
        /// 城池缺粮后每回合逃跑的士兵比例
        /// </summary>
        [JsonProperty] public float runawayWhenCityFoodNotEnough = 0.1f;

        /// <summary>
        /// 民心对于收入的影响最低值
        /// </summary>
        [JsonProperty] public float popularSupportInfluenceMax = 60;

        /// <summary>
        /// 民心影响的正负范围
        /// </summary>
        [JsonProperty] public float popularSupportInfluence = 0.2f;

        /// <summary>
        /// 治安对于收入的影响最低值
        /// </summary>
        [JsonProperty] public float securityInfluenceMax = 70;

        /// <summary>
        /// 治安影响的正负范围
        /// </summary>
        [JsonProperty] public float securityInfluence = 0.1f;

        /// <summary>
        /// 治安每一点对征兵的影响值比例
        /// </summary>
        [JsonProperty] public float securityInfluenceRecruitTroops = 0.005f;

        /// <summary>
        /// 建筑最大回合数
        /// </summary>
        [JsonProperty]
        public int BuildMaxTurn = 10;

        /// <summary>
        /// 粮食倍率
        /// </summary>
        [JsonProperty] public float foodFactor = 1f;

        /// <summary>
        /// 金币倍率
        /// </summary>
        [JsonProperty] public float goldFactor = 1f;

        /// <summary>
        /// 每月变化的关系值
        /// </summary>
        [JsonProperty] public int relationChangePerMonth = -200;

        /// <summary>
        /// 每月的关系变化率
        /// </summary>
        [JsonProperty] public int relationChangeChance = 50;

        /// <summary>
        /// 破城时候的抓捕率(百分比)
        /// </summary>
        [JsonProperty] public int captureChangceWhenCityFall = 10;

        /// <summary>
        /// 最后一城时候的抓捕率(百分比)
        /// </summary>
        [JsonProperty] public int captureChangceWhenLastCityFall = 40;

        /// <summary>
        /// 队伍溃败时候的抓捕率(百分比)
        /// </summary>
        [JsonProperty] public int captureChangceWhenTroopFall = 5;

        /// <summary>
        /// 进攻时候留守最低的兵力
        /// </summary>
        [JsonProperty] public int minTroopsKeepWhenAttack = 30000;

        /// <summary>
        /// 进攻时候留守最低的粮食
        /// </summary>
        [JsonProperty] public int minFoodKeepWhenAttack = 10000;

        /// <summary>
        /// 防御时候留守最低的兵力
        /// </summary>
        [JsonProperty] public int minTroopsKeepWhenDefence = 6000;

        /// <summary>
        /// 防御时候留守最低的粮食
        /// </summary>
        [JsonProperty] public int minFoodKeepWhenDefence = 1000;

        /// <summary>
        /// 近战击溃部队缴获的金钱比例(百分比)
        /// </summary>
        [JsonProperty] public int defeatTroopCanGainGoldFactor = 60;

        /// <summary>
        /// 近战击溃部队缴获的粮食比例(百分比)
        /// </summary>
        [JsonProperty] public int defeatTroopCanGainFoodFactor = 30;

        /// <summary>
        /// 城市沦陷可以保留金钱比例(百分比)
        /// </summary>
        [JsonProperty] public int [] cityFallCanKeepGoldFactor = new int[4] { 1, 2, 4, 1 };

        /// <summary>
        /// 城市沦陷可以保留粮食比例(百分比)
        /// </summary>
        [JsonProperty] public int [] cityFallCanKeepFoodFactor = new int[4] { 1, 2, 4, 1 };

        /// <summary>
        /// 城市沦陷可以保留士兵比例(百分比)
        /// </summary>
        [JsonProperty] public int [] cityFallCanKeepTroopsFactor = new int[4] { 1, 2, 4, 1 };

        /// <summary>
        /// 城市沦陷可以保留库存比例(百分比)
        /// </summary>
        [JsonProperty] public int [] cityFallCanKeepItemFactor = new int[4] { 1, 2, 4, 1 };

        /// <summary>
        /// 城市沦陷可以保留农业比例(百分比)
        /// </summary>
        [JsonProperty] public int[] cityFallCanKeepAgriculture = new int[4] { 1, 2, 4, 1 };

        /// <summary>
        /// 城市沦陷可以保留开发比例(百分比)
        /// </summary>
        [JsonProperty] public int[] cityFallCanKeepCommerce = new int[4] { 1, 2, 4, 1 };


        /// <summary>
        /// 每级兵种适应力对技能释放成功率的加成(百分比) 需要>=A级适应力(2)
        /// </summary>
        [JsonProperty] public int skillSuccessRateAddByAbility = 5;

        /// <summary>
        /// 每级兵种适应力对技能暴击率的加成(百分比) 需要>C级适应力(1)
        /// </summary>
        [JsonProperty] public int skillCriticalRateAddByAbility = 1;

        /// <summary>
        /// 每级兵种适应力对技能暴击率的加成(百分比) 需要>C级适应力(1)
        /// </summary>
        [JsonProperty] public int baseSkillCriticalRate = 5;

        /// <summary>
        /// 武力对暴击的加成值x (武力-60) * x / 10  大于60武力,每10点武力能加成x, 只取整数部分
        /// </summary>
        [JsonProperty] public int skillCriticalRateAddByStength = 1;

        /// <summary>
        /// 暴击倍率(百分比)
        /// </summary>
        [JsonProperty] public int skillCriticalFactor = 150;

        /// <summary>
        /// 每一季度治安下降最大数
        /// </summary>
        [JsonProperty] public int securityChangeOnSeasonStart = -5;

        /// <summary>
        /// 适应名称
        /// </summary>
        [JsonProperty] public string[] personAbilityName = new string[] { "Ｃ", "Ｂ", "Ａ", "Ｓ"};

        /// <summary>
        /// 属性名称
        /// </summary>
        [JsonProperty] public string[] attributeName = new string[] { "统率", "武力", "智力", "政治", "魅力"};

        /// <summary>
        /// 属性名称-带颜色
        /// </summary>
        [JsonProperty] public string[] attributeNameWithColor = new string[] { 
            "<color=#F58080>统率</color>",
            "<color=#9866DB>武力</color>",
            "<color=#E5B462>智力</color>",
            "<color=#61E28F>政治</color>",
            "<color=#78CBF3>魅力</color>" };

        /// <summary>
        /// 基础越狱概率(万分比)
        /// </summary>
        [JsonProperty] public int baseEscapeProbabllity = 100;

        /// <summary>
        /// 越狱概率每回合增长(万分比)
        /// </summary>
        [JsonProperty] public int baseEscapeProbablilityAddByTurn = 50;

        /// <summary>
        /// 寻路安全次数限制
        /// </summary>
        [JsonProperty] public int pathfindingSafeCount = 100000;

        public float DifficultyDamageFactor
        {
            get
            {
                if (difficulty >= 0 && difficulty < fight_damage_difficulty_factor.Length)
                {
                    return fight_damage_difficulty_factor[difficulty];
                }
                return 1;
            }
        }

        public string GetAbilityName(int lvl)
        {
            if (lvl < personAbilityName.Length)
                return personAbilityName[lvl];
            else
            {
                if (GameEvent.OnGetAbilityName != null)
                    return GameEvent.OnGetAbilityName(lvl);
                return personAbilityName[personAbilityName.Length - 1];
            }
        }

        public string GetAttributeName(int lvl)
        {
            if (lvl < attributeName.Length)
                return attributeName[lvl];
            else
            {
                if (GameEvent.OnGetAttributeName != null)
                    return GameEvent.OnGetAttributeName(lvl);
                return attributeName[attributeName.Length - 1];
            }
        }

        public string GetAttributeNameWithColor(int lvl)
        {
            if (lvl < attributeNameWithColor.Length)
                return attributeNameWithColor[lvl];
            else
            {
                if (GameEvent.OnGetAttributeNameWithColor != null)
                    return GameEvent.OnGetAttributeNameWithColor(lvl);
                return attributeNameWithColor[attributeNameWithColor.Length - 1];
            }
        }
    }
}
