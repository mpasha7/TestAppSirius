using System;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;


namespace TestAppSirius
{
    public class Shape
    {
        private static int counter = 1;
        public const int SHAPE_WIDTH = 150;
        public const int SHAPE_HEIGHT = 60;
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private int left;
        private int top;

        public Canvas MainCanvas { get; set; }
        public Rectangle Rectangle { get; set; }
        public ShapeTextBlock TextBlock { get; set; }
        public int Number { get; set; }

        public Shape(Canvas canvas)
        {
            MainCanvas = canvas;
            left = rnd.Next((int)(MainCanvas.Width - SHAPE_WIDTH));
            top = rnd.Next((int)(MainCanvas.Height - SHAPE_HEIGHT));
            DeawRectangle();
            DrawTextBlock();
        }
        
        /// <summary>
        /// Добавление на холст прямоугольника фигуры
        /// </summary>
        public void DeawRectangle()
        {
            byte[] color = new byte[] { (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255) };
            Rectangle rectangle = new Rectangle()
            {
                Width = SHAPE_WIDTH,
                Height = SHAPE_HEIGHT,
                Fill = Brushes.White,
                StrokeThickness = 3,
                Stroke = new SolidColorBrush(Color.FromRgb(color[0], color[1], color[2]))
            };
            Rectangle = rectangle;
            Canvas.SetLeft(rectangle, left);
            Canvas.SetTop(rectangle, top);
            MainCanvas.Children.Add(rectangle);
        }

        /// <summary>
        /// Добавление на холст текста фигуры
        /// </summary>
        private void DrawTextBlock()
        {
            ShapeTextBlock textBlock = new ShapeTextBlock(Rectangle)
            {
                Text = $"Shape {counter}",
                Width = SHAPE_WIDTH / 1.5,
                Height = SHAPE_HEIGHT / 3,
                TextAlignment = TextAlignment.Center,
                FontFamily = new FontFamily("Times New Roman"),
                FontSize = SHAPE_HEIGHT / 4
            };
            Number = counter;
            TextBlock = textBlock;
            counter++;
            Canvas.SetLeft(textBlock, left + SHAPE_WIDTH / 6);
            Canvas.SetTop(textBlock, top + SHAPE_HEIGHT / 3);
            MainCanvas.Children.Add(textBlock);
        }
    }
}
