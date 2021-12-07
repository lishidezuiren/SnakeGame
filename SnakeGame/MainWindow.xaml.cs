using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random random = new Random();

        // 分数
        private static int eatCount = 0;
        // 像素大小
        private readonly int SquareSize = 20;
        // 绿色像素颜色刷
        private SolidColorBrush greenBrush = Brushes.Green;
        // 白色像素颜色刷
        private SolidColorBrush whiteBrush = Brushes.Green;
        // 当前像素颜色标志
        private bool isGreen = true;
        // 初始化运动方向
        private MoveDirection moveDirection = MoveDirection.Right;
        // 初始化蛇
        private List<Snake> Snakes = new List<Snake>();
        // 初始化食物
        private Button food;
        // 吃到食物
        private bool IsEat = false;
        // 运动点
        private int NextX = 0;
        private int NextY = 0;
        // 定时器 让蛇自动移动
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        /// <summary>
        /// 定义蛇
        /// </summary>
        public class Snake
        {
            public int PointX { get; set; }
            public int PointY { get; set; }

            public UIElement UIElement { get; set; }
        }
        /// <summary>
        /// 方向枚举
        /// </summary>
        public enum MoveDirection { Up, Down, Left, Right }
        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
            dispatcherTimer.Tick += DispatherTimer_Fun;

        }
        /// <summary>
        /// 游戏初始化时绘制地图 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            int X = 0;
            int Y = 0;
            while (true)
            {
                Rectangle rectangle = new Rectangle()
                {
                    Width = SquareSize,
                    Height = SquareSize,
                    Fill = isGreen ? greenBrush : whiteBrush
                };
                GameArea.Children.Add(rectangle);
                isGreen = !isGreen;
                Canvas.SetLeft(rectangle, X);
                Canvas.SetTop(rectangle, Y);
                X += 20;
                if (X >= GameArea.ActualWidth)
                {
                    isGreen = !isGreen;
                    X = 0;
                    Y += 20;
                }
                if (Y >= GameArea.ActualHeight)
                {
                    break;
                }
            }
            //RandomFood();
            //DrawSnake();
            //dispatcherTimer.IsEnabled = true;
            StartNewGame();
        }
        public void StartNewGame()
        {
            GameArea.Children.Remove(food);
            foreach (var snake in Snakes)
            {
                GameArea.Children.Remove(snake.UIElement);
            }
            Snakes.Clear();
            NextX = NextY = 0;
            moveDirection = MoveDirection.Right;
            RandomFood();
            DrawSnake();
            dispatcherTimer.IsEnabled = true;

        }

        /// <summary>
        /// 随机画出一个食物
        /// </summary>
        public void RandomFood()
        {
            int X = random.Next(0, (int)(GameArea.ActualWidth / SquareSize)) * SquareSize;
            int Y = random.Next(0, (int)(GameArea.ActualWidth / SquareSize)) * SquareSize;
            food = new Button()
            {
                Width = SquareSize,
                Height = SquareSize,
                Background = Brushes.DarkOrchid,
                Content="马"
            };
            GameArea.Children.Add(food);
            Canvas.SetTop(food, Y);
            Canvas.SetLeft(food, X);
        }
        /// <summary>
        /// 画蛇
        /// </summary>
        public void DrawSnake()
        {

            if (Snakes.Count > 0)
            {
                if (!IsEat)
                {
                    GameArea.Children.Remove(Snakes[0].UIElement);
                    Snakes.RemoveAt(0);

                }
                else
                {
                    IsEat = false;
                }
            }

            Rectangle ellipse = new Rectangle();
            ellipse.Width = SquareSize;
            ellipse.Height = SquareSize;
            ellipse.Fill = Brushes.Red;

            Snake snake = new Snake();
            snake.PointX = NextX;
            snake.PointY = NextY;
            snake.UIElement = ellipse;

            GameArea.Children.Add(snake.UIElement);
            Canvas.SetLeft(snake.UIElement, snake.PointX);
            Canvas.SetTop(snake.UIElement, snake.PointY);

            Snakes.Add(snake);

        }
        /// <summary>
        /// 
        /// 朝指定方向移动
        /// </summary>
        public void MoveSnake()
        {
            switch (moveDirection)
            {
                case MoveDirection.Up:
                    NextY -= 20;
                    break;
                case MoveDirection.Down:
                    NextY += 20;
                    break;
                case MoveDirection.Left:
                    NextX -= 20;
                    break;
                case MoveDirection.Right:
                    NextX += 20;
                    break;
                default:
                    break;
            }
            IsEateFood();
            // 重绘蛇
            DrawSnake();

        }

        // 判断是否迟到食物
        public void IsEateFood()
        {
            if (NextX == Canvas.GetLeft(food) && NextY == Canvas.GetTop(food))
            {
                GameArea.Children.Remove(food);
                RandomFood();
                IsEat = true;
                eatCount++;
                updateState();
            }
            if (NextX >= GameArea.ActualWidth || NextX < 0 || NextY < 0 || NextY >= GameArea.ActualHeight)
            {
                MessageBox.Show("Game Over");
                dispatcherTimer.IsEnabled = false;
                StartNewGame();
            }
        }

        /// <summary>
        /// 更新分数
        /// </summary>
        private void updateState()
        {
            this.Title = $"you have eat {eatCount} foods";
        }

        /// <summary>
        /// 根据键盘的输入调整移动方向
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            string direction = e.Key.ToString();
            switch (direction)
            {
                case "Up":
                    moveDirection = MoveDirection.Up;
                    break;
                case "Down":
                    moveDirection = MoveDirection.Down;
                    break;
                case "Left":
                    moveDirection = MoveDirection.Left;
                    break;
                case "Right":
                    moveDirection = MoveDirection.Right;
                    break;
                default:
                    break;
            }
            MoveSnake();
        }
        public void DispatherTimer_Fun(object sender, EventArgs args)
        {
            MoveSnake();
        }
    }
}
