﻿using DataModels.App.InternalDataBaseInstanceComponents;
using DataLayer.Shared.ExtentionMethods;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using DataLayer.Shared.DataModels;
using DataModels.App.Shared.ExtentionMethods;

namespace DataLayer
{
    
     [Serializable]
     public class DataBaseInstance
     {
         //fields
         List<Table> _tablesDB = new List<Table>();
         string _name;
         //properties
         public string Name { get => _name; set => _name = value; }
         public List<Table> TablesDB { get => _tablesDB; set => _tablesDB = value; }
         
         /// <summary>
         /// DB constructor
         /// </summary>
         /// <param name="name"></param>
         public DataBaseInstance(string name)
         {
             _name = name;
         }
        //
        /// <summary>
        /// Add table to this Database
        /// </summary>
        /// <param name="bufTable"></param>
        public void AddTable(string name)
        {
            Table bufTable = new Table(name);
            AddTable(bufTable);
        } //UI done
        //
        /// <summary>
        /// adds table to db
        /// </summary>
        /// <param name="bufTable"></param>
        public void AddTable(Table bufTable)
        {
            if (bufTable.Name.isThereNoUndefinedSymbols())
            {
                if (isTableExists(bufTable.Name)) throw new FormatException("Invalid table name. Some table in this database have same name!");
                TablesDB.Add(bufTable);
            }
            else throw new FormatException("There is invalid symbols in table's name!");
        } //UI done
        //
        /// <summary>
        /// Delete table by name
        /// </summary>
        /// <param name="name"></param>
        public void DeleteTable(string name)
        {
            if(TablesDB.Count==0) throw new NullReferenceException();
            if (indexOfTable(name) != -1)
            {
                Table tableToDelete = GetTableByName(name);
                List<int> keys = new List<int>();
                for (int i = 0; i < tableToDelete.Columns[0].DataList.Count; i++)
                {
                    keys.Add((int)tableToDelete.Columns[0].DataList[i].Data);
                }
                foreach (int key in keys) tableToDelete.DeleteTableElementByPrimaryKey(key);
                for (int i = 0; i < tableToDelete.Columns.Count; i++)
                {
                    if (tableToDelete.Columns[i].IsFkey)
                    {
                        string LinkedTableName = default(string);
                        LinkedTableName = tableToDelete.Columns[i].Name.Substring(5);
                        Console.WriteLine(LinkedTableName);
                        UnLinkTables(tableToDelete, GetTableByName(LinkedTableName));
                    }
                }
                for (int i = 0; i < TablesDB.Count; i++)
                {
                    if (TablesDB[i].isColumnExists("FK_" + tableToDelete.Columns[0].Name))
                    {
                        TablesDB[i].GetColumnByName("FK_" + tableToDelete.Columns[0].Name).SetFkeyProperty(false);
                        TablesDB[i].DeleteColumn("FK_" + tableToDelete.Columns[0].Name);
                    }
                }
                TablesDB.RemoveAt(indexOfTable(name));
            }
            else throw new NullReferenceException();
        } //UI done
        //
        /// <summary>
        /// Rename table
        /// </summary>
        /// <param name="currentName"></param>
        /// <param name="futureName"></param>
        public void RenameTable(string currentName, string futureName)
        {
            if (isTableExists(currentName))
            {
                if (futureName.isThereNoUndefinedSymbols()) GetTableByName(currentName).Name = futureName;
                else throw new ArgumentException("Your name contains undefined symbols!");
            }
            throw new ArgumentNullException("there is no such table in this database!");
        } //UI done
        //
        /// <summary>
        /// check if this database already contains table with such name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool isTableExists(string name)
        {
            if (TablesDB.Count == 0) return false;
            else
            {
                foreach (Table tbl in TablesDB)
                if (tbl.Name == name) return true;
            }
            return false;
        }
        //
        /// <summary>
        /// Add's link many-to-one (second parameter table will be general)
        /// </summary>
        /// <param name="tableToLink"></param>
        /// <param name="tableToLinkWith"></param>
        public void LinkTables(Table tableToLink, Table tableToLinkWith, bool isCascadeDelete)
        {
            LinkColumn newLink = new LinkColumn("FK_"+tableToLinkWith.Columns[0].Name, typeof(int), false, 0, tableToLink, tableToLinkWith.Columns[0]);
            newLink.IsCascadeDeleteOn = isCascadeDelete;
                for (int i = 0; i < tableToLink.Columns[0].DataList.Count; i++)
                {
                    newLink.DataList.Add(new Shared.DataModels.DataObject(newLink.GetHashCode(), newLink.Default));
                }
            tableToLink.Columns.Add(newLink);
            if(newLink.IsCascadeDeleteOn)
            tableToLinkWith.cascadeDelete += tableToLink.ExecuteCascadeDelete;
        } //UI (second table will be general)
        //
        public void EditCascadeDeleteOption(Table tableToEditLink, Table tableToEditLinkWith, bool isCascadeDelete)
        {
            if (tableToEditLink.isColumnExists("FK_" + tableToEditLinkWith.Columns[0].Name))
            {
                if (tableToEditLink.GetColumnByName("FK_" + tableToEditLinkWith.Columns[0].Name).IsCascadeDeleteOn)
                {
                    if(!isCascadeDelete) tableToEditLinkWith.cascadeDelete -= tableToEditLink.ExecuteCascadeDelete;
                }
                else if(!tableToEditLink.GetColumnByName("FK_" + tableToEditLinkWith.Columns[0].Name).IsCascadeDeleteOn)
                {
                    if(isCascadeDelete) tableToEditLinkWith.cascadeDelete += tableToEditLink.ExecuteCascadeDelete;
                }
            }
            else if (tableToEditLinkWith.isColumnExists("FK_" + tableToEditLink.Columns[0].Name))
            {
                if (tableToEditLinkWith.GetColumnByName("FK_" + tableToEditLink.Columns[0].Name).IsCascadeDeleteOn)
                {
                    if (!isCascadeDelete) tableToEditLink.cascadeDelete -= tableToEditLinkWith.ExecuteCascadeDelete;
                }
                else if (!tableToEditLinkWith.GetColumnByName("FK_" + tableToEditLink.Columns[0].Name).IsCascadeDeleteOn)
                {
                    if (isCascadeDelete) tableToEditLink.cascadeDelete += tableToEditLinkWith.ExecuteCascadeDelete;
                }
            }
            else throw new NullReferenceException("There's no link between this tables");
        } //UI (do not care about what table is on the first place and what table on the second)
        /// <summary>
        /// Unlinks two tables
        /// </summary>
        /// <param name="TableToUnlink"></param>
        /// <param name="TableToUnlinkWith"></param>
        public void UnLinkTables(Table TableToUnlink, Table TableToUnlinkWith)
        {
            if (TableToUnlink.isColumnExists("FK_" + TableToUnlinkWith.Columns[0].Name))
            {
                TableToUnlink.GetColumnByName("FK_" + TableToUnlinkWith.Columns[0].Name).SetFkeyProperty(false);
                TableToUnlink.DeleteColumn("FK_" + TableToUnlinkWith.Columns[0].Name);
                TableToUnlinkWith.cascadeDelete -= TableToUnlink.ExecuteCascadeDelete;
            }
            else if (TableToUnlinkWith.isColumnExists("FK_" + TableToUnlink.Columns[0].Name))
            {
                TableToUnlinkWith.GetColumnByName("FK_" + TableToUnlink.Columns[0].Name).SetFkeyProperty(false);
                TableToUnlinkWith.DeleteColumn("FK_" + TableToUnlink.Columns[0].Name);
                TableToUnlink.cascadeDelete -= TableToUnlinkWith.ExecuteCascadeDelete;
            }
            else throw new NullReferenceException("There's no link between this tables");
        } //UI (do not care about places of table's in parametres)
        //
        int indexOfTable(string name)
        {
            if (TablesDB.Count == 0) throw new NullReferenceException();
            for (int i = 0; i < TablesDB.Count; i++)
            {
                if (TablesDB[i].Name == name) return i;
            }
            return -1;
        }
        /// <summary>
        /// returns table by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Table GetTableByName(string name)
        {
            if (TablesDB.Count != 0)
            {
                if (isTableExists(name))
                {
                    return TablesDB[indexOfTable(name)];
                }
                throw new NullReferenceException("There's no such table in Database");
            }
            throw new NullReferenceException("There's no tables in Database");
        }

