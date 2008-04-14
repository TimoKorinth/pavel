// Part of PAVEl: PAVEl (Paretoset Analysis Visualization and Evaluation) is a tool for
// interactively displaying and evaluating large sets of highdimensional data.
// Its main intended use is the analysis of result sets from multi-objective evolutionary algorithms.
//
// Copyright (C) 2007  PG500, ISF, University of Dortmund
//      PG500 are: Christoph Begau, Christoph Heuel, Raffael Joliet, Jan Kolanski,
//                 Mandy Kröller, Christian Moritz, Daniel Niggemann, Mathias Stöber,
//                 Timo Stönner, Jan Varwig, Dafan Zhai
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>.
//
// For more information and contact details visit http://pavel.googlecode.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Pavel.Framework;
using System.Resources;

namespace Pavel.GUI.Visualizations {

    /// <summary>
    /// This class creates the visualization as a table and handles 
    /// the interaction of this visualisation
    /// </summary>
    public class Listing : Visualization {

        #region InnerClass ListViewTableToolStrip

        private class ListViewTableToolStrip : VisualizationStandardToolStrip {

            public ToolStripButton jumpToButton;

            public ListViewTableToolStrip(Listing listing)
                : base(listing) {

                jumpToButton = new ToolStripButton(Pavel.Properties.Resources.MoveDown);
                jumpToButton.ToolTipText = "Jump to next selected point";
                jumpToButton.ImageTransparentColor = System.Drawing.Color.Magenta;
                jumpToButton.Click += delegate(object sender, EventArgs e) {
                    listing.JumpToSelectedLine();
                };
                this.Items.Add(jumpToButton);
            }
        }
        #endregion

        #region Fields & Properties
        private CustomListView listView;
        private ListViewTableToolStrip toolStrip;

        public enum ColoringStyles { None, Selection, Value };

        /// <summary>
        /// Flag if cells are colored according to their values, selections or none
        /// </summary>
        private ColoringStyles coloringStyle = ColoringStyles.Value;

        /// <summary>
        /// list is sorted by this column
        /// </summary>
        private int sortColumn = 0;  

        //Caches for used Point and their maps
        private Pavel.Framework.Point[] cacheTable;
        private int[] map;

        private int lastSelectedIndex = -1;

        /// <summary>
        /// After columns have been reordered their index is wrong, because
        /// ListView maps them internally to improve speed
        /// Thus we need the correct index, the listView must be reinitialized
        /// columnsReordered = true ensures a reinit the next time the listView is drawn to screen
        /// </summary>
        private bool columnsReordered = false;

        [ShowInProperties]
        [Category ("Display")]
        [DisplayName("Style")]
        [Description("Enables different visual styles: \n'Value': cells are colored due to their value in relation to column properties.\n'Selection': Using color style like other visualizations.\n'None': no coloring.")]
        public ColoringStyles Style {
            get { return coloringStyle; }
            set {
                coloringStyle = value;
                listView.Refresh();
            }
        }

