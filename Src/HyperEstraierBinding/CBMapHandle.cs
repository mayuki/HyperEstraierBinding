using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// cbmapopen �ŊJ���ꂽ�|�C���^�����b�v���܂��B�s�v�ɂȂ������_�� cbmapclose �ŉ�����܂��B
    /// </summary>
    internal class CBMapHandle : SafeHandle
    {
        private CBMapHandle()
            : base(IntPtr.Zero, true)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ownsHandle">CBMapHandle �ɂ���ă|�C���^���������ꍇ�� true�B����ȊO�̏ꍇ(�������Ԃ�Hyper Estraier���Ǘ����Ă���ꍇ��)�� false �B</param>
        public CBMapHandle(IntPtr handle, Boolean ownsHandle)
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
            Unmanaged.cbmapclose(handle);
            return true;
        }
    }
}
