using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace HyperEstraier
{
    internal class CBList : IDisposable, IList<String>
    {
        private CBListHandle _cblistHandle;
        
        /// <summary>
        /// �V���� CBList �I�u�W�F�N�g�𐶐����܂��B
        /// </summary>
        public CBList() : this(Unmanaged.cblistopen())
        {
        }

        /// <summary>
        /// CBLIST �\���̂̃|�C���^���w�肵�� CBList �I�u�W�F�N�g�𐶐����܂��B
        /// </summary>
        /// <param name="cblistPtr">CBLIST �\���̂̃n���h��</param>
        public CBList(CBListHandle cblistHandle)
        {
            _cblistHandle = cblistHandle;
        }


        #region IDisposable �����o

        public void Dispose()
        {
            if (_cblistHandle != null)
            {
                _cblistHandle.Close();
                _cblistHandle = null;
            }
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// �f�X�g���N�^
        /// </summary>
        ~CBList()
        {
            Dispose();
        }

        /// <summary>
        /// �I�u�W�F�N�g���j������Ă��邩�ǂ������擾���܂��B
        /// </summary>
        public Boolean IsDisposed
        {
            get
            {
                return (_cblistHandle == null || _cblistHandle.IsClosed);
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g�����ɔj������Ă��邩�ǂ������m�F���A�j������Ă���ꍇ�ɂ͗�O System.ObjectDisposedException �𔭐����܂��B
        /// </summary>
        private void CheckDispose()
        {
            if (_cblistHandle == null || _cblistHandle.IsClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        /// <summary>
        /// ���X�g�ɒl��ǉ����܂��B
        /// </summary>
        /// <param name="value"></param>
        public void Push(String value)
        {
            CheckDispose();
            Unmanaged.cblistpush(_cblistHandle, value, Encoding.UTF8.GetByteCount(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void AddRange(IEnumerable<String> values)
        {
            foreach (String value in values)
            {
                Add(value);
            }
        }

        /// <summary>
        /// ���X�g�̓��e�� System.String �^�̔z��Ƃ��ĕԂ��܂��B
        /// </summary>
        /// <returns></returns>
        public String[] ToArray()
        {
            String[] values = new string[Count];
            CopyTo(values, 0);

            return values;
        }

        /// <summary>
        /// ���X�g�� CBLIST �\���̂ւ̃|�C���^��Ԃ��܂��B
        /// </summary>
        public CBListHandle Handle
        {
            get
            {
                return _cblistHandle;
            }
        }

        #region IList<string> �����o

        public int IndexOf(string item)
        {
            CheckDispose();
            return Unmanaged.cblistlsearch(_cblistHandle, item, Encoding.UTF8.GetByteCount(item));
        }

        public void Insert(int index, string item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            CheckDispose();
            if (index < 0 || index > Count - 1)
                throw new ArgumentOutOfRangeException("index");

            Int32 size;
            MallocHandle handle = Unmanaged.cblistremove(_cblistHandle, index, out size);
            handle.Close();
        }

        public string this[int index]
        {
            get
            {
                CheckDispose();
                if (index < 0 || index > Count - 1)
                    throw new ArgumentOutOfRangeException("index");

                Int32 size;
                IntPtr valuePtr;
                valuePtr = Unmanaged.cblistval(_cblistHandle, index, out size);
                
                if (valuePtr == IntPtr.Zero)
                    return null;

                return Unmanaged.PtrToString(valuePtr);
            }
            set
            {
                CheckDispose();
                if (index < 0 || index > Count - 1)
                    throw new ArgumentOutOfRangeException("index");

                Unmanaged.cblistover(_cblistHandle, index, value, Encoding.UTF8.GetByteCount(value));
            }
        }

        #endregion

        #region ICollection<string> �����o

        public void Add(string item)
        {
            Push(item);
        }

        public void Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(string item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (arrayIndex >= array.Length)
                throw new ArgumentException("arrayIndex �� array �̒����ȏ�ł��B", "arrrayIndex");
            if ((array.Length - arrayIndex) < this.Count)
                throw new ArgumentException("�R�s�[���̗v�f�����AarrayIndex ����R�s�[��� array �̖����܂łɊi�[�ł��鐔�𒴂��Ă��܂��B", "array");

            for (Int32 i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        /// <summary>
        /// ���X�g�Ɋ܂܂��v�f�̐���Ԃ��܂��B
        /// </summary>
        public int Count
        {
            get
            {
                CheckDispose();
                return Unmanaged.cblistnum(_cblistHandle);
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            Int32 index = IndexOf(item);
            if (index == -1)
                return false;

            RemoveAt(index);
            return true;
        }

        #endregion

        #region IEnumerable<string> �����o

        public IEnumerator<string> GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable �����o

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
