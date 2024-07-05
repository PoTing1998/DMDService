using System;
using System.Collections.Generic;
using ASI.Wanda.CMFT.DB.Models.Sync;

namespace ASI.Wanda.CMFT.DB.Tables.Sync
{
    public class syncLinkedSetting : ASI.Wanda.CMFT.DB.Tables.Table<sync_linked_setting>
    {
        #region Methods
        static public void InsertLinkedSetting(string paFolderID, Guid dmdMessageID, Guid pidsMessageID)
        {
            Insert(  paFolderID
                   , dmdMessageID
                   , pidsMessageID );
        }
        static public void UpdateLinkedSetting(string paFolderID, Guid dmdMessageID, Guid pidsMessageID)
        {
            Update(  paFolderID
                   , dmdMessageID
                   , pidsMessageID);
        }
        static public void DeleteLinkedSetting(string paFolderID)
        {
            Delete(paFolderID );
        }
        
        #endregion

        #region Source

        #endregion
    }
}
