﻿using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;
using System;
using System.Collections.Generic;

namespace UILayer.InterpreterMethods
{
    class InsertMethods
    {
        static List<string> _keywords = new List<string>()
        {
            "VALUES",
            "COLUMN"
        };

        public static void Execute(string query)
        {
            try { 
                if (Interpreter.ConnectionString != null)
                {
                    char[] separator = new char[] { ' ' };
                    string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (queryList.Length == 2)
                    {
                        if (IsKeyword(queryList[0]))
                        {
                            var _inst = new InsertMethods();
                            string _methodName = "Insert" + queryList[0];
                            var _method = _inst.GetType().GetMethod(_methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
                            _method?.Invoke(_inst, new object[] { queryList[1] });
                        }
                        else throw new Exception("\nERROR: Invalid command syntax\n");
                    }
                    else throw new Exception("\nERROR: Invalid number of variables\n");
                }
                else throw new Exception("\nERROR: There is no connection to database\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void InsertColumn(string tableName)
        {
            try {
                var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                var _table = _inst.GetTableByName(tableName);
                Console.WriteLine("\nName(s) and type(s) of column(s):\n" + _table.ColumnType() + "\nEnter column(s)\n");
                string param = Console.ReadLine();

                char[] _separator = new char[] { ';' };
                string[] _colParams = param.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                foreach (var _column in _colParams)
                {
                    char[] _tempery = new char[] { ',', ' ' };
                    string[] _colParam = _column.Split(_tempery, StringSplitOptions.RemoveEmptyEntries);
                    if (_colParam.Length == 4)
                    {
                        _table.AddColumn(GetColumn(_colParam, _table));
                    }
                    else
                        throw new Exception("\nERROR: Ivalid numbers of variables\n");
                }
                Console.WriteLine("\nColumns successfully inserted\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void InsertValues(string tabelName)
        {
            try
            {
                var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                var _table = _inst.GetTableByName(tabelName);
                if (_table.Columns.Count - 1 != 0)
                {
                    Console.WriteLine("\nName(s) and type(s) of column(s):\n" + _table.ColumnType() + "\nEnter value(s)\n");
                    string _data = Console.ReadLine();
                    char[] _separator = new char[] { ';' };
                    string[] _values = _data.Split(_separator);
                    foreach (var _val in _values)
                    {
                        char[] _separators = new char[] { ',' };
                        string[] _valuesList = _val.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
                        if (_table.Columns.Count - 1 == _valuesList.Length)
                        {
                            object[] _colData = new object[_valuesList.Length];
                            for (int i = 0; i < _valuesList.Length; i++)
                            {
                                _colData[i] = GetData(_valuesList[i], _table.Columns[i + 1]);
                            }
                            _table.AddTableElement(_colData);
                        }
                        else
                            throw new Exception("\nERROR: Count of values doesn't equals count of columns");
                    }
                    Console.WriteLine("\nAll data successfully inserted\n");
                }
                else
                    throw new Exception("\nERROR: There is no columns in this table");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static Column GetColumn(string[] _variables, Table thisTable)
        {
            string _colName = _variables[0];
            Type _colType = GetType(_variables[1]);
            bool _isAllowNull = Convert.ToBoolean(_variables[2]);
            object _defValue = GetDefaultValue(_variables[3], _colType);

            if (_colType != _defValue.GetType())
                Console.WriteLine("\nType of default value doesn't equals column type. Default value will be set by default\n");
            return new Column(_colName, _colType, _isAllowNull, _defValue, thisTable);
        }

        static object GetDefaultValue(string value, Type _colType)
        {
            if (_colType == typeof(int))
                return Convert.ToInt32(value);
            else if (_colType == typeof(string))
                return value;
            else if (_colType == typeof(double))
            {
                string val = value.Replace('.', ',');
                return Convert.ToDouble(value);
            }
            else if (_colType == typeof(bool))
                return Convert.ToBoolean(value);
            else throw new Exception($"\nERROR: Type '{_colType.Name}' doesn't exist");
        }

        static Type GetType(string _typeName)
        {
            string _name = _typeName.ToLower();
            switch (_name)
            {
                case "int": return typeof(int);
                case "string": return typeof(string);
                case "double": return typeof(double);
                case "bool": return typeof(bool);
                default: throw new Exception($"\nERROR: Type {_typeName} doesn't exist");
            }


        }

        static bool IsKeyword(string word)
        {
            string _key = word.ToUpper();
            foreach (var k in _keywords)
                if (_key == k)
                    return true;
            return false;
        }

        static object GetData(string value, Column column)
        {
            if (column.DataType == typeof(string))
                return value;
            else if (column.DataType == typeof(int))
                return Convert.ToInt32(value);
            else if (column.DataType == typeof(double))
                return Convert.ToDouble(value);
            else if (column.DataType == typeof(bool))
            {
                value = value.Replace('.', ',');
                return Convert.ToBoolean(value);
            }
            else if (value == "null")
                return null;
            else return null;
        }
        
    }
}
