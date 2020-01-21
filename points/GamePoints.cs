using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace points
{
    public class GamePoints
    {

        Window gameWindow;
        Grid grid1;

        // Хранятся ссылки на все линии поля
        List<UIElement> Lines = new List<UIElement>();
        // Хранятся ссылки на все точки
        List<UIElement> Ellipses = new List<UIElement>();
        // Хранятся координаты, куда можно поставить точки
        Point[,] coordMatr = new Point[19,19];
        // Хранятся поставленные точки { 0 - место не занято, 1 - первый игрок, 2 - второй игрок  }
        int[,] pointMatr = new int[19, 19];
        Ellipse[,] linksOnPoints = new Ellipse[19, 19];
        // 'Пучки' точек игроков. Одна точка считается отдельным 'пучком'
        List<List<MPoint>>[] bunches = new List<List<MPoint>>[2];
        // Окно отладки
        DebugWindow d;
        // Конструктор 
        public GamePoints(Window curWindow,Grid g)
        {
            gameWindow = curWindow;
            grid1 = g;
            d = new DebugWindow();
            d.Show();
        }

        // Отрисовка поля
        public void drawField()
        {
            double WidthWorkArea = grid1.Width-5;
            //vert
            double x1 = 0, y1 = 0, x2 = 0, y2 = WidthWorkArea;
            // 21 итерация (ещё одна для отрисовки последней линии поля)
            for (int i = 0; i <= 20; i++)
            {
                Line l = new Line();
                if (i == 0 || i == 20)
                {
                    l.StrokeThickness = 3;
                }

                // Выставления координат, куда можно поставить точки (X)
                else if (i != 0 && i!=20)
                {
                    for (int c = 0; c < 19; c++)
                    {
                        coordMatr[c, i-1].X = x1;
                    }
                }
                l.X1 = x1;
                l.Y1 = y1;
                l.X2 = x1;
                l.Y2 = y2;
                l.Stroke = Brushes.Black;
                x1 += (WidthWorkArea) / 20;
                x2 += x1;
                grid1.Children.Add(l);
                Lines.Add(l);
            }

            //horz
            x1 = 0;
            y1 = 0;
            x2 = WidthWorkArea;
            y2 = y1;
            for (int i = 0; i <= 20; i++)
            {
                Line l = new Line();
                if (i == 0 || i == 20)
                {
                    l.StrokeThickness = 3;
                }

                // Выставления координат, куда можно поставить точки (Y)
                else if (i != 0 && i != 20)
                {
                    for (int c = 0; c < 19; c++)
                    {
                        coordMatr[i-1, c].Y = y1;
                    }
                }

                l.X1 = x1;
                l.Y1 = y1;
                l.X2 = x2;
                l.Y2 = y1;
                l.Stroke = Brushes.Black;
                y1 += (WidthWorkArea) / 20;
                y2 += y1;
                grid1.Children.Add(l);
                Lines.Add(l);
            }
        }

        // Уничтожение поля
        public void destroyField()
        {
            foreach (UIElement u in Lines)
            {
                grid1.Children.Remove(u);
            }
            ClearField();
        }

        // Очищение поля
        public void ClearField()
        {
            foreach (var item in Ellipses)
            {
                grid1.Children.Remove(item);
            }
            pointMatr = new int[19, 19];
            bunches = new List<List<MPoint>>[2];

        }

        // Установка точки на поле (Если возможно)
        public bool SetPoint(Point p,Player CurPlayer)
        {
            for (int i = 0; i < 19; i++)
            {
                if (coordMatr[i,0].Y-5 <= p.Y && p.Y <= coordMatr[i, 0].Y + 5)
                {
                    for (int j = 0; j < 19; j++)
                    {
                        if (coordMatr[i, j].X - 5 <= p.X && p.X <= coordMatr[i, j].X + 5)
                        {
                            if (pointMatr[i, j] == 0)
                            {
                                Ellipse r = new Ellipse();
                                r.Fill = CurPlayer.Color;
                                r.Width = 10;
                                r.Height = 10;
                                r.VerticalAlignment = VerticalAlignment.Top;
                                r.HorizontalAlignment = HorizontalAlignment.Left;
                                Thickness t = new Thickness();
                                t.Left = coordMatr[i,j].X - 5;
                                t.Top = coordMatr[i, j].Y - 5;
                                r.Margin = t;
                                grid1.Children.Add(r);
                                linksOnPoints[i, j] = r;
                                pointMatr[i, j] = CurPlayer.id;
                                Ellipses.Add(r);
                                GenerateBunches(new MPoint(i,j), CurPlayer.id);
                                CheckCatchBunch();
                                d.RefreshDebugPointMatr(pointMatr);
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return false;
        }

        // Проверка можно ли захватить точку
        public List<MPoint> CheckCatchPoint(MPoint p, int id)
        {
            int[,] tempArr = new int[19, 19];
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                    if ((pointMatr[i, j] != id) || (i == p.i && j == p.j)) tempArr[i, j] = pointMatr[i, j];

                Queue<MPoint> WaitingPoints = new Queue<MPoint>();
            List<MPoint> Edges = new List<MPoint>();
            WaitingPoints.Enqueue(p);
            do
            {
                MPoint curPoint = WaitingPoints.Dequeue();

                    tempArr[curPoint.i, curPoint.j] = id;
                    List<MPoint> tempList = FindPath(curPoint,false);

                    for (int c = 0; c < tempList.Count; c++)
                    {
                        int i = tempList[c].i;
                        int j = tempList[c].j;

                        if (tempArr[i, j] <= 0)
                        {
                            WaitingPoints.Enqueue(new MPoint(i, j));
                            if (curPoint.i == 0 || curPoint.i == 18 || curPoint.i == 0 || curPoint.i == 18)
                                return null;
                        }
                        else if (tempArr[i, j] != id && tempArr[i, j]>0 
                                && Edges.Where(s=>(s.i==i)&&(s.j == j)).Count()==0)
                        {
                            Edges.Add(new MPoint(i, j));
                        }
                    }
            }
            while (WaitingPoints.Count > 0);

            List<MPoint> t = new List<MPoint>();
            t = Edges;
            return t;
        }

        //Проверка на захват целого 'пучка' точек
        public void CheckCatchBunch()
        {
            List<MPoint> Edges=null;
            Stack<List<MPoint>> BunchForDel = new Stack<List<MPoint>>();
            List<MPoint> pointsForDel = new List<MPoint>();
            for (int i = 0; i < 2; i++)
            {
                int catchedPoints = 0;
                if (bunches.Where(t => t != null).Count() > 1)
                {
                    foreach (var item in bunches[i])
                    {
                        Edges = CheckCatchPoint(item[0], i + 1);
                        if (Edges != null)
                        {
                            BunchForDel.Push(item);
                            sortEdges(ref Edges);
                            Edges.Add(Edges[0]);
                            for (int j = 1; j < Edges.Count; j++)
                            {
                                Line l = new Line();
                                Point p1 = coordMatr[Edges[j - 1].i, Edges[j - 1].j];
                                Point p2 = coordMatr[Edges[j].i, Edges[j].j];
                                l.X1 = p1.X;
                                l.Y1 = p1.Y;
                                l.X2 = p2.X;
                                l.Y2 = p2.Y;
                                l.Stroke = Brushes.Green;
                                l.StrokeThickness = 3;
                                grid1.Children.Add(l);
                            }
                        }
                    }
                }

                // Подсчёт захваченных точек
                for (int j = 0; j < BunchForDel.Count; j++)
                {
                    pointsForDel = BunchForDel.Pop();
                    catchedPoints += pointsForDel.Count();
                    foreach (var point in pointsForDel)
                    {
                        pointMatr[point.i, point.j] *= -1;
                    }
                    bunches[i].Remove(pointsForDel);
                    MessageBox.Show($"Gotcha {catchedPoints}");
                }
            }
        }

        // Возвращает ответвления от переданной точки
        public List<MPoint> FindPath(MPoint p, int id,bool isForBunch)
        {         
            List<MPoint> temp = new List<MPoint>();
            if (p.i > 0) if (pointMatr[p.i - 1, p.j] == id) temp.Add(new MPoint(p.i - 1, p.j));
            if (p.i < 18) if (pointMatr[p.i + 1, p.j] == id) temp.Add(new MPoint(p.i + 1, p.j));
            if (p.j > 0) if (pointMatr[p.i, p.j - 1] == id) temp.Add(new MPoint(p.i, p.j - 1));
            if (p.j < 18) if (pointMatr[p.i, p.j + 1] == id) temp.Add(new MPoint(p.i, p.j + 1));

            if (!isForBunch)
            {
                if (p.i > 0 && p.j > 0) if (pointMatr[p.i - 1, p.j - 1] == id) temp.Add(new MPoint(p.i - 1, p.j - 1));
                if (p.i < 18 && p.j < 18) if (pointMatr[p.i + 1, p.j + 1] == id) temp.Add(new MPoint(p.i + 1, p.j + 1));
                if (p.i < 18 && p.j > 0) if (pointMatr[p.i + 1, p.j - 1] == id) temp.Add(new MPoint(p.i + 1, p.j - 1));
                if (p.i > 0 && p.j < 18) if (pointMatr[p.i - 1, p.j + 1] == id) temp.Add(new MPoint(p.i - 1, p.j + 1));
            }
            return temp;
        }

        public List<MPoint> FindPath(MPoint p,bool isAllPoints)
        {
            List<MPoint> temp = new List<MPoint>();
            if (p.i > 0) temp.Add(new MPoint(p.i - 1, p.j));
            if (p.i < 18) temp.Add(new MPoint(p.i + 1, p.j));
            if (p.j > 0)  temp.Add(new MPoint(p.i, p.j - 1));
            if (p.j < 18)  temp.Add(new MPoint(p.i , p.j + 1));

            if (isAllPoints)
            {
                if (p.i > 0 && p.j > 0) temp.Add(new MPoint(p.i - 1, p.j - 1));
                if (p.i < 18 && p.j < 18) temp.Add(new MPoint(p.i + 1, p.j + 1));
                if (p.i < 18 && p.j > 0) temp.Add(new MPoint(p.i + 1, p.j - 1));
                if (p.i > 0 && p.j < 18) temp.Add(new MPoint(p.i - 1, p.j + 1));
            }

            return temp;
        }

        // Генерирует пучки, которые можно захватить
        public void GenerateBunches(MPoint newP,int id)
        {
            List<List<MPoint>> CurBunches = bunches[id - 1] ?? new List<List<MPoint>>();
            List<List<MPoint>> FoundBunches = new List<List<MPoint>>();
            List<MPoint> Paths = FindPath(newP,id,true);
            int i = 0;
            int j,c;

            //Попытка присоединения точки к какому-нибудь 'пучку'
            while (i < CurBunches.Count)
            {
                j = 0;
                while (j < CurBunches[i].Count)
                {
                    c = 0;
                    while (c < Paths.Count)
                    {
                        if (Paths[c] == CurBunches[i][j])
                        {
                            FoundBunches.Add(CurBunches[i]);
                        }
                        c++;
                    }
                    j++;
                }
                i++;
            }

            List<MPoint> tempBunch = new List<MPoint>();

            if (FoundBunches.Count > 1)
            {
                while (FoundBunches.Count > 0)
                {
                    tempBunch.AddRange(FoundBunches[0]);
                    CurBunches.Remove(FoundBunches[0]);
                    FoundBunches.Remove(FoundBunches[0]);
                }
                tempBunch.Add(newP);
                CurBunches.Add(tempBunch);
            }
            else if(FoundBunches.Count==1)
            {
                CurBunches[CurBunches.IndexOf(FoundBunches[0])].Add(newP);
            }
            else
            {
                tempBunch.Add(newP);
                CurBunches.Add(tempBunch);
            }
            bunches[id - 1] = CurBunches;
        }

        public void sortEdges(ref List<MPoint> Edges)
        {
            List<MPoint> outputList = new List<MPoint>();
            outputList.Add(Edges[0]);

            while (outputList.Count!=Edges.Count)
            {
                int id = pointMatr[Edges[0].i, Edges[0].j];
                List<MPoint> tempList = FindPath(outputList.Last(), id, false);

                for (int i = 0; i < outputList.Count; i++)
                {
                    if (tempList.Where(t=>t==outputList[i]).Count()>0)
                    {
                        var temp = (tempList.Where(t => t == outputList[i])).First();
                        tempList.Remove(tempList[tempList.IndexOf(temp)]);
                    }
                }
                
                bool f = false;
                int j = 0;
                while (j<tempList.Count && !f)
                {
                    int i = 1;
                    while (i<Edges.Count && !f)
                    {
                        if (tempList[j] == Edges[i])
                        {
                            f = true;
                            outputList.Add(tempList[j]);
                        }
                        i++;
                    }
                    j++;
                }
            }
            Edges = outputList;
        }
    }
}
