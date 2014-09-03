using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// Hyper Estraier のデータベースを表します。
    /// </summary>
    public class Database : IDisposable
    {
        private DatabaseHandle _dbHandle;

        /// <summary>
        /// データベースを表すオブジェクトを生成します。
        /// </summary>
        /// <param name="name">データベースのデータの存在するディレクトリ名</param>
        /// <param name="oMode">データベースを開くモード</param>
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
        /// 検索条件に該当する文章の一覧を取得します。
        /// </summary>
        /// <param name="cond">検索条件オブジェクト</param>
        /// <returns>該当した文章のIDの配列</returns>
        public Int32[] SearchForIds(Condition cond)
        {
            return SearchForIds(cond, null);
        }
        /// <summary>
        /// 検索条件に該当する文章の一覧を取得します。
        /// </summary>
        /// <param name="cond">検索条件オブジェクト</param>
        /// <param name="hints">検索語ごとの文章数のコレクション</param>
        /// <returns>該当した文章のIDの配列</returns>
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
        /// 検索条件に該当する文章の一覧を取得します。
        /// </summary>
        /// <param name="cond">検索条件オブジェクト</param>
        /// <returns>検索結果セットオブジェクト</returns>
        public Result Search(Condition cond)
        {
            return Search(cond, null);
        }
        /// <summary>
        /// 検索条件に該当する文章の一覧を取得します。
        /// </summary>
        /// <param name="cond">検索条件オブジェクト</param>
        /// <param name="hints">検索語ごとの文章数を格納するコレクション</param>
        /// <returns>検索結果セットオブジェクト</returns>
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

            // この MallocHandle は Result が開放する
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
        /// データベースから指定した ID の文章を取得します。
        /// </summary>
        /// <param name="id">文章の ID </param>
        /// <returns></returns>
        public Document GetDocument(Int32 id)
        {
            return GetDocument(id, GetDocumentOptions.All);
        }

        /// <summary>
        /// データベースから指定した ID の文章を取得します。
        /// </summary>
        /// <param name="id">文章の ID </param>
        /// <param name="options">取得するオプション</param>
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
        /// 直前に発生したエラーのエラーコードを取得します。
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
        /// データベースに致命的なエラーが発生したかどうかを取得します。
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
        /// データベースの名前を取得します。
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
        /// データベースに登録された文章の数を取得します。
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
        /// データベースに登録された異なり語の数を取得します。
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
        /// データベースのサイズを取得します。
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
        /// データベースのキャッシュ内の索引語を全てフラッシュします。
        /// </summary>
        /// <returns></returns>
        public Boolean Flush()
        {
            return Flush(0);
        }

        /// <summary>
        /// データベースのキャッシュ内の索引語をフラッシュします。
        /// </summary>
        /// <param name="max">フラッシュする語の最大数</param>
        /// <returns></returns>
        public Boolean Flush(Int32 max)
        {
            CheckDispose();
            return Unmanaged.est_db_flush(_dbHandle, max);
        }

        /// <summary>
        /// データベースの更新内容を同期します。
        /// </summary>
        /// <returns></returns>
        public Boolean Sync()
        {
            CheckDispose();
            return Unmanaged.est_db_sync(_dbHandle);
        }

        /// <summary>
        /// データベースに属性の絞り込み用またはソート用のインデックスを追加します。
        /// </summary>
        /// <param name="name">属性の名前</param>
        /// <param name="type">インデックスのデータ型</param>
        /// <returns></returns>
        public Boolean AddAttributeIndex(String name, AddAttributeIndexTypes type)
        {
            CheckDispose();
            return Unmanaged.est_db_add_attr_index(_dbHandle, name, type);
        }

        /// <summary>
        /// データベースを最適化します。
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Boolean Optimize(OptimizeOptions options)
        {
            CheckDispose();
            return Unmanaged.est_db_optimize(_dbHandle, options);
        }

        /// <summary>
        /// 別なデータベースをマージします。
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Boolean Merge(MergeOptions options)
        {
            CheckDispose();
            return Unmanaged.est_db_merge(_dbHandle, options);
        }

        /// <summary>
        /// 文章オブジェクトをデータベースに追加します。
        /// 指定された文章オブジェクトのURI属性がデータベース内の既存の文章と一致する場合、既存の文章は削除されます。
        /// </summary>
        /// <param name="doc">文章オブジェクト</param>
        /// <param name="options">追加時のオプション</param>
        /// <returns></returns>
        public Boolean PutDocument(Document doc, PutDocumentOptions options)
        {
            CheckDispose();
            return Unmanaged.est_db_put_doc(_dbHandle, doc.Handle, options);
        }

        /// <summary>
        /// 指定されたIDの文章をデータベースから削除します。
        /// </summary>
        /// <param name="id">削除する文章のID</param>
        /// <param name="options">削除時のオプション</param>
        /// <returns></returns>
        public Boolean OutDocument(Int32 id, OutDocumentOptions options)
        {
            CheckDispose();
            return Unmanaged.est_db_out_doc(_dbHandle, id, options);
        }

        /// <summary>
        /// データベース内の指定されたドキュメントの属性を編集します。
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public Boolean EditDocument(Document doc)
        {
            CheckDispose();
            return Unmanaged.est_db_edit_doc(_dbHandle, doc.Handle);
        }

        /// <summary>
        /// 指定されたIDの文章の属性値を取得します。
        /// </summary>
        /// <param name="id">属性値を取得する文章のID</param>
        /// <param name="name">属性の名前</param>
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
        /// URIに対応する文章のIDを取得します。
        /// </summary>
        /// <param name="uri">文章のURI</param>
        /// <returns></returns>
        public Int32 UriToId(String uri)
        {
            CheckDispose();
            return Unmanaged.est_db_uri_to_id(_dbHandle, uri);
        }

        // TODO: search_meta

        /// <summary>
        /// 文書が検索条件のフレーズに完全に一致するかを調べ、結果を返します。
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
        /// データベースのキャッシュメモリの最大サイズを指定します。
        /// </summary>
        /// <param name="size">インデックス用のキャッシュメモリのサイズ(デフォルトは64MB)。負の値を指定した場合、現状の設定を変更しません。</param>
        /// <param name="anum">文章の属性用のキャッシュのレコード数(デフォルトは8192個)。負の値を指定した場合、現状の設定を変更しません。</param>
        /// <param name="tnum">文章のテキスト用のキャッシュのレコード数(デフォルトは1024個)。負の値を指定した場合、現状の設定を変更しません。</param>
        /// <param name="rnum">出現結果用のキャッシュのレコード数(デフォルトは256個)。負の値を指定した場合、現状の設定を変更しません。</param>
        public void SetCacheSize(Int32 size, Int32 anum, Int32 tnum, Int32 rnum)
        {
            CheckDispose();
            Unmanaged.est_db_set_cache_size(_dbHandle, size, anum, tnum, rnum);
        }

        /// <summary>
        /// 疑似インデックスのパスをデータベースに追加します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Boolean AddPseudoIndex(String path)
        {
            CheckDispose();
            return Unmanaged.est_db_add_pseudo_index(_dbHandle, path);
        }

        /// <summary>
        /// オブジェクトが破棄されているかどうかを取得します。
        /// </summary>
        public Boolean IsDisposed
        {
            get
            {
                return (_dbHandle == null || _dbHandle.IsClosed);
            }
        }

        /// <summary>
        /// オブジェクトが既に破棄されているかどうかを確認し、破棄されている場合には例外 System.ObjectDisposedException を発生します。
        /// </summary>
        private void CheckDispose()
        {
            if (_dbHandle == null || _dbHandle.IsClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable メンバ

        /// <summary>
        /// オブジェクトを破棄し、アンマネージリソースを解放します。
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
        /// デストラクタ
        /// </summary>
        ~Database()
        {
            Dispose();
        }
    }

    /// <summary>
    /// AddAttributeIndex メソッドでインデックスを追加する際のタイプを表します。
    /// </summary>
    public enum AddAttributeIndexTypes : int
    {
        /// <summary>
        /// 多目的のシーケンシャルアクセス用
        /// </summary>
        Sequential = 0,
        /// <summary>
        /// 文字列の絞り込み用
        /// </summary>
        String = 1,
        /// <summary>
        /// 数値型の絞り込み用
        /// </summary>
        Number = 2
    }

    /// <summary>
    /// Optimize メソッドでデータベースに文章を追加する際のオプションを表します。
    /// </summary>
    [Flags]
    public enum OptimizeOptions : int
    {
        /// <summary>
        /// 特別な指定は行いません。
        /// </summary>
        None = 0,
        /// <summary>
        /// 削除された文章の情報を消去する処理を省略します。
        /// </summary>
        NoPurge = 1 << 0,
        /// <summary>
        /// データベースファイルの最適化を省略します。
        /// </summary>
        NoDBOptimize = 1 << 1
    }

    /// <summary>
    /// Merge メソッドでデータベースに文章を追加する際のオプションを表します。
    /// </summary>
    [Flags]
    public enum MergeOptions : int
    {
        /// <summary>
        /// 特別な指定は行いません。
        /// </summary>
        None = 0,
        /// <summary>
        /// 削除された文書の領域を整理します。
        /// </summary>
        Clean = 1 << 0
    }

    /// <summary>
    /// PutDocument メソッドでデータベースに文章を追加する際のオプションを表します。
    /// </summary>
    [Flags]
    public enum PutDocumentOptions : int
    {
        /// <summary>
        /// 特別な指定は行いません。
        /// </summary>
        None = 0,
        /// <summary>
        /// 上書きされた文書の領域を整理します。
        /// </summary>
        Clean = 1 << 0,
        /// <summary>
        /// インデクシングの際に重みづけ属性を静的に適用します。
        /// </summary>
        Weight = 1 << 1
    }

    /// <summary>
    /// OutDocument メソッドでデータベースから文章を削除する際のオプションを表します。
    /// </summary>
    [Flags]
    public enum OutDocumentOptions : int
    {
        /// <summary>
        /// 特別な指定は行いません。
        /// </summary>
        None = 0,
        /// <summary>
        /// 削除された文章の領域を整理します。
        /// </summary>
        Clean = 1 << 0
    }

    /// <summary>
    /// GetDocument メソッドで文章を取得する際のオプションを表します。
    /// </summary>
    [Flags]
    public enum GetDocumentOptions : int
    {
        /// <summary>
        /// すべて取得します。
        /// </summary>
        All = 0,
        /// <summary>
        /// 属性を取得しません。
        /// </summary>
        NoAttributes = 1 << 0,
        /// <summary>
        /// 本文を取得しません。
        /// </summary>
        NoText = 1 << 1,
        /// <summary>
        /// キーワードを取得しません。
        /// </summary>
        NoKeywords = 1 << 2
    }

    /// <summary>
    /// データベースのエラーコードを表します。
    /// </summary>
    public enum DatabaseErrors : int
    {
        /// <summary>
        /// エラーは発生していません。
        /// </summary>
        NoError         = 0,
        /// <summary>
        /// 引数が不正です。
        /// </summary>
        InvalidArgument = 1,
        /// <summary>
        /// アクセスが禁止されています。
        /// </summary>
        AccessForbidden = 2,
        /// <summary>
        /// ロックに失敗しました。
        /// </summary>
        LockFailure     = 3,
        /// <summary>
        /// データベースに問題があります。
        /// </summary>
        DatabaseProblem = 4,
        /// <summary>
        /// データの読み書きで問題が発生しました。
        /// </summary>
        IOProblem       = 5,
        /// <summary>
        /// 該当するアイテムがありません。
        /// </summary>
        NoItem          = 6,
        /// <summary>
        /// その他のエラーが発生しました。
        /// </summary>
        Misc            = 9999
    }

    /// <summary>
    /// データベースのモードを表します。
    /// </summary>
    [Flags]
    public enum DatabaseModes : int
    {
        /// <summary>
        /// リーダ(読み込みモード)で開きます。
        /// </summary>
        Reader = 1 << 0,
        /// <summary>
        /// ライタ(書き込みモード)で開きます。
        /// </summary>
        Writer = 1 << 1,
        /// <summary>
        /// 書き込みモード指定時にデータベースが存在しない場合に新規に作成します。
        /// </summary>
        Create = 1 << 2,
        /// <summary>
        /// 書き込みモード指定時にデータベースが存在する場合に新規に作成します。
        /// </summary>
        Truncate = 1 << 3,
        /// <summary>
        /// ファイルロックを行いません。
        /// </summary>
        NoLock = 1 << 4,
        /// <summary>
        /// ブロックせずにファイルロックを行います。
        /// </summary>
        LockNoBlock = 1 << 5,
        /// <summary>
        /// Create を指定してデータベースをした場合に文章の欧文も完全な N-gram 法で処理します。
        /// </summary>
        PerfectNGramAnalyzer = 1 << 10,
        /// <summary>
        /// Create を指定してデータベースをした場合に文章をキャラクタカテゴリ解析で処理します。
        /// </summary>
        CharacterCategoryAnalyzer = 1 << 11,
        /// <summary>
        /// Create を指定してデータベースをした場合に50000件未満の文章を登録することを想定したインデックスチューニングを行います。
        /// </summary>
        Small = 1 << 20,
        /// <summary>
        /// Create を指定してデータベースをした場合に300000件以上の文章を登録することを想定したインデックスチューニングを行います。
        /// </summary>
        Large = 1 << 21,
        /// <summary>
        /// Create を指定してデータベースをした場合に1000000件未満の文章を登録することを想定したインデックスチューニングを行います。
        /// </summary>
        Huge = 1 << 22,
        /// <summary>
        /// Create を指定してデータベースをした場合にスコア情報を破棄します。
        /// </summary>
        ScoreAsVoid = 1 << 25,
        /// <summary>
        /// Create を指定してデータベースをした場合にスコア情報を32ビットの数値として記録します。
        /// </summary>
        ScoreAsInt = 1 << 26,
        /// <summary>
        /// Create を指定してデータベースをした場合にスコア情報をそのまま保存したうえで、検索時に調節されないようにします。
        /// </summary>
        ScoreAsIs = 1 << 27
    }

    /// <summary>
    /// データベースでエラーが発生したことを表す例外です。
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
        /// Hyper Estraierのエラーコードを返します。
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
