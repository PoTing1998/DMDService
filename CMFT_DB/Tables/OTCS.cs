using System;
using System.Collections.Generic;
using System.Linq;
using ASI.Wanda.CMFT.DB.Models.OTCS;

namespace ASI.Wanda.CMFT.DB.Tables.OTCS
{
    public class otcsPreRecordMessage : ASI.Wanda.CMFT.DB.Tables.Table<otcs_pre_record_message>
    {
        #region Methods
        public static otcs_pre_record_message SelectMessage(Guid messageID)
        {
            return Select(messageID);
        }
        public static void InsertPreRecordMessage(Guid messageID, string messageName, int messageType, int messagePriority, int moveMode, int moveSpeed, string interval, string messageContentCHN, string fontTypeCHN, int fontSizeCHN, string fontColorCHN, string messageContentENG, string fontTypeENG, int fontSizeENG, string fontColorENG)
        {
            Insert(messageID, messageName, messageType, messagePriority, moveMode, moveSpeed, interval
                , messageContentCHN, fontTypeCHN, fontSizeCHN, fontColorCHN
                , messageContentENG, fontTypeENG, fontSizeENG, fontColorENG);
        }
        public static void UpdatePreRecordMessage(Guid messageID, string messageName, int messageType, int messagePriority, int moveMode, int moveSpeed, string interval, string messageContentCHN, string fontTypeCHN, int fontSizeCHN, string fontColorCHN, string messageContentENG, string fontTypeENG, int fontSizeENG, string fontColorENG)
        {
            Update(messageID, messageName, messageType, messagePriority, moveMode, moveSpeed, interval
                , messageContentCHN, fontTypeCHN, fontSizeCHN, fontColorCHN
                , messageContentENG, fontTypeENG, fontSizeENG, fontColorENG);
        }
        public static void DeletePreRecordMessage(Guid messageID)
        {
            Delete(messageID);
        }
        #endregion
    }

    public class otcsTrain : ASI.Wanda.CMFT.DB.Tables.Table<otcs_train>
    {
        #region Methods
        
 
        #endregion
    }
    public class otcsEquipStatus : ASI.Wanda.CMFT.DB.Tables.Table<otcs_equip_status>
    {
        #region Methods


        #endregion
    }
    public class otcsEventList : ASI.Wanda.CMFT.DB.Tables.Table<otcs_event_list>
    {
        #region Methods


        #endregion
    }
    public class otcsEvent : ASI.Wanda.CMFT.DB.Tables.Table<otcs_event>
    {
        #region Methods
        /// <summary>
        /// 出現異常 高(1) 未確認
        /// 出現異常 高(1) 已確認
        /// 異常回復 高(1) 未確認 select 
        /// 異常回復 高(1) 已確認 select  
        /// </summary>
        /// <returns></returns>
        public static List<ASI.Wanda.CMFT.DB.Models.OTCS.otcs_event> SelectUnReleaseedEvents()
        {
            var temp = SelectAll().Where(x => string.IsNullOrEmpty(x.release_time) == true).ToList();
            return temp;
        }
        /// <summary>
        /// 出現異常 高(1) 未確認 select 
        /// 出現異常 高(1) 已確認 select 
        /// 異常回復 高(1) 未確認 select 
        /// 異常回復 高(1) 已確認
        /// </summary>
        /// <returns></returns>
        public static List<ASI.Wanda.CMFT.DB.Models.OTCS.otcs_event> SelectDisplayEvent()
        {
            string whereString = string.Format("where (release_time = '{0}' and check_time = '{1}') or (release_time = '{2}') or (check_time = '{3}')"
               , string.Empty
               , string.Empty
               , string.Empty
               , string.Empty);

            var temp = SelectWhere(whereString);

            return temp;
        }
        /// <summary>
        /// 出現異常 高(1) 未確認
        /// 出現異常 高(1) 已確認
        /// 異常回復 高(1) 未確認
        /// 異常回復 高(1) 已確認 select 
        /// </summary>
        /// <returns></returns>
        public static List<ASI.Wanda.CMFT.DB.Models.OTCS.otcs_event> SelectHistoryEvent()
        {
            var temp = SelectAll()
                      .Where(x => string.IsNullOrEmpty(x.release_time) == false && string.IsNullOrEmpty(x.check_account_id) == false)
                      .ToList();

            return temp;
        }



        #endregion
    }
}
