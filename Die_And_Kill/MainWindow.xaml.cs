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
            Wrap p;
            for (int i = 0; i < 350; i = i + 100)
            {
                p = new Wrap(-100, 0, turns.Peek().CanvasXPos + i, 10, 40, 40);
                Image img = new Image();
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\bazooka.gif");
                image.EndInit();
                ImageBehavior.SetAnimatedSource(img, image);
                Canvas.SetTop(img, p.CanvasPosY);
                Canvas.SetLeft(img, p.CanvasPosX);
                MyCanvas.Children.Add(img);

                wraper.Add(new Tuple<Image, Wrap>(img, p));
            }
        }

        private void SelectBat(object sender, RoutedEventArgs e)
        {
            potencia.Visibility = System.Windows.Visibility.Visible;
            potencia2.Visibility = System.Windows.Visibility.Visible;
            var worm = turns.Peek();
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\bat.gif");
            image.EndInit();
            ImageBehavior.SetAnimatedSource(worm.icon, image);
            BatControl.Start();
        }

        private void SelectBazooka(object sender, RoutedEventArgs e)
        {
            if (wraper.Count == 0)
            {
                potencia.Visibility = System.Windows.Visibility.Visible;
                potencia2.Visibility = System.Windows.Visibility.Visible;

                var worm = turns.Peek();
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\16.gif");
                image.EndInit();
                ImageBehavior.SetAnimatedSource(worm.icon, image);
                BazookaControl.Start();
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (!turns.Peek().onAir)
                    turns.Peek().yspeed = transitionSpeed;
            }
            if (e.Key == Key.Down)
                    turns.Enqueue(turns.Dequeue());
            if (e.Key == Key.Left)
                MoveLeft(turns.Peek());
            if (e.Key == Key.Right)
                MoveRight(turns.Peek());

        }

        private void UpdaterForBat(object sender, EventArgs e)
        {
            var mousePosX = Mouse.GetPosition(MyCanvas).X;
            var mousePosY = Mouse.GetPosition(MyCanvas).Y - 80;
            var batPosX = turns.Peek().CanvasXPos + 15;
            var batPosY = turns.Peek().CanvasYPos - 30;

            #region Angulo de ataque
            var controller = ImageBehavior.GetAnimationController(turns.Peek().icon);

            var angle = Math.Atan((mousePosY - batPosY) / (mousePosX - batPosX)) + (Math.PI / 2); // va de 0 a Pi
            var frame = 0.0;
            if (mousePosX < batPosX)
            {
                frame = (angle * 31) / (Math.PI);
                ScaleTransform scale = new ScaleTransform();

                scale.ScaleX = 1;
                scale.CenterX = 45;
                TransformGroup myTransformGroup = new TransformGroup();
                myTransformGroup.Children.Add(scale);
                turns.Peek().icon.RenderTransform = myTransformGroup;
                try
                {
                    controller.GotoFrame((int)frame);
                }
                catch { }

            }
            else
            {
                ScaleTransform scale = new ScaleTransform();
                scale.ScaleX = -1;
                scale.CenterX = 45;
                TransformGroup myTransformGroup = new TransformGroup();
                myTransformGroup.Children.Add(scale);
                turns.Peek().icon.RenderTransform = myTransformGroup;
                frame = -1 * (((angle * 31) / (Math.PI)) - 14) + 17;
                controller.GotoFrame((int)frame);
            }
            #endregion
            #region Barra de Potencia


            var speedx = mousePosX - batPosX;
            var speedy = mousePosY - batPosY;
            var speed = Math.Sqrt(Math.Pow(speedx, 2) + Math.Pow(speedy, 2));
            var barraPotencia = (160.0 / speed) * 950;
            //lb.Content = barraPotencia;
            if (barraPotencia >= 33)
            {
                potencia.Margin = new Thickness(33, 600, barraPotencia, 10);
            }
            #endregion
            #region Batear


            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                potencia.Visibility = System.Windows.Visibility.Hidden;
                potencia2.Visibility = System.Windows.Visibility.Hidden;
                //Dañar worms cercanos
                foreach (UserControlWorms w in worms)
                {
                    if (w == turns.Peek())
                        continue;
                    if (Math.Sqrt(Math.Pow(w.CanvasXPos - batPosX, 2) + Math.Pow(w.CanvasYPos - batPosY, 2)) < 60)
                    {
                        w.worm.hp -= 20;
                        var xspeed = (mousePosX - batPosX) * 10;
                        var yspeed = (mousePosY - batPosY) * 10;
                        var maxspeed = 6000;
                        if (Math.Sqrt(Math.Pow(xspeed, 2) + Math.Pow(yspeed, 2)) > maxspeed)
                        {
                            xspeed = (xspeed / Math.Sqrt(Math.Pow(xspeed, 2) + Math.Pow(yspeed, 2))) * maxspeed;
                            yspeed = (yspeed / Math.Sqrt(Math.Pow(xspeed, 2) + Math.Pow(yspeed, 2))) * maxspeed;
                        }
                        w.xspeed = xspeed;
                        w.yspeed = yspeed;

                    }
                }
                BatControl.Stop();
                ChangeOfShift();

            }
            #endregion
        }

        private void UpdaterForBazooka(object sender, EventArgs e)
        {
            var mousePosX = Mouse.GetPosition(MyCanvas).X;
            var mousePosY = Mouse.GetPosition(MyCanvas).Y - 80;
            var bazookaPosX = turns.Peek().CanvasXPos + 15;
            var bazookaPosY = turns.Peek().CanvasYPos - 30;
            // Angulo de ataque
            var controller = ImageBehavior.GetAnimationController(turns.Peek().icon);

            var angle = Math.Atan((mousePosY - bazookaPosY) / (mousePosX - bazookaPosX)) + (Math.PI / 2); // va de 0 a Pi
            var frame = 0.0;
            if (mousePosX < bazookaPosX)
            {
                frame = (angle * 31) / (Math.PI);
                ScaleTransform scale = new ScaleTransform();

                scale.ScaleX = 1;
                scale.CenterX = 45;
                TransformGroup myTransformGroup = new TransformGroup();
                myTransformGroup.Children.Add(scale);
                turns.Peek().icon.RenderTransform = myTransformGroup;
                controller.GotoFrame((int)frame);
            }
            else
            {
                ScaleTransform scale = new ScaleTransform();
                scale.ScaleX = -1;
                scale.CenterX = 45;
                TransformGroup myTransformGroup = new TransformGroup();
                myTransformGroup.Children.Add(scale);
                turns.Peek().icon.RenderTransform = myTransformGroup;
                frame = -1 * (((angle * 31) / (Math.PI)) - 14) + 17;
                controller.GotoFrame((int)frame);
            }
            //Barra de potencia

            var speedx = mousePosX - bazookaPosX;
            var speedy = mousePosY - bazookaPosY;
            var speed = Math.Sqrt(Math.Pow(speedx, 2) + Math.Pow(speedy, 2));
            var barraPotencia = (160.0 / speed) * 950;
            //lb.Content = barraPotencia;
            if (barraPotencia >= 33)
            {
                potencia.Margin = new Thickness(33, 600, barraPotencia, 10);
            }



            //lb.Content = "frame = " + frame;
            if (Mouse.LeftButton == MouseButtonState.Pressed && wraper.Count == 0)
            {
                potencia.Visibility = System.Windows.Visibility.Hidden;
                potencia2.Visibility = System.Windows.Visibility.Hidden;

                Wrap p = new Wrap(speedx, speedy, turns.Peek().CanvasXPos, turns.Peek().CanvasYPos, 90, 40);
                Image img = new Image();
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\bazooka.gif");
                image.EndInit();
                ImageBehavior.SetAnimatedSource(img, image);
                Canvas.SetTop(img, p.CanvasPosY);
                Canvas.SetLeft(img, p.CanvasPosX);
                MyCanvas.Children.Add(img);

                wraper.Add(new Tuple<Image, Wrap>(img, p));
                BazookaControl.Stop();

            }
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
                //Deleting from wraper
                foreach (Tuple<Image, Wrap> p in toDelete)
                {
                    wraper.Remove(p);
                }
            }
            #endregion
            #region Control Worms
            List<UserControlWorms> toDelete2 = new List<UserControlWorms>();
            foreach (UserControlWorms w in worms)
            {
                #region Control Air
                if (w.yspeed != 0 || w.onAir || true)
                {
                    w.onAir = true;
                    if (Keyboard.IsKeyDown(Key.Left) && w.Equals(turns.Peek()))
                        MoveLeft(w);
                    if (Keyboard.IsKeyDown(Key.Right) && w.Equals(turns.Peek()))
                        MoveRight(w);
                    try
                    {
                        if (core.map.grid[(int)((w.CanvasXPos + 15.0) / 30.0), (int)((w.CanvasYPos + 68) / 30.0)] != "1" || w.yspeed < 0)
                        {
                            if (w.yspeed < 0 && core.map.grid[(int)((w.CanvasXPos + 15.0) / 30.0), 
                                (int)((w.CanvasYPos + 30) / 30.0)] == "1")
                                w.yspeed = 0;
                            w.CanvasYPos += w.yspeed * 0.001;
                            Canvas.SetTop(w, w.CanvasYPos);
                            if ((w.xspeed > 0 && (core.map.grid[(int)(w.CanvasXPos + 31) / 30, 
                                (int)(w.CanvasYPos + 30) / 30] != "1")) || (w.xspeed < 0 && (core.map.grid[(int)(w.CanvasXPos - 1) / 30, 
                                (int)(w.CanvasYPos + 30) / 30] != "1")))
                            {
                                w.CanvasXPos += w.xspeed * 0.001;
                                Canvas.SetLeft(w, w.CanvasXPos);
                            }
                            w.yspeed += gravity * 0.001;
                        }
                        else
                        {
                            w.onAir = false;
                            w.yspeed = 0;
                            w.xspeed = 0;
                        }
                    }
                    catch
                    {
                        if (w == turns.Peek())
                            ChangeOfShift();
                        MyCanvas.Children.Remove(w);
                        turns = ExtensionMethods.RemoveFromQueue(w, turns);
                        toDelete2.Add(w);
                    }
                }
                #endregion
                #region Control Hp
                if (w.worm.hp <= 0)
                {
                    User(w.CanvasXPos, w.CanvasYPos, 60, 10);
                    MyCanvas.Children.Remove(w);
                    turns = ExtensionMethods.RemoveFromQueue(w, turns);
                    toDelete2.Add(w);
                }
                else
                {
                    w.HP.Content = w.worm.hp;
                }

                #endregion
            }
            //Deleting worms:
            foreach (UserControlWorms w in toDelete2)
            {
                worms.Remove(w);
            }
            #endregion
            #region Control de Victoria
            bool victoriaRojo = true;
            bool victoriaAzul = true;
            foreach (UserControlWorms w in worms)
            {
                if (w.worm.equip == Team.teams.blue)
                    victoriaRojo = false;
                else victoriaAzul = false;
            }
            if (victoriaRojo || victoriaAzul)
            {
                Victory();
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
            Image explosion = new Image();

            MyCanvas.Children.Add(explosion);
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\explotion.gif");
            image.EndInit();

            ImageBehavior.SetAnimatedSource(explosion, image);
            Canvas.SetLeft(explosion, xpos - 50);
            Canvas.SetTop(explosion, ypos);
            ImageBehavior.SetRepeatBehavior(explosion, new System.Windows.Media.Animation.RepeatBehavior(TimeSpan.FromMilliseconds(530)));
            //ImageBehavior.AnimationCompletedEvent.


            foreach (UserControlWorms w in worms)
            {
                if (Math.Sqrt((w.CanvasXPos - xpos) * (w.CanvasXPos - xpos) + (w.CanvasYPos - ypos) * (w.CanvasYPos - ypos)) < radio)
                {
                    w.worm.hp -= (int)dano;
                }
            }

            //Destruction of bricks
            for (int x = 0; x < core.map.weight; x++)
            {
                for (int y = 0; y < core.map.height; y++)
                {
                    if (Math.Pow(x * 30 - xpos, 2) + Math.Pow(y * 30 - ypos, 2) < radio * radio)
                    {
                        core.map.grid[x, y] = "0";
                        MyCanvas.Children.Remove(blocks[x, y]);
                    }
                }
            }
        }

        private void MoveLeft(UserControlWorms worm)
        {
            var canvasposleft = (double)worm.GetValue(Canvas.LeftProperty);
            var canvaspostop = (double)worm.GetValue(Canvas.TopProperty) + 30.0;
            try
            {
                if (core.map.grid[(int)((canvasposleft - 2) / 30), (int)Math.Ceiling(canvaspostop / 30)] != "1")
                {
                    var velocidad = 1000;
                    worm.CanvasXPos = (double)worm.GetValue(Canvas.LeftProperty) - velocidad * 0.002;
                    Canvas.SetLeft(worm, worm.CanvasXPos);
                }
            }
            catch { }
            Thread.Sleep(1);
            worm.onAir = true;
        }

        private void MoveRight(UserControlWorms worm)
        {
            var canvasposleft = (double)worm.GetValue(Canvas.LeftProperty);
            var canvaspostop = (double)worm.GetValue(Canvas.TopProperty) + 30.0;
            try
            {
                if (core.map.grid[(int)((canvasposleft + 30) / 30), (int)Math.Ceiling(canvaspostop / 30)] != "1")
                {
                    var velocidad = 1000;
                    worm.CanvasXPos = (double)worm.GetValue(Canvas.LeftProperty) + velocidad * 0.002;
                    Canvas.SetLeft(worm, worm.CanvasXPos);
                }
            }
            catch { }
            System.Threading.Thread.Sleep(1);
            worm.onAir = true;
        }

        public void Victory()
        {
            foreach (UserControlWorms worm in worms)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"D:\repos\Die_And_KILL\Die_And_Kill\Resources\winner.gif");
                image.EndInit();
                ImageBehavior.SetAnimatedSource(worm.icon, image);
                TriggerUpdater.Stop();
                this.KeyDown -= MainWindow_KeyDown;
                bazookabtn.Click -= SelectBazooka;
                bat.Click -= SelectBat;

            }
        }
    }
}
