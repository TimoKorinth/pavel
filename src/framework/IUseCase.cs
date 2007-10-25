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
// For more information and contact details visit http://www.sourceforge.net/projects/pavel

using System;
using System.Collections.Generic;
using System.Text;
using Pavel.GUI;
using Pavel.GUI.Visualizations;
using Pavel.GUI.SolutionVisualizations;

namespace Pavel.Framework {
    /// <summary>
    /// Used to describe Use Cases consisting of a label, a parser,
    /// a solution-instance and a ProjectStarterPageHandler.
    /// </summary>
    /// <remarks>
    /// To create a Plugin DLL with a UseCase, implement IUseCase in the DLL-Assembly.
    /// The class implementing IUseCase should not contain any fields and remain static in
    /// nature (It's not possible in C# to extend static classes, hence this restriction of
    /// making the class quasi-static). Upon loading the assembly, the PluginManager instantiates
    /// the IUseCase implementation and adds this instance to the list of available UseCases.
    /// 
    /// The class implementing IUseCase has to have the Serializable attribute, so projects using
    /// this UseCase can be saved and restored.
    /// </remarks>
    public interface IUseCase {

        #region Properties

        /// <value>Gets a label for the plugin</value>
        String Label { get; }

        /// <value>Gets a new solution-instance for this UseCase</value>
        Solution SolutionInstance { get; }

        /// <value>Gets a ProjectStarterPageHandler instance for this UseCase</value>
        ProjectStarterPageHandler ProjectStarterPages { get; }

        /// <value>Gets the minimum columnSet a solution must have to be visualized or sets it</value>
        /// <remarks>Return null if you don't want any filtering options</remarks>
        ColumnSet SolutionColumnSet { get; set;}

        #endregion

        #region Methods

        /// <summary>
        /// Returns a String describing this UseCase.
        /// </summary>
        /// <returns>A String describing this UseCase</returns>
        String ToString();

        #endregion
    }
}
