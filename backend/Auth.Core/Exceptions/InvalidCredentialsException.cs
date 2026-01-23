using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Exceptions
{
    /// <summary>
    /// Exception levée lors de credentials invalides.
    /// </summary>
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Les identifiants fournis sont invalides")
        {
        }

        public InvalidCredentialsException(string message)
            : base(message)
        {
        }

        public InvalidCredentialsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
