﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.R.Core.AST;
using Microsoft.R.Core.AST.Functions.Definitions;
using Microsoft.R.Core.AST.Scopes.Definitions;
using Microsoft.R.Support.Help.Definitions;
using Microsoft.R.Support.Help.Functions;

namespace Microsoft.R.Editor {
    public static class AstRootExtensions {
        public static IFunctionInfo GetUserFunctionInfo(this AstRoot ast, string functionName, int position) {
            var scope = ast.GetNodeOfTypeFromPosition<IScope>(position);
            if (scope == null) {
                return null;
            }
            var fd = scope.FindFunctionByName(functionName, position)?.Value as IFunctionDefinition;
            if (fd != null) {
                var fi = new FunctionInfo(functionName);
                fi.Signatures = new ISignatureInfo[] { fd.MakeSignature(functionName) };
                return fi;
            }
            return null;
        }
    }
}
