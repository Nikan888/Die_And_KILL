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
using WpfAnimatedGif;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using Logic;

namespace Die_And_Kill
{
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
        }

        public static Queue<UserControlWorms> RemoveFromQueue(UserControlWorms item, Queue<UserControlWorms> queue)
        {

            var list = queue.ToList();
            list.Remove(item);
            var finalQueue = new Queue<UserControlWorms>();
            foreach (UserControlWorms i in list)
            {
                finalQueue.Enqueue(i);
            }
            return finalQueue;

        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Core core = new Core();
        public Random randy = new Random();
        public List<UserControlWorms> worms = new List<UserControlWorms>();
        public List<Tuple<Image, Wrap>> wraper = new List<Tuple<Image, Wrap>>();
        public Queue<UserControlWorms> turns = new Queue<UserControlWorms>();
        public Image[,] blocks;
        public int gravity = 300000;
        public double transitionSpeed = -8000;
        DispatcherTimer BazookaControl;
        DispatcherTimer BatControl;
        DispatcherTimer TriggerUpdater;

        public MainWindow()
        {
            InitializeComponent();
            LoadMap();

            TriggerUpdater = new DispatcherTimer();
            TriggerUpdater.Interval = new TimeSpan(0, 0, 0, 0, 10);
            TriggerUpdater.Tick += Updater;
            TriggerUpdater.Start();

            BazookaControl = new DispatcherTimer();
            BazookaControl.Interval = new TimeSpan(0, 0, 0, 0, 1);
            BazookaControl.Tick += UpdaterForBazooka;

            BatControl = new DispatcherTimer();
            BatControl.Interval = new TimeSpan(0, 0, 0, 0, 1);
            BatControl.Tick += UpdaterForBat;

            this.WindowState = WindowState.Maximized;
            this.KeyDown += MainWindow_KeyDown;
            bazookabtn.Click += SelectBazooka;
            bat.Click += SelectBat;

            airstrike.Click += airstrike_Click;

            ChangeOfShift();
        }

        private void airstrike_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SelectBat(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SelectBazooka(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdaterForBat(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdaterForBazooka(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Updater(object sender, EventArgs e)
        {
            #region Control for wrap
            if(wraper.Count != 0)
            {
                List<Tuple<Image, Wrap>> toDelete = new List<Tuple<Image, Wrap>>();
                foreach (Tuple<Image, Wrap> p in wraper)
                {
                    p.Item2.CanvasPosX += p.Item2.SpeedX * 0.001;
                    p.Item2.CanvasPosY += p.Item2.SpeedY * 0.001;
                    p.Item2.SpeedY += gravity * 0.001;
                    Canvas.SetLeft(p.Item1, p.Item2.CanvasPosX);
                    Canvas.SetTop(p.Item1, p.Item2.CanvasPosY);
                    
                    var controller = ImageBehavior.GetAnimationController(p.Item1);
                    var angle = Math.Atan((p.Item2.SpeedY) / (p.Item2.SpeedX)) + (Math.PI / 2);
                    var frame = 0.0;
                    if (p.Item2.SpeedX > 0)
                    {
                        frame = (angle * 16) / (Math.PI);
                    }
                    else
                    {
                        frame = (angle * 16) / (Math.PI) + 16;
                    }
                    controller.GotoFrame((int)frame);


                    var gridx = (int)((p.Item2.CanvasPosX + 15) / 30.0);
                    var gridy = (int)((p.Item2.CanvasPosY + 15) / 30.0);
                    var element = "";
                    try
                    {
                        element = core.map.grid[gridx, gridy];
                    }
                    catch
                    {
                        element = "0";
                    }
                    if (element != "0") 
                    {
                        User(p.Item2.CanvasPosX, p.Item2.CanvasPosY, p.Item2.radio, p.Item2.dano);
                        MyCanvas.Children.Remove(p.Item1);
                        toDelete.Add(p);
                        ChangeOfShift();
                    }

                    if (p.Item2.CanvasPosY > 600)
                    {
                        MyCanvas.Children.Remove(p.Item1);
                        toDelete.Add(p);
                        ChangeOfShift();
                        break;
                    }
                }
                //Deleting from wrapper
                foreach (Tuple<Image, Wrap> p in toDelete)
                {
                    wraper.Remove(p);
                }
            }
            #endregion
        }

        public void LoadMap()
        {
            #region #region LoadFloat
            var converter = new ImageSourceConverter();
            var source = (ImageSource)converter.ConvertFromString(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\Block.jpg");
            blocks = new Image[core.map.weight, core.map.height];
            for (int x = 0; x < core.map.weight; x++)
            {
                for (int y = 0; y < core.map.height; y++)
                {
                    if (core.map.grid[x, y] == "1")
                    {
                        var img = new Image()
                        {
                            Source = source,
                            Height = 30,
                            Width = 30,
                        };

                        MyCanvas.Children.Add(img);
                        Canvas.SetLeft(img, x * 30);
                        Canvas.SetTop(img, y * 30);
                        blocks[x, y] = img;
                    }
                }
            }
            var transform = 42.0 / core.map.weight;
            MyCanvas.LayoutTransform = new ScaleTransform(transform, transform);
            #endregion

            #region Load Worms
            var i = 0;
            foreach (Worm worm in core.worms)
            {
                worms.Add(new UserControlWorms(worm));

                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\9.gif");
                image.EndInit();
                ImageBehavior.SetAnimatedSource(worms[i].icon, image);

                Canvas.SetLeft(worms[i], worm.position.x * 30);
                Canvas.SetTop(worms[i], worm.position.y * 30 - 36);
                worms[i].CanvasXPos = worm.position.x * 30;
                worms[i].CanvasYPos = worm.position.y * 30 - 36;
                MyCanvas.Children.Add(worms[i]);

                i++;
            }
            #endregion

            #region Turn queue
            foreach (UserControlWorms w in worms)
                turns.Enqueue(w);

            #endregion
        }

        public void ChangeOfShift()
        {
            //Return the appearance to the worm that become inactive 
            var worm = turns.Dequeue();
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\9.gif");
            image.EndInit();
            ImageBehavior.SetAnimatedSource(worm.icon, image);
            turns.Enqueue(worm);

            //Make active stati of worm
            worm = turns.Peek();
            image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\19.gif");
            image.EndInit();
            ImageBehavior.SetAnimatedSource(worm.icon, image);
        }

        private void User(double xpos, double ypos, double radio, double dano)
        {

        }

        private void MoveLeft(UserControlWorms worm)
        {

        }

        private void MoveRight(UserControlWorms worm)
        {

        }
    }
}
