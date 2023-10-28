using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestAppSirius
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private Rectangle? CurrentRectangle = null;
        private List<Shape> Shapes = new List<Shape>();
        private List<Arrow> Arrows = new List<Arrow>();

        public MainWindow()
        {
            InitializeComponent();
        }        

        /// <summary>
        /// Обработчик кнопки добавления объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddShape_Click(object sender, RoutedEventArgs e)
        {
            Shape shape = new Shape(MainCanvas);
            DrawArrow(shape);
            Shapes.Add(shape);
        }

        /// <summary>
        /// Обработчик кнопки удаления объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Shape? deletedShape = Shapes.FirstOrDefault(s => s.Rectangle == CurrentRectangle);
            MainCanvas.Children.Remove(deletedShape?.TextBlock);
            MainCanvas.Children.Remove(CurrentRectangle);
            Shapes.Remove(deletedShape!);

            Arrow? previousArrow = Arrows.FirstOrDefault(a => a.NextNumber == deletedShape?.Number);
            previousArrow?.RemoveArrow();
            Arrows.Remove(previousArrow!);
            Arrow? nextArrow = Arrows.FirstOrDefault(a => a.PreviousNumber == deletedShape?.Number);
            nextArrow?.RemoveArrow();
            Arrows.Remove(nextArrow!);

            if (previousArrow is not null && nextArrow is not null)
            {
                CurrentRectangle = (Shapes.Where(s => s.Number < deletedShape?.Number).MaxBy(s => s.Number))?.Rectangle;
                Shape nextShape = Shapes.Where(s => s.Number > deletedShape?.Number).MinBy(s => s.Number)!;
                DrawArrow(nextShape);
            }
        }

        /// <summary>
        /// Обработчик нажатия на фигуру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle? rectangle = null;
            if (e.OriginalSource is Rectangle)
            {
                rectangle = e.OriginalSource as Rectangle;
            }
            else if (e.OriginalSource is ShapeTextBlock)
            {
                rectangle = (e.OriginalSource as ShapeTextBlock)?.Rectangle;
            }

            if (rectangle is not null)
            {
                ChangeCurrentShape(rectangle);
            }
            else
            {
                ResetCurrentShape(true);
            }
        }

        /// <summary>
        /// Выделение новой фигуры
        /// </summary>
        /// <param name="rectangle">Новая выделенная фигура</param>
        private void ChangeCurrentShape(Rectangle? rectangle)
        {
            ResetCurrentShape(false);

            if (rectangle is not null)
            {
                CurrentRectangle = rectangle;
                rectangle.Stroke = Brushes.Blue;
                rectangle.StrokeThickness = 5;
                btnDelete.IsEnabled = true;
            }
        }

        /// <summary>
        /// Сброс выделения фигуры
        /// </summary>
        /// <param name="nonCurrentShape">Нет новой фигуры для выделения</param>
        private void ResetCurrentShape(bool nonCurrentShape)
        {
            if (CurrentRectangle is not null)
            {
                CurrentRectangle.StrokeThickness = 3;
                byte[] color = new byte[] { (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255) };
                CurrentRectangle.Stroke = new SolidColorBrush(Color.FromRgb(color[0], color[1], color[2]));
            }

            if (nonCurrentShape)
            {
                CurrentRectangle = null;
                btnDelete.IsEnabled = false;
            }
        }

        /// <summary>
        /// Отрисовка стрелки между двумя фигурами
        /// </summary>
        /// <param name="newRectangle">Новая фигура</param>
        private void DrawArrow(Shape newShape)
        {
            Rectangle newRectangle = newShape.Rectangle;
            Shape? previousShape = Shapes?.Where(s => s.Number < newShape.Number).MaxBy(s => s.Number);

            if (previousShape is not null)
            {
                Rectangle previousRectangle = previousShape.Rectangle;

                double newX = Canvas.GetLeft(newRectangle) + newRectangle.Width / 2;
                double newY = Canvas.GetTop(newRectangle) + newRectangle.Height / 2;
                double previousX = Canvas.GetLeft(previousRectangle) + previousRectangle.Width / 2;
                double previousY = Canvas.GetTop(previousRectangle) + previousRectangle.Height / 2;

                bool flag1 = newY - previousY >= 0 ? true : false;
                bool flag2 = newX - previousX >= 0 ? true : false;
                bool flag3 = Math.Abs((newX - previousX) / (newY - previousY)) < Shape.SHAPE_WIDTH / Shape.SHAPE_HEIGHT;
                
                Line line = new Line()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                if (flag1 && flag3)
                {
                    line.X1 = previousX;
                    line.Y1 = Canvas.GetTop(previousRectangle) + previousRectangle.Height;
                    line.X2 = newX;
                    line.Y2 = Canvas.GetTop(newRectangle);
                }
                else if (!flag1 && flag3)
                {
                    line.X1 = previousX;
                    line.Y1 = Canvas.GetTop(previousRectangle);
                    line.X2 = newX;
                    line.Y2 = Canvas.GetTop(newRectangle) + newRectangle.Height;
                }
                else if (flag2 && !flag3)
                {
                    line.X1 = Canvas.GetLeft(previousRectangle) + previousRectangle.Width;
                    line.Y1 = previousY;
                    line.X2 = Canvas.GetLeft(newRectangle);
                    line.Y2 = newY;
                }
                else if (!flag2 && !flag3)
                {
                    line.X1 = Canvas.GetLeft(previousRectangle);
                    line.Y1 = previousY;
                    line.X2 = Canvas.GetLeft(newRectangle) + newRectangle.Width;
                    line.Y2 = newY;
                }

                Arrow arrow = new Arrow(MainCanvas)
                {
                    PreviousNumber = previousShape.Number,
                    NextNumber = newShape.Number
                };
                arrow.Lines.Add(line);
                Arrows.Add(arrow);
                MainCanvas.Children.Add(line);

                DrawArrowhead(line, 15, arrow);
                DrawArrowhead(line, -15, arrow);                
            }
        }

        /// <summary>
        /// Отрисовка наконечника стрелки
        /// </summary>
        /// <param name="line">Линия, для которой рисуется стрелка</param>
        /// <param name="angle">Угол острия стрелки</param>
        private void DrawArrowhead(Line line, int angle, Arrow arrow)
        {
            double lineLength = Math.Sqrt((line.X1 - line.X2) * (line.X1 - line.X2) + (line.Y1 - line.Y2) * (line.Y1 - line.Y2));
            double dX = Math.Abs((line.X1 - line.X2) / lineLength);
            double dY = Math.Abs((line.Y1 - line.Y2) / lineLength);

            Line arrowline = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                X1 = line.X2,
                Y1 = line.Y2,
                X2 = line.X2 > line.X1 ? line.X2 - 10 * dX : line.X2 + 10 * dX,
                Y2 = line.Y2 > line.Y1 ? line.Y2 - 10 * dY : line.Y2 + 10 * dY
            };
            RotateTransform rotate1 = new RotateTransform(angle)
            {
                CenterX = line.X2,
                CenterY = line.Y2
            };
            arrowline.RenderTransform = rotate1;
            arrow.Lines.Add(arrowline);
            MainCanvas.Children.Add(arrowline);
        }
    }
}