        #if DEBUG
        [ShowInProperties]
        [Category("Display")]
        [DisplayName("Double Buffering")]
        [Description("Enables double buffering. Double Duffering activated is slower, but reduces flickering")]
        public bool DoubleBuffering {
            get { return listView.DoubleBuffering; }
            set { listView.DoubleBuffering = value; }
        }
        #endif
        public static System.Drawing.Bitmap Icon {
            get { return Pavel.Properties.Resources.TableHS; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Creates an new ListView from the currently set PointSet and Space
        /// </summary>
        /// <param name="vw">The VisualisationWindow with PointSet and Space</param>
        public Listing(VisualizationWindow vw) : base(vw) {
            listView = new CustomListView();
            listView.View = View.Details;
            listView.Dock = DockStyle.Fill;
            listView.MultiSelect = false;
            listView.FullRowSelect = true;
            listView.ShowItemToolTips = true;
            listView.VirtualMode = true;
            listView.AllowColumnReorder = true;
            listView.Font = new Font("Arial", 8);            
            toolStrip = new ListViewTableToolStrip(this);

            this.listView.DoubleBuffering = true;
            this.listView.OwnerDraw = true;
            this.listView.DrawColumnHeader += CustomListView_DrawColumnHeader;
            this.listView.DrawSubItem += CustomListView_DrawSubItem;

            //Event Binding
            this.listView.RetrieveVirtualItem += this.RetrieveVirtualItem;

            this.listView.ColumnClick += this.ReOrder;

            this.listView.ColumnReordered += delegate(object sender, ColumnReorderedEventArgs e) {
                this.VisualizationWindow.Space.MoveColumn(e.OldDisplayIndex, e.NewDisplayIndex);
                if (sortColumn == e.OldDisplayIndex) {
                     sortColumn = e.NewDisplayIndex;
                }
                else if (sortColumn == e.NewDisplayIndex) {
                    sortColumn = e.OldDisplayIndex;
                }
                columnsReordered = true;
            };

            this.listView.MouseClick += MouseClick;

            //Creating list view
            InitDrawing();
        }

        #endregion

        #region Virtual Mode Events (Fills the table)
        /// <summary>
        /// Fills the listView in virtual Mode
        /// is called everytime a item is needed, e contains the index of this item
        /// </summary>
        /// <param name="sender">caller</param>
        /// <param name="e"></param>
        private void RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
            if ( columnsReordered ) { columnsReordered = false; InitDrawing(); }

            //Creating item from values stored in cacheTable
            Pavel.Framework.Point cachePoint = cacheTable[e.ItemIndex];
            ListViewItem item = new ListViewItem(cachePoint[map[0]].ToString());
            item.Tag = cachePoint;
            for(int i=1;i<map.Length;i++) {
                item.SubItems.Add(cachePoint[map[i]].ToString());
            }
            e.Item = item;
        }        


        private void CustomListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e) {
            Color backgroundColor = Color.White;
            Color highLightColor = System.Drawing.SystemColors.Highlight;
            Color textColor = Color.Black;
            Pavel.Framework.Point cachePoint = (Pavel.Framework.Point)e.Item.Tag;

            if ( coloringStyle == ColoringStyles.Value ) {                
                double relValue = cachePoint.ScaledValue(map[e.ColumnIndex],
                    VisualizationWindow.Space.ColumnProperties[e.ColumnIndex]);

                if ( relValue > 1 ) { relValue = 1; }
                if ( relValue < 0 || Double.IsNaN(relValue)
                    || Double.IsInfinity(relValue)) { relValue = 0; }
                if ( relValue > 0.5 ) { backgroundColor = Color.FromArgb(255, (int)(255 - (relValue - 0.5) * 2 * 255), 0); }
                else { backgroundColor = Color.FromArgb((int)((relValue * 2) * 255), 255, 0); }
                if ( ProjectController.CurrentSelection.Contains(cachePoint) ) {
                    backgroundColor = System.Drawing.SystemColors.Highlight;
                    textColor = System.Drawing.SystemColors.HighlightText;
                }
            }
            else if ( coloringStyle == ColoringStyles.Selection ) {
                backgroundColor = ProjectController.GetSelectionColor(cachePoint).ToColor();
            }
            else if ( coloringStyle == ColoringStyles.None ) {
                if ( e.ItemIndex % 2 == 0 ) {
                    backgroundColor = Color.FromArgb(255, 245, 245, 245);
                }
                else backgroundColor = Color.White;
                if ( ProjectController.CurrentSelection.Contains(cachePoint) ) {
                    backgroundColor = System.Drawing.SystemColors.Highlight;
                    textColor = System.Drawing.SystemColors.HighlightText;
                }
            }
            
            e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);

            if ( e.ColumnIndex > 0 ) {
                e.Graphics.DrawString(e.SubItem.Text, listView.Font, new SolidBrush(textColor), e.Bounds);
            }
            else e.Graphics.DrawString(e.Item.Text, listView.Font, new SolidBrush(textColor), e.Bounds);
        }

