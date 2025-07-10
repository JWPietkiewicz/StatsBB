using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StatsBB.UserControls;

public partial class TeamInfoView : UserControl
{
    public TeamInfoView()
    {
        InitializeComponent();
    }

    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject? parent = VisualTreeHelper.GetParent(child);
        while (parent != null && parent is not T)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as T;
    }

    private void PlayerTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox tb)
            return;

        var grid = FindParent<DataGrid>(tb);
        if (grid == null)
            return;

        int row = grid.Items.IndexOf(grid.CurrentItem);
        int col = grid.Columns.IndexOf(grid.CurrentColumn);

        void Begin(int r, int c)
        {
            if (r < 0 || r >= grid.Items.Count || c < 0 || c >= grid.Columns.Count)
                return;
            grid.CommitEdit(DataGridEditingUnit.Cell, true);
            grid.SelectedIndex = r;
            grid.CurrentCell = new DataGridCellInfo(grid.Items[r], grid.Columns[c]);
            grid.ScrollIntoView(grid.Items[r]);
            grid.BeginEdit();
        }

        if (e.Key == Key.Up)
        {
            Begin(row - 1, col);
            e.Handled = true;
        }
        else if (e.Key == Key.Down)
        {
            Begin(row + 1, col);
            e.Handled = true;
        }
        else if (e.Key == Key.Tab)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                Begin(row, col - 1);
            }
            else
            {
                Begin(row, col + 1);
            }
            e.Handled = true;
        }
    }
}
