// Part of STLNormalSwitcher: A program to switch normal vectors in STL-files
//
// Copyright (C) 2007  PG500, ISF, University of Dortmund
//      PG500 are: Christoph Begau, Christoph Heuel, Raffael Joliet, Jan Kolanski,
//                 Mandy Kröller, Christian Moritz, Daniel Niggemann, Mathias Stöber,
//                 Timo Stönner, Jan Varwig, Dafan Zhai
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
// The licence can also be found at: http://www.gnu.org/licenses/old-licenses/gpl-2.0.txt
//
// For more information and contact details look at STLNormalSwitchers website:
//      http://normalswitcher.sourceforge.net/
//
// Check out PAVEl (http://pavel.sourceforge.net/) another great program brought to you by PG500.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;


namespace STLNormalSwitcher {
    /// <summary>
    /// The Form displayed, when choosing Help -> Info,
    /// showing a logo and the information from AssemblyInfo.
    /// </summary>
    partial class About : Form {

        #region Properties

        /// <value> Gets the AssemblyTitle </value>
        public string AssemblyTitle {
            get {
                // Get all title-attributes in this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // At least one title-attribute exists
                if (attributes.Length > 0) {
                    // Choose first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // Return title, if it isn't empty
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there is no title-attribute or the title is empty, return the exe-name
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <value> Gets the AssemblyVersion </value>
        public string AssemblyVersion {
            get {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        /// <value> Gets the AssemblyDescription </value>
        public string AssemblyDescription {
            get {
                // Get all description-attributes in this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // Return an empty string, if there is no description-attribute
                if (attributes.Length == 0)
                    return "";
                // Return the value of the description-attribute, if it exists
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        /// <value> Gets the AssemblyProduct </value>
        public string AssemblyProduct {
            get {
                // Get all product-attributes in this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // Return an empty string, if there is no product-attribute
                if (attributes.Length == 0)
                    return "";
                // Return the value of the product-attribute, if it exists
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        /// <value> Gets the AssemblyCopyright </value>
        public string AssemblyCopyright {
            get {
                // Get all copyright-attributes in this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // Return an empty string, if there is no copyright-attribute
                if (attributes.Length == 0)
                    return "";
                // Return the value of the copyright-attribute, if it exists
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <value> Gets the AssemblyCompany </value>
        public string AssemblyCompany {
            get {
                // Get all company-attributes in this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // Return an empty string, if there is no company-attribute
                if (attributes.Length == 0)
                    return "";
                // Return the value of the company-attribute, if it exists
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Fills the Form with the information from AssemblyInfo.
        /// </summary>
        public About() {
            InitializeComponent();

            // Initialize AboutBox to display productinformations from the Assemblyinformation.
            // Change the Assemblyinformation in one of the following ways:
            //  - Projekt->Eigenschaften->Anwendung->Assemblyinformationen
            //  - AssemblyInfo.cs
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version: {0}", AssemblyVersion);
            this.labelCopyright.Text = String.Format("Copyright: {0}", AssemblyCopyright);
            this.labelCompanyName.Text = String.Format("Company: {0}", AssemblyCompany);
            this.textBoxDescription.Text = AssemblyDescription;
        }

        #endregion

    }
}
