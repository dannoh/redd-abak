using System;

namespace AbakHelper.Integration.Models
{
    public class EventArgs<T> : EventArgs
    {
        public T Data { get; set; }

        public EventArgs(T data)
        {
            Data = data;
        }
    }
}
