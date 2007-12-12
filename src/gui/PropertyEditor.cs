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
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Pavel.Framework;
using Pavel.GUI.Visualizations;
using System.Reflection;

namespace Pavel.GUI {    

    #region AxesEditor

    /// <summary>
    /// A DropDown ListBox displaying the Axes in the PropertyControl.
    /// </summary>
    class AxesEditor : UITypeEditor {
        /// <summary>
        /// Handles the selection of Axes in the ListBox.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="provider">The service provider</param>
        /// <param name="value">Value</param>
        /// <returns>The selected item</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            ColumnProperty currentCP = (context as System.Windows.Forms.GridItem).Value as ColumnProperty;
            Space      space        = ((Visualization)context.Instance).VisualizationWindow.Space;
            ListBox        listBox   = new ListBox();
            listBox.Items.Add("(none)");
            listBox.Items.AddRange(space.ColumnProperties);
            listBox.SelectedItem = currentCP;
            IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            listBox.SelectedValueChanged += delegate(object sender, EventArgs e) { editorService.CloseDropDown(); };
            editorService.DropDownControl(listBox);
            return listBox.SelectedItem as ColumnProperty;
        }

        /// <summary>
        /// Returns the EditStyle.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>UITypeEditorEditStyle.DropDown</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    #endregion

    #region RangeAttribute

    /// <summary>
    /// Attribute to set a range of values for PropertySliderEditor
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute :Attribute {

        #region Fields

        private int min;
        private int max;

        #endregion

        #region Properties

        /// <value> Gets the minimum </value>
        public int Min {
            get { return min; }
        }

        /// <value> Gets the maximum </value>
        public int Max {
            get { return max; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Sets the minimum and maximum
        /// </summary>
        /// <param name="min">The minimum</param>
        /// <param name="max">The maximum</param>
        public RangeAttribute(int min, int max) {
            this.min = min;
            this.max = max;
        }

        #endregion
    }

    #endregion

    #region PropertySliderEditor

    /// <summary>
    /// A slider for integer values in the PropertyControl.
    /// </summary>
    public class PropertySliderEditor : UITypeEditor {

        #region Fields

        private int min;
        private int max;

        #endregion

        /// <summary>
        /// If the value is outside the limits it is be set to the nearest bound.
        /// </summary>
        /// <param name="value">The entered value</param>
        /// <param name="min">The minimum</param>
        /// <param name="max">The maximum</param>
        /// <returns>The value that was actually set</returns>
        public static int BoundValue(int value, int min, int max) {
            if ( value < min ) {
                return min;
            }
            else if ( value > max ) {
                return max;
            }
            return value;
        }

        /// <summary>
        /// Handles the changes to the value of the PropertySlider.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="provider">The service provider</param>
        /// <param name="value">The value</param>
        /// <returns>The value that was actually set</returns>
        public override object EditValue(ITypeDescriptorContext context,
            IServiceProvider provider, object value) {
            if ( context == null || provider == null || context.Instance == null )
                return base.EditValue(provider, value);
            //Read the RangeAttribute
            if (context.PropertyDescriptor.Attributes[typeof(RangeAttribute)] != null){
                RangeAttribute rangeAtt = (RangeAttribute)context.PropertyDescriptor.Attributes[typeof(RangeAttribute)];
                this.min = rangeAtt.Min;
                this.max = rangeAtt.Max;
            }

            IWindowsFormsEditorService wfes = provider.GetService(
                typeof(IWindowsFormsEditorService)) as
                IWindowsFormsEditorService;        

            if (wfes != null) {
                PropertySliderForm propertySliderForm = new PropertySliderForm((int)value,min,max);

                propertySliderForm.wfes = wfes;

                wfes.DropDownControl(propertySliderForm);
                value = propertySliderForm.slider.Value;

            }
            return value;
        }

        /// <summary>
        /// Returns UITypeEditorEditStyle.DropDown.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>UITypeEditorEditStyle.DropDown</returns>
        public override UITypeEditorEditStyle GetEditStyle(
            ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    #endregion
}
