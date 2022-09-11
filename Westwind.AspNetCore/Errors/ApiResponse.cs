namespace Westwind.AspNetCore.Messages
{
    /// <summary>
    /// A base API response class that communicates error status
    /// and data property that holds the actual data
    /// </summary>
    public class ApiResponse
    {
        public bool IsError { get; set;  }
        public string StatusCode { get; set; }
        public string Message { get; set;  }
        public object Data { get; set; }


        public void SetResult(object data, string message = null)
        {
            IsError = false;
            Message = message;
            Data = data;
        }

        public void SetError(string message, object data = null)
        {
            IsError = true;
            Message = message;
            if (data != null)
                Data = data;
        }

        public void Clear()
        {
            IsError = false;
            Message = null;
            StatusCode = null;
            Data = null;
        }

    }

    /// <summary>
    /// A base API response class that communicates error status
    /// and data property that holds the actual data
    /// </summary>
    public class ApiResponse<TData>
    {
        public bool IsError { get; set;  }
        public string StatusCode { get; set; }
        public string Message { get; set;  }
        public TData Data { get; set; }
    }
}
