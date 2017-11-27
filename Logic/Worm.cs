using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Worm : Team
    {
        public int hp = 100;
        public Coordinate position;
        public string name;
        public teams equipo;

        public Worm(Random r, string name, teams equipo, List<Coordinate> pisable)
        {
            this.name = name;
            this.equipo = equipo;
            this.position = pisable[r.Next(0, pisable.Count)];

        }
    }
}
