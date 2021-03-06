﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using Microsoft.Common.Core;
using Microsoft.Common.Core.UI.Commands;
using Microsoft.R.Components.InteractiveWorkflow;
using Microsoft.R.Host.Client;

namespace Microsoft.R.Components.Help.Commands {
    public sealed class HelpHomeCommand : IAsyncCommand {
        private readonly ExportProvider _exportProvider;

        public HelpHomeCommand(ExportProvider exportProvider) {
            _exportProvider = exportProvider;
        }

        public CommandStatus Status => CommandStatus.SupportedAndEnabled;

        public async Task InvokeAsync() {
            await TaskUtilities.SwitchToBackgroundThread();

            var workflow = _exportProvider.GetExportedValue<IRInteractiveWorkflowProvider>().GetOrCreate();
            var session = workflow.RSession;
            if (session.IsHostRunning) {
                try {
                    await session.EvaluateAsync("help.start()", REvaluationKind.Normal);
                } catch (OperationCanceledException) { }
            }
        }
    }
}
