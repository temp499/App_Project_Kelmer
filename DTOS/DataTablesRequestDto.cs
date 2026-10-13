namespace App_Proyecto.DTOS
{
    public class DataTablesRequestDto
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableSeachDto? Search { get; set; }
        public List<DataTableOrderDto> Order { get; set; } = [];
        public List<DataTableColumnDto> Columns { get; set; } = [];


    }
    public class DataTableSeachDto
    {
        public string? Value { get; set; } = string.Empty;
        public bool Regex { get; set; }
    }

    public class DataTableOrderDto
    {
        public int Column { get; set; }
        public string Dir { get; set; } = "asc";
    }

    public class DataTableColumnDto
    {
        public string? Data { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTableSeachDto? Search { get; set; }
    }



}
