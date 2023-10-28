using System.Windows.Shapes;
using System.Windows.Controls;

namespace TestAppSirius
{
    public class ShapeTextBlock : TextBlock
    {
        public Rectangle Rectangle { get; set; }

        public ShapeTextBlock(Rectangle rectangle) : base()
        {
            Rectangle = rectangle;
        }

    }
}
