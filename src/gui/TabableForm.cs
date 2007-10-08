using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Pavel.GUI {
    /// <summary>
    /// Extends a form with a tabPage to be show in a tab controller of the mdiForm
    /// </summary>
    public abstract class TabableForm : Form {
        protected TabControl tabCtrl;
        protected TabPage tabPag = new TabPage();

        /// <summary>
        /// Gets the tabPage of this Form
        /// </summary>
        public TabPage TabPag {
            get { return tabPag; }
        }

        /// <summary>
        /// Initialize a tabPage to show this Form in a tabController
        /// </summary>
        /// <param name="tabCtrl"></param>
        public void EnableTab(TabControl tabCtrl) {
            this.tabCtrl = tabCtrl;
            tabPag.Parent = tabCtrl;
            tabPag.Show();
            this.Activated += TabableForm_Activated;
            this.FormClosing += TabableForm_Closing;
        }

        private void TabableForm_Closing(object sender, EventArgs e) {
            //Destroy the corresponding Tabpage when closing MDI child form
            this.tabPag.Dispose();

            //If no Tabpage left
            if ( !tabCtrl.HasChildren ) {
                tabCtrl.Visible = false;
            }
        }

        private void TabableForm_Activated(object sender, System.EventArgs e) {
            //Activate the corresponding Tabpage
            tabCtrl.SelectedTab = tabPag;

            if ( !tabCtrl.Visible ) {
                tabCtrl.Visible = true;
            }
        }
    }
}
