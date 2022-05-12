using System.Collections.Generic;
using System.Linq;

namespace classmaker_models.Dtos
{
    public class Result
    {
        public bool IsSuccess => Errors != null && !Errors.Any();
        public List<string> Errors { get; set; }

        public Result()
        {
            Errors = new List<string>();
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }

        public Result()
        {

        }

        public Result(T value)
        {
            Value = value;
        }

        public Result(T value, List<string> errors)
        {
            Value = value;
            Errors = errors;
        }
    }
}