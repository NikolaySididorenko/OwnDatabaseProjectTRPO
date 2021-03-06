﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataModels.App.Shared.ExtentionMethods
{
    static public class ArrayExtentionMethods
    {
      public static bool  IsArrayContainOnlyTValues(this object[] array, Type T)
      {
            foreach (var item in array)
            {
                if (item != null)
                    if (item.GetType() != T) return false;
            }
            return true;
      }
        public static bool IsArrayContainThisValue(this object[] array, int value)
        {
            foreach (object item in array)
            {
                if(item!=null)
                if ((int)item == value) return true;
            }
            return false;
        }
        public static bool IsArrayContainThisValue(this object[] array, double value)
        {
            foreach (object item in array)
            {
                if (item != null)
                    if ((double)item == value) return true;
            }
            return false;
        }
        public static bool IsArrayContainThisValue(this object[] array, bool value)
        {
            foreach (object item in array)
            {
                if (item != null)
                    if ((bool)item == value) return true;
            }
            return false;
        }
        public static bool IsArrayContainThisValue(this object[] array, string value)
        {
            foreach (object item in array)
            {
                if (item != null)
                    if ((string)item == value) return true;
            }
            return false;
        }

    }
}