using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Transversals.Business.Core.Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public sealed class MaxValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MaxValueException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MaxValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private MaxValueException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
