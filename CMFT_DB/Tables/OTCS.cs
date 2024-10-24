using System;
using System.Collections.Generic;
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
    
}
