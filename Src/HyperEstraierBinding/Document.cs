using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// Hyper Estraier の文章オブジェクトを表します。
    /// </summary>
    public class Document : IDisposable
    {
        private DocumentHandle _estDocPtr;
        private Boolean _isDisposed = false;
        private AttributeDictionary _attribute;

        /// <summary>
        /// 指定したESTDOCポインタで文章オブジェクトを初期化します。
        /// </summary>
        /// <param name="estDocPtr"></param>
        private void Initialize(DocumentHandle estDocPtr)
        {
            _estDocPtr = estDocPtr;
            _attribute = new AttributeDictionary(this);
        }

        /// <summary>
        /// 新しい文章オブジェクトを生成します。
        /// </summary>
        /// <returns></returns>
        public Document()
        {
            Initialize(Unmanaged.est_doc_new());
        }

        /// <summary>
        /// ドラフトから新しい文章オブジェクトを生成します。
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        public Document(String draft)
        {
            Initialize(Unmanaged.est_doc_new_from_draft(draft));
        }

        /// <summary>
        /// 文章オブジェクトのポインタから文章オブジェクトを生成します。
        /// </summary>
        /// <param name="docPtr"></param>
        internal Document(DocumentHandle docPtr)
        {
            Initialize(docPtr);
        }

        /// <summary>
        /// この文章オブジェクトのIDを返します。
        /// </summary>
        public Int32 Id
        {
            get { CheckDisposed(); return Unmanaged.est_doc_id(_estDocPtr); }
        }

        /// <summary>
        /// 本文を文章オブジェクトに追加します。
        /// </summary>
        /// <param name="text"></param>
        public void AddText(String text)
        {
            CheckDisposed();
            Unmanaged.est_doc_add_text(_estDocPtr, text);
        }

        /// <summary>
        /// 隠しテキストを文章オブジェクトに追加します。
        /// </summary>
        /// <param name="text"></param>
        public void AddHiddenText(String text)
        {
            CheckDisposed();
            Unmanaged.est_doc_add_hidden_text(_estDocPtr, text);
        }

        /// <summary>
        /// 本文を連結して返します。
        /// </summary>
        /// <returns></returns>
        public String CatTexts()
        {
            CheckDisposed();
            using (MallocHandle textsPtr = Unmanaged.est_doc_cat_texts(_estDocPtr))
            {
                return Unmanaged.PtrToString(textsPtr);
            }
        }

        /// <summary>
        /// 文章オブジェクトに設定された本文を返します。
        /// </summary>
        /// <returns></returns>
        public String[] Texts()
        {
            CheckDisposed();
            CBListHandle cbListHandle = new CBListHandle(Unmanaged.est_doc_texts(_estDocPtr), false);
            return (new CBList(cbListHandle)).ToArray();
        }

        /// <summary>
        /// キーワードを設定します。
        /// </summary>
        /// <param name="keywordMap">キーワードとスコアの組み合わせのリスト</param>
        public void SetKeywords(Dictionary<String, Int32> keywordMap)
        {
            CheckDisposed();
            using (CBMap map = new CBMap())
            {
                foreach (KeyValuePair<String, Int32> keyword in keywordMap)
                {
                    map[keyword.Key] = keyword.Value.ToString();
                }
                Unmanaged.est_doc_set_keywords(_estDocPtr, map.Handle);
            }
        }

        /// <summary>
        /// 文章オブジェクトの属性を操作できるコレクションを取得します。
        /// </summary>
        public AttributeDictionary Attributes
        {
            get { CheckDisposed(); return _attribute; }
        }

        /// <summary>
        /// 属性を文章オブジェクトに追加します。
        /// </summary>
        /// <param name="text"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddAttr(String name, String value)
        {
            CheckDisposed();
            if (value != null)
            {
                Unmanaged.est_doc_add_attr(_estDocPtr, name, value);
            }
            else
            {
                // 削除
                Unmanaged.est_doc_add_attr(_estDocPtr, name, null);
            }
        }

        /// <summary>
        /// 文章オブジェクトから指定された属性の値を返します。存在しない場合には null を返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String Attr(String name)
        {
            CheckDisposed();
            return Unmanaged.est_doc_attr(_estDocPtr, name);
        }

        /// <summary>
        /// 文章オブジェクトに設定されている属性名のリストを返します。
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String[] AttrNames()
        {
            CheckDisposed();
            using (CBListHandle namesHandle = Unmanaged.est_doc_attr_names(_estDocPtr))
            using (CBList names = new CBList(namesHandle))
            {
                return names.ToArray();
            }
        }

        /// <summary>
        /// 代替スコアを文書オブジェクトに設定します。
        /// </summary>
        /// <param name="score">文章オブジェクトに設定する代替スコア</param>
        public void SetScore(Int32 score)
        {
            CheckDisposed();
            Unmanaged.est_doc_set_score(_estDocPtr, score);
        }

        /// <summary>
        /// 文章オブジェクトから文章ドラフトを生成します。
        /// </summary>
        /// <returns></returns>
        public String DumpDraft()
        {
            CheckDisposed();
            using (MallocHandle draftHandle = Unmanaged.est_doc_dump_draft(_estDocPtr))
            {
                return Unmanaged.PtrToString(draftHandle);
            }
        }

        /// <summary>
        /// 文章オブジェクトの本文から切り抜きを作成します。
        /// </summary>
        /// <param name="words">ハイライトする語句のリスト</param>
        /// <param name="wwidth">結果の文字列全体の長さ</param>
        /// <param name="hwidth">本文の冒頭から切り出す文字列の長さ</param>
        /// <param name="awidth">ハイライトされる語の周辺から抽出する幅</param>
        /// <returns></returns>
        public String MakeSnippet(String[] words, Int32 wwidth, Int32 hwidth, Int32 awidth)
        {
            CheckDisposed();
            using (CBList wordsList = new CBList())
            {
                wordsList.AddRange(words);
                using (MallocHandle snipetHandle = Unmanaged.est_doc_make_snippet(_estDocPtr, wordsList.Handle, wwidth, hwidth, awidth))
                {
                    return Unmanaged.PtrToString(snipetHandle);
                }
            }
        }

        /// <summary>
        /// 文章オブジェクトのハンドルを返します。
        /// </summary>
        internal DocumentHandle Handle
        {
            get
            {
                return _estDocPtr;
            }
        }

        /// <summary>
        /// 文章オブジェクトの属性のコレクションです。コレクションを操作することで文章オブジェクトの属性を直接操作できます。
        /// </summary>
        public class AttributeDictionary : IDictionary<String, String>, IDisposable
        {
            private Document _estDoc;

            internal AttributeDictionary(Document estDoc)
            {
                _estDoc = estDoc;
            }

            /// <summary>
            /// 文章オブジェクトの属性を取得または設定します。
            /// </summary>
            /// <param name="name">属性の名前</param>
            /// <returns>指定された属性の値</returns>
            public String this[String name]
            {
                get
                {
                    String s;
                    TryGetValue(name, out s);
                    return s;
                }
                set
                {
                    Add(name, value);
                }
            }

            #region IDictionary<string,string> メンバ

            public void Add(string key, string value)
            {
                _estDoc.AddAttr(key, value);
            }

            public bool ContainsKey(string key)
            {
                String val = _estDoc.Attr(key);
                return (val != null);
            }

            public ICollection<string> Keys
            {
                get
                {
                    return _estDoc.AttrNames();
                }
            }

            public bool Remove(string key)
            {
                _estDoc.AddAttr(key, null);
                return true;
            }

            public bool TryGetValue(string key, out string value)
            {
                value = _estDoc.Attr(key);
                if (value == null)
                    return false;

                return true;
            }

            public ICollection<string> Values
            {
                get
                {
                    List<String> values = new List<string>();
                    foreach (String key in Keys)
                    {
                        values.Add(this[key]);
                    }
                    return values.ToArray();
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
                {
                    Remove(key);
                }
            }

            public bool Contains(KeyValuePair<string, string> item)
            {
                return (this[item.Key] == item.Value) ;
            }

            public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
            {
                // TODO: 実装する
                throw new Exception("The method or operation is not implemented.");
            }

            public int Count
            {
                get { return Keys.Count; }
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
                foreach (String key in Keys)
                {
                    yield return new KeyValuePair<String, String>(key, this[key]);
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
                _estDoc = null;
            }

            #endregion
        }

        #region デストラクタ
        /// <summary>
        /// 
        /// </summary>
        ~Document()
        {
            Dispose();
        }
        #endregion

        #region Dispose 関連
        /// <summary>
        /// オブジェクトが破棄されているかどうかを返します。
        /// </summary>
        public Boolean IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        /// オブジェクトが破棄されているかどうかを確認し、破棄されている場合には <remarks>System.ObjectDisposedException</remarks> を発生します。
        /// </summary>
        [DebuggerStepThrough]
        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new System.ObjectDisposedException(this.ToString());
        }


        #region IDisposable メンバ

        /// <summary>
        /// ドキュメントを破棄し、アンマネージリソースを全て開放します。
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _attribute.Dispose();
                _attribute = null;

                _estDocPtr.Close();
                _estDocPtr = null;
            }
            _isDisposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion
        #endregion
    }
}
