using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

namespace HyperEstraier
{
    /// <summary>
    /// Hyper Estraier のコントロールコードの名前を定義するクラスです。
    /// </summary>
    public static class ControlCodes
    {
        /// <summary>
        /// キーワードベクタのコントロールコードを表します。
        /// </summary>
        public const String Vector = "%VECTOR";
        /// <summary>
        /// 代替スコアのコントロールコードを表します。
        /// </summary>
        public const String Score = "%SCORE";
        /// <summary>
        /// シャドウドキュメントのコントロールコードを表します。
        /// </summary>
        public const String Shadow = "%SHADOW";
    }

    /// <summary>
    /// Hyper Estraier が定義している属性の名前を定義するクラスです。
    /// </summary>
    public static class SystemAttributeNames
    {
        /// <summary>
        /// 文章オブジェクトのIDを表します。
        /// </summary>
        public const String ID = "@id";
        /// <summary>
        /// 文章オブジェクトのURIを表します。
        /// </summary>
        public const String URI = "@uri";
        /// <summary>
        /// 文章オブジェクトのダイジェストを表します。
        /// </summary>
        public const String Digest = "@digest";
        /// <summary>
        /// 文章オブジェクトの作成日時を表します。
        /// </summary>
        public const String CDate = "@cdate";
        /// <summary>
        /// 文章オブジェクトの変更日時を表します。
        /// </summary>
        public const String MDate = "@mdate";
        /// <summary>
        /// 文章オブジェクトの最終アクセス日時を表します。
        /// </summary>
        public const String ADate = "@adate";
        /// <summary>
        /// 文章オブジェクトのタイトルを表します。
        /// </summary>
        public const String Title = "@title";
        /// <summary>
        /// 文章オブジェクトの著者を表します。
        /// </summary>
        public const String Author = "@author";
        /// <summary>
        /// 文章オブジェクトの内容の種類(コンテントタイプ等)を表します。
        /// </summary>
        public const String Type = "@type";
        /// <summary>
        /// 文章オブジェクトの言語を表します。
        /// </summary>
        public const String Lang = "@lang";
        /// <summary>
        /// 文章オブジェクトのジャンルを表します。
        /// </summary>
        public const String Genre = "@genre";
        /// <summary>
        /// 文章オブジェクトのサイズを表します。
        /// </summary>
        public const String Size = "@size";
        /// <summary>
        /// 文章オブジェクトのスコアウエイトを表します。
        /// </summary>
        public const String Weight = "@weight";
        /// <summary>
        /// 文章オブジェクトのその他の情報を表します。
        /// </summary>
        public const String Misc = "@misc";
    }
}
