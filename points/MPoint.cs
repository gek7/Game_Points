using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace points
{

    public class MPoint
    {
        public int i { get; set; }
        public int j { get; set; }

        public MPoint(int i,int j)
        {
            this.i = i;
            this.j = j;
        }

        public static bool operator != (MPoint p1, MPoint p2)
        {
            if ((p1.i != p2.i) || (p1.j != p2.j))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator == (MPoint p1,MPoint p2)
        {
            if ((p1.i == p2.i) && (p1.j == p2.j))
            {
                return true;
            }
             else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"{i},{j}";
        }

        public override bool Equals(object obj)
        {
            MPoint p1 = this;
            MPoint p2 = obj as MPoint;
            if ((p1.i == p2.i) && (p1.j == p2.j))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
