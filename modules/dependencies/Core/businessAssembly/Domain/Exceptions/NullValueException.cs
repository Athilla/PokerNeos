using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Transversals.Business.Core.Domain.Exceptions
{
    /// <summary>
    /// Represents a null value exception.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public sealed class NullValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NullValueException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NullValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private NullValueException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
