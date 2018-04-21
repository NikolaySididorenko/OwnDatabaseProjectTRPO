﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
//using DataLayer.InternalDataBaseInstanceComponents;
using System.Reflection;
using UILayer.InterpreterMethods;

namespace UILayer
{
    class Interpreter
    {
        static object _lockObj = new object();
        static Interpreter _instance;
        public static List<string> _keywords = new List<string>()
        {
            "CREATE",
            "CLEAR",
            "CONNECT",
            "INFO",
            "LOADDB",
            "SAVEDB",
            "INSERT",
            "RENAME",
            "DELETE",
            "DATABASES",
            "LINK",
            "UNLINK",
            "CASCADE",
            "SELECT",//to do
            "UPDATE"// to do
        };
        public static string ConnectionString { get; set; }


        static Interpreter GetInstance()
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new Interpreter();
                        return _instance;
                    }
                }
            }
            return _instance;
        }

        public static void Run()
        {
            ConnectionString = null;
            while (true)
            {
                string _query = default(string);
                _query = Console.ReadLine();
                if (_query.Any(x => char.IsLetterOrDigit(x)))
                {
                    char[] _separator = new char[] { ' ' };
                    string _keyword = _query.Split(_separator, StringSplitOptions.RemoveEmptyEntries)[0];
                    if (GetInstance().IsKeyword(_keyword))
                    {
                        var _method = GetInstance().GetType().GetMethod(_keyword, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                        object[] param = new object[] { _query };
                        _method?.Invoke(GetInstance(), param);
                    }
                    else
                    {
                        Console.WriteLine($"\nERROR: Command '{_keyword}' doesn't exist\n");
                    }
                }
            }
        }


        #region LocalMethods
        bool IsKeyword(string command)
        {
            foreach (var key in _keywords)
                if (key == command)
                    return true;
            return false;
        }
        #endregion

        #region MainMetods
        /// <summary>
        /// Connect to |dbName|
        /// </summary>
        /// <param name="query"></param>
        private static void Connect(string query)
        {
            try
            {
                char[] separator = new char[] { ' ' };
                string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (queryList[1].ToLower() == "to")
                {
                    if (queryList.Length == 3)
                    {
                        if (Kernel.isDatabaseExists(queryList[2]))
                        {
                            ConnectionString = queryList[2];
                            Console.WriteLine($"\nNow you connected to database '{queryList[2]}'\n");
                        }
                        else
                            throw new Exception($"\nERROR: Database with name '{queryList[2]}' doesn't exist\n");
                    }
                    else
                        throw new Exception("\nERROR: Invalid number of variables\n");
                }
                else
                    throw new Exception($"\nERROR: This command doesn't exist\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// Info/Info |dbName|
        /// </summary>
        /// <param name="query"></param>
        private static void Info(string query)
        {
            try
            {
                char[] separator = new char[] { ' ' };
                string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (queryList.Length == 2)
                {
                    if (Kernel.isDatabaseExists(queryList[1]))
                    {
                        Kernel.OutDatabaseInfo(queryList[1]);
                    }
                    else
                        throw new Exception($"\nERROR: Database with name '{queryList[1]}' doesn't exist\n");
                }
                else if (queryList.Length == 1)
                {
                    Console.WriteLine($"\nConnection string - {ConnectionString}");
                    Kernel.OutDatabaseInfo();
                }
                else
                {
                    throw new Exception($"\nERROR: Invalid numbers of variables\n");
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// SAVEDB
        /// SAVEDB |dbName|
        /// </summary>
        /// <param name="query"></param>
        private static void SaveDb(string query)
        {
            try {
                char[] separator = new char[] { ' ' };
                string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (queryList.Length == 1)
                {
                    Kernel.SaveAllDatabases();
                    Console.WriteLine("\nAll databases saved\n");
                }
                else if (query.Length == 2)
                {
                    if (Kernel.isDatabaseExists(queryList[1]))
                    {
                        var _inst = Kernel.GetInstance(queryList[1]);
                        _inst.SaveDataBaseInstanceToFolder();
                        Console.WriteLine($"\nDatabase {queryList[1]} successfully saved\n");
                    }
                    else
                        throw new Exception($"\nERROR: Database {queryList[1]} doesn't exist\n");
                }
                else
                    throw new Exception($"\nERROR: Invalid number of variables");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// LOADDB
        /// </summary>
        /// <param name="query"></param>
        private static void LoadDb(string query)
        {
            try {
                char[] separator = new char[] { ' ' };
                string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (queryList.Length == 1)
                {
                    Kernel.LoadAllDatabases(true);
                    Console.WriteLine("\nDatabases loaded\n");
                }
                else
                    throw new Exception($"\nERROR: Invalid number of variables");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// create database |dbName|
        /// create table |tableName|
        /// create table |tableName| (ColName,ColType,IsAllowNull(true/false),DefaultValue;...)
        /// </summary>
        /// <param name="query"></param>
        private static void Create(string query)
        {
            string _command = query.Substring(6);
            CreateMethods.Execute(_command);
           
        }
        //
        /// <summary>
        /// Insert column |tableName| (ColName,ColType,IsAllowNull(true/false),DefaultValue;...)
        /// Insert values |tableName| (|params|)
        /// Insert values_into |tableName| (colName,...) values (|params|4) //////////TO DO
        /// </summary>
        /// <param name="query"></param>
        private static void Insert(string query)
        {
            string param = query.Substring(6);
            InsertMethods.Execute(param);
        }
        //
        /// <summary>
        /// DELETE TABLE table |tableName|
        /// DELETE COLUMN |tableName| |colName|
        /// DELETE ELEMENT |tableName| |ID(int)|
        /// DELETE ELEMENT |tableName| WHERE (colName=value,...)
        /// DELETE ELEMENT |tableName| *-----
        /// </summary>
        /// <param name="query"></param>
        private static void Delete(string query)
        {
            string _command = query.Substring(6);
            DeleteMethods.Execute(_command);
        }
        //
        /// <summary>
        ///  UPDATE |tableName| ELEMENT |ElementID| (ColName=value,...)
        ///  UPDATE |tableName| VALUES (colName=Param,...)----
        ///  UPDATE |tableName| VALUES (colName=Param,...) WHERE (colName=value,...)-----
        ///  UPDATE |tableName| VALUES (colName=Param,...) WHERE |colName| BETWEEN (1,10)
        ///  UPDATE |tableName| VALUES (colName=Param,...) WHERE |colName| IN (1,2,3,4)
        ///  UPDATE |tableName| DEFAULT VALUE |colName| |value|
        ///  UPDATE |tableName| NULLPROPERTY |colName| |true/false|
        ///  UPDATE |tableName| TYPE |colName| |type|
        /// </summary>
        /// <param name="query"></param>
        private static void Update(string query)
        {
            string _tableName = query.Substring(6);
            UpdateMethods.Execute(_tableName);
        }
        //
        /// <summary>
        /// Link |TableNameWithFK| |GeneralTableName| |true/false|
        /// </summary>
        /// <param name="query"></param>
        private static void Link(string query)
        {
            try {
                if (ConnectionString != null)
                {
                    char[] _separator = new char[] { ' ' };
                    string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (_params.Length == 4)
                    {
                        var _inst = Kernel.GetInstance(ConnectionString);
                        if (_inst.isTableExists(_params[1]))
                        {
                            var _tableWithFk = _inst.GetTableByName(_params[1]);
                            if (_inst.isTableExists(_params[2]))
                            {
                                var _generalTable = _inst.GetTableByName(_params[2]);
                                bool _isCascadeDelete = Convert.ToBoolean(_params[3]);
                                _inst.LinkTables(_tableWithFk, _generalTable, _isCascadeDelete);
                            }
                            else throw new NullReferenceException($"There is no table '{_params[2]}' in database '{_inst.Name}'!");
                        }
                        else throw new NullReferenceException($"There is no table '{_params[1]}' in database '{_inst.Name}'!");
                    }
                    else throw new Exception("\nERROR: Invalid number of variables\n");
                }
                else throw new Exception("\nERROR: Invalid number of variables\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// UnLink |TableNameWithFK| |GeneralTableName|
        /// </summary>
        /// <param name="query"></param>
        private static void UnLink(string query)
        {
            try {
                if (ConnectionString != null)
                {
                    char[] _separator = new char[] { ' ' };
                    string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (_params.Length == 3)
                    {
                        var _inst = Kernel.GetInstance(ConnectionString);
                        if (_inst.isTableExists(_params[1]))
                        {
                            if (_inst.isTableExists(_params[3]))
                            {
                                _inst.UnLinkTables(_inst.GetTableByName(_params[1]), _inst.GetTableByName(_params[3]));
                            }
                            else throw new NullReferenceException($"There is no table '{_params[3]}' in database '{_inst.Name}'!");

                        }
                        else throw new NullReferenceException($"There is no table '{_params[1]}' in database '{_inst.Name}'!");
                    }
                    else throw new Exception("\nERROR: Invalid number of variables\n");
                }
                else throw new Exception("\nERROR: There is no connection to database\n");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// Cascade dalete |tableNameFK| |GeneralTableName| |true/fasle|
        /// </summary>
        /// <param name="query"></param>
        private static void Cascade(string query)
        {
            try
            {
                if (ConnectionString != null)
                {
                    char[] _seprator = new char[] { ' ' };
                    string[] _params = query.Split(_seprator, StringSplitOptions.RemoveEmptyEntries);
                    if (_params.Length == 5)
                    {
                        if (_params[1] == "DELETE")
                        {
                            var _inst = Kernel.GetInstance(ConnectionString);
                            var _tableFk = _inst.GetTableByName(_params[2]);
                            var _tableGenral = _inst.GetTableByName(_params[3]);
                            bool _isCascadeDelete = Convert.ToBoolean(_params[4]);
                            _inst.EditCascadeDeleteOption(_tableFk, _tableGenral, _isCascadeDelete);
                        }
                        else throw new Exception("\nERROR: Invalid command syntax\n");
                    }
                    else throw new Exception("\nERROR: Invalid number of variables\n");
                }
                else throw new Exception("\nERROR: There is no connection to database\n");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// clear
        /// </summary>
        /// <param name="query"></param>
        private static void Clear(string query)
        {
            Console.Clear();
        }
        //
        /// <summary>
        /// Rename database |oldName| |newName|
        /// Rename table |oldName| |newName|
        /// Rename column |oldName| |newName|
        /// </summary>
        /// <param name="query"></param>
        private static void Rename(string query)
        {
            string param = query.Substring(6);
            RenameMethods.Execute(param);
        }
        //
        /// <summary>
        /// Databases
        /// </summary>
        /// <param name="query"></param>
        private static void Databases(string query)
        {
            try {
                char[] separator = new char[] { ' ' };
                string[] command = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (command.Length == 1)
                {
                    Console.WriteLine();
                    Kernel.OutNamesOfExistingDBs();
                    Console.WriteLine();
                }
                else
                    throw new Exception();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

    }
}
