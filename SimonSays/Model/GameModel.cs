using System.Collections.Generic;
using System;

namespace SimonSays.Model
{
    public class GameModel
    {
        private Random _random = new Random();

        public List<string> Sequence { get; private set; } = new List<string>();

        public void ResetGame()
        {
            Sequence.Clear();
        }

        public void AddRandomStep()
        {
            var colors = new[] { "Red", "Blue", "Green", "Yellow", "Purple", "Fuchsia", "DarkKhaki", "Turquoise", "Coral" };
            Sequence.Add(colors[_random.Next(colors.Length)]);
        }

        public bool CheckStep(int index, string color)
        {
            return Sequence[index] == color;
        }
    }
}
