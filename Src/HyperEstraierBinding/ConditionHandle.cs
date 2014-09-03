using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// est_cond_new で開かれたポインタをラップします。不要になった時点で est_cond_delete で解放します。
    /// </summary>
    internal class ConditionHandle : SafeHandle
    {
        private ConditionHandle()
            : base(IntPtr.Zero, true)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ownsHandle">ConditionHandle によってポインタを解放する場合は true。それ以外の場合(生存期間をHyper Estraierが管理している場合等)は false 。</param>
        public ConditionHandle(IntPtr handle, Boolean ownsHandle)
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
            Unmanaged.est_cond_delete(handle);
            return true;
        }
    }
}
