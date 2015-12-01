﻿namespace Mvc_App_Crud_ControleReceitasDespesas.Views.Shared {

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Data.Odbc;
    using System.Data.OleDb;
    using System.Data.OracleClient;
 /*
 Copyright  Code4Forever 2012.  All rights reserved.
 Visit code4forever.blogspot.com for more information about us.
 */

    namespace PowerFiles.PowerDAL
    {
        /// <summary>
        /// dbHelper is a helper class that takes the common data classes and allows you
        /// to specify the provider to use, execute commands, add parameters, and return datasets.
        /// See examples for usage.
        /// </summary>
        public class DBHelper
        {
            #region private members
            private string _connectionstring = "";
            private DbConnection _connection;
            private DbCommand _command;
            private DbProviderFactory _factory = null;
            #endregion

            #region properties

            /// <summary>
            /// Gets or Sets the connection string for the database
            /// </summary>
            public string connectionstring
            {
                get
                {
                    return _connectionstring;
                }
                set
                {
                    if (value != "")
                    {
                        _connectionstring = value;
                    }
                }
            }

            /// <summary>
            /// Gets the connection object for the database
            /// </summary>
            public DbConnection connection
            {
                get
                {
                    return _connection;
                }
            }

            /// <summary>
            /// Gets the command object for the database
            /// </summary>
            public DbCommand command
            {
                get
                {
                    return _command;
                }
            }

            #endregion

            #region methods

            /// <summary>
            /// Determines the correct provider to use and sets up the connection and command
            /// objects for use in other methods
            /// </summary>
            /// <param name="connectString">The full connection string to the database</param>
            /// <param name="providerlist">The enum value of providers from dbutilities.Providers</param>
            public void CreateDBObjects(string connectString, Providers providerList)
            {
                //CreateDBObjects(connectString, providerList, null);
                switch (providerList)
                {
                    case Providers.SqlServer:
                        _factory = SqlClientFactory.Instance;
                        break;
                    case Providers.Oracle:
                        _factory = OracleClientFactory.Instance;
                        break;
                    case Providers.OleDB:
                        _factory = OleDbFactory.Instance;
                        break;
                    case Providers.ODBC:
                        _factory = OdbcFactory.Instance;
                        break;
                }

                _connection = _factory.CreateConnection();
                _command = _factory.CreateCommand();

                _connection.ConnectionString = connectString;
                _command.Connection = connection;
            }

            #region parameters

            /// <summary>
            /// Creates a parameter and adds it to the command object
            /// </summary>
            /// <param name="name">The parameter name</param>
            /// <param name="value">The paremeter value</param>
            /// <returns></returns>
            public int AddParameter(string name, object value)
            {
                DbParameter parm = _factory.CreateParameter();
                parm.ParameterName = name;
                parm.Value = value;
                return command.Parameters.Add(parm);
            }

            /// <summary>
            /// Creates a parameter and adds it to the command object
            /// </summary>
            /// <param name="parameter">A parameter object</param>
            /// <returns></returns>
            public int AddParameter(DbParameter parameter)
            {
                return command.Parameters.Add(parameter);
            }

            #endregion

            #region transactions

            /// <summary>
            /// Starts a transaction for the command object
            /// </summary>
            private void BeginTransaction()
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                command.Transaction = connection.BeginTransaction();
            }

            /// <summary>
            /// Commits a transaction for the command object
            /// </summary>
            private void CommitTransaction()
            {
                command.Transaction.Commit();
                connection.Close();
            }

            /// <summary>
            /// Rolls back the transaction for the command object
            /// </summary>
            private void RollbackTransaction()
            {
                command.Transaction.Rollback();
                connection.Close();
            }

            #endregion

            #region execute database functions

            /// <summary>
            /// Executes a statement that does not return a result set, such as an INSERT, UPDATE, DELETE, or a data definition statement
            /// </summary>
            /// <param name="query">The query, either SQL or Procedures</param>
            /// <param name="commandtype">The command type, text, storedprocedure, or tabledirect</param>
            /// <param name="connectionstate">The connection state</param>
            /// <returns>An integer value</returns>
            public int ExecuteNonQuery(string query, CommandType commandtype, ConnectionState connectionstate)
            {
                command.CommandText = query;
                command.CommandType = commandtype;
                int i = -1;
                try
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    BeginTransaction();

                    i = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    RollbackTransaction();
                    throw (ex);
                }
                finally
                {
                    CommitTransaction();
                    command.Parameters.Clear();

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Dispose();
                        //command.Dispose();
                    }
                }

                return i;
            }

            /// <summary>
            /// Executes a statement that returns a single value. 
            /// If this method is called on a query that returns multiple rows and columns, only the first column of the first row is returned.
            /// </summary>
            /// <param name="query">The query, either SQL or Procedures</param>
            /// <param name="commandtype">The command type, text, storedprocedure, or tabledirect</param>
            /// <param name="connectionstate">The connection state</param>
            /// <returns>An object that holds the return value(s) from the query</returns>
            public object ExecuteScaler(string query, CommandType commandtype, ConnectionState connectionstate)
            {
                command.CommandText = query;
                command.CommandType = commandtype;
                object obj = null;
                try
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    BeginTransaction();
                    obj = command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    RollbackTransaction();
                    throw (ex);
                }
                finally
                {
                    CommitTransaction();
                    command.Parameters.Clear();

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Dispose();
                        command.Dispose();
                    }
                }

                return obj;
            }

            /// <summary>
            /// Executes a SQL statement that returns a result set.
            /// </summary>
            /// <param name="query">The query, either SQL or Procedures</param>
            /// <param name="commandtype">The command type, text, storedprocedure, or tabledirect</param>
            /// <param name="connectionstate">The connection state</param>
            /// <returns>A datareader object</returns>
            public DbDataReader ExecuteReader(string query, CommandType commandtype, ConnectionState connectionstate)
            {
                command.CommandText = query;
                command.CommandType = commandtype;
                DbDataReader reader = null;
                try
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    if (connectionstate == System.Data.ConnectionState.Open)
                    {
                        reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    }
                    else
                    {
                        reader = command.ExecuteReader();
                    }
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    command.Parameters.Clear();
                }

                return reader;
            }

            /// <summary>
            /// Generates a dataset
            /// </summary>
            /// <param name="query">The query, either SQL or Procedures</param>
            /// <param name="commandtype">The command type, text, storedprocedure, or tabledirect</param>
            /// <param name="connectionstate">The connection state</param>
            /// <returns>A dataset containing data from the database</returns>
            public DataSet GetDataSet(string query, CommandType commandtype, ConnectionState connectionstate)
            {
                DbDataAdapter adapter = _factory.CreateDataAdapter();
                command.CommandText = query;
                command.CommandType = commandtype;
                adapter.SelectCommand = command;
                DataSet ds = new DataSet();
                try
                {
                    adapter.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    command.Parameters.Clear();

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Dispose();
                        command.Dispose();
                    }
                }
                return ds;
            }

            #endregion

            #endregion

            #region enums

            /// <summary>
            /// A list of data providers
            /// </summary>
            public enum Providers
            {
                SqlServer,
                OleDB,
                ODBC,
                Oracle,
            }

            #endregion
        }
    }
}

