using GDBA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInventory
{
    public class MasterInventory<T> where T : GameData
    {
        public List<T> inventory = new List<T>();
        public T this[int index]
        {
            get { return inventory[index]; }
            set { inventory[index] = value; }
        }
        //인벤토리내에 아이템개수 확인
        public int Count
        {
            get { return inventory.Count; }
        }

        //아이템 추가
        public virtual void Add(T item)
        {
            inventory.Add(item);
        }

        //아이템 제거
        public void Remove(T item)
        {
            inventory.Remove(item);
        }

        //인벤토리 초기화
        public void Clear()
        {
            inventory.Clear();
        }
    }
}
