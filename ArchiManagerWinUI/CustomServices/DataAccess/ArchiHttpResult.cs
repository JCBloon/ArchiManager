using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ArchiManagerWinUI.CustomServices.DataAccess
{
    public class ArchiHttpResult<T>
    {
        public T? Value { get; set; }
        public string? Error { get; set; }
        public bool? IsSuccess { get; set; }
        public bool? HasNext { get; set; }
        public bool? NoConnection { get; set; }

        // Con static facilitamos crear la clase, ya que no se necesitaría el "new" en "new ArchiHttpResult"
        // Por defecto value es "default", ya que no existe un tipo específico para un conjunto genérico como <T>
        public static ArchiHttpResult<T> Success(T? value = default, bool? hasNext = null)
        {
            return new ArchiHttpResult<T> { Value = value, IsSuccess = true, HasNext = hasNext };
        }
        public static ArchiHttpResult<T> Failure(string? error)
        {
            return new ArchiHttpResult<T> { Error = error, IsSuccess = false };
        }
        public static ArchiHttpResult<T> ConnectionError(string? error = null)
        {
            return new ArchiHttpResult<T> { Error = error, NoConnection = true, IsSuccess = false };
        }
    }
}
