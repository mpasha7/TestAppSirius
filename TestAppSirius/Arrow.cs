using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace TestAppSirius
{
    public class Arrow
    {
        public Canvas MainCanvas { get; set; }
        public List<Line> Lines { get; set; }
        public int PreviousNumber { get; set; }
        public int NextNumber { get; set; }

        public Arrow(Canvas canvas)
        {
            MainCanvas = canvas;
            Lines = new List<Line>();
        }

        /// <summary>
        /// Удаление стрелки с холста
        /// </summary>
        public void RemoveArrow()
        {
            foreach (Line line in Lines)
            {
                MainCanvas.Children.Remove(line);
            }
        }

    }
}
