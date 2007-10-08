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

using Node = Pavel.Clustering.HierarchicalClusterList.Node;
using Pavel.Framework;

namespace Pavel.Clustering {
    /// <summary>
    /// Calculates a Clustering by an extended HACM algorithm. Depending on the
    /// Inter-Cluster distance, nodes will be combined until one node remains. This
    /// dendogramm can than be used to call Clusterings on several stages of chunkiness
    /// </summary>
    [Serializable]
    public class ExtendedHACM : ClusteringAlgorithm {

        #region Fields

        // Specific HACM-Clustering-Arguments
        private int mode = (int)MODE.MODE_SINGLE_LINK;
        private int defaultClusterCount = 10;
        private int randomSeed = (int)DateTime.Now.Ticks;
        private int numberOfDistances = 5000;
        private bool shortcut = false;

        /// <value>Enumeration for single-link and complete-link mode </value>
        public enum MODE { MODE_SINGLE_LINK, MODE_COMPLETE_LINK, MODE_AVERAGE_LINK }

        #endregion

        #region Properties

        /// <value>Gets the clustering mode (single-link, complete-link or average-lnk) or set it</value>
        [ComboBox("Cluster-Distance-Mode",
            "The distancefunction that determines the interclusterdistance",
            new string[] { "Single-Link", "Complete-Link", "Average-Link" })]
        public int Mode {
            get { return mode; }
            set { mode = value; }
        }

        /// <value>Gets the default number of clusters or set it</value>
        [Spinner("Default number of clusters",
            "Sets the default Distance to a value that leads to the given number of clusters",
            1, int.MaxValue, 0, 1)]
        public int DefaultClusterCount {
            get { return defaultClusterCount; }
            set { defaultClusterCount = value; }
        }

        /// <value>Gets the random seed or set it</value>
        [Spinner("Random Seed",
                "Initializes the Clustering-Algorithm-Random-Generator. Use for repeatable behaviour",
                int.MinValue, int.MaxValue, 0, 1)]
        public int RandomSeed {
            get { return randomSeed; }
            set { randomSeed = value; }
        }

        /// <value>Gets the maximum number of distances or set it</value>
        [Spinner("Maximum number of Distances",
            "Limits the number of distances in the distance matrix for faster computation",
            1, int.MaxValue, 0, 1)]
        public int NumberOfDistances {
            get { return numberOfDistances; }
            set { numberOfDistances = value; }
        }

        [CheckBox("Cut short clustering",
            "Cut Short Clustering if default number of clusters is reached. Results in useless Clusterings for chunkier Views on hierarchical Clustering. For default number of clusters quality is guaranteed")]
        public bool Shortcut {
            get { return shortcut; }
            set { shortcut = value; }
        }

        #endregion

        #region Methods

        #region DoClustering

