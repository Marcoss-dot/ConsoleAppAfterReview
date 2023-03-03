namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DataReader
    {
        private const string DatabaseType = "DATABASE";

        List<ImportedObject> _importedObjects = new List<ImportedObject>();

        public void ImportData(string fileToImport)
        {
            var importedObjectsBase = CsvFileHelper.GetRecords<ImportedObjectBaseClass, ImportedObjectClassMap>(fileToImport);

            // convert ImportedObjectBaseClass to ImportedObject and assign number of children
            _importedObjects = importedObjectsBase.Select(i => new ImportedObject(i, importedObjectsBase.Count(c => c.ParentType == i.Type && c.ParentName == i.Name))).ToList();
        }

        public void PrintData()
        {
            var _tablesColumns = _importedObjects.GroupJoin(_importedObjects,
                t => (t.Type, t.Name),
                c => (c.ParentType.ToUpper(), c.ParentName),
                (table, columns) => (Table: table, Columns: columns));

            var joinedData = _importedObjects.Where(i => i.Type == DatabaseType).GroupJoin(_tablesColumns,
                d => (d.Type, d.Name),
                tc => (tc.Table.ParentType.ToUpper(), tc.Table.ParentName),
                (database, tables) => (DbName: database.Name, DbChildrenCount: database.NumberOfChildren, Tables: tables));

            foreach (var database in joinedData)
            {
                Console.WriteLine($"Database '{database.DbName}' ({database.DbChildrenCount} tables)");

                // print all database's tables
                foreach (var (table, columns) in database.Tables)
                {
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                    // print all table's columns
                    foreach (var column in columns)
                        Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable ? "accepts nulls" : "with no nulls")}");
                }
            }
        }
    }
}
