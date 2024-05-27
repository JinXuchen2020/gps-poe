namespace POEMgr.Application.TransferModels
{
    public class PaginationList<T>
    {
        public IEnumerable<T> List { get; set; }

        public int Total { get; set; }
    }
}
