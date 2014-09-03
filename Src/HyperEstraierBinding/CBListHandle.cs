using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// cblistopen で開かれたポインタをラップします。不要になった時点で cblistclose で解放します。
    /// </summary>
    internal class CBListHandle : SafeHandle
    {
        private CBListHandle()
            : base(IntPtr.Zero, true)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ownsHandle"> CBListHandle によってポインタを解放する場合は true。それ以外の場合(生存期間をHyper Estraierが管理している場合等)は false 。</param>
        public CBListHandle(IntPtr handle, Boolean ownsHandle)
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
            Unmanaged.cblistclose(handle);
            return true;
        }
    }
}