        /// <summary>
        /// Does the Clustering and creates a HierarchicalClusterList
        /// </summary>
        /// <returns>A PointList</returns>
        protected override Pavel.Framework.PointList DoClustering() {
            #region Initializiation
            // Test for validity
            if (PointSet.Length < 2) {
                ErrorMessage = "No Hierarchical Clustering of less than two Points";
                return null;
            }

            // Determin minimum of the complete matrix and the upper bound for distances
            int maxDistances = MatrixSize(PointSet.Length);
            if (NumberOfDistances < maxDistances)
                maxDistances = NumberOfDistances;
            
            // Init Random
            Random random = new Random(RandomSeed);
            
            
            #endregion

            #region Each Point in a seperate Cluster
            // Nodes of clusters with indices of the contained Points
            Node[] clusters;
            try {
                clusters = new Node[PointSet.Length];
                int vIndex = 0;
                for (int plIndex = 0; plIndex < PointSet.PointLists.Count; plIndex++) {
                    int[] superColumnSetMap = ColumnSet.SuperSetMap(PointSet.PointLists[plIndex].ColumnSet);
                    for (int index = 0; index < PointSet.PointLists[plIndex].Count; index++) {
                        // Init Cluster
                        Cluster cluster = new Cluster("Base-Cluster " + vIndex,
                            PointSet.PointLists[plIndex][index].Trim(ColumnSet, superColumnSetMap));
                        // One PointSet to each cluster
                        cluster.PointSet = new PointSet("", PointSet.ColumnSet);
                        // Create PointList-Structure for each PointSet
                        for (int listIndex = 0; listIndex < PointSet.PointLists.Count; listIndex++) {
                            cluster.PointSet.Add(new PointList(PointSet.PointLists[listIndex].ColumnSet));
                        }
                        // Add single point to cluster
                        cluster.PointSet.PointLists[plIndex].Add(PointSet.PointLists[plIndex][index]);
                        // Add cluster to dendogrammLine
                        clusters[vIndex] = new Node(cluster, int.MaxValue, null, null, new int[] { vIndex });
                        vIndex++;
                    }
                }
            } catch {
                ErrorMessage = "Not enought Memory for Clustering yet!";
                System.GC.Collect();
                return null;
            }

            #endregion

            #region Calculate initial distances if all distances will be used
            int count = 0;
            // Sorted distance-matrix used for combination of clusters
            ISortedMatrix<float> matrix;
            try {
                matrix = new SortedMatrix<float>(PointSet.Length, maxDistances);
                // Fill only if it will be filled up
                if (maxDistances != NumberOfDistances) {
                    for (int i = 0; i < PointSet.Length; i++) {
                        for (int j = i + 1; j < PointSet.Length; j++) {
                            // Compute distance
                            float distance = (float)Point.Distance(ScaledFlatData[i], ScaledFlatData[j]);
                            matrix.Set(j, i, distance);
                            count++;
                            if (count >= maxDistances) {
                                // Maximum exceeded
                                goto Next;
                            }
                            if (count % 100 == 0) { SignalProgress(0, "Calculating Distances: " + count + "/" + maxDistances); }
                        }
                    }
                }
            } catch (OutOfMemoryException) {
                ErrorMessage = "Not enought Memory for all Distances! Try to reduce the number of distances";
                System.GC.Collect();
                return null;
            }

            #endregion

        Next:

            #region Create dendogramm (Clustering)
            // Number of Nodes remaining
            int nodes = PointSet.Length;
            Node newNode;
            do {
                #region Refill matrix
                // Fill matrix up the the given limit (NumberOfDistances)
                try {
                    int size = MatrixSize(nodes);
                    while (matrix.Count < size && matrix.Count < maxDistances) {
                        int i;
                        int j;

                        do {
                            do {
                                i = random.Next(1, clusters.Length);
                            } while (clusters[i] == null);
                            do {
                                j = random.Next(0, i);
                            } while (clusters[j] == null);
                        } while (matrix.Exist(i, j));
                        // Empty position for valid Clusters found
                        float distance = (float)Point.Distance(ScaledFlatData[i], ScaledFlatData[j]);
                        matrix.Set(i, j, distance);
                        SignalProgress(0, "Refill matrix: " + matrix.Count + "/" + maxDistances);
                    }
                } catch (OutOfMemoryException) {
                    ErrorMessage = "Not enought Memory for all Distances! Try to reduce the number of distances";
                    System.GC.Collect();
                    return null;
                }

                #endregion

                SignalProgress((int)((1 - ((float)nodes / (float)PointSet.Length)) * 1000f), "Creating Dendogramm: "
                    + (PointSet.Length - nodes) + "/" + PointSet.Length);

                // Indices of the Clusters with minimal Inter-Cluster-distance
                int first = matrix.Minimum.Row;
                int second = matrix.Minimum.Column;
                // Merge Nodes (Clusters) with minimum distance
                newNode = HierarchicalClusterList.MergeNodes(clusters[first], clusters[second], nodes--);
                // Update second Node
                clusters[second] = newNode;
                // Remove first Node
                clusters[first] = null;
                
                // Run Column and update distances
                foreach (MatrixEntry<float> entry in matrix.Column(second)) {
                    MatrixEntry<float> dist;
                    // Depending on the situation, mapping is needed to use only the lower
                    // triangle matrix
                    if (first > entry.Row) {
                        dist = matrix.ElementAt(first, entry.Row);
                    } else if (first < entry.Row) {
                        dist = matrix.ElementAt(entry.Row, first);
                    } else {
                        continue;
                    }
                    
                    // If reference-node is found, use it to speed up computation
                    if (dist != null) {
                        if (Mode == (int)MODE.MODE_SINGLE_LINK) {
                            if (dist.Item < entry.Item) {
                                matrix.Update(entry, dist.Item);
                            }
                        } else if (Mode == (int)MODE.MODE_COMPLETE_LINK) {
                            if (dist.Item > entry.Item) {
                                matrix.Update(entry, dist.Item);
                            }
                        }
                        else if (Mode == (int)MODE.MODE_AVERAGE_LINK) {
                            matrix.Update(entry, Distance(clusters[entry.Row],
                                clusters[entry.Column], ScaledFlatData, Mode));
                        } else {
                            ErrorMessage = "Invalid Clustering Mode.";
                            return null;
                        }
                    } else {
                        // Compute Distance
                        matrix.Update(entry,
                            Distance(clusters[entry.Row], clusters[entry.Column], ScaledFlatData, Mode));
                    }

                }

                // Run Row and update Distances
                foreach (MatrixEntry<float> entry in matrix.Row(second)) {
                    MatrixEntry<float> dist;
                    // Depending on the situation, mapping is needed to use only the lower
                    // triangle matrix
                    if (first > entry.Column) {
                        dist = matrix.ElementAt(first, entry.Column);
                    } else if (first < entry.Column) {
                        dist = matrix.ElementAt(entry.Column, first);
                    } else {
                        continue;
                    }

                    // If reference-node is found, use it to speed up computation
                    if (dist != null) {
                        if (Mode == (int)MODE.MODE_SINGLE_LINK) {
                            if (dist.Item < entry.Item) {
                                matrix.Update(entry, dist.Item);
                            }
                        } else if (Mode == (int)MODE.MODE_COMPLETE_LINK) {
                            if (dist.Item > entry.Item) {
                                matrix.Update(entry, dist.Item);
                            }
                        } else if (Mode == (int)MODE.MODE_AVERAGE_LINK) {
                            matrix.Update(entry, Distance(clusters[entry.Row],
                                clusters[entry.Column], ScaledFlatData, Mode));
                        } else {
                            ErrorMessage = "Invalid Clustering Mode.";
                            return null;
                        }
                    } else {
                        // Compute Distance
                        matrix.Update(entry,
                            Distance(clusters[entry.Row], clusters[entry.Column], ScaledFlatData, Mode));
                    }
                }

                // Delete Row
                matrix.RemoveRow(first);
                // Delete Col
                matrix.RemoveColumn(first);
                
                // Exit if dendogramm is finished OR if user asks to abort OR cut short is used
            } while (matrix.Count > 0 && !SaveAbortRequested && !(Shortcut && (nodes == DefaultClusterCount)));

            #endregion

            #region Finish dendogramm

            // Finish dendogramm without taking care of distances.
            try {
                if (SaveAbortRequested || (Shortcut && (nodes > 1))) {
                    // Close Nodes to root
                    SignalProgress(0, "Finishing work... ");
                    // Find first
                    int first = 0;
                    while (clusters[first] == null) { first++; }

                    // Speed up computation by ignoring PointIndices
                    clusters[first].PointIndices = null;
                    int mergedNodes = 0;
                    for (int i = first + 1; i < clusters.Length; i++) {
                        if (clusters[i] != null) {
                            clusters[first] = HierarchicalClusterList.MergeNodes(clusters[first], clusters[i], count--);
                            mergedNodes++;
                            if (mergedNodes % 200 == 0) {
                                //SignalProgress((int)(((float)mergedNodes / (float)nodes) * 1000f), "Finishing work... (Remaining: " + (nodes - mergedNodes) + " nodes to merge)");
                            }
                        }
                        
                    }
                    int split = ((nodes < DefaultClusterCount) ? DefaultClusterCount : nodes);
                    return new HierarchicalClusterList(ColumnSet, clusters[first], split);
                } else {
                    return new HierarchicalClusterList(ColumnSet, newNode, DefaultClusterCount);
                }
            } catch (OutOfMemoryException) {
                ErrorMessage = "Not enought Memory! Try to reduce the number of distances";
                System.GC.Collect();
                return null;
            }
            #endregion
        }

