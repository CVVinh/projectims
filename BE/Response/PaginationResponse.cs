namespace BE.Response
{
    public class PaginationResponse<T> : BaseResponse<T>
    {
        public int _totalPages { get; set; }
        public int _totalItems { get; set; }
        public int _itemIndex { get; set; }
        public PaginationResponse(bool success, string message, T data, int totalPages, int totalItems, int itemIndex) : base(success, message, data)
        {
            _totalPages = totalPages;
            _totalItems = totalItems;
            _itemIndex = itemIndex;
        }
    }
}
