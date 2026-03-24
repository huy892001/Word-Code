using UnityEngine;

namespace WordCode
{
    public class LevelController : MonoBehaviour
    {
        private LevelData _levelData;

        public void Init(LevelData levelData)
        {
            _levelData = levelData;
        }
    }
}