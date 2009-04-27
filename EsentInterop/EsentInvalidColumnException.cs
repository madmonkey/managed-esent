﻿//-----------------------------------------------------------------------
// <copyright file="EsentInvalidColumnException.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
    /// <summary>
    /// Base class for ESENT exceptions
    /// </summary>
    [Serializable]
    public class EsentInvalidColumnException : EsentException
    {
        /// <summary>
        /// Initializes a new instance of the EsentInvalidColumnException class.
        /// </summary>
        internal EsentInvalidColumnException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EsentInvalidColumnException class. This constructor
        /// is used to deserialize a serialized exception.
        /// </summary>
        /// <param name="info">The data needed to deserialize the object.</param>
        /// <param name="context">The deserialization context.</param>
        protected EsentInvalidColumnException(SerializationInfo info, StreamingContext context) :
                base(info, context)
        {
        }

        /// <summary>
        /// Gets a text message describing the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return "Column is not valid for this operation";
            }
        }
    }
}
