using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// プラットフォーム呼び出しのために System.String 型を UTF-8 のバイト配列としてマーシャリングします。
    /// </summary>
    class UTF8StringMarshaler : ICustomMarshaler
    {
        private GCHandle _handle;
        public static ICustomMarshaler GetInstance(String pstrCookie)
        {
            return new UTF8StringMarshaler();
        }

        #region ICustomMarshaler メンバ

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            _handle.Free();
        }

        public int GetNativeDataSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj == null)
                return IntPtr.Zero;

            if (!(ManagedObj is String))
                throw new Exception("対象の型が正しくありません。UTF8StringMarshaler は System.String 型に指定してください。");

            String stringValue = ManagedObj as String;
            Byte[] bytes = new Byte[Encoding.UTF8.GetByteCount(stringValue)+1];
            Encoding.UTF8.GetBytes(stringValue, 0, stringValue.Length, bytes, 0);
            bytes[bytes.Length - 1] = 0;

            _handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            //IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);

            return _handle.AddrOfPinnedObject();
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
                return null;

            return Unmanaged.PtrToString(pNativeData);
        }

        #endregion
    }
}
