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
using static points.Player;

namespace points
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GamePoints mainGame;
        int CurPlayerId = 1;
        public MainWindow()
        {
            InitializeComponent();
            mainGame = new GamePoints(this, grid1,ScorePlayer1,ScorePlayer2);
            mainGame.drawField();
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Players.Count > 0)
            {
                if (mainGame.SetPoint(e.GetPosition(grid1), FindPlayer(CurPlayerId)))
                {
                    CurPlayerId++;
                    if (CurPlayerId > Players.Count) CurPlayerId = 1;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Players.Clear();
            mainGame.ClearField();
            CurPlayerId = 1;
            Player p1 = new Player("Player1", Brushes.Red);
            Player p2 = new Player("Player2", Brushes.Blue);
        }
    }
}
