using System;
namespace Field.Model.Bomb
{
    public interface IBombModel
    {
        int Index { get; }
        float TimeLeft { get; }
        UnicalID Id { get; }
    }

    public class BombModel : IBombModel, ITimeUpdateableModel
    {

        public float TimeLeft { get; private set; }

        public int Index { get; private set; }

        public UnicalID Id { get; private set; }

        public BombModel(int index, float time)
        {
            Id = UnicalID.Create();
            TimeLeft = time;
            Index = index;
        }

        public void Update(float deltaTime)
        {
            TimeLeft -= deltaTime;

            if (TimeLeft <= 0)
                TimeLeft = 0;
        }
    }
}
