namespace ConnectMyDoc_API_Layer.Models
{
    public class PaginatedResult<T>
    {
      
        public int TotalCountOfPatients { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
