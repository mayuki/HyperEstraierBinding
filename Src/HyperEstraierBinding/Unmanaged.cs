using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Security;
using System.Runtime.ConstrainedExecution;

namespace HyperEstraier
{
    /// <summary>
    /// アンマネージコードを呼び出すためのクラス
    /// </summary>
    [SuppressUnmanagedCodeSecurity()]
    internal static class Unmanaged
    {
        private const String HyperEstraierLibrary = @"estraier.dll";
        private const String QDBMLibrary = @"qdbm.dll";

        #region Hyper Estraier API

        #region Document API
        [DllImport(HyperEstraierLibrary)]
        public static extern DocumentHandle est_doc_new();

        [DllImport(HyperEstraierLibrary)]
        public static extern DocumentHandle est_doc_new_from_draft([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String draft);

        [DllImport(HyperEstraierLibrary)]
        [ReliabilityContract(Consistency.MayCorruptProcess, Cer.MayFail)]
        public static extern void est_doc_delete(IntPtr doc);

        [DllImport(HyperEstraierLibrary)]
        public static extern Int32 est_doc_id(DocumentHandle doc);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_doc_add_text(DocumentHandle doc, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(UTF8StringMarshaler))]String text);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_doc_add_hidden_text(DocumentHandle doc, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String text);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_doc_set_keywords(DocumentHandle doc, CBMapHandle kwords);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_doc_add_attr(DocumentHandle doc, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String name, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String value);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_doc_set_score(DocumentHandle doc, Int32 score);

        // 戻り値は解放してはいけない
        [DllImport(HyperEstraierLibrary)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]
        public static extern String est_doc_attr(DocumentHandle doc, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String name);

        [DllImport(HyperEstraierLibrary)]
        public static extern CBListHandle est_doc_attr_names(DocumentHandle doc);

        [DllImport(HyperEstraierLibrary)]
        public static extern MallocHandle est_doc_cat_texts(DocumentHandle doc);

        // 戻り値は解放してはいけない
        [DllImport(HyperEstraierLibrary)]
        public static extern IntPtr est_doc_texts(DocumentHandle doc);

        [DllImport(HyperEstraierLibrary)]
        public static extern MallocHandle est_doc_dump_draft(DocumentHandle doc);

        [DllImport(HyperEstraierLibrary)]
        public static extern MallocHandle est_doc_make_snippet(DocumentHandle doc, CBListHandle words, Int32 wwidth, Int32 hwidth, Int32 awidth);
        #endregion

        #region Condition API
        // condition
        [DllImport(HyperEstraierLibrary)]
        public static extern ConditionHandle est_cond_new();

        [DllImport(HyperEstraierLibrary)]
        [ReliabilityContract(Consistency.MayCorruptProcess, Cer.MayFail)]
        public static extern void est_cond_delete(IntPtr cond);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_cond_set_phrase(ConditionHandle cond, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String phrase);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_cond_add_attr(ConditionHandle cond, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String expr);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_cond_set_order(ConditionHandle cond, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String expr);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_cond_set_max(ConditionHandle cond, Int32 max);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_cond_set_skip(ConditionHandle cond, Int32 skip);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_cond_set_options(ConditionHandle cond, ConditionOptions options);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_cond_set_auxiliary(ConditionHandle cond, Int32 min);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_cond_set_eclipse(ConditionHandle cond, Double limit);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_cond_set_mask(ConditionHandle cond, Int32 mask);
        #endregion

        #region Database API
        // condition
        [DllImport(HyperEstraierLibrary)]
        public static extern DatabaseHandle est_db_open([MarshalAs(UnmanagedType.LPStr)]String name, DatabaseModes omode, out Int32 ecp);
        //public static extern DatabaseHandle est_db_open([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String name, DatabaseModes omode, out Int32 ecp);

