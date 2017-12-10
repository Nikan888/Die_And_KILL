using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Die_And_Kill
{
    public class Wrap
    {
        public double SpeedX = 0;
        public double SpeedY = 0;
        public double CanvasPosX = 0;
        public double CanvasPosY = 0;
        public double radio;
        public double dano;

        public Wrap(double SpeedX, double SpeedY, double canvX, double canvY, double _radio, double _dano)
        {
            var CoefOfSpeed = 50.0; // Speed for change place for mouce 
            var maxSpeedOfChange= 200;
            if (Math.Sqrt(SpeedX * SpeedX + SpeedY * SpeedY) > maxSpeedOfChange) //If speed replace speed very much
            {
                SpeedX = (SpeedX / Math.Sqrt(SpeedX * SpeedX + SpeedY * SpeedY)) * maxSpeedOfChange;
                SpeedY = (SpeedY / Math.Sqrt(SpeedX * SpeedX + SpeedY * SpeedY)) * maxSpeedOfChange;
            }
            this.SpeedX = SpeedX * CoefOfSpeed;
            this.SpeedY = SpeedY * CoefOfSpeed;
            CanvasPosX = canvX;
            CanvasPosY = canvY;
            radio = _radio;
            dano = _dano;

        }
    }
}
