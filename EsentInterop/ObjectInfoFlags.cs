﻿//-----------------------------------------------------------------------
// <copyright file="ObjectInfoFlags.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Isam.Esent.Interop
{
    using System;

    /// <summary>
    /// Flags for ESENT objects (tables).
    /// </summary>
    [Flags]
    internal enum ObjectInfoFlags : uint
    {
        /// <summary>
        /// Object is for internal use only.
        /// </summary>
        System = 0x80000000,

        /// <summary>
        /// Table's DDL is fixed.
        /// </summary>
        TableFixedDDL = 0x40000000,

        /// <summary>
        /// Table's DDL is inheritable (implied FixedDDL).
        /// </summary>
        TableTemplate = 0x20000000,

        /// <summary>
        /// Table's DDL is inherited from a template table.
        /// </summary>
        TableDerived = 0x10000000,

        /// <summary>
        /// Used in conjunction with JET_bitObjectTableTemplate.
        /// </summary>
        TableNoFixedVarColumnsInDerivedTables = 0x04000000,
    }
}
