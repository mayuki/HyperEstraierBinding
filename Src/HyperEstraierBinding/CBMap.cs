using System;
using System.Collections.Generic;
using System.Text;

namespace HyperEstraier
{
    /// <summary>
    /// CBMAP 構造体をラップし、アクセスを簡便にします。
    /// </summary>
    internal class CBMap : IDictionary<String, String>, IDisposable
    {
        private CBMapHandle _cbmapPtr;

        /// <summary>
        /// 新しくCBMapを生成します。
        /// </summary>
        public CBMap()
        {
            _cbmapPtr = Unmanaged.cbmapopen();
        }

        /// <summary>
        /// 指定されたポインタからCBMapを生成します。
        /// </summary>
        /// <param name="mapPtr">CBMap を生成する元となる CBMAP 構造体のポインタを指定します。</param>
        public CBMap(CBMapHandle mapPtr)
        {
            _cbmapPtr = mapPtr;
        }

        /// <summary>
        /// CBMAP 構造体のハンドルを返します。
        /// </summary>
        internal CBMapHandle Handle
        {
            get
            {
                CheckDispose();
                return _cbmapPtr;
            }
        }

        #region IDictionary<string,string> メンバ

        public void Add(string key, string value)
        {
            Add(key, value, true);
        }

        /// <summary>
        /// マップに要素を追加します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">設定する値</param>
        /// <param name="over">上書き</param>
        public void Add(string key, string value, Boolean over)
        {
            CheckDispose();

            Boolean isSuccess = Unmanaged.cbmapput(_cbmapPtr, key, Encoding.UTF8.GetByteCount(key), value, Encoding.UTF8.GetByteCount(value), (over ? 1 : 0));
            if (!isSuccess)
            {
                // 失敗した
                throw new InvalidOperationException("Add に失敗しました。");
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

        #region ICollection<KeyValuePair<string,string>> メンバ

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

        #region IEnumerable<KeyValuePair<string,string>> メンバ

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

        #region IEnumerable メンバ

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IDisposable メンバ

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
        /// デストラクタ
        /// </summary>
        ~CBMap()
        {
            Dispose();
        }

        /// <summary>
        /// オブジェクトが破棄されているかどうかを取得します。
        /// </summary>
        public Boolean IsDisposed
        {
            get
            {
                return (_cbmapPtr == null || _cbmapPtr.IsClosed);
            }
        }

        /// <summary>
        /// オブジェクトが既に破棄されているかどうかを確認し、破棄されている場合には例外 System.ObjectDisposedException を発生します。
        /// </summary>
        private void CheckDispose()
        {
            if (_cbmapPtr == null || _cbmapPtr.IsClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }
    }
}
