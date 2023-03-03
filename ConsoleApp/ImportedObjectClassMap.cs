using CsvHelper.Configuration;

namespace ConsoleApp
{
    public class ImportedObjectClassMap : ClassMap<ImportedObjectBaseClass>
    {
        public ImportedObjectClassMap()
        {
            Map(i => i.Type).Name("Type").Convert(i => i.Row.GetField("Type").FullTrim().ToUpper());
            Map(i => i.Name).Name("Name").Convert(i => i.Row.GetField("Name").FullTrim());
            Map(i => i.Schema).Name("Schema").Convert(i => i.Row.GetField("Schema").FullTrim());
            Map(i => i.ParentName).Name("ParentName").Convert(i => i.Row.GetField("ParentName").FullTrim());
            Map(i => i.ParentType).Name("ParentType").Convert(i => i.Row.GetField("ParentType").FullTrim());
            Map(i => i.DataType).Name("DataType");
            Map(i => i.IsNullable).Name("IsNullable").Convert(i =>
            {
                var value = i.Row.GetField("IsNullable");
                return value == "1" || value == "nullable";
            });
        }
    }
}