        public override string ToString()
        {
            string info = "|DATABASE| " + Name + " contains " + TablesDB.Count + " tables ";
            for (int i = 0; i < TablesDB.Count; i++)
            {
                info += "\n"+ TablesDB[i].ToString();
            }
            info += "\n|DATABASE| "+Name +" END\n";
            return info; 
        }

        //query methods
        /// <summary>
        /// selects columns from a table and return result
        /// </summary>
        /// <param name="ColumnNames"></param>
        /// <param name="tableForQuery"></param>
        /// <returns></returns>
        public Table QueryColumnSelection(List<string> ColumnNames, Table tableForQuery)
        {
            foreach (string name in ColumnNames) if (!name.Contains(".") || !tableForQuery.isColumnExists(name)) throw new ArgumentException("Invalid column name!");
            Table queryresult = new Table("queryResult", false);
            foreach (string name in ColumnNames)
            {
                Column oldColumn = tableForQuery.GetColumnByName(name);
                Column toAdd = new Column(oldColumn.SystemName, oldColumn.DataType, oldColumn.AllowsNull, oldColumn.Default, queryresult);
                toAdd.DataList = oldColumn.CloneData();
                queryresult.Columns.Add(toAdd);
            }
            return queryresult;

        }

        /// <summary>
        /// Sorts table by values in column
        /// </summary>
        /// <param name="columnNameSortBy"></param>
        /// <param name="tableForQuery"></param>
        /// <param name="isAscending"></param>
        /// <returns></returns>
        public Table QuerySortTable(string columnNameSortBy, Table tableForQuery, bool isAscending)
        {
            Table queryResult = new Table(tableForQuery);
            if (queryResult.isColumnExists(columnNameSortBy))
            {
                List<DataObject[]> data = new List<DataObject[]>();
                for (int i = 0; i < queryResult.Columns[0].DataList.Count; i++)
                    data.Add(queryResult.GetDataByIndex(i));

                int columnSortIndex = queryResult.getIndexOfColumn(columnNameSortBy);
                Column columnToSort = queryResult.Columns[columnSortIndex];
                List<object> columnData = new List<object> ();

                for (int i = 0; i < columnToSort.DataList.Count; i++)
                    columnData.Add(columnToSort.DataList[i].Data);
                columnData.Sort();
                if (!isAscending) columnData.Reverse();
                for (int i = 0; i < columnData.Count; i++)
                {
                    for (int j = 0; j < data.Count; j++)
                    {
                        object[] dataArray = new object[queryResult.Columns.Count];
                        bool finded = false;
                        if (data[j][columnSortIndex].Data == columnData[i])
                        {
                            queryResult.EditTableElementByIndex(i, data[j]);
                            data.RemoveAt(j);
                            finded = true;
                        }
                        if(finded)
                        break;
                    }
                }
                
            } else throw new ArgumentException("There is no " + columnNameSortBy + " column in " + tableForQuery.Name + "!");
            return queryResult;
        }
        
