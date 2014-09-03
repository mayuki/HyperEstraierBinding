using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Collections;

namespace HyperEstraier
{
    /// <summary>
    /// �������ʂ�\���܂��B
    /// </summary>
    public class Result : IDisposable
    {
        private Int32 _num;
        private MallocHandle _returnHandle;
        private List<KeyValuePair<String, Int32>> _hints;

        internal Result(MallocHandle returnHandle, Int32 num, List<KeyValuePair<String, Int32>> hints)
        {
            _num = num;
            _returnHandle = returnHandle;
            _hints = hints;
        }

        /// <summary>
        /// �������ʂ̃q�b�g��
        /// </summary>
        public Int32 DocumentNumber
        {
            get { return _num; }
        }

        /// <summary>
        /// �������ʈꗗ�̃C���f�b�N�X���當�͂�ID���擾���܂��B
        /// </summary>
        /// <param name="index">���ʈꗗ�̃C���f�b�N�X</param>
        /// <returns></returns>
        public Int32 GetDocumentId(Int32 index)
        {
            CheckDispose();

            if (index >= DocumentNumber)
                throw new ArgumentOutOfRangeException("index", index, "index >= DocumentNumber");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "index < 0");

            Boolean success = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                _returnHandle.DangerousAddRef(ref success);
                IntPtr ptr = _returnHandle.DangerousGetHandle();
                if (ptr == IntPtr.Zero)
                    throw new Exception("Fatal Error: est_db_search / return null");

                return Marshal.ReadInt32(ptr, index * sizeof(Int32));
                //unsafe
                //{
                //    return *(((Int32*)ptr)+index);
                //}
            }
            finally
            {
                if (success)
                    _returnHandle.DangerousRelease();
            }
        }


        /// <summary>
        /// �q���g�̒P�ꃊ�X�g���擾���܂��B
        /// </summary>
        public String[] HintWords
        {
            get
            {
                List<String> words = new List<string>();
                foreach (KeyValuePair<String, Int32> pair in _hints)
                {
                    words.Add(pair.Key);
                }
                return words.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public String Hint(String word)
        {
            // TODO:
            return null;
        }

        /// <summary>
        /// �I�u�W�F�N�g���j������Ă��邩�ǂ������擾���܂��B
        /// </summary>
        public Boolean IsDisposed
        {
            get
            {
                return (_returnHandle == null || _returnHandle.IsClosed);
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g�����ɔj������Ă��邩�ǂ������m�F���A�j������Ă���ꍇ�ɂ͗�O System.ObjectDisposedException �𔭐����܂��B
        /// </summary>
        private void CheckDispose()
        {
            if (_returnHandle == null || _returnHandle.IsClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        /// <summary>
        /// �f�X�g���N�^
        /// </summary>
        ~Result()
        {
            Dispose();
        }

        #region IDisposable �����o

        public void Dispose()
        {
            if (_returnHandle != null)
            {
                if (!_returnHandle.IsClosed && !_returnHandle.IsInvalid)
                {
                    _returnHandle.Close();
                }
                _returnHandle = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
