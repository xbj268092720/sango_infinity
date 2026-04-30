using Sango.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Sango.Render
{
    /// <summary>
    /// 地图标注管理类
    /// </summary>
    public class MapBuildinglSet : MapProperty
    {
        /// <summary>
        /// 标注集合
        /// </summary>
        public List<MapBuilding> buildings = new List<MapBuilding>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="map">地图渲染器</param>
        public MapBuildinglSet(MapRender map) : base(map)
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            base.Init();
            // 初始化标注集合
            buildings.Clear();
        }

        public MapBuilding Create(int id, int objType, int bindId, int modelId, Vector3 position, Vector3 rot, Vector3 scale)
        {
            if (modelId == 0) return null;

            // 创建标注游戏对象
            GameObject labelObj = new GameObject($"MapBuilding_{buildings.Count}");
            labelObj.transform.parent = MapRender.modelRoot;

            // 添加MapLabel组件
            MapBuilding obj = labelObj.AddComponent<MapBuilding>();
            obj.objId = id;
            obj.objType = objType;
            obj.bindId = bindId;
            obj.modelId = modelId;
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.Euler(rot);
            obj.transform.localScale = scale;
            obj.bounds = new Sango.Tools.Rect(0, 0, 32, 32);

            obj.instanceFlag = !Tools.MapEditor.IsEditOn;

            // 添加到集合
            buildings.Add(obj);

            //map.LoadModel(obj);
            if (bindId > 0)
                map.BindModel(obj);

            map.AddStatic(obj);

            return obj;
        }

        /// <summary>
        /// 移除标注
        /// </summary>
        /// <param name="label">要移除的标注</param>
        public void Remove(MapBuilding label)
        {
            if (buildings.Contains(label))
            {
                // 从地图静态对象中移除
                map.Remove(label);

                // 从集合中移除
                buildings.Remove(label);

                // 销毁标注对象
                label.Destroy();
            }
        }

        /// <summary>
        /// 移除所有标注
        /// </summary>
        public void ClearLabels()
        {
            foreach (var label in buildings)
            {
                map.Remove(label);
                label.Destroy();
            }
            buildings.Clear();
        }

        /// <summary>
        /// 根据位置查找标注
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="distance">搜索距离</param>
        /// <returns>找到的标注，未找到返回null</returns>
        public MapBuilding FindLabelAtPosition(Vector3 position, float distance = 10f)
        {
            foreach (var label in buildings)
            {
                if (Vector3.Distance(label.position, position) <= distance)
                {
                    return label;
                }
            }
            return null;
        }

        public MapBuilding FindMapBuilding(int objId)
        {
            foreach (var label in buildings)
            {
                if (label.objId == objId)
                {
                    return label;
                }
            }
            return null;
        }

        public MapBuilding FindMapBuildingByBindId(int objId)
        {
            foreach (var label in buildings)
            {
                if (label.bindId == objId)
                {
                    return label;
                }
            }
            return null;
        }

        /// <summary>
        /// 保存标注数据
        /// </summary>
        /// <param name="writer">二进制写入器</param>
        internal override void OnSave(BinaryWriter writer)
        {
            // 写入标注数量
            writer.Write(buildings.Count);

            // 写入每个标注的数据
            foreach (var obj in buildings)
            {
                writer.Write(obj.objId);
                writer.Write(obj.objType);
                writer.Write(obj.bindId);
                writer.Write(obj.modelId);
                writer.Write(obj.position.x);
                writer.Write(obj.position.y);
                writer.Write(obj.position.z);
                writer.Write(obj.rotation.x);
                writer.Write(obj.rotation.y);
                writer.Write(obj.rotation.z);
                writer.Write(obj.scale.x);
                writer.Write(obj.scale.y);
                writer.Write(obj.scale.z);
                writer.Write(obj.neighbors.Count);
                for (int i = 0; i < obj.neighbors.Count; i++)
                    writer.Write(obj.neighbors[i]);
            }
        }

        /// <summary>
        /// 加载标注数据
        /// </summary>
        /// <param name="versionCode">版本号</param>
        /// <param name="reader">二进制读取器</param>
        internal override void OnLoad(int versionCode, BinaryReader reader)
        {
            // 清除现有标注
            ClearLabels();

            // 只有版本号>=10时才加载标注数据
            if (versionCode >= 10)
            {
                // 读取标注数量
                int count = reader.ReadInt32();

                // 读取每个标注的数据
                for (int i = 0; i < count; i++)
                {
                    int objId = reader.ReadInt32();
                    int objType = reader.ReadInt32();
                    int bindId = 0;
                    if (versionCode >= 4)
                        bindId = reader.ReadInt32();
                    int modelId = reader.ReadInt32();
                    Vector3 pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Vector3 rot = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Vector3 scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    MapBuilding mapBuilding = Create(objId, objType, bindId, modelId, pos, rot, scale);
                    int neighborCount = reader.ReadInt32();
                    List<int> neighbor = new List<int>();
                    for (int j = 0; j < neighborCount; j++)
                    {
                        neighbor.Add(reader.ReadInt32());
                    }
                    if (mapBuilding != null)
                        mapBuilding.neighbors = neighbor;
                }
            }
        }

        /// <summary>
        /// 清除资源
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            ClearLabels();
        }

        /// <summary>
        /// 更新渲染
        /// </summary>
        public override void UpdateRender()
        {
            // 可以在这里添加渲染更新逻辑
        }
    }
}