        #endregion

        #region Helper

        /// <summary>
        /// Calculates the sum of the values from one to "<paramref name="dimension"/> minus one".
        /// </summary>
        /// <param name="dimension">A value to calculate that sum for</param>
        /// <returns>The sum of the values from one to "<paramref name="dimension"/> minus one"</returns>
        private int MatrixSize(int dimension) {
            long result = ((long)dimension * (long)dimension - (long)dimension) / 2L;
            if (result > int.MaxValue) {
                return int.MaxValue;
            } else {
                return (int)result;
            }
        }

        /// <summary>
        /// Determines the distance between two nodes (their clusters)
        /// </summary>
        /// <param name="nodeA">First node</param>
        /// <param name="nodeB">Second node</param>
        /// <param name="precomputedValues">Array of scaled values to speedup computation</param>
        /// <param name="mode">Mode for cluster-distance</param>
        /// <returns>float-value of cluster-distance</returns>
        public static float Distance(Node nodeA, Node nodeB, double[][] precomputedValues, int mode) {
            double distance = 0.0, newdistance;
            int[] indicesA = nodeA.PointIndices;
            int countA = indicesA.Length;
            int[] indicesB = nodeB.PointIndices;
            int countB = indicesB.Length;
                
            if (mode == (int)MODE.MODE_SINGLE_LINK) {
                distance = double.PositiveInfinity;
                for (int i = 0; i < countA; i++) {
                    for (int j = 0; j < countB; j++) {
                        newdistance = Point.Distance(
                            precomputedValues[indicesA[i]],
                            precomputedValues[indicesB[j]]);
                        if (newdistance < distance) { distance = newdistance; }
                    }
                }
            } else if (mode == (int)MODE.MODE_COMPLETE_LINK) {
                for (int i = 0; i < countA; i++) {
                    for (int j = 0; j < countB; j++) {
                        newdistance = Point.Distance(
                            precomputedValues[indicesA[i]],
                            precomputedValues[indicesB[j]]);
                        if (newdistance > distance) { distance = newdistance; }
                    }
                }
            }
            else if (mode == (int)MODE.MODE_AVERAGE_LINK) {
                distance = Point.Distance(nodeA.Cluster.Values, nodeB.Cluster.Values);
            } else {
                throw new ArgumentException("Mode \"" + mode + "\" is not defined");
            }

            return (float)distance;
        }

