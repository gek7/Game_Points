using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace points
{
    public class Player
    {
        // Список всех игроков
        public static List<Player> Players = new List<Player>();


        public int id{ get; set; }
        public string NickName { get; set; }
        public SolidColorBrush Color { get; set; }
        public Player(string name, SolidColorBrush Color)
        {
            NickName = name;
            id = Players.Count+1;
            this.Color = Color;
            Players.Add(this);
        }
        public static Player FindPlayer(int temp_id)
        {
            foreach (Player item in Players)
            {
                if (item.id == temp_id) return item;
            }
            return null;
        }
    }
}
