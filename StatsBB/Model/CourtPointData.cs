using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StatsBB.Model
{
    public class CourtPointData
    {
        public Point Point { get; }
        public bool IsThreePoint { get; }
        public bool IsLeftSide { get; }

        public CourtPointData(Point point, bool isThreePoint, bool isLeftSide)
        {
            Point = point;
            IsThreePoint = isThreePoint;
            IsLeftSide = isLeftSide;
        }
        override public string ToString()
        {
            /*
            string output = "";
            if (IsLeft)
                output += "team A";
            else
                output += "team B";
            if (IsBehind3)
                output += " rzucila za 3";
            else
                output += " rzucila za 2";
            return output;*/

            return "x: " + Point.X + " y: " + Point.Y + " isBehind3: " + IsThreePoint + " isLeft: " + IsLeftSide + "\n";
        }
    }
}