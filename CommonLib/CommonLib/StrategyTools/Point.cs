using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class Point : IComparable
    {
        public double X { get; set; }
        public double Y { get; set; }

        public ComparisonType TypeOfComparison { get; set; }

        public Point()
        {

        }

        public Point(double _X, double _Y)
        {
            X = _X;
            Y = _Y;
            TypeOfComparison = ComparisonType.X;
        }

        public Point(double _X, double _Y, ComparisonType _TypeOfComparison)
        {
            X = _X;
            Y = _Y;
            TypeOfComparison = _TypeOfComparison;
        }

        public double Distance(Point P2)
        {
            return Math.Sqrt((P2.X - X) * (P2.X - X) + (P2.Y - Y) * (P2.Y - Y));
        }

        public double Slope(Point P2)
        {
            return (P2.Y - Y) / (P2.X - X);
        }

        public double Intercept(Point P2)
        {
            return Y - Slope(P2) * X;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Point)
            {
                if (TypeOfComparison == ComparisonType.X)
                {
                    Point temp = (Point)obj;
                    return X.CompareTo(temp.X);
                }
                else if (TypeOfComparison == ComparisonType.Y)
                {
                    Point temp = (Point)obj;
                    return Y.CompareTo(temp.Y);
                }
                else if (TypeOfComparison == ComparisonType.MAG)
                {
                    Point temp = (Point)obj;
                    return (X * X + Y * Y).CompareTo((temp.X * temp.X + temp.Y * temp.Y));
                }
            }

            throw new ArgumentException("object is not a Point");
        }
       
        public bool IsEqual(Point y)
        {
            if (this == null && y == null)
                return true;
            else if (this == null || y == null)
                return false;
            else if (this.X == y.X && this.Y == y.Y)
                return true;
            else
                return false;
        }        
    }
}
