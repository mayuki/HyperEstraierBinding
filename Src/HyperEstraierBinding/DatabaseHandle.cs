using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// cbmapopen �ŊJ���ꂽ�|�C���^�����b�v���܂��B�s�v�ɂȂ������_�� cbmapclose �ŉ�����܂��B
    /// </summary>
    internal class DatabaseHandle : SafeHandle
    {
        private DatabaseHandle()
            : base(IntPtr.Zero, true)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ownsHandle">DatabaseHandle �ɂ���ă|�C���^���������ꍇ�� true�B����ȊO�̏ꍇ(�������Ԃ�Hyper Estraier���Ǘ����Ă���ꍇ��)�� false �B</param>
        public DatabaseHandle(IntPtr handle, Boolean ownsHandle)
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
            Int32 errorCode;
            // TODO: �G���[�R�[�h
            return Unmanaged.est_db_close(handle, out errorCode);
        }
    }
}