        [DllImport(HyperEstraierLibrary)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern Boolean est_db_close(IntPtr db, out Int32 ecp);

        // 戻り値は解放してはいけないのでそのままString
        [DllImport(HyperEstraierLibrary)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]
        public static extern String est_err_msg(Int32 errorCode);

        [DllImport(HyperEstraierLibrary)]
        public static extern Int32 est_db_error(DatabaseHandle db);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_fatal(DatabaseHandle db);

        [DllImport(HyperEstraierLibrary)]
        public static extern Double est_db_size(DatabaseHandle db);

        [DllImport(HyperEstraierLibrary)]
        public static extern MallocHandle est_db_search(DatabaseHandle db, ConditionHandle cond, out int nump, CBMapHandle hints);

        [DllImport(HyperEstraierLibrary)]
        public static extern DocumentHandle est_db_get_doc(DatabaseHandle db, Int32 id, GetDocumentOptions options);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_flush(DatabaseHandle db, Int32 max);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_sync(DatabaseHandle db);

        [DllImport(HyperEstraierLibrary)]
        public static extern IntPtr est_db_name(DatabaseHandle db);

        [DllImport(HyperEstraierLibrary)]
        public static extern Int32 est_db_doc_num(DatabaseHandle db);

        [DllImport(HyperEstraierLibrary)]
        public static extern Int32 est_db_word_num(DatabaseHandle db);

        [DllImport(HyperEstraierLibrary)]
        public static extern Int32 est_db_uri_to_id(DatabaseHandle db, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String uri);

        [DllImport(HyperEstraierLibrary)]
        public static extern MallocHandle est_db_get_doc_attr(DatabaseHandle db, Int32 id, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String name);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_edit_doc(DatabaseHandle db, DocumentHandle doc);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_put_doc(DatabaseHandle db, DocumentHandle doc, PutDocumentOptions options);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_out_doc(DatabaseHandle db, Int32 id, OutDocumentOptions options);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_add_attr_index(DatabaseHandle db, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String name, AddAttributeIndexTypes type);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_optimize(DatabaseHandle db, OptimizeOptions options);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_merge(DatabaseHandle db, MergeOptions options);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_scan_doc(DatabaseHandle db, DocumentHandle doc, ConditionHandle cond);

        [DllImport(HyperEstraierLibrary)]
        public static extern void est_db_set_cache_size(DatabaseHandle db, Int32 size, Int32 anum, Int32 tnum, Int32 rnum);

        [DllImport(HyperEstraierLibrary)]
        public static extern Boolean est_db_add_pseudo_index(DatabaseHandle db, String path);

        #endregion

        #endregion

        #region QDBM Library API
        //
        // QDBM
        //

        // cblist

        [DllImport(QDBMLibrary)]
        public static extern CBListHandle cblistopen();

        [DllImport(QDBMLibrary)]
        [ReliabilityContract(Consistency.MayCorruptProcess, Cer.MayFail)]
        public static extern void cblistclose(IntPtr cblist);

        [DllImport(QDBMLibrary)]
        public static extern Int32 cblistnum(CBListHandle cblist);

        [DllImport(QDBMLibrary)]
        public static extern void cblistpush(CBListHandle cblist, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String value, Int32 size);

        [DllImport(QDBMLibrary)]
        public static extern MallocHandle cblistpop(CBListHandle cblist, out Int32 size);

        [DllImport(QDBMLibrary)]
        public static extern IntPtr cblistval(CBListHandle cblist, Int32 index, out Int32 size);

        [DllImport(QDBMLibrary)]
        public static extern Int32 cblistlsearch(CBListHandle cblist, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String value, Int32 size);

        [DllImport(QDBMLibrary)]
        public static extern MallocHandle cblistremove(CBListHandle cblist, Int32 index, out Int32 size);

        [DllImport(QDBMLibrary)]
        public static extern void cblistover(CBListHandle cblist, Int32 index, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String value, Int32 size);

        // cbmap

        [DllImport(QDBMLibrary)]
        public static extern CBMapHandle cbmapopen();

        [DllImport(QDBMLibrary)]
        public static extern CBMapHandle cbmapopenex(Int32 size);

        [DllImport(QDBMLibrary)]
        [ReliabilityContract(Consistency.MayCorruptProcess, Cer.MayFail)]
        public static extern void cbmapclose(IntPtr cbmapPtr);

        [DllImport(QDBMLibrary)]
        public static extern IntPtr cbmapget(CBMapHandle mapPtr, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String kbuf, Int32 ksiz, ref Int32 sp);

        [DllImport(QDBMLibrary)]
        public static extern Boolean cbmapput(CBMapHandle mapPtr, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String kbuf, Int32 ksiz, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String vbuf, Int32 vsiz, Int32 over);

        [DllImport(QDBMLibrary)]
        public static extern Boolean cbmapout(CBMapHandle mapPtr, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String kbuf, Int32 ksiz);

        [DllImport(QDBMLibrary)]
        public static extern CBListHandle cbmapkeys(CBMapHandle cbmapPtr);

        [DllImport(QDBMLibrary)]
        public static extern CBListHandle cbmapvals(CBMapHandle cbmapPtr);

        [DllImport(QDBMLibrary)]
        public static extern Int32 cbmaprnum(CBMapHandle cbmapPtr);

        [DllImport(QDBMLibrary)]
        public static extern Int32 cbmapiterinit(CBMapHandle cbmapPtr);

        [DllImport(QDBMLibrary)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]
        public static extern String cbmapiternext(CBMapHandle cbmapPtr, Int32 size);

        [DllImport(QDBMLibrary)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]
        public static extern String cbmapiterval([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]String kbuf, Int32 size);

        #endregion

        #region アンマネージヘルパーメソッド

        /// <summary>
        /// MallocHandle から UTF-8 のバイト列として文字列を読み出します。
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static String PtrToString(MallocHandle handle)
        {
            Boolean success = false;
            try
            {
                handle.DangerousAddRef(ref success);
                return PtrToString(handle.DangerousGetHandle());
            }
            finally
            {
                if (success)
                    handle.DangerousRelease();
            }
        }

        /// <summary>
        /// ポインタから UTF-8 のバイト列として文字列を読み出します。
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static String PtrToString(IntPtr ptr)
        {
            using (MemoryStream memStream = new MemoryStream(1024))
            {
                unsafe
                {
                    Byte* bp = (Byte*)ptr;
                    while (*bp != 0)
                    {
                        memStream.WriteByte(*(bp++));
                    }
                }
                //Int32 i = 0;
                //Byte b = 0;
                //while ((b = Marshal.ReadByte(ptr, i++)) != 0)
                //{
                //    memStream.WriteByte(b);
                //}
                return Encoding.UTF8.GetString(memStream.ToArray());
            }
        }

        /// <summary>
        /// MallocHandle から UTF-8 のバイト列として文字列を読み出します。
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static String PtrToString(MallocHandle handle, Int32 len)
        {
            Boolean success = false;
            try
            {
                handle.DangerousAddRef(ref success);
                return PtrToString(handle.DangerousGetHandle(), len);
            }
            finally
            {
                if (success)
                    handle.DangerousRelease();
            }
        }

        /// <summary>
        /// ポインタから UTF-8 のバイト列として文字列を読み出します。
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static String PtrToString(IntPtr ptr, Int32 len)
        {
            Byte[] bytes = new Byte[len];
            for (Int32 i = 0; i < len; i++)
            {
                bytes[i] = Marshal.ReadByte(ptr, i);
            }
            return Encoding.UTF8.GetString(bytes);
        }

        #endregion
    }
}
