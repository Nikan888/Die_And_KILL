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
using Logic;

namespace Die_And_Kill
{
    /// <summary>
    /// Interaction logic for UserControlWorms.xaml
    /// </summary>
    public partial class UserControlWorms : UserControl
    {
        public Worm worm;

        public double yspeed = 0;
        public double xspeed = 0;
        public double CanvasXPos;
        public double CanvasYPos;
        public bool onAir = false;

        public UserControlWorms(Worm worm)
        {
            InitializeComponent();

            this.worm = worm;
            name.Content = worm.name;
            name.Foreground = (worm.equip == Team.teams.red) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Blue);
            name.FontSize = 14;
            HP.Content = worm.hp;
            HP.Foreground = (worm.equip == Team.teams.red) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Blue);
            HP.FontSize = 14;
        }
    }
}
