﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace Microsoft.UnitTests.Core.XUnit
{
    [ExcludeFromCodeCoverage]
    public class TestForTypesParameters : TestParameters
    {
        public TestForTypesParameters(ITestMethod testMethod, IAttributeInfo factAttribute) : base(testMethod, factAttribute)
        {
            Types = factAttribute.GetNamedArgument<Type[]>("Types");
        }

        public Type[] Types { get; set; }
    }
}