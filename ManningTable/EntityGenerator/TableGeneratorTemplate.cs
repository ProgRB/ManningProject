/***********************************************************/
/**********   Generated at 21.11.2016 08:37:40     ********/
/*********************************************************/
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data;
using Oracle.DataAccess.Client;
using System.Data.Linq.Mapping;

namespace EntityGenerator
{
    
    [Table(Name="SUBDIV_PART_TYPE"), SchemaName("APSTAFF")]
    public partial class SubdivPartType : RowEntityBase
    {
        #region Class Members
        /// <summary>
        /// Уникальный год номер типа подструктуры
        /// </summary>
        [Column(Name="SUBDIV_PART_TYPE_ID", CanBeNull=false)]
        public Decimal? SubdivPartTypeID 
        {
            get
            {
                return this.GetDataRowField<Decimal?>(() => SubdivPartTypeID);
            }
            set
            {
                UpdateDataRow<Decimal?>(() => SubdivPartTypeID, value);
            }
        }
        /// <summary>
        /// Код типа подструктуры
        /// </summary>
        [Column(Name="SUBDIV_PART_TYPE_CODE")]
        public String SubdivPartTypeCode 
        {
            get
            {
                return this.GetDataRowField<String>(() => SubdivPartTypeCode);
            }
            set
            {
                UpdateDataRow<String>(() => SubdivPartTypeCode, value);
            }
        }
        /// <summary>
        /// Наименование типа подструктуры
        /// </summary>
        [Column(Name="SUBDIV_PART_TYPE_NAME")]
        public String SubdivPartTypeName 
        {
            get
            {
                return this.GetDataRowField<String>(() => SubdivPartTypeName);
            }
            set
            {
                UpdateDataRow<String>(() => SubdivPartTypeName, value);
            }
        }
                #endregion
        
        		#region Ссылка на классы по другим данным
        
        		#endregion
    }


    [Table(Name="SUBDIV_PARTITION"), SchemaName("APSTAFF")]
    public partial class SubdivPartition : RowEntityBase
    {
        #region Class Members
        /// <summary>
        /// Уникальный номер подструктуры
        /// </summary>
        [Column(Name="SUBDIV_PARTITION_ID")]
        public Decimal? SubdivPartitionID 
        {
            get
            {
                return this.GetDataRowField<Decimal?>(() => SubdivPartitionID);
            }
            set
            {
                UpdateDataRow<Decimal?>(() => SubdivPartitionID, value);
            }
        }
        /// <summary>
        /// Ссылка на уникальный тип структуры подразделения
        /// </summary>
        [Column(Name="SUBDIV_PART_TYPE_ID", CanBeNull=false)]
        public Decimal? SubdivPartTypeID 
        {
            get
            {
                return this.GetDataRowField<Decimal?>(() => SubdivPartTypeID);
            }
            set
            {
                UpdateDataRow<Decimal?>(() => SubdivPartTypeID, value);
            }
        }
        /// <summary>
        /// Номер подструктуры
        /// </summary>
        [Column(Name="SUBDIV_NUMBER", CanBeNull=false)]
        public String SubdivNumber 
        {
            get
            {
                return this.GetDataRowField<String>(() => SubdivNumber);
            }
            set
            {
                UpdateDataRow<String>(() => SubdivNumber, value);
            }
        }
        /// <summary>
        /// Ссылка на родительский элемент структуры
        /// </summary>
        [Column(Name="PARENT_SUBDIV_ID")]
        public Decimal? ParentSubdivID 
        {
            get
            {
                return this.GetDataRowField<Decimal?>(() => ParentSubdivID);
            }
            set
            {
                UpdateDataRow<Decimal?>(() => ParentSubdivID, value);
            }
        }
        /// <summary>
        /// Подразделение которому принадлежит струкутра
        /// </summary>
        [Column(Name="SUBDIV_ID", CanBeNull=false)]
        public Decimal? SubdivID 
        {
            get
            {
                return this.GetDataRowField<Decimal?>(() => SubdivID);
            }
            set
            {
                UpdateDataRow<Decimal?>(() => SubdivID, value);
            }
        }
        /// <summary>
        /// Дата начала действия структуры
        /// </summary>
        [Column(Name="DATE_START_SUBDIV_PART", CanBeNull=false)]
        public DateTime? DateStartSubdivPart 
        {
            get
            {
                return this.GetDataRowField<DateTime?>(() => DateStartSubdivPart);
            }
            set
            {
                UpdateDataRow<DateTime?>(() => DateStartSubdivPart, value);
            }
        }
        /// <summary>
        /// Дата окончания действия структуры
        /// </summary>
        [Column(Name="DATE_END_SUBDIV_PART")]
        public DateTime? DateEndSubdivPart 
        {
            get
            {
                return this.GetDataRowField<DateTime?>(() => DateEndSubdivPart);
            }
            set
            {
                UpdateDataRow<DateTime?>(() => DateEndSubdivPart, value);
            }
        }
        /// <summary>
        /// Наименование структурного подразделения
        /// </summary>
        [Column(Name="SUBDIV_PART_NAME", CanBeNull=false)]
        public String SubdivPartName 
        {
            get
            {
                return this.GetDataRowField<String>(() => SubdivPartName);
            }
            set
            {
                UpdateDataRow<String>(() => SubdivPartName, value);
            }
        }
                #endregion
        
