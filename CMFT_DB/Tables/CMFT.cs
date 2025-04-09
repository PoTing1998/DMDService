using System;
using System.Collections.Generic;
using ASI.Wanda.CMFT.DB.Models.CMFT;

namespace ASI.Wanda.CMFT.DB.Tables.CMFT
{
    public class cmftAccount : ASI.Wanda.CMFT.DB.Tables.Table<cmft_account>
    {
        #region Methods
        static public cmft_account SelectAccount(string accountID)
        {
            return Select(accountID);
        }
        static public void InsertAccount(string accountID, string accountName, string accountPassword, string department, Guid roleID, bool isEnable, int remainCount)
        {
            Insert(
                      accountID
                    , accountName
                    , accountPassword
                    , department
                    , roleID
                    , isEnable
                    , remainCount
                    );
        }
        static public void UpdateAccount(string accountID, string accountName, string accountPassword, string department, Guid roleID, bool isEnable, int remainCount)
        {
            Update(
                      accountID
                    , accountName
                    , accountPassword
                    , department
                    , roleID
                    , isEnable
                    , remainCount);
        }
        static public void DeleteAccount(string accountID)
        {
            Delete(accountID);
        }
        #endregion
    }

    public class cmftOper : ASI.Wanda.CMFT.DB.Tables.Table<cmft_oper>
    {
        #region Methods
        static public void DeleteAccount(string accountID)
        {
            Delete(accountID);
        }
        #endregion
    }

    public class cmftRole : ASI.Wanda.CMFT.DB.Tables.Table<cmft_role>
    {
        #region Methods
        static public cmft_role SelectRole(Guid roleID)
        {
            return Select(roleID);
        }
        static public void InsertRole(Guid roleID, string roleName, string roleDescription)
        {
            Insert(roleID
                  , roleName
                  , roleDescription);
        }
        static public void UpdateRole(Guid roleID, string roleName, string roleDescription)
        {
            Update(
                 roleID
                 , roleName
                 , roleDescription);
        }
        static public void DeleteRole(Guid roleID)
        {
            Delete(roleID);
        }
        #endregion
    }

    public class cmftRoleAuth : ASI.Wanda.CMFT.DB.Tables.Table<cmft_role_auth>
    {
        #region Methods
        static public cmft_role_auth SelectRoleAuth(Guid roleID, string operationID)
        {
            return Select(roleID, operationID);
        }
        static public List<cmft_role_auth> SelectAllRoleAuths(Guid roleID)
        {
            return SelectWhere(string.Format("where role_id = '{0}'", roleID));
        }
        static public void InsertRoleAuth(Guid roleID, string operationID, bool allowBrowsing, bool allowSetting)
        {
            Insert(roleID, operationID, allowBrowsing, allowSetting);
        }
        static public void UpdateRoleAuth(Guid roleID, string operationID, bool allowBrowsing, bool allowSetting)
        {
            Update(roleID, operationID, allowBrowsing, allowSetting);
        }
        static public void DeleteRoleAuth(Guid roleID, string operationID)
        {
            Delete(roleID, operationID);
        }
        static public void DeleteRoleAuths(Guid roleID)
        {
            string whereString = string.Format("where role_id = '{0}'", roleID);
            DeleteWhere(whereString);
        }
        #endregion
    }

    public class cmftCommand : ASI.Wanda.CMFT.DB.Tables.Table<cmft_command>
    {
        #region Methods
        static public cmft_command SelectCommand(string seatID,int msgID)
        {
            return Select(seatID, msgID);
        }

        /// <summary>
        /// 取得指定條件的最新一筆命令紀錄
        /// </summary>
        /// <param name="seatID"></param>
        /// <param name="systemID"></param>
        /// <param name="stationID"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        static public cmft_command GetLastCommand(string seatID, string systemID, string stationID, string responseType)
        {
            string sWhereString = $"where seat_id = '{seatID}' and system_id = '{systemID}' and station_id = '{stationID}' and response_type = '{responseType}' ";
            List<cmft_command> oCMFTCommandList = SelectWhere(sWhereString, eSortWay.Desc);
            if (oCMFTCommandList != null && oCMFTCommandList.Count >= 0)
            {
                return oCMFTCommandList[0];
            }

            return null;
        }

        static public int UpdateCommand(string seatID, int msgID, string systemID, string jsonObjectName, string stationID, string resPonseType, string isSuccess)
        {
            int impactRow = 0;
            impactRow = Update(seatID,  msgID,  systemID,  jsonObjectName,  stationID,  resPonseType,  isSuccess);
            if (impactRow <= 0)
            {
                impactRow = Insert(seatID, msgID, systemID, jsonObjectName, stationID, resPonseType, isSuccess);
            }

            return impactRow;
        }

        static public void DeleteCommand(string seatID, int msgID)
        {
            string whereString = $"where seat_id = '{seatID}' and msg_id = '{msgID}'";
            DeleteWhere(whereString);
        }

        #endregion
    }
}
