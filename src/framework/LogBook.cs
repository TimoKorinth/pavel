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
using System.Windows.Forms;
using System.Text;
using Pavel.GUI;
using System.Drawing;

namespace Pavel.Framework {

    #region LogLevelEnum

    /// <value>Enum for the log levels </value>
    public enum LogLevelEnum { Message = 1, Warning = 2, Error = 4 }

    #endregion

    /// <summary>
    /// This class manages the complete handling of messages, warnings and errors.
    /// It can show a MessageBox when a log is added.
    /// </summary>
    public class LogBook {

        #region Fields

        private List<LogEntry> logs;

        /// <value> This event is fired, if the log was changed./ </value>
        public event LogChangedEvent LogChanged;

        #endregion

        #region Properties

        /// <value> Gets the complete list of log entries </value>
        public List<LogEntry> Logs {
            [CoverageExclude]
            get { return logs; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the LogBook class, creates a new list of log entries.
        /// </summary>
        [CoverageExclude]
        public LogBook() {
            logs = new List<LogEntry>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// General method to add a log.
        /// </summary>
        /// <param name="desc">Message description</param>
        /// <param name="showMsgBox">True to display a MessageBox</param>
        /// <param name="level">Loglevel</param>
        private void AddLog(String desc, bool showMsgBox, LogLevelEnum level) {
            Logs.Add(new LogEntry(desc, level));
            if (showMsgBox) {
                MessageBoxIcon mBI;
                switch (level) {
                    case LogLevelEnum.Error:
                        mBI = MessageBoxIcon.Error;
                        break;
                    case LogLevelEnum.Warning:
                        mBI = MessageBoxIcon.Warning;
                        break;
                    default:
                        mBI = MessageBoxIcon.Information;
                        break;
                }
                MessageBox.Show(desc, "LogBook", MessageBoxButtons.OK, mBI);
            }
            if (LogChanged != null) { LogChanged(this, null); }
        }

        /// <summary>
        /// Display a message with loglevel "Error".
        /// </summary>
        /// <param name="desc">Description of the error</param>
        /// <param name="showMsgBox">True to display a MessageBox</param>
        public void Error(String desc, bool showMsgBox) {
            AddLog(desc, showMsgBox, LogLevelEnum.Error);
        }

        /// <summary>
        /// Display a message with loglevel "Error".
        /// </summary>
        /// <param name="desc">Description of the error</param>
        public void Error(String desc) {
            AddLog(desc, false, LogLevelEnum.Error);
        }

        /// <summary>
        /// Display a message with loglevel "Warning".
        /// </summary>
        /// <param name="desc">Description of the warning</param>
        /// <param name="showMsgBox">True to display a MessageBox</param>
        public void Warning(String desc, bool showMsgBox) {
            AddLog(desc, showMsgBox, LogLevelEnum.Warning);
        }

        /// <summary>
        /// Display a message with loglevel "Warning".
        /// </summary>
        /// <param name="desc">Description of the warning</param>
        public void Warning(String desc) {
            AddLog(desc, false, LogLevelEnum.Warning);
        }

        /// <summary>
        /// Display a message with loglevel "Message".
        /// </summary>
        /// <param name="desc">Description of the message</param>
        /// <param name="showMsgBox">True to display a MessageBox</param>
        public void Message(String desc, bool showMsgBox) {
            AddLog(desc, showMsgBox, LogLevelEnum.Message);
        }

        /// <summary>
        /// Display a message with loglevel "Message".
        /// </summary>
        /// <param name="desc">Description of the message</param>
        public void Message(String desc) {
            AddLog(desc, false, LogLevelEnum.Message);
        }

        /// <summary>
        /// Returns the log entries which match the parameter <paramref name="lle"/>.
        /// </summary>
        /// <param name="lle">Enum of loglevel which should be returned. Can be combined, eg.
        /// LogLevelEnum.Warning | LogLevelEnum.Error</param>
        /// <returns>List of log entries matching the enum, can be empty</returns>
        public List<LogEntry> GetLogs(LogLevelEnum lle) {
            List<LogEntry> tmpList = new List<LogEntry>();
            foreach (LogEntry lE in logs) {
                if ((lle & lE.LogLevel) == lE.LogLevel) {
                    tmpList.Add(lE);
                }
            }
            return tmpList;
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// This eventhandler is used for indicating that a log is added.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">not used, only for specification reason</param>
        public delegate void LogChangedEvent(object sender, EventArgs e);

        #endregion

        #region Inner class LogEntry

        /// <summary>
        /// One entity of a logbook.
        /// </summary>
        public class LogEntry {

            #region Fields

            private LogLevelEnum logLevel;
            private String logDescription;
            private DateTime logDateTime;

            #endregion

            #region Properties

            /// <value> Gets the logLevel or sets it </value>
            public LogLevelEnum LogLevel {
                [CoverageExclude]
                get { return logLevel; }
                [CoverageExclude]
                set { logLevel = value; }
            }

            /// <value> Gets the logDescription or sets it </value>
            public String LogDescription {
                [CoverageExclude]
                get { return logDescription; }
                [CoverageExclude]
                set { logDescription = value; }
            }

            /// <value> Gets the logDateTime or sets it </value>
            public DateTime LogDateTime {
                [CoverageExclude]
                get { return logDateTime; }
                [CoverageExclude]
                set { logDateTime = value; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Standard constructor. The logLevel is set to Message,
            /// the logDescription to (none) and the logDateTime to the current DateTime.
            /// </summary>
            public LogEntry() {
                logLevel = LogLevelEnum.Message;
                logDescription = "(none)";
                logDateTime = DateTime.Now;
            }

            /// <summary>
            /// The logLevel is set to <paramref name="level"/>, the logDescription to <paramref name="desc"/>
            /// and the logDateTime to the current DateTime.
            /// </summary>
            /// <param name="desc">Value for logDescription</param>
            /// <param name="level">Value for logLevel</param>
            public LogEntry(String desc, LogLevelEnum level) {
                logLevel = level;
                logDescription = desc;
                logDateTime = DateTime.Now;
            }

            #endregion

        }

        #endregion
    }
}
