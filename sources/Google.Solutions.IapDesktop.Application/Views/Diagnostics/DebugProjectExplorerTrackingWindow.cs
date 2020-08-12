﻿//
// Copyright 2020 Google LLC
//
// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
//

using Google.Solutions.Common.Diagnostics;
using Google.Solutions.IapDesktop.Application.ObjectModel;
using Google.Solutions.IapDesktop.Application.Services.Integration;
using Google.Solutions.IapDesktop.Application.Views.ProjectExplorer;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Google.Solutions.IapDesktop.Application.Views.Diagnostics
{
    [ComVisible(false)]
    [SkipCodeCoverage("For debug purposes only")]
    public partial class DebugProjectExplorerTrackingWindow
        : ProjectExplorerTrackingToolWindow<DebugProjectExplorerTrackingViewModel>
    {
        public DebugProjectExplorerTrackingWindow(IServiceProvider serviceProvider)
            : base(
                  serviceProvider.GetService<IMainForm>().MainPanel,
                  serviceProvider.GetService<IProjectExplorer>(),
                  serviceProvider.GetService<IEventService>(),
                  serviceProvider.GetService<IExceptionDialog>())
        {
            InitializeComponent();
        }

        //---------------------------------------------------------------------
        // ProjectExplorerTrackingToolWindow.
        //---------------------------------------------------------------------

        protected override Task SwitchToNodeAsync(IProjectExplorerNode node)
        {
            if (node is IProjectExplorerVmInstanceNode vmNode)
            {
                this.instanceNameLabel.Text = vmNode.InstanceName;
            }
            else
            {
                // We cannot handle other types or node, so ignore.
            }

            return Task.CompletedTask;
        }
    }
}