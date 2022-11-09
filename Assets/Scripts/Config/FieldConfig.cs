using System;
using System.Collections.Generic;
using UnityEngine;


namespace Config
{
    [CreateAssetMenu(fileName = "Field config", menuName = "Field config", order = 0)]
    public class FieldConfig : ScriptableObject
    {
        [SerializeField] private int _fieldWidth;
        [SerializeField] private int _fieldHeight;
        [SerializeField] private float _bombDelay;

        [SerializeField] private List<WalkerTypeData> _walkerTypes;
        [SerializeField] private List<WalkerSpawnPointData> _walkerSpawns;
        [SerializeField] private List<WallData> _walls;

        public int FieldWidth => _fieldWidth;
        public int FieldHeight => _fieldHeight;
        public float BombDelay => _bombDelay;
        public IReadOnlyList<WalkerTypeData> WalkerTypes => _walkerTypes;
        public IReadOnlyList<WalkerSpawnPointData> WalkerSpawnPoints => _walkerSpawns;
        public IReadOnlyList<WallData> Walls => _walls;
    }

    [Serializable]
    public class WallData
    {
        [SerializeField] private E_CellType _type;
        [SerializeField] private int _index;

        public E_CellType Type => _type;
        public int Index => _index;
    }

    [Serializable]
    public class WalkerSpawnPointData
    {
        [SerializeField] private E_WalkerType _type;
        [SerializeField] private int _startIndex;

        public E_WalkerType Type => _type;
        public int StartIndex => _startIndex;
    }

    [Serializable]
    public class WalkerTypeData
    {
        [SerializeField] private E_WalkerType _type;
        [SerializeField] private float _speed;

        public E_WalkerType Type => _type;
        public float Speed => _speed;
    }
}
