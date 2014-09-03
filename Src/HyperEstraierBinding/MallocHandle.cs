using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// malloc ���ꂽ�|�C���^�����b�v���܂��B
    /// ���̃N���X�Ń|�C���^�̐������Ԃ��Ǘ�����ꍇ�A�|�C���^���s�v�ɂȂ������_�� free ���܂��B
    /// </summary>
    internal class MallocHandle : SafeHandle
    {
        /// <summary>
        /// MallocHandle �̃C���X�^���X�𐶐����܂�(�v���b�g�t�H�[���Ăяo����)�B
        /// </summary>
        private MallocHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// MallocHandle �̃C���X�^���X�𐶐����܂��B
        /// </summary>
        /// <param name="handle">malloc �Ŏ擾���ꂽ�|�C���^</param>
        /// <param name="ownsHandle"> MallocHandle �ɂ���ă|�C���^���������ꍇ�� true�B����ȊO�̏ꍇ(�������Ԃ�Hyper Estraier���Ǘ����Ă���ꍇ��)�� false �B</param>
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
