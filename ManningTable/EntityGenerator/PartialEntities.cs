using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EntityGenerator
{
    public partial class SubdivPartition
    {
        Subdiv _subdiv;
        /// <summary>
        /// Подразделение к которому привязана структура
        /// </summary>
        public Subdiv Subdiv
        {
            get
            {
                //if (_subdiv==null)
                    _subdiv = GetParentEntity<Subdiv, decimal?>(() => SubdivID);
                return _subdiv;
            }
        }

        List<SubdivPartition> _listChildren;
        /// <summary>
        /// Дочерние элементы построения подструктуры
        /// </summary>
        public List<SubdivPartition> PartitionChildren
        {
            get
            {
                if (_listChildren == null)
                {
                    _listChildren = DataTable.Rows.OfType<DataRow>().Where(r => r.RowState != DataRowState.Deleted && r.Field2<decimal?>("PARENT_SUBDIV_ID") == SubdivPartitionID)
                           .Select(r => new SubdivPartition() { DataRow = r }).OrderBy(r => r.SubdivNumber).ToList();
                    foreach (var p in _listChildren)
                        p.ParentSubdivPartition = this;
                }
                return _listChildren;
            }
        }

        SubdivPartition _parentSubdivPartition;
        /// <summary>
        /// Ссылка на родительское структурное подразделение
        /// </summary>
        public SubdivPartition ParentSubdivPartition
        {
            get
            {
                if (_parentSubdivPartition == null)
                    _parentSubdivPartition = GetParentEntity<SubdivPartition>("SUBDIV_PARTITION", DataSet, "SUBDIV_PARTITION_ID", this.ParentSubdivID);
                return _parentSubdivPartition;
            }
            set
            {
                _parentSubdivPartition = value;
                RaisePropertyChanged(() => ParentSubdivPartition);
            }
        }

        /// <summary>
        /// Уровень иерахии структурного подразделения
        /// </summary>
        public int SubLevel
        {
            get
            {
                if (ParentSubdivID == null)
                    return 1;
                else
                    return ParentSubdivPartition.SubLevel + 1;
            }
        }

        SubdivPartType _subdivPartType;
        /// <summary>
        /// Ссылка на тип подразделения
        /// </summary>
        public SubdivPartType SubdivPartType
        {
            get
            {
                //if (_subdivPartType==null)
                    _subdivPartType = GetParentEntity<SubdivPartType, decimal?>(() => SubdivPartTypeID);
                return _subdivPartType;
            }
        }

        /// <summary>
        /// Код структурного подразделения
        /// </summary>
        public string CodeSubdivPartition
        {
            get
            {
                return string.Format("{0}.{1}.{2}", (Subdiv == null ? "000" : Subdiv.CodeSubdiv), SubdivPartType != null ? SubdivPartType.SubdivPartTypeCode : string.Format("{0:00}", (SubdivPartTypeID??0)), SubdivNumber);
            }
        }

        /// <summary>
        /// Поиск элемента в дереве узлов по айдишнику узла
        /// </summary>
        /// <param name="subdivPartitionID"></param>
        /// <returns></returns>
        public SubdivPartition FindNode(decimal? subdivPartitionID)
        {
            if (DataTable != null)
            {
                if (SubdivPartitionID == subdivPartitionID)
                    return this;
                else
                    foreach (var p in PartitionChildren)
                    {
                        var finded = p.FindNode(subdivPartitionID);
                        if (finded != null)
                            return finded;
                    }
                return null;
            }
            else
                return null;
        }


    }
}
