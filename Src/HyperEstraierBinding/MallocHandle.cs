using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// malloc されたポインタをラップします。
    /// このクラスでポインタの生存期間を管理する場合、ポインタが不要になった時点で free します。
    /// </summary>
    internal class MallocHandle : SafeHandle
    {
        /// <summary>
        /// MallocHandle のインスタンスを生成します(プラットフォーム呼び出し時)。
        /// </summary>
        private MallocHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// MallocHandle のインスタンスを生成します。
        /// </summary>
        /// <param name="handle">malloc で取得されたポインタ</param>
        /// <param name="ownsHandle"> MallocHandle によってポインタを解放する場合は true。それ以外の場合(生存期間をHyper Estraierが管理している場合等)は false 。</param>
        public MallocHandle(IntPtr handle, Boolean ownsHandle)
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
            free(handle);
            return true;
        }

        [DllImport("msvcrt")]
        private static extern void free(IntPtr ptr);
    }
}
