using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WordCode
{
    public class GameManager : SerializedMonoBehaviour
    {
        [SerializeField] private TextAsset _json;
        [SerializeField] private Dictionary<int, LevelData> _levelData = new Dictionary<int, LevelData>();

        private void Start()
        {
            Convert();
        }

        public void Convert()
        {
            LevelData[] rawLevels = JsonConvert.DeserializeObject<LevelData[]>(_json.text);
            for (int i = 0; i < 100; i++)
            {
                _levelData.Add(i + 1, rawLevels[i]);
            }
        }
    }
}