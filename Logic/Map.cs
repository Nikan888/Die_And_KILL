using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Map
    {
        public string[,] grid;
        public int height;
        public int weight;

        public Map(string nombre)
        {
            #region Upload map from sourse
            using (StreamReader r = new StreamReader("..\\..\\..\\" + nombre + ".txt"))
            {
                string[] dimensiones = r.ReadLine().Split(new string[] { "x" }, StringSplitOptions.None);
                height = Int32.Parse(dimensiones[0]);
                weight = Int32.Parse(dimensiones[1]);
                grid = new string[weight, height];

                string line;
                int x = 0;
                int y = 0;
                while ((line = r.ReadLine()) != null)
                {
                    foreach (char c in line)
                    {
                        if (c.ToString() == "1")
                        {
                            grid[x, y] = "1";
                        }
                        else
                        {
                            grid[x, y] = "0";
                        }
                        x++;
                    }
                    x = 0;
                    y++;
                }
            }
            #endregion
            // "1" = block, "2" = worm, "0" = empty space
        }

        public Map()
        {
            #region Crear Mapa aleatorio
            height = 15;
            weight = 50;
            grid = new string[weight, height];
            Random randy = new Random();
            for (int x = 0; x < weight; x++)
            {
                for (int y = height - 1; y > randy.Next(5, height - 1); y--)
                {
                    grid[x, y] = "1";
                }
            }
            for (int x = 0; x < weight; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == null)
                        grid[x, y] = "0";
                }
            }


            #endregion
        }

        public List<Coordinate> SuperficiesPisables()
        {
            List<Coordinate> result = new List<Coordinate>();
            for (int x = 0; x < weight; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == "1" && grid[x, y - 1] == "0" && (x == 0 || 
                        grid[x - 1, y - 1] != "2") && (x + 1 == weight || grid[x + 1, y - 1] != "2"))
                    {
                        result.Add(new Coordinate(x, y - 1));
                    }
                }
            }
            return result;
        }
    }
}
