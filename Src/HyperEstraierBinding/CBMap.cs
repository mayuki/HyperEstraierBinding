using System;
using System.Collections.Generic;
using System.Text;

namespace HyperEstraier
{
    /// <summary>
    /// CBMAP �\���̂����b�v���A�A�N�Z�X���ȕւɂ��܂��B
    /// </summary>
    internal class CBMap : IDictionary<String, String>, IDisposable
    {
        private CBMapHandle _cbmapPtr;

        /// <summary>
        /// �V����CBMap�𐶐����܂��B
        /// </summary>
        public CBMap()
        {
            _cbmapPtr = Unmanaged.cbmapopen();
        }

        /// <summary>
        /// �w�肳�ꂽ�|�C���^����CBMap�𐶐����܂��B
        /// </summary>
        /// <param name="mapPtr">CBMap �𐶐����錳�ƂȂ� CBMAP �\���̂̃|�C���^���w�肵�܂��B</param>
        public CBMap(CBMapHandle mapPtr)
        {
            _cbmapPtr = mapPtr;
        }

        /// <summary>
        /// CBMAP �\���̂̃n���h����Ԃ��܂��B
        /// </summary>
        internal CBMapHandle Handle
        {
            get
            {
                CheckDispose();
                return _cbmapPtr;
            }
        }

        #region IDictionary<string,string> �����o

        public void Add(string key, string value)
        {
            Add(key, value, true);
        }

        /// <summary>
        /// �}�b�v�ɗv�f��ǉ����܂��B
        /// </summary>
        /// <param name="key">�L�[</param>
        /// <param name="value">�ݒ肷��l</param>
        /// <param name="over">�㏑��</param>
        public void Add(string key, string value, Boolean over)
        {
            CheckDispose();

            Boolean isSuccess = Unmanaged.cbmapput(_cbmapPtr, key, Encoding.UTF8.GetByteCount(key), value, Encoding.UTF8.GetByteCount(value), (over ? 1 : 0));
            if (!isSuccess)
            {
                // ���s����
                throw new InvalidOperationException("Add �Ɏ��s���܂����B");
            }
        }

        public bool ContainsKey(string key)
        {
            //CheckDispose();
            String value;
            return TryGetValue(key, out value);
            //return Keys.Contains(key);
        }

        public ICollection<string> Keys
        {
            get
            {
                CheckDispose();

                using (CBListHandle cbListHandle = Unmanaged.cbmapkeys(_cbmapPtr))
                using (CBList cbList = new CBList(cbListHandle))
                {
                    return cbList.ToArray();
                }
            }
        }

        public bool Remove(string key)
        {
            CheckDispose();

            Boolean result = Unmanaged.cbmapout(_cbmapPtr, key, Encoding.UTF8.GetByteCount(key));
            return result;
        }

        public bool TryGetValue(string key, out string value)
        {
            CheckDispose();

            Int32 sp = 0;
            value = null;
            IntPtr retVal = Unmanaged.cbmapget(_cbmapPtr, key, Encoding.UTF8.GetByteCount(key), ref sp);
            if (retVal == IntPtr.Zero)
                return false;

            value = Unmanaged.PtrToString(retVal);
            return true;
        }

        public ICollection<string> Values
        {
            get
            {
                using (CBListHandle cbListHandle = Unmanaged.cbmapvals(_cbmapPtr))
                using (CBList cbList = new CBList(cbListHandle))
                {
                    return cbList.ToArray();
                } 
            }
        }

        public string this[string key]
        {
            get
            {
                String value;
                TryGetValue(key, out value);
                return value;
            }
            set
            {
                Add(key, value, true);
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> �����o

        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            foreach (String key in Keys)
                Remove(key);
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return this[item.Key] == item.Value;
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { CheckDispose(); return Unmanaged.cbmaprnum(_cbmapPtr);  }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> �����o

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            CheckDispose();
            Unmanaged.cbmapiterinit(_cbmapPtr);
            String val;
            while ((val = Unmanaged.cbmapiternext(_cbmapPtr, 0)) != null)
            {
                yield return new KeyValuePair<String, String>(val, this[val]);
            }
        }

        #endregion

        #region IEnumerable �����o

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IDisposable �����o

        public void Dispose()
        {
            if (_cbmapPtr != null)
            {
                _cbmapPtr.Close();
                _cbmapPtr = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// �f�X�g���N�^
        /// </summary>
        ~CBMap()
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
                return (_cbmapPtr == null || _cbmapPtr.IsClosed);
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g�����ɔj������Ă��邩�ǂ������m�F���A�j������Ă���ꍇ�ɂ͗�O System.ObjectDisposedException �𔭐����܂��B
        /// </summary>
        private void CheckDispose()
        {
            if (_cbmapPtr == null || _cbmapPtr.IsClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }
    }
}
