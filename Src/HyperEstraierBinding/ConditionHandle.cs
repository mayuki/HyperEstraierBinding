using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// est_cond_new �ŊJ���ꂽ�|�C���^�����b�v���܂��B�s�v�ɂȂ������_�� est_cond_delete �ŉ�����܂��B
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
        /// <param name="ownsHandle">ConditionHandle �ɂ���ă|�C���^���������ꍇ�� true�B����ȊO�̏ꍇ(�������Ԃ�Hyper Estraier���Ǘ����Ă���ꍇ��)�� false �B</param>
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