        		#region Ссылка на классы по другим данным
        
        		#endregion
    }


    [Table(Name="INDIVID_PROTECTION"), SchemaName("APSTAFF")]
    public partial class IndividProtection : RowEntityBase
    {
        #region Class Members
        [Column(Name="INDIVID_PROTECTION_ID", CanBeNull=false)]
        public Decimal? IndividProtectionID 
        {
            get
            {
                return this.GetDataRowField<Decimal?>(() => IndividProtectionID);
            }
            set
            {
                UpdateDataRow<Decimal?>(() => IndividProtectionID, value);
            }
        }
        [Column(Name="CODE_PROTECTION", CanBeNull=false)]
        public String CodeProtection 
        {
            get
            {
                return this.GetDataRowField<String>(() => CodeProtection);
            }
            set
            {
                UpdateDataRow<String>(() => CodeProtection, value);
            }
        }
        [Column(Name="NAME_PROTECTION")]
        public String NameProtection 
        {
            get
            {
                return this.GetDataRowField<String>(() => NameProtection);
            }
            set
            {
                UpdateDataRow<String>(() => NameProtection, value);
            }
        }
        [Column(Name="TYPE_INDIVID_PROTECTION_ID", CanBeNull=false)]
        public Decimal? TypeIndividProtectionID 
        {
            get
            {
                return this.GetDataRowField<Decimal?>(() => TypeIndividProtectionID);
            }
            set
            {
                UpdateDataRow<Decimal?>(() => TypeIndividProtectionID, value);
            }
        }
                #endregion
        
        		#region Ссылка на классы по другим данным
        
        		#endregion
    }


    [Table(Name="TYPE_INDIVID_PROTECTION"), SchemaName("APSTAFF")]
    public partial class TypeIndividProtection : RowEntityBase
    {
        #region Class Members
        [Column(Name="TYPE_INDIVID_PROTECTION_ID", CanBeNull=false)]
        public Decimal? TypeIndividProtectionID 
        {
            get
            {
                return this.GetDataRowField<Decimal?>(() => TypeIndividProtectionID);
            }
            set
            {
                UpdateDataRow<Decimal?>(() => TypeIndividProtectionID, value);
            }
        }
        [Column(Name="TYPE_PROTECTION_NAME", CanBeNull=false)]
        public String TypeProtectionName 
        {
            get
            {
                return this.GetDataRowField<String>(() => TypeProtectionName);
            }
            set
            {
                UpdateDataRow<String>(() => TypeProtectionName, value);
            }
        }
                #endregion
        
        		#region Ссылка на классы по другим данным
        
        		#endregion
    }

}
