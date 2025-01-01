using System.Data;
using System.Runtime.CompilerServices;

namespace CPUWindowsFormFramework
{
    public class WindowsFormUtility
    {
        public static void SetListBinding(ComboBox lst, DataTable sourcedt, DataTable? targetdt, string tablename)
        {
            lst.DataSource = sourcedt;
            lst.ValueMember = tablename + "Id";
            lst.DisplayMember = lst.Name.Substring(3);
            if (targetdt != null)
            {
                lst.DataBindings.Add("SelectedValue", targetdt, lst.ValueMember, false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        public static void SetControlBinding(Control ctrl, BindingSource bindsource)
        {
            string propertyname = "";
            string controlname = ctrl.Name.ToLower();
            string controltype = controlname.Substring(0, 3);
            string columnname = controlname.Substring(3);

            switch (controltype)
            {
                case "txt":
                case "lbl":
                    propertyname = "Text";
                    break;
                case "dtp":
                    propertyname = "Value";
                    break;
                case "ckb":
                    propertyname = "Checked";
                    break;
            }

            if (propertyname != "" && columnname != "")
            {
                ctrl.DataBindings.Add(propertyname, bindsource, columnname, true, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        public static void FormatGridForView(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            grid.ReadOnly = true;
            grid.ClearSelection();
        }

        public static void FormatGridForSearch(DataGridView grid, string tablename)
        {
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DoFormatGrid(grid, tablename);
        }

        public static void FormatGridForEdit(DataGridView grid, string tablename)
        {
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            DoFormatGrid(grid, tablename);
        }

        private static void DoFormatGrid(DataGridView grid, string tablename)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            grid.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.RowHeadersWidth = 25;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Name.EndsWith("Id"))
                {
                    col.Visible = false;
                }
            }
            string pkname = tablename + "Id";
            if (grid.Columns.Contains(pkname))
            {
                grid.Columns[pkname].Visible = false;
            }
        }

        public static int GetIdFromGrid(DataGridView grid, int rowindex, string columnname)
        {
            int id = 0;
            if (
                rowindex < grid.Rows.Count
                &&
                grid.Columns.Contains(columnname)
                &&
                grid.Rows[rowindex].Cells[columnname].Value != DBNull.Value
                &&
                grid.Rows[rowindex].Cells[columnname].Value is int
                )
            {
                id = (int)grid.Rows[rowindex].Cells[columnname].Value;
            }
            //id = (int)gPresident.Rows[rowindex].Cells["PresidentId"].Value;

            return id;
        }

        public static int GetIdFromCombobox(ComboBox lst)
        {
            int value = 0;
            if (lst.SelectedValue != null && lst.SelectedValue is int)
            {
                value = (int)lst.SelectedValue;
            }
            return value;
        }

        public static string GetNameFromCombobox(ComboBox lst)
        {
            string value = "";
            if (lst.SelectedText != null && lst.SelectedText is string)
            {
                value = (string)lst.SelectedText;
            }
            return value;
        }

        public static void AddComboBoxToGrid(DataGridView grid, DataTable datasource, string tablename, string displaymember)
        {
            DataGridViewComboBoxColumn c = new();
            c.DataSource = datasource;
            c.DisplayMember = displaymember;
            c.ValueMember = tablename + "Id";
            c.DataPropertyName = c.ValueMember;
            c.HeaderText = tablename;
            grid.Columns.Insert(0,c);
        }

        public static void AddDeleteButtonToGrid(DataGridView grid, string deletecolumnname)
        {
            grid.Columns.Add(new DataGridViewButtonColumn() { Text = "X", HeaderText = "Delete", Name = deletecolumnname, UseColumnTextForButtonValue = true });
        }

        public static (bool, Form) IsFormOpen(Type formtype, int pkvalue = 0)
        {
            bool exists = false;
            Form newfrm = new();
            foreach (Form frm in Application.OpenForms)
            {
                int frmpkvalue = 0;
                if (frm.Tag != null && frm.Tag is int)
                {
                    frmpkvalue = (int)frm.Tag;
                }
                if (frm.GetType() == formtype && frmpkvalue == pkvalue)
                {
                    newfrm = frm;
                    //frm.Activate();
                    exists = true;
                    break;
                }
            }
            return (exists, newfrm);
        }

        public static void SetupNav(ToolStrip ts)
        {
            ts.Items.Clear();
            foreach (Form f in Application.OpenForms)
            {
                if (f.IsMdiContainer == false)
                {
                    ToolStripButton btn = new();
                    btn.Text = f.Text;
                    btn.Tag = f;


                    ts.Items.Add(btn);
                    ts.Items.Add(new ToolStripSeparator());
                    SetupNavColor(f, btn);
                    f.Activated += (s, args) => SetupNavColor(f, btn);
                    btn.Click += (s, args) => Btn_Click(ts, btn);
                }
            }
        }

        private static void SetupNavColor(Form f, ToolStripButton btn)
        {
            btn.BackColor = f.WindowState == FormWindowState.Maximized ? Color.LightGray : default;
        }

        private static void Btn_Click(ToolStrip ts, ToolStripButton btn)
        {
            foreach (ToolStripItem i in ts.Items)
            {
                if (i is ToolStripButton b && i != null)
                {
                    b.BackColor = default;
                }
            }
            if (btn.Tag != null && btn.Tag is Form)
            {
                ((Form)btn.Tag).Activate();
            }
        }
    }
}

