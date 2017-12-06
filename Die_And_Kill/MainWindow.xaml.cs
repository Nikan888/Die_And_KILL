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
            throw new NotImplementedException();
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

        }
    }
}
