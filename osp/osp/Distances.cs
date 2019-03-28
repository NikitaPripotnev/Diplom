using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* 
 *  Функции для вычисления расстояний. 
 *  Все функции имеют одинаковый интерфейс, что позволяет делегирование. 
 */

namespace osp
{
    public delegate double Distance(Node nA, Node nB);

    public static class Distances
    {
        public static double Distance_1(Node nA, Node nB)
        {
            return 1;
        }

        public static double Distance_EXPLICIT(Node nA, Node nB)
        {
            return 0;
        }

        public static double Distance_CEIL_2D(Node nA, Node nB)
        {
            double xd = nA.Coords.X - nB.Coords.X, yd = nA.Coords.Y - nB.Coords.Y;
            return Math.Ceiling(Math.Sqrt(xd * xd + yd * yd));
        }

        public static double Distance_CEIL_3D(Node nA, Node nB)
        {
            double xd = nA.Coords.X - nB.Coords.X, yd = nA.Coords.Y - nB.Coords.Y, zd = nA.Coords.Z - nB.Coords.Z;
            return Math.Ceiling(Math.Sqrt(xd * xd + yd * yd + zd * zd));
        }

        public static double Distance_EUC_2D(Node nA, Node nB)
        {
            double xd = nA.Coords.X - nB.Coords.X, yd = nA.Coords.Y - nB.Coords.Y;
            return Math.Sqrt(xd * xd + yd * yd);
        }

        public static double  Distance_EUC_3D(Node nA, Node nB)
        {
            double xd = nA.Coords.X - nB.Coords.X, yd = nA.Coords.Y - nB.Coords.Y, zd = nA.Coords.Z - nB.Coords.Z;
            return Math.Sqrt(xd * xd + yd * yd + zd * zd);
        }

        public static double  Distance_MAN_2D(Node nA, Node nB)
        {
            return (Math.Abs(nA.Coords.X - nB.Coords.X) + Math.Abs(nA.Coords.Y - nB.Coords.Y));
        }

        public static double Distance_MAN_3D(Node nA, Node nB)
        {
            return (Math.Abs(nA.Coords.X - nB.Coords.X) + Math.Abs(nA.Coords.Y - nB.Coords.Y) + Math.Abs(nA.Coords.Z - nB.Coords.Z));
        }

        public static double Distance_MAX_2D(Node nA, Node nB)
        {
            int dx = (int) (Math.Abs(nA.Coords.X - nB.Coords.X) + 0.5),
                dy = (int) (Math.Abs(nA.Coords.Y - nB.Coords.Y) + 0.5);
            return dx > dy ? dx : dy;
        }

        public static double Distance_MAX_3D(Node nA, Node nB)
        {
            int dx = (int) (Math.Abs(nA.Coords.X - nB.Coords.X) + 0.5),
                dy = (int) (Math.Abs(nA.Coords.Y - nB.Coords.Y) + 0.5),
                dz = (int) (Math.Abs(nA.Coords.Z - nB.Coords.Z) + 0.5);
            if (dy > dx)
                dx = dy;
            return dx > dz ? dx : dz;
        }

        public static double Distance_ATT(Node nA, Node nB)
        {
            double xd = nA.Coords.X - nB.Coords.X, yd = nA.Coords.Y - nB.Coords.Y;
            return Math.Ceiling(Math.Sqrt((xd * xd + yd * yd) / 10.0));
        }


    }
}
