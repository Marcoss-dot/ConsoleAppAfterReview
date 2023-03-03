namespace ConsoleApp
{
    class ImportedObject : ImportedObjectBaseClass
    {
        public int NumberOfChildren { get; set; }

        public ImportedObject(ImportedObjectBaseClass baseClass, int numberOfChildren)
        {
            Name = baseClass.Name;
            Type= baseClass.Type;
            Schema = baseClass.Schema;
            ParentName = baseClass.ParentName;
            ParentType = baseClass.ParentType;
            DataType = baseClass.DataType;
            IsNullable = baseClass.IsNullable;

            NumberOfChildren = numberOfChildren;
        }
        public ImportedObject()
        {
        }
    }
}
