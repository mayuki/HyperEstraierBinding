using System;
using System.Collections.Generic;
using System.Text;

namespace HyperEstraier
{
    /// <summary>
    /// 検索条件オブジェクトを表します。
    /// </summary>
    public class Condition : IDisposable
    {
        private ConditionHandle _condHandle;

        /// <summary>
        /// 新しい検索条件オブジェクトを生成します。
        /// </summary>
        public Condition()
        {
            _condHandle = Unmanaged.est_cond_new();
        }

        /// <summary>
        /// 
        /// </summary>
        internal ConditionHandle Handle
        {
            get
            {
                return _condHandle;
            }
        }

        /// <summary>
        /// 検索フレーズを検索条件オブジェクトに設定します。
        /// </summary>
        public String Phrase
        {
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_phrase(_condHandle, value);
            }
        }
        /// <summary>
        /// 属性検索条件を検索条件オブジェクトに追加します。
        /// </summary>
        /// <param name="expr"></param>
        public void AddAttr(String expr)
        {
            CheckDispose();
            Unmanaged.est_cond_add_attr(_condHandle, expr);
        }
        /// <summary>
        /// 順序を検索条件オブジェクトに設定します。
        /// </summary>
        public String Order
        {
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_order(_condHandle, value);
            }
        }
        /// <summary>
        /// 取得文章数の最大数を検索条件オブジェクトに設定します。
        /// </summary>
        public Int32 Max
        {
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_max(_condHandle, value);
            }
        }
        /// <summary>
        /// 検索結果からスキップする文章数を検索条件オブジェクトに設定します。
        /// </summary>
        public Int32 Skip
        {
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_skip(_condHandle, value);
            }
        }
        /// <summary>
        /// 検索オプションを検索条件オブジェクトに設定します。
        /// </summary>
        public ConditionOptions Options
        {
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_options(_condHandle, value);
            }
        }

        // TODO: 以下enum
        /// <summary>
        /// 類似隠蔽における下限類似度を検索条件オブジェクトに設定します。
        ///
        /// 隠蔽される文書の下限の類似度を0.0から1.0までの値で指定しますが、
        /// `ESTECLSIMURL' を加算するとURLを重み付けに使うようになります。
        /// `ESTECLSERV' を指定すると類似度を無視して同じサーバの文書を隠蔽します。
        /// `ESTECLDIR' を指定すると類似度を無視して同じディレクトリの文書を隠蔽します。
        /// `ESTECLSERV' を指定すると類似度を無視して同じファイルの文書を隠蔽します。
        /// </summary>
        public Double Eclipse
        {
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_eclipse(_condHandle, value);
            }
        }

        /// <summary>
        /// メタ検索の対象のマスクを検索条件オブジェクトに設定します。
        /// 1は1番目の対象、2は2番目の対象、4は3番目の対象といった2の累乗の値の合計で検索を抑止する対象を指定します
        /// </summary>
        public Int32 Mask
        {
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_mask(_condHandle, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Auxiliary
        {
            // TODO: 説明
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_auxiliary(_condHandle, value);
            }
        }

        /// <summary>
        /// オブジェクトが破棄されているかどうかを取得します。
        /// </summary>
        public Boolean IsDisposed
        {
            get
            {
                return (_condHandle == null || _condHandle.IsClosed);
            }
        }

        /// <summary>
        /// オブジェクトが既に破棄されているかどうかを確認し、破棄されている場合には例外 System.ObjectDisposedException を発生します。
        /// </summary>
        private void CheckDispose()
        {
            if (_condHandle == null || _condHandle.IsClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable メンバ

        public void Dispose()
        {
            if (_condHandle != null)
            {
                _condHandle.Close();
                _condHandle = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        ~Condition()
        {
            Dispose();
        }
    }

    /// <summary>
    /// 検索条件のオプションを表します。
    /// </summary>
    [Flags]
    public enum ConditionOptions : int
    {
        /// <summary>
        /// 全ての N-gram キーを検索します。
        /// </summary>
        Sure    = 1 << 0,
        /// <summary>
        /// N-gram キーを1個おきで検査します。デフォルトです。
        /// </summary>
        Usual   = 1 << 1,
        /// <summary>
        /// N-gram キーを2個おきで検査します。
        /// </summary>
        Fast    = 1 << 2,
        /// <summary>
        /// N-gram キーを3個おきで検査します。
        /// </summary>
        Agito   = 1 << 3,
        /// <summary>
        /// TF-IDF 法による重みづけを省略します。
        /// </summary>
        NoIDF   = 1 << 4,
        /// <summary>
        /// 検索フレーズを簡便書式として扱います。
        /// </summary>
        Simple  = 1 << 10,
        /// <summary>
        /// 検索フレーズを粗略書式として扱います。
        /// </summary>
        Rough   = 1 << 11,
        /// <summary>
        /// 検索フレーズを論理和書式(OR 結合)として扱います。
        /// </summary>
        Union   = 1 << 15,
        /// <summary>
        /// 検索フレーズを論理積書式(AND 結合)として扱います。
        /// </summary>
        Intersect   = 1 << 16,
        /// <summary>
        /// スコアをフィードバックします(デバッグ専用) 
        /// </summary>
        FeedBackScore = 1 << 30
    }

    /// <summary>
    /// 検索条件オペレータを表します。
    /// </summary>
    public static class SearchOperators
    {
        public const String UniversalSet = "[UVSET]";
        public const String ID = "[ID]";
        public const String Similar = "[SIMILAR]";
        public const String Rank = "[RANK]";

        public const String Union = "OR";
        public const String Intersection = "AND";
        public const String Difference = "ANDNOT";
        public const String WildCardBeginsWith = "[BW]";
        public const String WildCardEndsWith = "[EW]";
        public const String With = "WITH";

        public const String StringEqual = "STREQ";
        public const String StringNotEqual = "STRNE";
        public const String StringBeginsWith = "STRBW";
        public const String StringEndsWith = "STREW";
        public const String StringOr = "STROR";
        public const String StringOrEqual = "STROREQ";

        public const String NumberEqual = "NUMEQ";
        public const String NumberNotEqual = "NUMNE";
        public const String NumberGreaterThan = "NUMGT";
        public const String NumberGreaterThanOrEqual = "NUMGE";
        public const String NumberLessThan = "NUMLT";
        public const String NumberLessThanOrEqual = "NUMLE";
        public const String NumberBetween = "NUMBT";
    }

    /// <summary>
    /// 検索時の並び順オペレータを表します。
    /// </summary>
    public static class OrderOperators
    {
        /// <summary>
        /// ID 昇順で並べます。
        /// </summary>
        public const String IDAscend = "[IDA]";
        /// <summary>
        /// ID 降順で並べます。
        /// </summary>
        public const String IDDescend = "[IDD]";
        /// <summary>
        /// スコア 昇順で並べます。
        /// </summary>
        public const String ScoreAscend = "[SCA]";
        /// <summary>
        /// スコア 降順で並べます。
        /// </summary>
        public const String ScoreDescend = "[SCD]";
        /// <summary>
        /// 文字列(辞書順) 昇順で並べます。
        /// </summary>
        public const String StringAscend = "STRA";
        /// <summary>
        /// 文字列(辞書順) 降順で並べます。
        /// </summary>
        public const String StringDescend = "STRD";
        /// <summary>
        /// 数値または日付 昇順で並べます。
        /// </summary>
        public const String NumberAscend = "NUMA";
        /// <summary>
        /// 数値または日付 降順で並べます。
        /// </summary>
        public const String NumberDescend = "NUMD";

    }
}
