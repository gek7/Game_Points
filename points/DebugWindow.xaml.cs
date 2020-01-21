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
using System.Windows.Shapes;

namespace points
{
    /// <summary>
    /// Логика взаимодействия для DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }
        

        public void RefreshDebugPointMatr(int[,] debugPointMatr)
        {
            ListPointMatr.Items.Clear();
            string line;
            for (int i = 0; i < 19; i++)
            {
                line = "";
                for (int j = 0; j < 19; j++)
                {
                    line += debugPointMatr[i, j] + "  ";
                }
                ListPointMatr.Items.Add(line);
            }
        }

        public void RefreshDebugFillMatr(int[,] debugFillMatr)
        {
            ListFill.Items.Clear();
            string line;
            for (int i = 0; i < 19; i++)
            {
                line = "";
                for (int j = 0; j < 19; j++)
                {
                    line += debugFillMatr[i, j] + "  ";
                }
                ListFill.Items.Add(line);
            }
        }
    }
}