        #endregion

        /// <summary>
        /// Overrides the ToString method to return "Extended HACM".
        /// </summary>
        /// <returns>"Extended HACM"</returns>
        public override string ToString() { return "Extended HACM"; }

        #endregion
 
    }

    #region Interface ISortedMatrix<T>

    /// <summary>
    /// A sorted matrix of generic type. Support for fast Minimum-Computation. Set of entries.
    /// Removal of rows and columns, updating of entries and complete rows and columns
    /// </summary>
    /// <typeparam name="T">Any type that supports IComparable of T"/></typeparam>
    interface ISortedMatrix<T> where T : IComparable<T> {
        int Count { get; }
        MatrixEntry<T> Minimum { get; }
        MatrixEntry<T> ElementAt(int row, int column);
        bool Exist(int row, int column);
        void Set(int row, int column, T item);
        void Set(MatrixEntry<T> entry);
        IEnumerable<MatrixEntry<T>> Column(int columnIndex);
        IEnumerable<MatrixEntry<T>> Row(int rowIndex);
        void RemoveColumn(int columnIndex);
        void RemoveRow(int rowIndex);
        void Update(MatrixEntry<T> entry, T newItem);
    }

    #endregion

    #region SortedMatrix<T>

    /// <summary>
    /// A Matrix for comparable items (symmetric distance). Entries are accessible by row, col,
    /// itemvalue. Optimized for Distance values that musst be accessed by acending value and updated or removed by
    /// row- and column-index
    /// </summary>
    /// <typeparam name="T">Any comparable type</typeparam>
    public class SortedMatrix<T> : ISortedMatrix<T> where T : IComparable<T> {
        #region Fields

        /// <summary>
        /// maximum size of matrix rows and columns
        /// </summary>
        private int maximumSize;

        private ListSorted<MatrixEntry<T>> sortedEntriesByColumn;
        private ListSorted<MatrixEntry<T>> sortedEntriesByRow;
        private ListSorted<MatrixEntry<T>> sortedEntriesByItem;

        #endregion
        #region Contstructors

