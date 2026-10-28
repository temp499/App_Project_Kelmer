namespace App_Proyecto.DTOS
{
    public class DataTableResponseDto<T>
    {

        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsTotalFiltered { get; set; }
        public List<T> Data { get; set; } = [];
    }
}
