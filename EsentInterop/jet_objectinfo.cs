﻿//-----------------------------------------------------------------------
// <copyright file="jet_objectinfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Isam.Esent.Interop
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The native version of the JET_OBJECTINFO structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.NamingRules",
        "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",
        Justification = "This should match the unmanaged API, which isn't capitalized.")]
    internal struct NATIVE_OBJECTINFO
    {
        /// <summary>
        /// Size of the structure.
        /// </summary>
        public uint cbStruct;

        /// <summary>
        /// Holds the JET_OBJTYP of the structure. Currently only tables will be
        /// returned (that is, <see cref="JET_objtyp.Table"/>.
        /// </summary>
        public uint objtyp;

        /// <summary>
        /// Obsolete. Do not use.
        /// </summary>
        public double ignored1;

        /// <summary>
        /// Obsolete. Do not use.
        /// </summary>
        public double ignored2;

        /// <summary>
        /// A group of bits that contain table options.
        /// </summary>
        public uint grbit;

        /// <summary>
        /// Table type flags.
        /// </summary>
        public uint flags;

        /// <summary>
        /// Number of records in the table.
        /// </summary>
        public uint cRecord;

        /// <summary>
        /// Number of pages used by the table.
        /// </summary>
        public uint cPage;
    }

    /// <summary>
    /// The JET_OBJECTINFO structure holds information about an object.
    /// Tables are the only object types that are currently supported.
    /// </summary>
    public class JET_OBJECTINFO
    {
        /// <summary>
        /// Sets the fields of the object from a native JET_OBJECTINFO struct.
        /// </summary>
        /// <param name="value">
        /// The native objectlist to set the values from.
        /// </param>
        internal void SetFromNativeObjectinfo(NATIVE_OBJECTINFO value)
        {
        }
    }
}