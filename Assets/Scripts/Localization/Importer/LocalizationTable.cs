namespace CuroLocalization
{
    //first dimension is column, second is row

    public sealed class Table
    {
        private readonly string[,] _data;
        
        public readonly TableIndex ByTableIndex;
        
        public Table(string[,] data)
        {
            _data = data;
            ByTableIndex = new(this);
        }
        
        public string GetValue(int columnIndex, int rowIndex) => _data[columnIndex, rowIndex];
        public int GetLength(int dimensionIndex) => _data.GetLength(dimensionIndex);
    }
    
    public sealed class TableIndex
    {
        private readonly Table _table;
        
        public TableIndex(Table table)
        {
            _table = table;
        }
        
        public string this[int columnIndex, int rowIndex] => _table.GetValue(columnIndex, rowIndex); 
    }
}