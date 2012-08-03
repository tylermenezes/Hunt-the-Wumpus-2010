using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.Utilities;
namespace HuntTheWumpus.UICore.Controls
{
    class Table : Control
    {
        private List<Control[]> _content;
        public int ColumnLength { get; private set; }

        public Control this[int i, int j]
        {
            get
            {
                return _content[i][j];
            }
            set
            {
                _content[i][j] = value;
            }
        }

        public void AddRow()
        {
            _content.Add(new Control[ColumnLength]);
        }

        public void AddRow(Control[] content)
        {
            _content.Add(new Control[ColumnLength]);
        }

        public void DeleteRow(int i)
        {
            _content.RemoveAt(i);
        }

        public void Refresh()
        {
            //
        }

        public Table(Vector2 location, Vector2 size)
            : base(location, size) { }
    }
}
