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
    /// Hyper Estraier �̃R���g���[���R�[�h�̖��O���`����N���X�ł��B
    /// </summary>
    public static class ControlCodes
    {
        /// <summary>
        /// �L�[���[�h�x�N�^�̃R���g���[���R�[�h��\���܂��B
        /// </summary>
        public const String Vector = "%VECTOR";
        /// <summary>
        /// ��փX�R�A�̃R���g���[���R�[�h��\���܂��B
        /// </summary>
        public const String Score = "%SCORE";
        /// <summary>
        /// �V���h�E�h�L�������g�̃R���g���[���R�[�h��\���܂��B
        /// </summary>
        public const String Shadow = "%SHADOW";
    }

    /// <summary>
    /// Hyper Estraier ����`���Ă��鑮���̖��O���`����N���X�ł��B
    /// </summary>
    public static class SystemAttributeNames
    {
        /// <summary>
        /// ���̓I�u�W�F�N�g��ID��\���܂��B
        /// </summary>
        public const String ID = "@id";
        /// <summary>
        /// ���̓I�u�W�F�N�g��URI��\���܂��B
        /// </summary>
        public const String URI = "@uri";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̃_�C�W�F�X�g��\���܂��B
        /// </summary>
        public const String Digest = "@digest";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̍쐬������\���܂��B
        /// </summary>
        public const String CDate = "@cdate";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̕ύX������\���܂��B
        /// </summary>
        public const String MDate = "@mdate";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̍ŏI�A�N�Z�X������\���܂��B
        /// </summary>
        public const String ADate = "@adate";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̃^�C�g����\���܂��B
        /// </summary>
        public const String Title = "@title";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̒��҂�\���܂��B
        /// </summary>
        public const String Author = "@author";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̓��e�̎��(�R���e���g�^�C�v��)��\���܂��B
        /// </summary>
        public const String Type = "@type";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̌����\���܂��B
        /// </summary>
        public const String Lang = "@lang";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̃W��������\���܂��B
        /// </summary>
        public const String Genre = "@genre";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̃T�C�Y��\���܂��B
        /// </summary>
        public const String Size = "@size";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̃X�R�A�E�G�C�g��\���܂��B
        /// </summary>
        public const String Weight = "@weight";
        /// <summary>
        /// ���̓I�u�W�F�N�g�̂��̑��̏���\���܂��B
        /// </summary>
        public const String Misc = "@misc";
    }
}
