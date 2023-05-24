﻿namespace BE.Response
{
    public class BaseResponse<T>
    {
        private bool success;
        private string message;
        private int id;

        public bool _success { get; set; }
        public string _Message { get; set; }
        public T _Data { get; set; }

        public BaseResponse(bool success, string message, T data)
        {
            _success = success;
            _Message = message;
            _Data = data;
        }

        public BaseResponse(bool success, string message, int id)
        {
            this.success = success;
            this.message = message;
            this.id = id;
        }
    }
}
