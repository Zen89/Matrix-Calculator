using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Calculator
{
    public class MatrixElement
    {
        private string _color;

        public MatrixElement(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                if (ColorChanged != null)
                    ColorChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler ColorChanged;
    }
}
