using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDBA
{
    public class GameDataTable<T> : ScriptableObject where T : GameData
    {
        // 데이터의 크기가 정해져 있고, 추가적인 삽입 삭제가 일어나지 않으며 검색을 필요로 할 때 유리
        public T[] table = null;

        public int Count { get { return table.Length; } }

    }
}
