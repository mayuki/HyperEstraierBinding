using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HyperEstraier
{
    /// <summary>
    /// �v���b�g�t�H�[���Ăяo���̂��߂� System.String �^�� UTF-8 �̃o�C�g�z��Ƃ��ă}�[�V�������O���܂��B
    /// </summary>
    class UTF8StringMarshaler : ICustomMarshaler
    {
        private GCHandle _handle;
        public static ICustomMarshaler GetInstance(String pstrCookie)
        {
            return new UTF8StringMarshaler();
        }

        #region ICustomMarshaler �����o

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
                throw new Exception("�Ώۂ̌^������������܂���BUTF8StringMarshaler �� System.String �^�Ɏw�肵�Ă��������B");

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
