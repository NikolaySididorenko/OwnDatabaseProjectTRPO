﻿using DataLayer;
using SecurityLayer.Modules;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataAccessLayer.Modules
{
    static class CacheModule
    {
       /// <summary>
       /// DataBase file saving
       /// </summary>
       /// <param name="_dataToSave"></param>
       /// <param name="_dataBaseName"></param>
        static internal void SaveDataBaseToFolder(this DataBaseInstance db)
        {
            CreateDirectoryForDataBaseIfThereAreNoOne();
            OpenFileAndWriteEncryptedDb(db);
        }

        private static void OpenFileAndWriteEncryptedDb(DataBaseInstance db)
        {
            using (FileStream _fileStream = new FileStream("./DataBases/" + db.Name + ".soos", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                EncryptAndWriteDbToFile(_fileStream, db);
                _fileStream.Close();
            }
        }

        /// <summary>
        /// Call EncryptionModule.EncryptDataBase for encryption purposes then save encrypted DB to file.
        /// </summary>
        /// <param name="fileStream">Stay at open state after method call </param>
        /// <param name="dataBaseToWrite"></param>
        private static void EncryptAndWriteDbToFile(FileStream fileStream, DataBaseInstance dataBaseToWrite)
        {

            MemoryStream streamOfEncryptedDataBase = EncryptionModule.EncryptDataBase(dataBaseToWrite);
            streamOfEncryptedDataBase.Position = 0;
            streamOfEncryptedDataBase.WriteTo(fileStream);
        }

        //
        /// <summary>
        /// Saves or updates all databases from _instance list to folder
        /// </summary>
        internal static void SaveAllDatabases(List<DataBaseInstance> listDB)
        {
            try
            {
                if (listDB.Count != 0)
                {
                    CreateDirectoryForDataBaseIfThereAreNoOne();
                    DirectoryInfo dirInfo = new DirectoryInfo("./DataBases/");
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DataBaseInstance bufInst in listDB)
                        bufInst.SaveDataBaseToFolder();
                }
                else throw new ArgumentNullException("There is no Databases to save!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void CreateDirectoryForDataBaseIfThereAreNoOne()
        {
            if (!SharedDataAccessMethods.isDirectoryExists())
                SharedDataAccessMethods.CreateDatabasesDirectory();
        }

    }
}
