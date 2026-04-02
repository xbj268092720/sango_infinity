using System;
using System.Collections.Generic;

namespace Sango.Core
{
    public class ItemStore : IAarryDataObject
    {
        public Dictionary<int, int> Items = new Dictionary<int, int>();
        public int TotalNumber { get; private set; }

        public IAarryDataObject FromArray(int[] values)
        {
            TotalNumber = 0;
            Items.Clear();
            if (values == null || values.Length == 0) return this;
            for (int i = 0; i < values.Length; i += 2)
            {
                int itemTypeId = values[i];
                int number = values[i + 1];
                TotalNumber += number;
                ItemType itemType = Scenario.Cur.CommonData.ItemTypes.Get(itemTypeId);
                if (itemType == null) continue;
                Items.Add(itemTypeId, number);
            }
            return this;
        }

        public int[] ToArray()
        {
            List<int> ints = new List<int>();
            foreach (int itemTypeId in Items.Keys)
            {
                int number = Items[itemTypeId];
                if (number > 0)
                {
                    ints.Add(itemTypeId);
                    ints.Add(number);
                }
            }
            return ints.ToArray();
        }

        public ItemStore Copy()
        {
            ItemStore copy = new ItemStore();
            foreach (int itemTypeId in Items.Keys)
            {
                int number = Items[itemTypeId];
                if (number > 0)
                    copy.Items.Add(itemTypeId, number);
            }
            copy.TotalNumber = TotalNumber;
            return copy;
        }

        public int Set(ItemType itemType, int number)
        {
            return Set(itemType.storeKind, number);
        }

        public int Set(int itemTypeId, int number)
        {
            if (Items.TryGetValue(itemTypeId, out int has))
            {
                TotalNumber -= has;
                Items[itemTypeId] = number;
                TotalNumber += number;
                return number;
            }

            TotalNumber += number;
            Items.Add(itemTypeId, number);
            return number;
        }

        public int Add(ItemType itemType, int number)
        {
            return Add(itemType.storeKind, number);
        }

        public int Add(int itemTypeId, int number)
        {
            if (Items.TryGetValue(itemTypeId, out int has))
            {
                has += number;
                Items[itemTypeId] = has;
                TotalNumber += number;
                return has;
            }

            TotalNumber += number;
            Items.Add(itemTypeId, number);
            return number;
        }


        public void Add(ItemStore itemStore)
        {
            if (itemStore == null) return;
            foreach (int itemTypeId in itemStore.Items.Keys)
            {
                int number = itemStore.Items[itemTypeId];
                if (number > 0)
                    Add(itemTypeId, number);
            }
        }

        public int Remove(ItemType itemType)
        {
            return Remove(itemType.storeKind);
        }

        public int Remove(int itemTypeId)
        {
            if (Items.TryGetValue(itemTypeId, out int has))
            {
                Items.Remove(itemTypeId);
                TotalNumber -= has;
                return has;
            }
            return 0;
        }

        public int Remove(ItemType itemType, int number)
        {
            return Remove(itemType.storeKind, number);
        }

        public int Remove(int itemTypeId, int number)
        {
            if (Items.TryGetValue(itemTypeId, out int has))
            {
                number = Math.Min(has, number);
                has -= number;
                Items[itemTypeId] = has;
                TotalNumber -= number;
                return has;
            }
            return 0;
        }

        public int Remove(ItemStore itemStore)
        {
            if (itemStore == null) return TotalNumber;

            foreach (int itemTypeId in itemStore.Items.Keys)
            {
                int number = itemStore.Items[itemTypeId];
                if (number > 0)
                    Remove(itemTypeId, number);
            }
            return TotalNumber;
        }

        public int GetNumber(int itemTypeId)
        {
            if (Items.TryGetValue(itemTypeId, out int has))
                return has;
            return 0;
        }

        public int GetNumber(int [] itemTypeId)
        {
            int total = 0;
            for(int i = 0; i < itemTypeId.Length; i++)
                total += GetNumber(itemTypeId[i]);
            return total;
        }

        public int GetNumber(ItemType itemType)
        {
            if (Items.TryGetValue(itemType.storeKind, out int has))
                return has;
            return 0;
        }

        public int this[ItemType itemType]
        {
            get { return GetNumber(itemType.storeKind); }
            set { Add(itemType.storeKind, value); }
        }

        public int this[int itemTypeId]
        {
            get { return GetNumber(itemTypeId); }
            set { Add(itemTypeId, value); }
        }

        public bool CheckItemEnough(int[] cost, int number)
        {
            if (cost == null || cost.Length == 0) return true;
            for (int i = 0; i < cost.Length; i += 2)
            {
                int itemTypeId = cost[i];
                int costN = cost[i + 1];
                int have = GetNumber(itemTypeId);
                int need = costN * number;
                if (need % 1000 == 0)
                    need = need / 1000;
                else
                    need = need / 1000 + 1;

                if (have < need)
                    return false;
            }
            return true;
        }

        public void Cost(int[] cost, int number)
        {
            if (cost == null || cost.Length == 0) return;
            for (int i = 0; i < cost.Length; i += 2)
            {
                int itemTypeId = cost[i];
                int costN = cost[i + 1] * number / 1000;
                Remove(itemTypeId, costN);
            }
        }

        public void Gain(int[] cost, int number)
        {
            if (cost == null || cost.Length == 0) return;
            for (int i = 0; i < cost.Length; i += 2)
            {
                int itemTypeId = cost[i];
                int costN = cost[i + 1] * number / 1000;
                Add(itemTypeId, costN);
            }
        }

        public int CheckCostMin(int[] cost, int number)
        {
            if (cost == null || cost.Length == 0) return number;
            for (int i = 0; i < cost.Length; i += 2)
            {
                int itemTypeId = cost[i];
                int costN = cost[i + 1];
                int have = GetNumber(itemTypeId) * 1000 / costN;
                number = Math.Min(number, have);
            }
            return number;
        }

        /// <summary>
        /// 分割一部分, part: 1到100的整数
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public ItemStore Split(int part, bool keep = false)
        {
            ItemStore itemStore = new ItemStore();
            if (part <= 0) return itemStore;
            if (part > 100) part = 100;
            foreach (int itemTypeId in Items.Keys)
            {
                int number = Items[itemTypeId];
                if (number > 0)
                {
                    int partNum = number * part / 100;
                    itemStore.Add(itemTypeId, partNum);
                }
            }

            if (!keep)
            {
                foreach (int itemTypeId in itemStore.Items.Keys)
                {
                    int number = Items[itemTypeId];
                    if (number > 0)
                    {
                        Items[itemTypeId] = Items[itemTypeId] - number;
                        TotalNumber -= number;
                    }
                }
            }
            return itemStore;
        }

        public void Clear()
        {
            Items.Clear();
            TotalNumber = 0;
        }

    }
}
