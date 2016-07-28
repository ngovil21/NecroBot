using System;
using System.Runtime.Serialization;

namespace PoGo.NecroBot.Logic.State
{
    [Serializable]
    internal class GoogleException : Exception
    {
        public GoogleException()
        {
        }

        public GoogleException(string message) : base(message)
        {
        }

        public GoogleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GoogleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}