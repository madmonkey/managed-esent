﻿//-----------------------------------------------------------------------
// <copyright file="jet_retinfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
    /// <summary>
    /// The native version of the JET_RETINFO structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NATIVE_RETINFO
    {
        public uint cbStruct;
        public uint ibLongValue;
        public uint itagSequence;
        public uint columnidNextTagged;
    }

    /// <summary>
    /// Contains optional input and output parameters for JetRetrieveColumn.
    /// </summary>
    public class JET_RETINFO
    {
        /// <summary>
        /// Gets or sets the offset to the first byte to be retrieved from a column of
        /// type JET_coltyp.LongBinary, or JET_coltyp.LongText
        /// </summary>
        public int ibLongValue { get; set; }

        /// <summary>
        /// Gets or sets the sequence number of value in a multi-valued column.
        /// Note that the array of values is one-based. The first value is
        /// sequence 1, not 0. If the record column has only one value then
        /// 1 should be passed as the itagSequence.
        /// </summary>
        public int itagSequence { get; set; }

        /// <summary>
        /// Gets the columnid of the retrieved tagged, multi-valued or
        /// sparse, column when all tagged columns are retrieved by passing
        /// 0 as the columnid to JetRetrieveColumn.
        /// </summary>
        public JET_COLUMNID columnidNextTagged { get; private set; }

        /// <summary>
        /// Get a NATIVE_RETINFO structure representing the object.
        /// </summary>
        /// <returns>A NATIVE_RETINFO whose members match the class.</returns>
        internal NATIVE_RETINFO GetNativeRetinfo()
        {
            var retinfo = new NATIVE_RETINFO();
            retinfo.cbStruct = (uint)Marshal.SizeOf(retinfo);
            retinfo.ibLongValue = (uint)this.ibLongValue;
            retinfo.itagSequence = (uint)this.itagSequence;
            return retinfo;
        }

        /// <summary>
        /// Sets the fields of the object from a NATIVE_RETINFO structure.
        /// </summary>
        /// <param name="value">The NATIVE_RETINFO which will be used to set the fields.</param>
        internal void SetFromNativeRetinfo(NATIVE_RETINFO value)
        {
            this.ibLongValue = (int)value.ibLongValue;
            this.itagSequence = (int)value.itagSequence;

            var columnid = new JET_COLUMNID();
            columnid.Value = value.columnidNextTagged;
            this.columnidNextTagged = columnid;
        }
    }
}