        /// <summary>
        /// Creates a new sorted Matrix
        /// </summary>
        /// <param name="maximumSize">Maximum size for rows or columns. Never can any
        /// operation exceed this limit for row- or columnindex.</param>
        /// <param name="expectedNumberOfEntries">The number of entries expected</param>
        public SortedMatrix(int maximumSize, int expectedNumberOfEntries) {
            this.maximumSize = maximumSize;
            sortedEntriesByColumn = new ListSorted<MatrixEntry<T>>(expectedNumberOfEntries, new ColumnComparer<T>());
            sortedEntriesByRow = new ListSorted<MatrixEntry<T>>(expectedNumberOfEntries, new RowComparer<T>());
            sortedEntriesByItem = new ListSorted<MatrixEntry<T>>(expectedNumberOfEntries);
        }

        #endregion

        #region ISortedMatrix<T> Members

        public IEnumerable<MatrixEntry<T>> Column(int columnIndex) {
            int? first = FindFirstInColumn(columnIndex);
            if (!first.HasValue) yield break;
            int index = first.Value;
            do {
                yield return sortedEntriesByColumn[index];
                index++;
            } while (index < sortedEntriesByColumn.Count
                && sortedEntriesByColumn[index].Column == columnIndex);
            yield break;
        }

        public MatrixEntry<T> ElementAt(int row, int column) {
            int index = sortedEntriesByColumn.BinarySearch(new MatrixEntry<T>(row, column, default(T)));
            if (index >= 0) {
                return sortedEntriesByColumn[index];
            } else {
                return null;
            }
        }

        public bool Exist(int row, int column) {
            return Exist(new MatrixEntry<T>(row, column, default(T)));
        }

        public bool Exist(MatrixEntry<T> entry) {
            return sortedEntriesByColumn.Contains(entry);
        }

        public void RemoveColumn(int columnIndex) {
            int? first = FindFirstInColumn(columnIndex);
            if (!first.HasValue) return;
            int index = first.Value;
            do {
                sortedEntriesByRow.Remove(sortedEntriesByColumn[index]);
                sortedEntriesByItem.Remove(sortedEntriesByColumn[index]);
                sortedEntriesByColumn.RemoveAt(index);
            } while (index < sortedEntriesByColumn.Count
                && sortedEntriesByColumn[index].Column == columnIndex);
            return;
        }

        public void RemoveRow(int rowIndex) {
            int? first = FindFirstInRow(rowIndex);
            if (!first.HasValue) return;
            int index = first.Value;
            do {
                sortedEntriesByColumn.Remove(sortedEntriesByRow[index]);
                sortedEntriesByItem.Remove(sortedEntriesByRow[index]);
                sortedEntriesByRow.RemoveAt(index);
            } while (index < sortedEntriesByRow.Count
                && sortedEntriesByRow[index].Row == rowIndex);
            return;
        }

        public IEnumerable<MatrixEntry<T>> Row(int rowIndex) {
            int? first = FindFirstInRow(rowIndex);
            if (!first.HasValue) yield break;
            int index = first.Value;
            do {
                yield return sortedEntriesByRow[index];
                index++;
            } while (index < sortedEntriesByRow.Count
                && sortedEntriesByRow[index].Row == rowIndex);
            yield break;
        }

        public void Set(int row, int column, T item) {
            Set(new MatrixEntry<T>(row, column, item));
        }

        public void Set(MatrixEntry<T> entry) {
            if (entry.Row <= entry.Column) throw new ArgumentException("Use lower triangle-matrix only");
            if (sortedEntriesByColumn.BinarySearch(entry) >= 0) {
                throw new ApplicationException();
            } else {
                //Insert
                sortedEntriesByColumn.AddSorted(entry);
                sortedEntriesByRow.AddSorted(entry);
                sortedEntriesByItem.AddSorted(entry);
            }
        }

        public void Update(MatrixEntry<T> entry, T newItem) {
            sortedEntriesByItem.Remove(entry);
            entry.Item = newItem;
            sortedEntriesByItem.AddSorted(entry);
        }

        public int Count {
            get { return sortedEntriesByItem.Count; }
        }

        public MatrixEntry<T> Minimum {
            get {
                return (sortedEntriesByItem.Count != 0) ?
                sortedEntriesByItem[0] :
                null;
            }
        }

