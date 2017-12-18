using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Core : Team
    {
        List<string> names = new List<string>();
        public Worm[] worms = new Worm[10];
        public Map map = new Map();
        public Random rnd = new Random();

        public Core()
        {
            Random randy = new Random();
            #region read standart names names.txt
            using (StreamReader r = new StreamReader("..\\..\\..\\names.txt"))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    names.Add(line);
                }
            }
            #endregion
            #region Create worm
            for (int i = 0; i < 10; i++)
            {
                teams equipo = teams.blue;
                if (i > 4)
                {
                    equipo = teams.red;
                }
                var index = randy.Next(0, names.Count() - 1);
                var nombre = names[index];
                names.RemoveAt(index);

                worms[i] = new Worm(randy, nombre, equipo, map.SuperficiesPisables());
                map.Add(worms[i].position, "2");


            }
            #endregion
        }


    }
}
