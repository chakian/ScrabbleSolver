using System.Windows.Controls;
using Model;

namespace Controls
{
    // Grid extended to add required column/row definitions when created.
    //
    // We derive from Grid (rather than simply using a Panel) so that we can be updated when 
    // the Grid-based attached properties change (e.g. Grid.Row, Grid.ColumnSpan, etc).
    //
    // It seems they only send updates when attached to visuals that are children of a Grid.

    public class ScrabbleGrid : Grid
    {
        public ScrabbleGrid()
        {
            for (var i = 0; i < Board.Columns; ++i)
                ColumnDefinitions.Add(new ColumnDefinition());

            for (var i = 0; i < Board.Rows; ++i)
                RowDefinitions.Add(new RowDefinition());
        }
    }
}