        #endregion
        #region Helper

        private int? FindFirstInColumn(int columnIndex) {
            int index = sortedEntriesByColumn.BinarySearch(new MatrixEntry<T>(0, columnIndex, default(T)));
            if (index < 0) {
                if (~index == sortedEntriesByColumn.Count) {
                    return null;
                } else {
                    index = ~index;
                }
            } else {
                if (index == sortedEntriesByColumn.Count) {
                    return null;
                }
            }
            if (sortedEntriesByColumn[index].Column == columnIndex) {
                return index;
            } else {
                return null;
            }
        }

        private int? FindFirstInRow(int rowIndex) {
            int index = sortedEntriesByRow.BinarySearch(new MatrixEntry<T>(rowIndex, 0, default(T)));
            if (index < 0) {
                if (~index == sortedEntriesByRow.Count) {
                    return null;
                } else {
                    index = ~index;
                }
            } else {
                if (index == sortedEntriesByRow.Count) {
                    return null;
                }
            }
            if (sortedEntriesByRow[index].Row == rowIndex) {
                return index;
            } else {
                return null;
            }
        }
        #endregion

        #region Comparer

        private class ColumnComparer<Ty> : IComparer<MatrixEntry<Ty>> where Ty : IComparable<Ty> {
            public int Compare(MatrixEntry<Ty> a, MatrixEntry<Ty> b) {
                int result = a.Column.CompareTo(b.Column);
                if (result == 0) {
                    result = a.Row.CompareTo(b.Row);
                }
                return result;
            }
        }

        private class RowComparer<Ty> : IComparer<MatrixEntry<Ty>> where Ty : IComparable<Ty> {
            public int Compare(MatrixEntry<Ty> a, MatrixEntry<Ty> b) {
                int result = a.Row.CompareTo(b.Row);
                if (result == 0) {
                    result = a.Column.CompareTo(b.Column);
                }
                return result;
            }
        }


        #endregion
    }

    #endregion

    #region MatrixEntry<T>

    /// <summary>
    /// An entry for a matrix
    /// </summary>
    /// <typeparam name="T">Any type that is IComparable of T</typeparam>
    public class MatrixEntry<T> : IComparable<MatrixEntry<T>> where T : IComparable<T> {
        private int row, column;
        private T item;

        /// <summary>
        /// Index of the entries column
        /// </summary>
        public int Column {
            get { return column; }
        }

        /// <summary>
        /// Index of the entries row
        /// </summary>
        public int Row {
            get { return row; }
        }

        /// <summary>
        /// The encapsulated item, stored in the distance-matrix
        /// </summary>
        public T Item {
            get { return item; }
            set { item = value; }
        }

        /// <summary>
        /// Creates a new matrixEntry
        /// </summary>
        /// <param name="row">Index of row</param>
        /// <param name="column">Index of the column</param>
        /// <param name="item">The wrapped item</param>
        public MatrixEntry(int row, int column, T item) {
            this.row = row;
            this.column = column;
            this.item = item;
        }

        /// <summary>
        /// ToString-Method for debugging purposes.
        /// </summary>
        /// <returns>Information about the MatrixEntry</returns>
        public override string ToString() {
            return this.Item.ToString() + " (" + this.Row + ", " + this.Column + ") " + base.ToString();
        }

        #region IComparable<MatrixEntry<T>> Member

        int IComparable<MatrixEntry<T>>.CompareTo(MatrixEntry<T> other) {
            int result = this.Item.CompareTo(other.Item);
            if (result == 0) {
                result = this.Column.CompareTo(other.Column);
                if (result == 0) {
                    result = this.Row.CompareTo(other.Row);
                }
            }
            return result;
        }

        #endregion
    }

    #endregion

    #region ListSorted<T>
    
    /// <summary>
    /// A Sorted List, sorted by the default IComparable implementation of the type T
    /// or by a given Comparer. Elements in the list cannot be undistiguishable.
    /// </summary>
    /// <typeparam name="T">Any type that is IComparable of T</typeparam>
    public class ListSorted<T> where T : IComparable<T> {
        private List<T> list;
        private IComparer<T> comparer = null;

        #region Constructors

