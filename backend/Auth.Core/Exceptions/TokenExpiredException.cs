using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Exceptions
{
    /// <summary>
    /// Exception levée lorsqu'un token est expiré.
    /// </summary>
    public class TokenExpiredException : Exception
    {
        public TokenExpiredException()
            : base("Le token a expiré")
        {
        }

        public TokenExpiredException(string message)
            : base(message)
        {
        }

        public TokenExpiredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
