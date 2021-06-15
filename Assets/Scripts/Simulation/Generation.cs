using System;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Simulation
{
    [Serializable]
    public class Generation
    {
        public int width;
        public int height;

        public Cell[,] Cells;

        public Generation(int width, int height, bool randomize)
        {
            this.width = width;
            this.height = height;

            Cells = new Cell[width, height];
            if (randomize)
            {
                Randomize();
            }
        }

        void Randomize()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SetAliveValueAt(x, y, Random.value >= 0.5);
                }
            }
        }

        public int CalculateLivingNeighbours(int _x, int _y)
        {
            var xLowerBound = _x - 1;
            var xUpperBound = _x + 1;

            var yLowerBound = _y - 1;
            var yUpperBound = _y + 1;

            int count = 0;
            for (var x = xLowerBound; x < xUpperBound + 1; x++)
            {
                for (var y = yLowerBound; y < yUpperBound + 1; y++)
                {
                    if (
                        x < 0 ||
                        y < 0 ||
                        x > width - 1 ||
                        y > height - 1 ||
                        (x == _x && y == _y)
                    )
                    {
                        continue;
                    }

                    count += GetAliveValueAt(x, y) ? 1 : 0;
                }
            }

            return count;
        }

        public bool GetAliveValueAt(int x, int y)
        {
            return Cells[x, y].IsAlive;
        }

        public void SetAliveValueAt(int x, int y, bool state)
        {
            Cells[x, y].IsAlive = state;
        }

        public void ToggleAliveValueAt(int x, int y)
        {
            Cells[x, y].IsAlive = !GetAliveValueAt(x, y);
        }

        private bool GetNextAliveState(bool aliveState, int neighbourCount)
        {
            bool state = false;

            // 1. Rule: Rebirth
            if (!aliveState && neighbourCount == 3) state = true;
            // 2. Rule: Loneliness
            // if (aliveState && neighbourCount < 2) state = false;
            // 3. Rule: Survive
            if (aliveState && (neighbourCount == 2 || neighbourCount == 3)) state = true;
            // 4. Rule: Overpopulation
            // if (aliveState && neighbourCount > 3) state = false;

            return state;
        }

        public Generation GetNextGeneration()
        {
            Generation b = new Generation(width, height, false);

            Parallel.For(0, width, (x) =>
                {
                    for (var y = 0; y < height; y++)
                    {
                        b.SetAliveValueAt(
                            x, y,
                            GetNextAliveState(
                                GetAliveValueAt(x, y),
                                CalculateLivingNeighbours(x, y)
                            )
                        );
                    }
                }
            );

            return b;
        }
    }
}