        /// <summary>
        /// Creates a new Sorted List.
        /// </summary>
        public ListSorted() {
            list = new List<T>();
        }

        /// <summary>
        /// Creates a new Sorted List with alternative comparer.
        /// </summary>
        /// <param name="comparer">Any IComparer.</param>
        public ListSorted(IComparer<T> comparer) : this() {
            this.comparer = comparer;
        }

        /// <summary>
        /// Creates a new Sorted List with initial items that will be sorted immediately.
        /// </summary>
        /// <param name="collection">Any IEnumerable collection.</param>
        public ListSorted(IEnumerable<T> collection) {
            list = new List<T>(collection);
            this.Sort();
        }

        /// <summary>
        /// Creates a new Sorted List with initial items that will be sorted immediately
        /// by the given IComparer.
        /// </summary>
        /// <param name="collection">Any IEnumerable collection.</param>
        /// <param name="comparer">Any IComparer</param>
        public ListSorted(IEnumerable<T> collection, IComparer<T> comparer) {
            list = new List<T>(collection);
            this.comparer = comparer;
            this.Sort();
        }

        /// <summary>
        /// Creates a new Sorted List with initial capacity for faster computation.
        /// </summary>
        /// <param name="capacity"></param>
        public ListSorted(int capacity) {
            list = new List<T>(capacity);
        }

        /// <summary>
        /// Creates a new Sorted List with initial capacity for faster computation and with
        /// alternative comparer
        /// </summary>
        /// <param name="capacity">Integer value for initial capacity</param>
        /// <param name="comparer">Any IComparer</param>
        public ListSorted(int capacity, IComparer<T> comparer) : this(capacity) {
            this.comparer = comparer;
        }

        #endregion

        /// <summary>
        /// Adds a item in the sorted List. Position is computed by BinarySearch with the given
        /// or a standard comparer.
        /// </summary>
        /// <param name="item">An Item</param>
        /// <exception cref="ArgumentException">If a non-comparable element is already in the list.</exception>
        public void AddSorted(T item) {
            int index = this.BinarySearch(item);
            if (index < 0) {
                index = ~index;
            } else {
                throw new ArgumentException("Same Element already in List");
            }
            list.Insert(index, item);
        }

        /// <summary>
        /// Adds a collection of items in the sorted List. Elements are resorted after adding with the given
        /// or a standard comparer.
        /// </summary>
        /// <param name="collection">A collection of items </param>
        /// <exception cref="ArgumentException">If a non-comparable element is already in the list.</exception>
        public void AddRangeSorted(IEnumerable<T> collection) {
            list.AddRange(collection);
            this.Sort();
        }

        /// <summary>
        /// Removes the given item from the sorted List.
        /// </summary>
        /// <param name="item">Any contained Item</param>
        public void Remove(T item) {
            list.RemoveAt(this.BinarySearch(item));
        }

        /// <summary>
        /// Removes the element at the given index.
        /// </summary>
        /// <param name="index">index of the element to remove</param>
        public void RemoveAt(int index) {
            list.RemoveAt(index);
        }

        /// <summary>
        /// Fast determination ot an items index.
        /// </summary>
        /// <param name="item">Any contained Item</param>
        /// <returns>the index of the Item. Index will be bitwise negated of items position
        /// if not contained in the list.</returns>
        public int BinarySearch(T item) {
            if (comparer != null) {
                return list.BinarySearch(item, comparer);
            } else {
                return list.BinarySearch(item);
            }
        }

        /// <summary>
        /// Sorts the list
        /// </summary>
        private void Sort() {
            if (comparer != null) {
                list.Sort(comparer);
            } else {
                list.Sort();
            }
        }

        /// <summary>
        /// Determines if element is in the list.
        /// </summary>
        /// <param name="item">Any item</param>
        /// <returns>True if item is in the list. False otherwise</returns>
        public bool Contains(T item) {
            int index = this.BinarySearch(item);
            if (index >= 0) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Indexer for elements in the sorted list
        /// </summary>
        /// <param name="index">index of an element</param>
        /// <returns>the element</returns>
        public T this[int index] {
            get { return list[index]; }
            set { list[index] = value; }
        }

        /// <summary>
        /// Count of elements in the list.
        /// </summary>
        public int Count {
            get { return list.Count; }
        }
    }

    #endregion
}
