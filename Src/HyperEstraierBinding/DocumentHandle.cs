using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// est_doc_new で開かれたポインタをラップします。不要になった時点で est_doc_delete で解放します。
    /// </summary>
    internal class DocumentHandle : SafeHandle
    {
        private DocumentHandle()
            : base(IntPtr.Zero, true)
        {
        }
        public DocumentHandle(IntPtr handle, Boolean ownsHandle)
            : base(IntPtr.Zero, ownsHandle)
        {
            SetHandle(handle);
        }
        public override bool IsInvalid
        {
            get { return IsClosed || handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            Unmanaged.est_doc_delete(handle);
            return true;
        }
    }
}
