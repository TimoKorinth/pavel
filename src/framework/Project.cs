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

namespace Pavel.Framework {
    [Serializable()]

    ///<summary>
    ///Container for all information to be saved in a Pavel-Project
    ///</summary>
    [CoverageExclude]
    public class Project {

        #region Fields

        /// <value> Name of the project. It is also the name of the project file (include path). </value>
        [NonSerialized]
        private String name;

        /// <value> List of PointSets in this Project </value>
        public List<PointSet> pointSets = new List<PointSet>();
        /// <value> List of Columns in this Project </value>
        public List<Column> columns = new List<Column>();
        /// <value> List of PointLists in this Project </value>
        public List<Space> spaces = new List<Space>();

        //[NonSerialized]
        private IUseCase useCase;

        /// <value> It is set if the project is changed. </value>
        [NonSerialized]
        private bool changed;


        #endregion

        #region Properties
        /// <value> Gets the name of project or sets it </value>
        public String Name {
            get { return name; }
            set { name = value; }
        }

        /// <value> Gets or sets the current UseCase </value>
        public IUseCase UseCase {
            get { return useCase; }
            set { useCase = value; }
        }

        /// <value>Gets or sets the changed state of the project</value>
        public bool Changed {
            get { return changed; }
            set { changed = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Standard constructor with useCase = null.
        /// </summary>
        public Project() {
            name = null;
            this.useCase = null;
            changed = false;
        }

        /// <summary>
        /// Constructor that sets the useCase to the given value.
        /// </summary>
        /// <param name="useCase">The useCase for this Project. Must implement IUseCase.</param>
        public Project(IUseCase useCase) {
            name = null;
            this.useCase = useCase;
            changed = false;
        }

        #endregion
    }
}
