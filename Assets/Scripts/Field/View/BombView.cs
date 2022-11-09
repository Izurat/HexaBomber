using Field.Model.Bomb;
using UnityEngine;

namespace Field.View
{
    public class BombView : MonoBehaviour
    {
        public int Index => _bomb.Index;
        public UnicalID Id => _bomb.Id;

        private IBombModel _bomb;

        public void Init(IBombModel bombModel)
        {
            _bomb = bombModel;
        }
    }
}
