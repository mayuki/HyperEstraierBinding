using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// Hyper Estraier �̃f�[�^�x�[�X��\���܂��B
    /// </summary>
    public class Database : IDisposable
    {
        private DatabaseHandle _dbHandle;

        /// <summary>
        /// �f�[�^�x�[�X��\���I�u�W�F�N�g�𐶐����܂��B
        /// </summary>
        /// <param name="name">�f�[�^�x�[�X�̃f�[�^�̑��݂���f�B���N�g����</param>
        /// <param name="oMode">�f�[�^�x�[�X���J�����[�h</param>
        public Database(String name, DatabaseModes oMode)
        {
            Int32 errorCode;
            _dbHandle = Unmanaged.est_db_open(name, oMode, out errorCode);

            if (_dbHandle.IsInvalid)
            {
                throw new HyperEstraierDatabaseException(errorCode);
            }
        }

        /// <summary>
        /// ���������ɊY�����镶�͂̈ꗗ���擾���܂��B
        /// </summary>
        /// <param name="cond">���������I�u�W�F�N�g</param>
        /// <returns>�Y���������͂�ID�̔z��</returns>
        public Int32[] SearchForIds(Condition cond)
        {
            return SearchForIds(cond, null);
        }
        /// <summary>
        /// ���������ɊY�����镶�͂̈ꗗ���擾���܂��B
        /// </summary>
        /// <param name="cond">���������I�u�W�F�N�g</param>
        /// <param name="hints">�����ꂲ�Ƃ̕��͐��̃R���N�V����</param>
        /// <returns>�Y���������͂�ID�̔z��</returns>
        public Int32[] SearchForIds(Condition cond, List<KeyValuePair<String, Int32>> hints)
        {
            CheckDispose();
            Int32 num;

            CBMapHandle hintsHandle = null;
            CBMap hintsMap = null;
            if (hints == null)
            {
                hintsHandle = new CBMapHandle(IntPtr.Zero, false);
            }
            else
            {
                hintsMap = new CBMap();
                hintsHandle = hintsMap.Handle;
            }

            Int32[] returnValues;
            using (MallocHandle retHandle = Unmanaged.est_db_search(_dbHandle, cond.Handle, out num, hintsHandle))
            {
                Boolean success = false;
                returnValues = new Int32[num];

                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    retHandle.DangerousAddRef(ref success);
                    IntPtr ptr = retHandle.DangerousGetHandle();
                    if (ptr == IntPtr.Zero)
                        throw new Exception("Fatal Error: est_db_search / return null");

                    Marshal.Copy(ptr, returnValues, 0, num);
                }
                finally
                {
                    if (success)
                        retHandle.DangerousRelease();
                }
            }

            if (hints != null)
            {
                foreach (KeyValuePair<String, String> pair in hintsMap)
                {
                    hints.Add(new KeyValuePair<string,int>(pair.Key, Int32.Parse(pair.Value)));
                }
            }

            return returnValues;
        }

        /// <summary>
        /// ���������ɊY�����镶�͂̈ꗗ���擾���܂��B
        /// </summary>
        /// <param name="cond">���������I�u�W�F�N�g</param>
        /// <returns>�������ʃZ�b�g�I�u�W�F�N�g</returns>
        public Result Search(Condition cond)
        {
            return Search(cond, null);
        }
        /// <summary>
        /// ���������ɊY�����镶�͂̈ꗗ���擾���܂��B
        /// </summary>
        /// <param name="cond">���������I�u�W�F�N�g</param>
        /// <param name="hints">�����ꂲ�Ƃ̕��͐����i�[����R���N�V����</param>
        /// <returns>�������ʃZ�b�g�I�u�W�F�N�g</returns>
        public Result Search(Condition cond, List<KeyValuePair<String, Int32>> hints)
        {
            CheckDispose();
            Int32 num;

            CBMapHandle hintsHandle = null;
            CBMap hintsMap = null;
            if (hints == null)
            {
                hintsHandle = new CBMapHandle(IntPtr.Zero, false);
            }
            else
            {
                hintsMap = new CBMap();
                hintsHandle = hintsMap.Handle;
            }

            // ���� MallocHandle �� Result ���J������
            MallocHandle retHandle = Unmanaged.est_db_search(_dbHandle, cond.Handle, out num, hintsHandle);
            System.Diagnostics.Debug.Assert(!retHandle.IsInvalid);

            if (hints != null && !hintsHandle.IsInvalid)
            {
                foreach (KeyValuePair<String, String> pair in hintsMap)
                {
                    hints.Add(new KeyValuePair<string,int>(pair.Key, Int32.Parse(pair.Value)));
                }
                hintsMap.Dispose();
            }

            Result result = new Result(retHandle, num, hints);
            return result;
        }

        /// <summary>
        /// �f�[�^�x�[�X����w�肵�� ID �̕��͂��擾���܂��B
        /// </summary>
        /// <param name="id">���͂� ID </param>
        /// <returns></returns>
        public Document GetDocument(Int32 id)
        {
            return GetDocument(id, GetDocumentOptions.All);
        }

        /// <summary>
        /// �f�[�^�x�[�X����w�肵�� ID �̕��͂��擾���܂��B
        /// </summary>
        /// <param name="id">���͂� ID </param>
        /// <param name="options">�擾����I�v�V����</param>
        /// <returns></returns>
        public Document GetDocument(Int32 id, GetDocumentOptions options)
        {
            DocumentHandle docHandle = Unmanaged.est_db_get_doc(_dbHandle, id, options);
            if (docHandle.IsInvalid)
            {
                return null;
            }

            return new Document(docHandle);
        }

        /// <summary>
        /// ���O�ɔ��������G���[�̃G���[�R�[�h���擾���܂��B
        /// </summary>
        public Int32 Error
        {
            get
            {
                CheckDispose();
                return Unmanaged.est_db_error(_dbHandle);
            }
        }

        /// <summary>
        /// �f�[�^�x�[�X�ɒv���I�ȃG���[�������������ǂ������擾���܂��B
        /// </summary>
        public Boolean Fatal
        {
            get
            {
                CheckDispose();
                return Unmanaged.est_db_fatal(_dbHandle);
            }
        }

        /// <summary>
        /// �f�[�^�x�[�X�̖��O���擾���܂��B
        /// </summary>
        public String Name
        {
            get
            {
                CheckDispose();

                Boolean success = false;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    _dbHandle.DangerousAddRef(ref success);
                    return Unmanaged.PtrToString(Unmanaged.est_db_name(_dbHandle));
                }
                finally
                {
                    if (success)
                        _dbHandle.DangerousRelease();
                }
            }
        }

        /// <summary>
        /// �f�[�^�x�[�X�ɓo�^���ꂽ���͂̐����擾���܂��B
        /// </summary>
        public Int32 DocumentNumber
        {
            get
            {
                CheckDispose();
                return Unmanaged.est_db_doc_num(_dbHandle);
            }
        }

        /// <summary>
        /// �f�[�^�x�[�X�ɓo�^���ꂽ�قȂ��̐����擾���܂��B
        /// </summary>
        public Int32 WordNumber
        {
            get
            {
                CheckDispose();
                return Unmanaged.est_db_word_num(_dbHandle);
            }
        }

        /// <summary>
        /// �f�[�^�x�[�X�̃T�C�Y���擾���܂��B
        /// </summary>
        public Double Size
        {
            get
            {
                CheckDispose();
                return Unmanaged.est_db_size(_dbHandle);
            }
        }

        /// <summary>
        /// �f�[�^�x�[�X�̃L���b�V�����̍������S�ăt���b�V�����܂��B
        /// </summary>
        /// <returns></returns>
        public Boolean Flush()
        {
            return Flush(0);
        }

        /// <summary>
        /// �f�[�^�x�[�X�̃L���b�V�����̍�������t���b�V�����܂��B
        /// </summary>
        /// <param name="max">�t���b�V�������̍ő吔</param>
        /// <returns></returns>
        public Boolean Flush(Int32 max)
        {
            CheckDispose();
            return Unmanaged.est_db_flush(_dbHandle, max);
        }

        /// <summary>
        /// �f�[�^�x�[�X�̍X�V���e�𓯊����܂��B
        /// </summary>
        /// <returns></returns>
        public Boolean Sync()
        {
            CheckDispose();
            return Unmanaged.est_db_sync(_dbHandle);
        }

        /// <summary>
        /// �f�[�^�x�[�X�ɑ����̍i�荞�ݗp�܂��̓\�[�g�p�̃C���f�b�N�X��ǉ����܂��B
        /// </summary>
        /// <param name="name">�����̖��O</param>
        /// <param name="type">�C���f�b�N�X�̃f�[�^�^</param>
        /// <returns></returns>
        public Boolean AddAttributeIndex(String name, AddAttributeIndexTypes type)
        {
            CheckDispose();
            return Unmanaged.est_db_add_attr_index(_dbHandle, name, type);
        }

        /// <summary>
        /// �f�[�^�x�[�X���œK�����܂��B
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Boolean Optimize(OptimizeOptions options)
        {
            CheckDispose();
            return Unmanaged.est_db_optimize(_dbHandle, options);
        }

        /// <summary>
        /// �ʂȃf�[�^�x�[�X���}�[�W���܂��B
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Boolean Merge(MergeOptions options)
        {
            CheckDispose();
            return Unmanaged.est_db_merge(_dbHandle, options);
        }

        /// <summary>
        /// ���̓I�u�W�F�N�g���f�[�^�x�[�X�ɒǉ����܂��B
        /// �w�肳�ꂽ���̓I�u�W�F�N�g��URI�������f�[�^�x�[�X���̊����̕��͂ƈ�v����ꍇ�A�����̕��͍͂폜����܂��B
        /// </summary>
        /// <param name="doc">���̓I�u�W�F�N�g</param>
        /// <param name="options">�ǉ����̃I�v�V����</param>
        /// <returns></returns>
        public Boolean PutDocument(Document doc, PutDocumentOptions options)
        {
            CheckDispose();
            return Unmanaged.est_db_put_doc(_dbHandle, doc.Handle, options);
        }

        /// <summary>
        /// �w�肳�ꂽID�̕��͂��f�[�^�x�[�X����폜���܂��B
        /// </summary>
        /// <param name="id">�폜���镶�͂�ID</param>
        /// <param name="options">�폜���̃I�v�V����</param>
        /// <returns></returns>
        public Boolean OutDocument(Int32 id, OutDocumentOptions options)
        {
            CheckDispose();
            return Unmanaged.est_db_out_doc(_dbHandle, id, options);
        }

        /// <summary>
        /// �f�[�^�x�[�X���̎w�肳�ꂽ�h�L�������g�̑�����ҏW���܂��B
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public Boolean EditDocument(Document doc)
        {
            CheckDispose();
            return Unmanaged.est_db_edit_doc(_dbHandle, doc.Handle);
        }

        /// <summary>
        /// �w�肳�ꂽID�̕��͂̑����l���擾���܂��B
        /// </summary>
        /// <param name="id">�����l���擾���镶�͂�ID</param>
        /// <param name="name">�����̖��O</param>
        /// <returns></returns>
        public String GetDocumentAttribute(Int32 id, String name)
        {
            CheckDispose();
            using (MallocHandle retval = Unmanaged.est_db_get_doc_attr(_dbHandle, id, name))
            {
                return Unmanaged.PtrToString(retval);
            }
        }

        /// <summary>
        /// URI�ɑΉ����镶�͂�ID���擾���܂��B
        /// </summary>
        /// <param name="uri">���͂�URI</param>
        /// <returns></returns>
        public Int32 UriToId(String uri)
        {
            CheckDispose();
            return Unmanaged.est_db_uri_to_id(_dbHandle, uri);
        }

        // TODO: search_meta

        /// <summary>
        /// ���������������̃t���[�Y�Ɋ��S�Ɉ�v���邩�𒲂ׁA���ʂ�Ԃ��܂��B
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cond"></param>
        /// <returns></returns>
        public Boolean ScanDocument(Document doc, Condition cond)
        {
            CheckDispose();
            return Unmanaged.est_db_scan_doc(_dbHandle, doc.Handle, cond.Handle);
        }

        /// <summary>
        /// �f�[�^�x�[�X�̃L���b�V���������̍ő�T�C�Y���w�肵�܂��B
        /// </summary>
        /// <param name="size">�C���f�b�N�X�p�̃L���b�V���������̃T�C�Y(�f�t�H���g��64MB)�B���̒l���w�肵���ꍇ�A����̐ݒ��ύX���܂���B</param>
        /// <param name="anum">���͂̑����p�̃L���b�V���̃��R�[�h��(�f�t�H���g��8192��)�B���̒l���w�肵���ꍇ�A����̐ݒ��ύX���܂���B</param>
        /// <param name="tnum">���͂̃e�L�X�g�p�̃L���b�V���̃��R�[�h��(�f�t�H���g��1024��)�B���̒l���w�肵���ꍇ�A����̐ݒ��ύX���܂���B</param>
        /// <param name="rnum">�o�����ʗp�̃L���b�V���̃��R�[�h��(�f�t�H���g��256��)�B���̒l���w�肵���ꍇ�A����̐ݒ��ύX���܂���B</param>
        public void SetCacheSize(Int32 size, Int32 anum, Int32 tnum, Int32 rnum)
        {
            CheckDispose();
            Unmanaged.est_db_set_cache_size(_dbHandle, size, anum, tnum, rnum);
        }

        /// <summary>
        /// �^���C���f�b�N�X�̃p�X���f�[�^�x�[�X�ɒǉ����܂��B
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Boolean AddPseudoIndex(String path)
        {
            CheckDispose();
            return Unmanaged.est_db_add_pseudo_index(_dbHandle, path);
        }

        /// <summary>
        /// �I�u�W�F�N�g���j������Ă��邩�ǂ������擾���܂��B
        /// </summary>
        public Boolean IsDisposed
        {
            get
            {
                return (_dbHandle == null || _dbHandle.IsClosed);
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g�����ɔj������Ă��邩�ǂ������m�F���A�j������Ă���ꍇ�ɂ͗�O System.ObjectDisposedException �𔭐����܂��B
        /// </summary>
        private void CheckDispose()
        {
            if (_dbHandle == null || _dbHandle.IsClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable �����o

        /// <summary>
        /// �I�u�W�F�N�g��j�����A�A���}�l�[�W���\�[�X��������܂��B
        /// </summary>
        public void Dispose()
        {
            if (_dbHandle != null)
            {
                _dbHandle.Close();
                _dbHandle = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// �f�X�g���N�^
        /// </summary>
        ~Database()
        {
            Dispose();
        }
    }

    /// <summary>
    /// AddAttributeIndex ���\�b�h�ŃC���f�b�N�X��ǉ�����ۂ̃^�C�v��\���܂��B
    /// </summary>
    public enum AddAttributeIndexTypes : int
    {
        /// <summary>
        /// ���ړI�̃V�[�P���V�����A�N�Z�X�p
        /// </summary>
        Sequential = 0,
        /// <summary>
        /// ������̍i�荞�ݗp
        /// </summary>
        String = 1,
        /// <summary>
        /// ���l�^�̍i�荞�ݗp
        /// </summary>
        Number = 2
    }

    /// <summary>
    /// Optimize ���\�b�h�Ńf�[�^�x�[�X�ɕ��͂�ǉ�����ۂ̃I�v�V������\���܂��B
    /// </summary>
    [Flags]
    public enum OptimizeOptions : int
    {
        /// <summary>
        /// ���ʂȎw��͍s���܂���B
        /// </summary>
        None = 0,
        /// <summary>
        /// �폜���ꂽ���͂̏����������鏈�����ȗ����܂��B
        /// </summary>
        NoPurge = 1 << 0,
        /// <summary>
        /// �f�[�^�x�[�X�t�@�C���̍œK�����ȗ����܂��B
        /// </summary>
        NoDBOptimize = 1 << 1
    }

    /// <summary>
    /// Merge ���\�b�h�Ńf�[�^�x�[�X�ɕ��͂�ǉ�����ۂ̃I�v�V������\���܂��B
    /// </summary>
    [Flags]
    public enum MergeOptions : int
    {
        /// <summary>
        /// ���ʂȎw��͍s���܂���B
        /// </summary>
        None = 0,
        /// <summary>
        /// �폜���ꂽ�����̗̈�𐮗����܂��B
        /// </summary>
        Clean = 1 << 0
    }

    /// <summary>
    /// PutDocument ���\�b�h�Ńf�[�^�x�[�X�ɕ��͂�ǉ�����ۂ̃I�v�V������\���܂��B
    /// </summary>
    [Flags]
    public enum PutDocumentOptions : int
    {
        /// <summary>
        /// ���ʂȎw��͍s���܂���B
        /// </summary>
        None = 0,
        /// <summary>
        /// �㏑�����ꂽ�����̗̈�𐮗����܂��B
        /// </summary>
        Clean = 1 << 0,
        /// <summary>
        /// �C���f�N�V���O�̍ۂɏd�݂Â�������ÓI�ɓK�p���܂��B
        /// </summary>
        Weight = 1 << 1
    }

    /// <summary>
    /// OutDocument ���\�b�h�Ńf�[�^�x�[�X���當�͂��폜����ۂ̃I�v�V������\���܂��B
    /// </summary>
    [Flags]
    public enum OutDocumentOptions : int
    {
        /// <summary>
        /// ���ʂȎw��͍s���܂���B
        /// </summary>
        None = 0,
        /// <summary>
        /// �폜���ꂽ���̗͂̈�𐮗����܂��B
        /// </summary>
        Clean = 1 << 0
    }

    /// <summary>
    /// GetDocument ���\�b�h�ŕ��͂��擾����ۂ̃I�v�V������\���܂��B
    /// </summary>
    [Flags]
    public enum GetDocumentOptions : int
    {
        /// <summary>
        /// ���ׂĎ擾���܂��B
        /// </summary>
        All = 0,
        /// <summary>
        /// �������擾���܂���B
        /// </summary>
        NoAttributes = 1 << 0,
        /// <summary>
        /// �{�����擾���܂���B
        /// </summary>
        NoText = 1 << 1,
        /// <summary>
        /// �L�[���[�h���擾���܂���B
        /// </summary>
        NoKeywords = 1 << 2
    }

    /// <summary>
    /// �f�[�^�x�[�X�̃G���[�R�[�h��\���܂��B
    /// </summary>
    public enum DatabaseErrors : int
    {
        /// <summary>
        /// �G���[�͔������Ă��܂���B
        /// </summary>
        NoError         = 0,
        /// <summary>
        /// �������s���ł��B
        /// </summary>
        InvalidArgument = 1,
        /// <summary>
        /// �A�N�Z�X���֎~����Ă��܂��B
        /// </summary>
        AccessForbidden = 2,
        /// <summary>
        /// ���b�N�Ɏ��s���܂����B
        /// </summary>
        LockFailure     = 3,
        /// <summary>
        /// �f�[�^�x�[�X�ɖ�肪����܂��B
        /// </summary>
        DatabaseProblem = 4,
        /// <summary>
        /// �f�[�^�̓ǂݏ����Ŗ�肪�������܂����B
        /// </summary>
        IOProblem       = 5,
        /// <summary>
        /// �Y������A�C�e��������܂���B
        /// </summary>
        NoItem          = 6,
        /// <summary>
        /// ���̑��̃G���[���������܂����B
        /// </summary>
        Misc            = 9999
    }

    /// <summary>
    /// �f�[�^�x�[�X�̃��[�h��\���܂��B
    /// </summary>
    [Flags]
    public enum DatabaseModes : int
    {
        /// <summary>
        /// ���[�_(�ǂݍ��݃��[�h)�ŊJ���܂��B
        /// </summary>
        Reader = 1 << 0,
        /// <summary>
        /// ���C�^(�������݃��[�h)�ŊJ���܂��B
        /// </summary>
        Writer = 1 << 1,
        /// <summary>
        /// �������݃��[�h�w�莞�Ƀf�[�^�x�[�X�����݂��Ȃ��ꍇ�ɐV�K�ɍ쐬���܂��B
        /// </summary>
        Create = 1 << 2,
        /// <summary>
        /// �������݃��[�h�w�莞�Ƀf�[�^�x�[�X�����݂���ꍇ�ɐV�K�ɍ쐬���܂��B
        /// </summary>
        Truncate = 1 << 3,
        /// <summary>
        /// �t�@�C�����b�N���s���܂���B
        /// </summary>
        NoLock = 1 << 4,
        /// <summary>
        /// �u���b�N�����Ƀt�@�C�����b�N���s���܂��B
        /// </summary>
        LockNoBlock = 1 << 5,
        /// <summary>
        /// Create ���w�肵�ăf�[�^�x�[�X�������ꍇ�ɕ��͂̉��������S�� N-gram �@�ŏ������܂��B
        /// </summary>
        PerfectNGramAnalyzer = 1 << 10,
        /// <summary>
        /// Create ���w�肵�ăf�[�^�x�[�X�������ꍇ�ɕ��͂��L�����N�^�J�e�S����͂ŏ������܂��B
        /// </summary>
        CharacterCategoryAnalyzer = 1 << 11,
        /// <summary>
        /// Create ���w�肵�ăf�[�^�x�[�X�������ꍇ��50000�������̕��͂�o�^���邱�Ƃ�z�肵���C���f�b�N�X�`���[�j���O���s���܂��B
        /// </summary>
        Small = 1 << 20,
        /// <summary>
        /// Create ���w�肵�ăf�[�^�x�[�X�������ꍇ��300000���ȏ�̕��͂�o�^���邱�Ƃ�z�肵���C���f�b�N�X�`���[�j���O���s���܂��B
        /// </summary>
        Large = 1 << 21,
        /// <summary>
        /// Create ���w�肵�ăf�[�^�x�[�X�������ꍇ��1000000�������̕��͂�o�^���邱�Ƃ�z�肵���C���f�b�N�X�`���[�j���O���s���܂��B
        /// </summary>
        Huge = 1 << 22,
        /// <summary>
        /// Create ���w�肵�ăf�[�^�x�[�X�������ꍇ�ɃX�R�A����j�����܂��B
        /// </summary>
        ScoreAsVoid = 1 << 25,
        /// <summary>
        /// Create ���w�肵�ăf�[�^�x�[�X�������ꍇ�ɃX�R�A����32�r�b�g�̐��l�Ƃ��ċL�^���܂��B
        /// </summary>
        ScoreAsInt = 1 << 26,
        /// <summary>
        /// Create ���w�肵�ăf�[�^�x�[�X�������ꍇ�ɃX�R�A�������̂܂ܕۑ����������ŁA�������ɒ��߂���Ȃ��悤�ɂ��܂��B
        /// </summary>
        ScoreAsIs = 1 << 27
    }

    /// <summary>
    /// �f�[�^�x�[�X�ŃG���[�������������Ƃ�\����O�ł��B
    /// </summary>
    public class HyperEstraierDatabaseException : Exception
    {
        public HyperEstraierDatabaseException(Int32 errorCode)
            : base(Unmanaged.est_err_msg(errorCode))
        {
            _errorCode = errorCode;
        }

        private Int32 _errorCode;

        /// <summary>
        /// Hyper Estraier�̃G���[�R�[�h��Ԃ��܂��B
        /// </summary>
        public Int32 ErrorCode
        {
            get
            {
                return _errorCode;
            }
        }
    }
}