        /// <summary>
        /// Condition selection
        /// </summary>
        /// <param name="tableForSelection"></param>
        /// <param name="columnName"></param>
        /// <param name="selectOperator"></param>
        /// <param name="selectObject"></param>
        /// <returns></returns>
        public Table QueryWhereConditionSelection(Table tableForSelection, string columnName, string selectOperator, object selectObject)
        {
            Table queryResult = new Table(tableForSelection.Name, false);
            for (int i = 0; i < tableForSelection.Columns.Count; i++)
                queryResult.Columns.Add(new Column(tableForSelection.Columns[i], queryResult));

            if (tableForSelection.isTableContainsData())
            {
                if (queryResult.isColumnExists(columnName))
                {
                    Column queryColumn = queryResult.GetColumnByName(columnName);
                    if (selectOperator.isSelectOperator())
                    {
                        if (queryColumn.DataType == selectObject.GetType())
                            switch (selectOperator)
                            {
                                case "=":
                                    {
                                        if (selectObject.GetType() == typeof(string))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                if ((string)queryColumn.DataList[i].Data != (string)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                        }
                                        else if (selectObject.GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((int)queryColumn.DataList[i].Data != (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        else if (selectObject.GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((double)queryColumn.DataList[i].Data != (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        else if (selectObject.GetType() == typeof(bool))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((bool)queryColumn.DataList[i].Data != (bool)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        return queryResult;
                                    }
                                case "!=":
                                    {
                                        if (selectObject.GetType() == typeof(string))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                if ((string)queryColumn.DataList[i].Data == (string)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                        }
                                        else if (selectObject.GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((int)queryColumn.DataList[i].Data == (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        else if (selectObject.GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((double)queryColumn.DataList[i].Data == (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        else if (selectObject.GetType() == typeof(bool))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((bool)queryColumn.DataList[i].Data == (bool)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        return queryResult;
                                    }
                                case ">":
                                    {
                                        if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                        if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                        Type type = selectObject.GetType();
                                        if (selectObject.GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((int)queryColumn.DataList[i].Data < (int)selectObject || (int)queryColumn.DataList[i].Data == (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        else if (selectObject.GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((double)queryColumn.DataList[i].Data < (double)selectObject || (double)queryColumn.DataList[i].Data == (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        return queryResult;
                                    }
                                case "<":
                                    {
                                        if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                        if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                        Type type = selectObject.GetType();
                                        if (selectObject.GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((int)queryColumn.DataList[i].Data > (int)selectObject || (int)queryColumn.DataList[i].Data == (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        else if (selectObject.GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((double)queryColumn.DataList[i].Data > (double)selectObject || (double)queryColumn.DataList[i].Data == (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        return queryResult;
                                    }
                                case "<=":
                                    {
                                        if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                        if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                        Type type = selectObject.GetType();
                                        if (selectObject.GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((int)queryColumn.DataList[i].Data > (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                        else if (selectObject.GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((double)queryColumn.DataList[i].Data > (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        return queryResult;
                                    }
                                case ">=":
                                    {
                                        if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                        if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                        Type type = selectObject.GetType();
                                        if (selectObject.GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((int)queryColumn.DataList[i].Data < (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        else if (selectObject.GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((double)queryColumn.DataList[i].Data < (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        return queryResult;
                                    }
                            }
                        else throw new ArgumentException("Invalid object for select!");
                    }
                    else { if(selectOperator.isSelectComplexOperator()) throw new ArgumentException(selectOperator+ " is complex operator, you need more selectObjects to use it!"); else throw new ArgumentException("Invalid operator"); }
                }
                else throw new ArgumentException("There is no " + columnName + " column in " + tableForSelection.Name + " table");
            }
            else return tableForSelection;
            return queryResult;
        }

        public Table QueryWhereConditionSelection(Table tableForSelection, string columnName, string selectOperator, object[] selectObjects)
        {
            Table queryResult = new Table(tableForSelection.Name, false);
            for (int i = 0; i < tableForSelection.Columns.Count; i++)
                queryResult.Columns.Add(new Column(tableForSelection.Columns[i], queryResult));

            if (tableForSelection.isTableContainsData())
            {
                if (queryResult.isColumnExists(columnName))
                {
                    Column queryColumn = queryResult.GetColumnByName(columnName);
                    if (selectOperator.isSelectComplexOperator())
                    {
                        Type typeColumn = queryColumn.DataType;
                        if (selectObjects.Length == 0) throw new ArgumentException("There is no objects to select!");
                        if (selectObjects.IsArrayContainOnlyTValues(typeColumn))
                        {
                            switch (selectOperator)
                            {
                                case "BETWEEN":
                                    {
                                        if (typeColumn == typeof(string)) throw new ArgumentException("String value is not compatible with BETWEEN operator!");
                                        if (typeColumn == typeof(bool)) throw new ArgumentException("Bool value is not compatible with BETWEEN operator!");

                                        Array.Sort(selectObjects);
                                        if (selectObjects.Length != 2) throw new ArgumentException("Complex select operator " + selectOperator + " works only with 2 objects for selection!");
                                        if (selectObjects[0].GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((int)queryColumn.DataList[i].Data <= (int)selectObjects[0]|| (int)queryColumn.DataList[i].Data >= (int)selectObjects[1]) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        else if (selectObjects[0].GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((double)queryColumn.DataList[i].Data <= (double)selectObjects[0] || (double)queryColumn.DataList[i].Data >= (double)selectObjects[1]) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        return queryResult;
                                    }
                                case "IN":
                                    {
                                        Array.Sort(selectObjects);
                                        if (selectObjects[0].GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if(!selectObjects.IsArrayContainThisValue((int)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                        else if (selectObjects[0].GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if (!selectObjects.IsArrayContainThisValue((double)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                        }
                                        else if (selectObjects[0].GetType() == typeof(string))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if (!selectObjects.IsArrayContainThisValue((string)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                        }
                                        else if (selectObjects[0].GetType() == typeof(bool))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if (!selectObjects.IsArrayContainThisValue((bool)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                        }
                                        return queryResult;
                                    }
                                case "NOT_BETWEEN":
                                    {
                                        if (typeColumn == typeof(string)) throw new ArgumentException("String value is not compatible with NOT_BETWEEN operator!");
                                        if (typeColumn == typeof(bool)) throw new ArgumentException("Bool value is not compatible with NOT_BETWEEN operator!");

                                        Array.Sort(selectObjects);
                                        if (selectObjects.Length != 2) throw new ArgumentException("Complex select operator " + selectOperator + "works only with 2 objects for selection!");
                                        if (selectObjects[0].GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((int)queryColumn.DataList[i].Data > (int)selectObjects[0] && (int)queryColumn.DataList[i].Data < (int)selectObjects[1]) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        else if (selectObjects[0].GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if ((double)queryColumn.DataList[i].Data > (double)selectObjects[0] && (double)queryColumn.DataList[i].Data < (double)selectObjects[1]) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                        }
                                        return queryResult;
                                    }
                                case "NOT_IN":
                                    {
                                        Array.Sort(selectObjects);
                                        if (selectObjects[0].GetType() == typeof(int))
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if (selectObjects.IsArrayContainThisValue((int)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                        else if (selectObjects[0].GetType() == typeof(double))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if (selectObjects.IsArrayContainThisValue((double)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                        }
                                        else if (selectObjects[0].GetType() == typeof(string))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if (selectObjects.IsArrayContainThisValue((string)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                        }
                                        else if (selectObjects[0].GetType() == typeof(bool))
                                        {
                                            for (int i = 0; i < queryColumn.DataList.Count; i++)
                                            {
                                                if (selectObjects.IsArrayContainThisValue((bool)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                        }
                                        return queryResult;
                                    }
                            } 

                        }  else throw new ArgumentException("Invalid objects for select!");

                    }
                    else { if (selectOperator.isSelectComplexOperator()) throw new ArgumentException(selectOperator + " is complex operator, you need more selectObjects to use it!"); else throw new ArgumentException("Invalid operator"); }
                }
                else throw new ArgumentException("There is no " + columnName + " column in " + tableForSelection.Name + " table");
            }
            else return tableForSelection;
            return queryResult;
        }
    }
    
}
