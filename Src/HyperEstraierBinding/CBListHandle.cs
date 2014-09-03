using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// cblistopen �ŊJ���ꂽ�|�C���^�����b�v���܂��B�s�v�ɂȂ������_�� cblistclose �ŉ�����܂��B
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
        /// <param name="ownsHandle"> CBListHandle �ɂ���ă|�C���^���������ꍇ�� true�B����ȊO�̏ꍇ(�������Ԃ�Hyper Estraier���Ǘ����Ă���ꍇ��)�� false �B</param>
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
