using System;
using System.Collections.Generic;
using System.Text;

namespace HyperEstraier
{
    /// <summary>
    /// ���������I�u�W�F�N�g��\���܂��B
    /// </summary>
    public class Condition : IDisposable
    {
        private ConditionHandle _condHandle;

        /// <summary>
        /// �V�������������I�u�W�F�N�g�𐶐����܂��B
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
        /// �����t���[�Y�����������I�u�W�F�N�g�ɐݒ肵�܂��B
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
        /// �����������������������I�u�W�F�N�g�ɒǉ����܂��B
        /// </summary>
        /// <param name="expr"></param>
        public void AddAttr(String expr)
        {
            CheckDispose();
            Unmanaged.est_cond_add_attr(_condHandle, expr);
        }
        /// <summary>
        /// ���������������I�u�W�F�N�g�ɐݒ肵�܂��B
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
        /// �擾���͐��̍ő吔�����������I�u�W�F�N�g�ɐݒ肵�܂��B
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
        /// �������ʂ���X�L�b�v���镶�͐������������I�u�W�F�N�g�ɐݒ肵�܂��B
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
        /// �����I�v�V���������������I�u�W�F�N�g�ɐݒ肵�܂��B
        /// </summary>
        public ConditionOptions Options
        {
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_options(_condHandle, value);
            }
        }

        // TODO: �ȉ�enum
        /// <summary>
        /// �ގ��B���ɂ����鉺���ގ��x�����������I�u�W�F�N�g�ɐݒ肵�܂��B
        ///
        /// �B������镶���̉����̗ގ��x��0.0����1.0�܂ł̒l�Ŏw�肵�܂����A
        /// `ESTECLSIMURL' �����Z�����URL���d�ݕt���Ɏg���悤�ɂȂ�܂��B
        /// `ESTECLSERV' ���w�肷��Ɨގ��x�𖳎����ē����T�[�o�̕������B�����܂��B
        /// `ESTECLDIR' ���w�肷��Ɨގ��x�𖳎����ē����f�B���N�g���̕������B�����܂��B
        /// `ESTECLSERV' ���w�肷��Ɨގ��x�𖳎����ē����t�@�C���̕������B�����܂��B
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
        /// ���^�����̑Ώۂ̃}�X�N�����������I�u�W�F�N�g�ɐݒ肵�܂��B
        /// 1��1�Ԗڂ̑ΏہA2��2�Ԗڂ̑ΏہA4��3�Ԗڂ̑ΏۂƂ�����2�̗ݏ�̒l�̍��v�Ō�����}�~����Ώۂ��w�肵�܂�
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
            // TODO: ����
            set
            {
                CheckDispose();
                Unmanaged.est_cond_set_auxiliary(_condHandle, value);
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g���j������Ă��邩�ǂ������擾���܂��B
        /// </summary>
        public Boolean IsDisposed
        {
            get
            {
                return (_condHandle == null || _condHandle.IsClosed);
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g�����ɔj������Ă��邩�ǂ������m�F���A�j������Ă���ꍇ�ɂ͗�O System.ObjectDisposedException �𔭐����܂��B
        /// </summary>
        private void CheckDispose()
        {
            if (_condHandle == null || _condHandle.IsClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable �����o

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
    /// ���������̃I�v�V������\���܂��B
    /// </summary>
    [Flags]
    public enum ConditionOptions : int
    {
        /// <summary>
        /// �S�Ă� N-gram �L�[���������܂��B
        /// </summary>
        Sure    = 1 << 0,
        /// <summary>
        /// N-gram �L�[��1�����Ō������܂��B�f�t�H���g�ł��B
        /// </summary>
        Usual   = 1 << 1,
        /// <summary>
        /// N-gram �L�[��2�����Ō������܂��B
        /// </summary>
        Fast    = 1 << 2,
        /// <summary>
        /// N-gram �L�[��3�����Ō������܂��B
        /// </summary>
        Agito   = 1 << 3,
        /// <summary>
        /// TF-IDF �@�ɂ��d�݂Â����ȗ����܂��B
        /// </summary>
        NoIDF   = 1 << 4,
        /// <summary>
        /// �����t���[�Y���ȕ֏����Ƃ��Ĉ����܂��B
        /// </summary>
        Simple  = 1 << 10,
        /// <summary>
        /// �����t���[�Y��e�������Ƃ��Ĉ����܂��B
        /// </summary>
        Rough   = 1 << 11,
        /// <summary>
        /// �����t���[�Y��_���a����(OR ����)�Ƃ��Ĉ����܂��B
        /// </summary>
        Union   = 1 << 15,
        /// <summary>
        /// �����t���[�Y��_���Ϗ���(AND ����)�Ƃ��Ĉ����܂��B
        /// </summary>
        Intersect   = 1 << 16,
        /// <summary>
        /// �X�R�A���t�B�[�h�o�b�N���܂�(�f�o�b�O��p) 
        /// </summary>
        FeedBackScore = 1 << 30
    }

    /// <summary>
    /// ���������I�y���[�^��\���܂��B
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
    /// �������̕��я��I�y���[�^��\���܂��B
    /// </summary>
    public static class OrderOperators
    {
        /// <summary>
        /// ID �����ŕ��ׂ܂��B
        /// </summary>
        public const String IDAscend = "[IDA]";
        /// <summary>
        /// ID �~���ŕ��ׂ܂��B
        /// </summary>
        public const String IDDescend = "[IDD]";
        /// <summary>
        /// �X�R�A �����ŕ��ׂ܂��B
        /// </summary>
        public const String ScoreAscend = "[SCA]";
        /// <summary>
        /// �X�R�A �~���ŕ��ׂ܂��B
        /// </summary>
        public const String ScoreDescend = "[SCD]";
        /// <summary>
        /// ������(������) �����ŕ��ׂ܂��B
        /// </summary>
        public const String StringAscend = "STRA";
        /// <summary>
        /// ������(������) �~���ŕ��ׂ܂��B
        /// </summary>
        public const String StringDescend = "STRD";
        /// <summary>
        /// ���l�܂��͓��t �����ŕ��ׂ܂��B
        /// </summary>
        public const String NumberAscend = "NUMA";
        /// <summary>
        /// ���l�܂��͓��t �~���ŕ��ׂ܂��B
        /// </summary>
        public const String NumberDescend = "NUMD";

    }
}
