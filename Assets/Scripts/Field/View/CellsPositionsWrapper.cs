using UnityEngine;
namespace Field.View
{
    public class CellsPositionsWrapper : MonoBehaviour
    {
        [SerializeField] private CellsFieldView _cellsView;

        public Vector3 GetPositionByIndex(int index)
        {
            return _cellsView.GetPositionByIndex(index);
        }
    }
}