        private void CustomListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e) {
            e.DrawBackground();
            e.DrawText(TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.Left);
            return;
        }
        #endregion

        #region private Methods

        /// <summary>
        /// Handles the MouseEvents for creating contextmenu and imitating the funtionality of
        /// windows-form list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseClick(object sender, MouseEventArgs e) {
            if ( e.Button == MouseButtons.Right ) {
                ContextMenu cm = CreateContextMenu(e.Location);
                cm.Show(listView, e.Location);
            } else {
                //Handling left-click -> build selections
                if ( listView.SelectedIndices.Count == 0 ) {
                    ProjectController.CurrentSelection.Clear();
                } else {
                    //Store which line is clicked
                    if ( lastSelectedIndex == -1 ) lastSelectedIndex = listView.SelectedIndices[0];
                    Pavel.Framework.Point p = cacheTable[(listView.SelectedIndices[0])];
                    //Ctrl and shift pressed -> Mark a range and add all items in the selection
                    if ( Control.ModifierKeys == (Keys.Control | Keys.Shift) ) {
                        List<Pavel.Framework.Point> points = new List<Pavel.Framework.Point>();
                        if ( listView.SelectedIndices[0] > lastSelectedIndex ) {
                            for ( int i = lastSelectedIndex; i <= listView.SelectedIndices[0]; i++ ) {
                                points.Add(cacheTable[i]);
                            }
                        } else {
                            for ( int i = listView.SelectedIndices[0]; i < lastSelectedIndex; i++ ) {
                                points.Add(cacheTable[i]);
                            }
                        }
                        ProjectController.CurrentSelection.AddRange(points);
                    }
                        //Shift pressed -> Mark a range, clear selection and add all items
                    else if ( Control.ModifierKeys == Keys.Shift ) {
                        List<Pavel.Framework.Point> points = new List<Pavel.Framework.Point>();
                        //Identify if the clicked line is above or below the last clicked line
                        //Mark lines between this lines
                        if ( listView.SelectedIndices[0] > lastSelectedIndex ) {
                            for ( int i = lastSelectedIndex; i <= listView.SelectedIndices[0]; i++ ) {
                                points.Add(cacheTable[i]);
                            }
                        } else {
                            for ( int i = listView.SelectedIndices[0]; i <= lastSelectedIndex; i++ ) {
                                points.Add(cacheTable[i]);
                            }
                        }
                        ProjectController.CurrentSelection.ClearAndAddRange(points);
                    } else if ( Control.ModifierKeys != Keys.Control ) {
                        ProjectController.CurrentSelection.ClearAndAdd(p);
                        if ( Control.ModifierKeys != Keys.Shift ) {
                            lastSelectedIndex = listView.SelectedIndices[0];
                        }
                    } else {
                        if ( !ProjectController.CurrentSelection.Contains(p) ) {
                            ProjectController.CurrentSelection.Add(p);
                        } else ProjectController.CurrentSelection.Remove(p);
                        if ( Control.ModifierKeys != Keys.Shift ) {
                            lastSelectedIndex = listView.SelectedIndices[0];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the parameters for the listView
        /// </summary>
        private void InitDrawing() {
            listView.Columns.Clear();
            //Init ColumnHeaders
            for ( int i = 0; i < VisualizationWindow.Space.Dimension; i++ ) {
                ColumnHeader newCol = new ColumnHeader();
                newCol.Tag = VisualizationWindow.Space.ColumnProperties[i];
                if ( VisualizationWindow.Space.ColumnProperties[i].IsAscendingOrder() ) {
                    newCol.Text = "\u25BC" + VisualizationWindow.Space.ColumnProperties[i].Label;
                }
                else { newCol.Text = "\u25B2" + VisualizationWindow.Space.ColumnProperties[i].Label; }
                listView.Columns.Add(newCol);
            }

            //Workaround for unindentifiable error: If so many items are deleted that all items cannot fill the listing
            //an ArgumentOutOfRange-Exception is thrown. But if this exception is ignored, everything is fine.
            //According to the .net-documentation the ListView.VirtualListSize-Property cannot throw this exception
            try {
                listView.VirtualListSize = VisualizationWindow.DisplayedPointSet.Length;
            }
            catch ( ArgumentOutOfRangeException ) { }

            //Creating cacheTable
            cacheTable = new Pavel.Framework.Point[VisualizationWindow.DisplayedPointSet.Length];
            map = VisualizationWindow.Space.CalculateMap(VisualizationWindow.DisplayedPointSet.ColumnSet);
            for ( int j = 0; j < VisualizationWindow.DisplayedPointSet.Length; j++ ) {
                Pavel.Framework.Point point = VisualizationWindow.DisplayedPointSet[j];
                cacheTable[j] = VisualizationWindow.DisplayedPointSet[j];
            }
            Array.Sort(cacheTable, OrderByValue);
            listView.Columns[sortColumn].Text = "* " + listView.Columns[sortColumn].Text;
        }

        /// <summary>
        /// Sorts the point after a given Column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">eventArgs contain the ColumnIndex</param>
        private void ReOrder(object sender, ColumnClickEventArgs e) {
            if (sortColumn != -1) {
                if ( VisualizationWindow.Space.ColumnProperties[sortColumn].IsAscendingOrder() ) {
                    listView.Columns[sortColumn].Text = "\u25BC" + VisualizationWindow.Space.ColumnProperties[sortColumn].Label;
                }
                else { listView.Columns[sortColumn].Text = "\u25B2" + VisualizationWindow.Space.ColumnProperties[sortColumn].Label; }
                sortColumn = e.Column;
                listView.Columns[e.Column].Text = "* " + listView.Columns[e.Column].Text;
                Array.Sort(cacheTable, OrderByValue);
                if ( !VisualizationWindow.Space.ColumnProperties[sortColumn].IsAscendingOrder() ) {
                    Array.Reverse(cacheTable);
                }
                listView.Refresh();
            }            
        }

        /// <summary>
        /// Comparer for ordering after a specific column (ascending order)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int OrderByValue(Pavel.Framework.Point a, Pavel.Framework.Point b) {
            double valueA = a[map[sortColumn]];
            double valueB = b[map[sortColumn]];
            if ( valueA == valueB ) return 0;
            if ( valueA < valueB ) return -1;
            else return 1;
        }

        /// <summary>
        /// Scroll the listView to the next previously visible selected line
        /// </summary>
        public void JumpToSelectedLine() {
            
            //No selection -> Nothing to do
            if ( ProjectController.CurrentSelection.Length == 0 ) { return; }

            int lastItemIndex = -1;
            //Indentify last visible line
            if ( VisualizationWindow.DisplayedPointSet.Length != 0 ) {   //PointSet may be empty
                lastItemIndex = cacheTable.Length;
                if ( listView.GetItemAt(1, listView.ClientRectangle.Bottom - 1) != null ) {
                    lastItemIndex = listView.GetItemAt(1, listView.ClientRectangle.Bottom - 1).Index;
                    if ( lastItemIndex != cacheTable.Length ) { lastItemIndex++; }
                }
            }
            int showline = lastItemIndex+1;
            //if last visible line is the last item in the table start at index 0 to
            //find next selected line
            if (lastItemIndex == cacheTable.Length-1) { showline = 0; }
            if ( lastItemIndex != -1 ) {
                while ( showline<cacheTable.Length-1 && 
                    !ProjectController.CurrentSelection.Contains(cacheTable[showline]) ) {
                    showline++;
                }
                // if no selected line was found under the last item, restart at the top of the list
                if ( showline > cacheTable.Length ) {
                    showline = 0;
                    while ( showline != lastItemIndex &&
                    !ProjectController.CurrentSelection.Contains(cacheTable[showline]) ) {
                        showline++;
                    }
                }
                // if any not visible line was found, show it
                if ( showline != lastItemIndex ) {
                    listView.EnsureVisible(showline);
                }
            }
        }

        /// <summary>
        /// Resets all settings shown in PropertyWindow
        /// </summary>
        private void ResetProperties( ) {
        }
     
        #endregion

        # region public accessors
        /// <summary>
        /// Returns the ToolStrip of the ListViewTable
        /// </summary>
        public override VisualizationStandardToolStrip ToolStrip {
            get { return toolStrip; }
        }

        /// <summary>
        /// Return the Control of the ListViewTable
        /// </summary>
        public override Control Control {
            get { return listView; }
        }

        /// <summary>
        /// Forces the ListView to redrawn, because the colors was changed
        /// </summary>
        public override void UpdateColors() {
            sortColumn = 0;
            InitDrawing();
        }

        /// <summary>
        /// Forces the ListView to redrawn, because the displayed space was changed
        /// </summary>
        public override void UpdateSpace() {
            sortColumn = 0;
            InitDrawing();
        }

        #endregion

        #region Selection-EventHandler

        /// Updates the selection in the visible part of the listView, if the listView has not the focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void SelectionModified(object sender, EventArgs e) {
            listView.Refresh();
        }

        /// <summary>
        /// Handling changes in the displayed PointSet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void PointSetModified(object sender, EventArgs e) {
            base.PointSetModified(sender, e);
            InitDrawing();
        }

        #endregion

        /// <summary>
        /// Save the visible part of the list to an image, 
        /// opens a dialog for selecting format & filename
        /// </summary>
        public override Bitmap Screenshot() {
            int w = this.Control.Width;
            int h = this.Control.Height;
            Bitmap bitmap = new Bitmap(w, h);
            this.Control.DrawToBitmap(bitmap,Rectangle.FromLTRB(0, 0, w, h));
            return bitmap;
        }

        #region Context menu creation

        /// <summary>
        /// Creating a context menu to manipulate the columns
        /// </summary>
        /// <param name="point">Position of the cursor, while clicking</param>
        /// <returns></returns>
        private ContextMenu CreateContextMenu(System.Drawing.Point point) {
            //if a (sub)item is clicked get the corresponding index
            ListViewItem item = listView.HitTest(point.X, point.Y).Item;
            int index = 0;
            if ( item.GetSubItemAt(point.X, point.Y) != null ) {
                index = item.SubItems.IndexOf(item.GetSubItemAt(point.X, point.Y));
            }

            //Create a context menu
            ContextMenu cm = new ContextMenu();
            MenuItem menuItem;
            if ( listView.Columns.Count > 2 ) {
                menuItem = new MenuItem("Delete Column");
                menuItem.Click += delegate(object sender, EventArgs e) {
                    this.VisualizationWindow.Space.RemoveColumn(index);
                    if ( sortColumn == index ) { sortColumn = 0; }
                    //if the list is sorted by a column behind the column to be
                    //deleted, the sort column moves one position
                    if ( sortColumn > index ) { sortColumn--; }
                    this.InitDrawing();
                };
                cm.MenuItems.Add(menuItem);
            }

            menuItem = new MenuItem("Change sorting orientation");
            menuItem.Click += delegate(object sender, EventArgs e) {
                this.VisualizationWindow.Space.ColumnProperties[index].SwitchOrientation();
                if ( sortColumn == index ) { ReOrder(this, new ColumnClickEventArgs(index)); }
                else listView.Refresh();
            };
            cm.MenuItems.Add(menuItem);

            menuItem = new MenuItem("Edit Column property");
            menuItem.Click += delegate(object sender, EventArgs e) {
                ColumnPropertyDialog cpn = new ColumnPropertyDialog(this.VisualizationWindow.Space.ColumnProperties[index]);
                cpn.Location = Cursor.Position;
                if ( cpn.ShowDialog() == DialogResult.OK ) {
                    listView.Refresh();
                }
            };
            cm.MenuItems.Add(menuItem);

            menuItem = new MenuItem("Add Column...");
            foreach ( Column col in VisualizationWindow.PointSet.ColumnSet.Columns ) {
                MenuItem mi = new MenuItem(col.Label);
                mi.Tag = col;
                mi.Click += delegate(object sender, EventArgs e) {
                    this.VisualizationWindow.Space.InsertColumn((Column)mi.Tag, index);
                    this.UpdateSpace();
                };
                menuItem.MenuItems.Add(mi);
            }
            cm.MenuItems.Add(menuItem);

            return cm;
        }

        #endregion

        private class CustomListView: ListView {
            public bool DoubleBuffering{
                get { return this.DoubleBuffered; }
                set { this.DoubleBuffered = value; }
            }
            //Override to make sure selection cannot modified by Keys
            //TODO: Implement Selection by cursor-keys
            protected override void OnKeyPress(KeyPressEventArgs e) {}
            protected override void OnKeyDown(KeyEventArgs e) {}
            protected override void OnKeyUp(KeyEventArgs e) {}
        }
    }
}