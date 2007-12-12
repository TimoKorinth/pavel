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
using System.IO;
using System.Reflection;
using Pavel.Clustering;
using Pavel.GUI;

namespace Pavel.Framework {
    /// <summary>
    /// Manages the plugins for Pavel.
    /// </summary>
    public class PluginManager {

        #region Fields

        private List<IUseCase>   availableUseCases = new List<IUseCase>();
        private List<ClusteringAlgorithm> availableClusteringAlgorithms = new List<ClusteringAlgorithm>();

        #endregion

        #region Properties

        /// <value> Gets the list of available useCases </value>
        public List<IUseCase>  AvailableUseCases  { get { return new List<IUseCase>(availableUseCases);   } }
        /// <value> Gets the list of available ClusteringAlgorithms </value>
        public List<ClusteringAlgorithm> AvailableClusteringAlgorithms {
            get { return new List<ClusteringAlgorithm>(availableClusteringAlgorithms); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a PluginManager.
        /// </summary>
        /// <param name="pluginPath">The path where plugins are searched for</param>
        public PluginManager(String pluginPath) {
            RegisterInternalPlugins();
            RegisterExternalPlugins(pluginPath);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Looks for DLLs in the pluginPath, scans them for valid plugins and registers
        /// these in the manager.
        /// </summary>
        /// <param name="pluginPath">Path to the PlugIns</param>
        private void RegisterExternalPlugins(String pluginPath) {
            if (null == pluginPath)
                throw new ArgumentNullException(pluginPath, "Parameter is null.");
            if (Directory.Exists(pluginPath)) {
                foreach (string pluginFilename in Directory.GetFiles(pluginPath, "*.dll")) {
                    try {
                        Assembly pluginAssembly = Assembly.LoadFrom(pluginFilename);
                        RegisterPluginsFrom(pluginAssembly);
                    } catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// Looks for valid plugins in an assembly and inserts them into
        /// the lists of available plugins
        /// </summary>
        /// <param name="pluginAssembly">Assembly that contains PlugIns</param>
        private void RegisterPluginsFrom(Assembly pluginAssembly) {
            foreach (Type type in pluginAssembly.GetTypes() ) {
                if ( type.IsPublic && !type.IsAbstract ) {
                    foreach (Type interFace in type.GetInterfaces()) {
                        if (interFace == typeof(IUseCase))
                            availableUseCases.Add( Activator.CreateInstance(type) as IUseCase );
                    }
                    if (type.IsSubclassOf(typeof(ClusteringAlgorithm))) {
                        availableClusteringAlgorithms.Add(Activator.CreateInstance(type) as ClusteringAlgorithm);
                    }
                }
            }
        }

        /// <summary>
        /// Registers the plugins internally stored in Pavel
        /// </summary>
        private void RegisterInternalPlugins() {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
                if (type.IsPublic && !type.IsAbstract) {
                    foreach (Type interFace in type.GetInterfaces()) {
                        if (interFace == typeof(IUseCase))
                            availableUseCases.Add(Activator.CreateInstance(type) as IUseCase);
                    }
                    if (type.IsSubclassOf(typeof(ClusteringAlgorithm))) {
                        availableClusteringAlgorithms.Add(Activator.CreateInstance(type) as ClusteringAlgorithm);
                    }
                }
            }
        }

        #endregion
    }
}
