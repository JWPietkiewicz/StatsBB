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
        Point Point { get; set; }
        public bool IsBehind3 {  get; set; }
        public bool IsLeft { get; set; }

        public CourtPointData(Point p, bool is3, bool isLeft)
        {
            Point = p;
            IsBehind3 = is3;
            IsLeft = isLeft;
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

            return "x: " + Point.X + " y: " + Point.Y + " isBehind3: " + IsBehind3 + " isLeft: " + IsLeft + "\n";
        }
    }
}
