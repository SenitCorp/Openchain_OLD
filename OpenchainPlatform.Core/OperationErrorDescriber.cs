using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core
{
    public class OperationErrorDescriber
    {
        /// <summary>
        /// Returns the default <see cref="OperationError"/>.
        /// </summary>
        /// <returns>The default <see cref="OperationError"/>.</returns>
        public virtual OperationError DefaultError()
        {
            return new OperationError
            {
                Code = nameof(DefaultError),
                Description = Resources.DefaultError
            };
        }

        /// <summary>
        /// Returns an <see cref="OperationError"/> indicating a concurrency failure.
        /// </summary>
        /// <returns>An <see cref="OperationError"/> indicating a concurrency failure.</returns>
        public virtual OperationError ConcurrencyFailure()
        {
            return new OperationError
            {
                Code = nameof(ConcurrencyFailure),
                Description = Resources.ConcurrencyFailure
            };
        }
    }
}
