﻿//
// Copyright 2022 Google LLC
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

using Google.Apis.Util;
using Google.Solutions.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Google.Solutions.Ssh.Native
{
    /// <summary>
    /// Represents an open file that can be read from/written to.
    /// </summary>
    public class SshSftpFileChannel : IDisposable
    {
        private readonly SshSession session;
        private readonly SshSftpChannelHandle channelHandle;
        private readonly SshSftpFileHandle fileHandle;
        private readonly string filePath;

        private bool disposed = false;

        private SshException CreateException(LIBSSH2_ERROR error)
        {
            if (error == LIBSSH2_ERROR.SFTP_PROTOCOL)
            {
                return SshSftpNativeException.GetLastError(
                    this.channelHandle,
                    this.filePath);
            }
            else
            {
                return this.session.CreateException((LIBSSH2_ERROR)error);
            }
        }

        internal SshSftpFileChannel(
            SshSession session,
            SshSftpChannelHandle channelHandle,
            SshSftpFileHandle fileHandle,
            string filePath)
        {
            this.session = session;
            this.channelHandle = channelHandle;
            this.fileHandle = fileHandle;
            this.filePath = filePath;
        }

        public uint Read(byte[] buffer)
        {
            this.fileHandle.CheckCurrentThreadOwnsHandle();
            Utilities.ThrowIfNull(buffer, nameof(buffer));

            using (SshTraceSources.Default.TraceMethod().WithoutParameters())
            {
                var bytesRead = UnsafeNativeMethods.libssh2_sftp_read(
                    this.fileHandle,
                    buffer,
                    new IntPtr(buffer.Length));

                if (bytesRead >= 0)
                {
                    return (uint)bytesRead;
                }
                else
                {
                    throw CreateException((LIBSSH2_ERROR)bytesRead);
                }
            }
        }

        public uint Write(byte[] buffer, int length)
        {
            this.channelHandle.CheckCurrentThreadOwnsHandle();
            Utilities.ThrowIfNull(buffer, nameof(buffer));

            Debug.Assert(length <= buffer.Length);

            using (SshTraceSources.Default.TraceMethod().WithoutParameters())
            {
                var bytesWritten = UnsafeNativeMethods.libssh2_sftp_write(
                    this.fileHandle,
                    buffer,
                    new IntPtr(length));

                if (bytesWritten >= 0)
                {
                    return (uint)bytesWritten;
                }
                else
                {
                    throw CreateException((LIBSSH2_ERROR)bytesWritten);
                }
            }
        }

        //---------------------------------------------------------------------
        // Dispose.
        //---------------------------------------------------------------------

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.fileHandle.Dispose();
                this.disposed = true;
            }
        }
    }
}