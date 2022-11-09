using System;
namespace Field.Model.Cell
{
    public interface ICellModel
    {
        public E_CellType type { get; }

        public int index { get; }

        event Action<E_CellType> OnTypeChanged;
    }

    public class CellModel : ICellModel
    {
        public E_CellType type { get; private set; }
        public int index { get; private set; }

        public event Action<E_CellType> OnTypeChanged;

        public CellModel(E_CellType cellType, int cellIndex)
        {
            type = cellType;
            index = cellIndex;
        }

        public void SetType(E_CellType cellType)
        {
            if (type == cellType)
                return;

            type = cellType;
            OnTypeChanged?.Invoke(type);
        }
    }
}
