using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ConsoleApp
{
    public static class CsvFileHelper
    {
        public static List<T> GetRecords<T, TClassMap>(string csvFilePath)
            where TClassMap : ClassMap
        {
            var validRecords = new List<T>();
            var invalidRecords = new List<string>();
            bool isInvalid = false;

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                BadDataFound = context =>
                {
                    isInvalid = true;
                    invalidRecords.Add(context.RawRecord);
                },
                MissingFieldFound = context =>
                {
                    isInvalid = true;
                    invalidRecords.Add(context.Context.Parser.RawRecord);
                },
            };


            using (var streamReader = File.OpenText(csvFilePath))
            using (var csvReader = new CsvReader(streamReader, configuration))
            {
                csvReader.Context.RegisterClassMap<TClassMap>();

                while (csvReader.Read())
                {
                    var record = csvReader.GetRecord<T>();

                    if (!isInvalid)
                        validRecords.Add(record);

                    isInvalid = false;
                }

                if (invalidRecords.Count != 0)
                {
                    Console.WriteLine($"[WARNING] there are some invalid records:");
                    
                    foreach (var invalidRecord in invalidRecords)
                        Console.WriteLine(invalidRecord);
                }
                
                return validRecords;
            }
        }
    }
}
