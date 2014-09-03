using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// Hyper Estraier �̕��̓I�u�W�F�N�g��\���܂��B
    /// </summary>
    public class Document : IDisposable
    {
        private DocumentHandle _estDocPtr;
        private Boolean _isDisposed = false;
        private AttributeDictionary _attribute;

        /// <summary>
        /// �w�肵��ESTDOC�|�C���^�ŕ��̓I�u�W�F�N�g�����������܂��B
        /// </summary>
        /// <param name="estDocPtr"></param>
        private void Initialize(DocumentHandle estDocPtr)
        {
            _estDocPtr = estDocPtr;
            _attribute = new AttributeDictionary(this);
        }

        /// <summary>
        /// �V�������̓I�u�W�F�N�g�𐶐����܂��B
        /// </summary>
        /// <returns></returns>
        public Document()
        {
            Initialize(Unmanaged.est_doc_new());
        }

        /// <summary>
        /// �h���t�g����V�������̓I�u�W�F�N�g�𐶐����܂��B
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        public Document(String draft)
        {
            Initialize(Unmanaged.est_doc_new_from_draft(draft));
        }

        /// <summary>
        /// ���̓I�u�W�F�N�g�̃|�C���^���當�̓I�u�W�F�N�g�𐶐����܂��B
        /// </summary>
        /// <param name="docPtr"></param>
        internal Document(DocumentHandle docPtr)
        {
            Initialize(docPtr);
        }

        /// <summary>
        /// ���̕��̓I�u�W�F�N�g��ID��Ԃ��܂��B
        /// </summary>
        public Int32 Id
        {
            get { CheckDisposed(); return Unmanaged.est_doc_id(_estDocPtr); }
        }

        /// <summary>
        /// �{���𕶏̓I�u�W�F�N�g�ɒǉ����܂��B
        /// </summary>
        /// <param name="text"></param>
        public void AddText(String text)
        {
            CheckDisposed();
            Unmanaged.est_doc_add_text(_estDocPtr, text);
        }

        /// <summary>
        /// �B���e�L�X�g�𕶏̓I�u�W�F�N�g�ɒǉ����܂��B
        /// </summary>
        /// <param name="text"></param>
        public void AddHiddenText(String text)
        {
            CheckDisposed();
            Unmanaged.est_doc_add_hidden_text(_estDocPtr, text);
        }

        /// <summary>
        /// �{����A�����ĕԂ��܂��B
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
        /// ���̓I�u�W�F�N�g�ɐݒ肳�ꂽ�{����Ԃ��܂��B
        /// </summary>
        /// <returns></returns>
        public String[] Texts()
        {
            CheckDisposed();
            CBListHandle cbListHandle = new CBListHandle(Unmanaged.est_doc_texts(_estDocPtr), false);
            return (new CBList(cbListHandle)).ToArray();
        }

        /// <summary>
        /// �L�[���[�h��ݒ肵�܂��B
        /// </summary>
        /// <param name="keywordMap">�L�[���[�h�ƃX�R�A�̑g�ݍ��킹�̃��X�g</param>
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
        /// ���̓I�u�W�F�N�g�̑����𑀍�ł���R���N�V�������擾���܂��B
        /// </summary>
        public AttributeDictionary Attributes
        {
            get { CheckDisposed(); return _attribute; }
        }

        /// <summary>
        /// �����𕶏̓I�u�W�F�N�g�ɒǉ����܂��B
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
                // �폜
                Unmanaged.est_doc_add_attr(_estDocPtr, name, null);
            }
        }

        /// <summary>
        /// ���̓I�u�W�F�N�g����w�肳�ꂽ�����̒l��Ԃ��܂��B���݂��Ȃ��ꍇ�ɂ� null ��Ԃ��܂��B
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
        /// ���̓I�u�W�F�N�g�ɐݒ肳��Ă��鑮�����̃��X�g��Ԃ��܂��B
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
        /// ��փX�R�A�𕶏��I�u�W�F�N�g�ɐݒ肵�܂��B
        /// </summary>
        /// <param name="score">���̓I�u�W�F�N�g�ɐݒ肷���փX�R�A</param>
        public void SetScore(Int32 score)
        {
            CheckDisposed();
            Unmanaged.est_doc_set_score(_estDocPtr, score);
        }

        /// <summary>
        /// ���̓I�u�W�F�N�g���當�̓h���t�g�𐶐����܂��B
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
        /// ���̓I�u�W�F�N�g�̖{������؂蔲�����쐬���܂��B
        /// </summary>
        /// <param name="words">�n�C���C�g������̃��X�g</param>
        /// <param name="wwidth">���ʂ̕�����S�̂̒���</param>
        /// <param name="hwidth">�{���̖`������؂�o��������̒���</param>
        /// <param name="awidth">�n�C���C�g������̎��ӂ��璊�o���镝</param>
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
        /// ���̓I�u�W�F�N�g�̃n���h����Ԃ��܂��B
        /// </summary>
        internal DocumentHandle Handle
        {
            get
            {
                return _estDocPtr;
            }
        }

        /// <summary>
        /// ���̓I�u�W�F�N�g�̑����̃R���N�V�����ł��B�R���N�V�����𑀍삷�邱�Ƃŕ��̓I�u�W�F�N�g�̑����𒼐ڑ���ł��܂��B
        /// </summary>
        public class AttributeDictionary : IDictionary<String, String>, IDisposable
        {
            private Document _estDoc;

            internal AttributeDictionary(Document estDoc)
            {
                _estDoc = estDoc;
            }

            /// <summary>
            /// ���̓I�u�W�F�N�g�̑������擾�܂��͐ݒ肵�܂��B
            /// </summary>
            /// <param name="name">�����̖��O</param>
            /// <returns>�w�肳�ꂽ�����̒l</returns>
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

            #region IDictionary<string,string> �����o

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

            #region ICollection<KeyValuePair<string,string>> �����o

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
                // TODO: ��������
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

            #region IEnumerable<KeyValuePair<string,string>> �����o

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                foreach (String key in Keys)
                {
                    yield return new KeyValuePair<String, String>(key, this[key]);
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
                _estDoc = null;
            }

            #endregion
        }

        #region �f�X�g���N�^
        /// <summary>
        /// 
        /// </summary>
        ~Document()
        {
            Dispose();
        }
        #endregion

        #region Dispose �֘A
        /// <summary>
        /// �I�u�W�F�N�g���j������Ă��邩�ǂ�����Ԃ��܂��B
        /// </summary>
        public Boolean IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        /// �I�u�W�F�N�g���j������Ă��邩�ǂ������m�F���A�j������Ă���ꍇ�ɂ� <remarks>System.ObjectDisposedException</remarks> �𔭐����܂��B
        /// </summary>
        [DebuggerStepThrough]
        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new System.ObjectDisposedException(this.ToString());
        }


        #region IDisposable �����o

        /// <summary>
        /// �h�L�������g��j�����A�A���}�l�[�W���\�[�X��S�ĊJ�����܂��B
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
