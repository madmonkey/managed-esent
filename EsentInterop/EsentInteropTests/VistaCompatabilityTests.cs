﻿//-----------------------------------------------------------------------
// <copyright file="VistaCompatabilityTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InteropApiTests
{
    /// <summary>
    /// Test the Api class functionality when we have an Vista version of Esent.
    /// </summary>
    [TestClass]
    public class VistaCompatabilityTests
    {
        /// <summary>
        /// The saved API, replaced when finished.
        /// </summary>
        private IJetApi savedImpl;

        /// <summary>
        /// Setup the mock object repository.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            this.savedImpl = Api.Impl;
            Api.Impl = new JetApi(Constants.VistaVersion);
        }

        /// <summary>
        /// Cleanup after the test.
        /// </summary>
        [TestCleanup]
        public void Teardown()
        {
            Api.Impl = this.savedImpl;
        }

        /// <summary>
        /// Verify that the Vista version of ESENT does support
        /// large keys.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        public void VerifyVistaDoesSupportLargeKeys()
        {
            Assert.IsTrue(EsentVersion.SupportsLargeKeys);
        }

        /// <summary>
        /// Verify that the Vista version of ESENT does support
        /// Windows Server 2003 features.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        public void VerifyVistaDoesSupportServer2003Features()
        {
            Assert.IsTrue(EsentVersion.SupportsServer2003Features);
        }

        /// <summary>
        /// Verify that the Vista version of ESENT does support
        /// Unicode paths.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        public void VerifyVistaDoesSupportUnicodePaths()
        {
            Assert.IsTrue(EsentVersion.SupportsUnicodePaths);
        }

        /// <summary>
        /// Verify that the Vista version of ESENT does support
        /// Windows Vista features.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        public void VerifyVistaDoesSupportVistaFeatures()
        {
            Assert.IsTrue(EsentVersion.SupportsVistaFeatures);
        }

        /// <summary>
        /// Verify that the Vista version of ESENT doesn't support
        /// Windows 7 features.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        public void VerifyVistaDoesNotSupportWindows7Features()
        {
            Assert.IsFalse(EsentVersion.SupportsWindows7Features);
        }
    }
}