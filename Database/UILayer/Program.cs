﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;
using DataLayer.Shared.ExtentionMethods;

namespace UILayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Interpreter.Run();



            // Interpreter.Run();
            //Kernel.AddDBInstance("inst2");
            //Kernel.AddDBInstance("inst2");
            //var inst = Kernel.GetInstance("inst2");

            //    //Kernel.SaveAllDatabases();
            //    //    Kernel.OutDatabaseInfo();
            //    // Console.WriteLine(inst.GetTableByName("Cars").OutTable());
            //    // Interpreter.Run();



            //var inst = Kernel.GetInstance("inst2");
            //inst.LinkTables(inst.GetTableByName("Persons"), inst.GetTableByName("Cars"), true);
            //inst.GetTableByName("Persons").GetColumnByName("FK_IDCars").EditColumnElementByPrimaryKey(1, 1.toObjectArray());
            //inst.GetTableByName("Persons").GetColumnByName("FK_IDCars").EditColumnElementByPrimaryKey(2, 1.toObjectArray());
            //inst.GetTableByName("Persons").GetColumnByName("FK_IDCars").EditColumnElementByPrimaryKey(3, 2.toObjectArray());
            //inst.GetTableByName("Cars").GetColumnByName("Man").SetNullableProperty(true);
            //inst.GetTableByName("Cars").GetColumnByName("Man").EditColumnElementByPrimaryKey(1, new object[] { null });
            //inst.GetTableByName("Cars").GetColumnByName("Price").SetNullableProperty(true);

            //inst.GetTableByName("Cars").GetColumnByName("Price").EditColumnElementByIndex(0, new object[] { null });
            //string l = "OK";
            //inst.setNewTable(inst.GetTableByName("Cars"), inst.QuerySortTable("Price", inst.GetTableByName("Cars"), true, ref l));
            //Kernel.OutDatabaseInfo();

            //string ou = "OK";
            ////Table query = inst.QueryInnerJoinSelection(inst.GetTableByName("Cars"), inst.GetTableByName("Persons"), new List<string> {"Persons.FK_IDCars", "Cars.IDCars" },ref ou);

            //Kernel.OutDatabaseInfo();
            //Console.WriteLine(query.OutTable());
            //Table query = inst.QueryWhereConditionSelection(inst.GetTableByName("Cars"), "Cars.DoubleValue", "NOT_BETWEEN",  new object[] { 0.0 , 120.5} );
           // query = inst.QuerySortTable("Cars.CarMark", query, false);
           // Console.WriteLine(query.OutTable());


        }

    }

   
}
