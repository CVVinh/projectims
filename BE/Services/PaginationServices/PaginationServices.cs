﻿using BE.Response;

namespace BE.Services.PaginationServices
{
    public interface IPaginationServices<T>
    {
        Task<PaginationResponse<ICollection<T>>> paginationListTableAsync(ICollection<T> tasksList, int? pageIndex, int pageSize);
    }

    public class PaginationServices<T> : IPaginationServices<T>
    {
        public Task<PaginationResponse<ICollection<T>>> paginationListTableAsync(ICollection<T> tasksList, int? pageIndex, int pageSize)
        {
            var success = true;
            var message = "Get all data";
            var data = tasksList;
            var toPage = 0.00;
            var totalPage = 0;
            var totalItems = tasksList.Count();
            var itemIndex = 0;
            if (pageSize > 0)
            {
                toPage = Math.Ceiling(tasksList.ToList().Count / (float)pageSize);
                totalPage = (int)toPage;
            }
            if (!pageIndex.HasValue)
            {
                var result = new PaginationResponse<ICollection<T>>(success, message, data, totalPage, totalItems, itemIndex);
                return Task.FromResult(result);
            }

            if (tasksList.Any())
            {
                //if ((double)pageIndex > toPage || pageIndex <= 0)
                //{
                //    success = false;
                //    message = "Trang này không tồn tại!";
                //    data = null;
                //    var result = new PaginationResponse<ICollection<T>>(success, message, data, totalPage);
                //    return Task.FromResult(result);
                //}

                itemIndex = (pageIndex.Value - 1) * pageSize + 1;

                message = $"Nhận tất cả dữ liệu trong trang {pageIndex}";
                data = tasksList.Skip((pageIndex.Value - 1) * pageSize).Take(pageSize).ToList();
                var resultPage = new PaginationResponse<ICollection<T>>(success, message, data, totalPage, totalItems, itemIndex);
                return Task.FromResult(resultPage);
            }

            return Task.FromResult(new PaginationResponse<ICollection<T>>(success, message, data, totalPage, totalItems, itemIndex));
        }

    }
}
