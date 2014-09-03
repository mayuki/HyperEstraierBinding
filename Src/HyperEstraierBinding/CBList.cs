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
        /// 新しい CBList オブジェクトを生成します。
        /// </summary>
        public CBList() : this(Unmanaged.cblistopen())
        {
        }

        /// <summary>
        /// CBLIST 構造体のポインタを指定して CBList オブジェクトを生成します。
        /// </summary>
        /// <param name="cblistPtr">CBLIST 構造体のハンドル</param>
        public CBList(CBListHandle cblistHandle)
        {
            _cblistHandle = cblistHandle;
        }


        #region IDisposable メンバ

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
        /// デストラクタ
        /// </summary>
        ~CBList()
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
                return (_cblistHandle == null || _cblistHandle.IsClosed);
            }
        }

        /// <summary>
        /// オブジェクトが既に破棄されているかどうかを確認し、破棄されている場合には例外 System.ObjectDisposedException を発生します。
        /// </summary>
        private void CheckDispose()
        {
            if (_cblistHandle == null || _cblistHandle.IsClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        /// <summary>
        /// リストに値を追加します。
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
        /// リストの内容を System.String 型の配列として返します。
        /// </summary>
        /// <returns></returns>
        public String[] ToArray()
        {
            String[] values = new string[Count];
            CopyTo(values, 0);

            return values;
        }

        /// <summary>
        /// リストの CBLIST 構造体へのポインタを返します。
        /// </summary>
        public CBListHandle Handle
        {
            get
            {
                return _cblistHandle;
            }
        }

        #region IList<string> メンバ

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

        #region ICollection<string> メンバ

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
                throw new ArgumentException("arrayIndex が array の長さ以上です。", "arrrayIndex");
            if ((array.Length - arrayIndex) < this.Count)
                throw new ArgumentException("コピー元の要素数が、arrayIndex からコピー先の array の末尾までに格納できる数を超えています。", "array");

            for (Int32 i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        /// <summary>
        /// リストに含まれる要素の数を返します。
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

        #region IEnumerable<string> メンバ

        public IEnumerator<string> GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable メンバ

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
