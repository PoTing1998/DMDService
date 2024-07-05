using ASI.Wanda.DMD.DB.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.DB.Tables.OCS
{
    public class ocsData : ASI.Wanda.DMD.DB.Tables.Table<ocs_data>
    {
        #region Methods

        static  public void insert()
        {

        }
        //static public ocsData SelectGroup(Guid groupID)
        //{
        //    //string whereString = string.Format("where group_id = '{0}'", groupID);
        //    //var group = SelectWhere(whereString);
        //    //return group.Count > 0 ? group[0] : null;
        //}
        static public void InsertGroup(int groupID, int groupName)
        {
            Insert(groupID, groupName);
        }
        static public void UpdateGroup(Guid groupID, string groupName, string groupDescription)
        {
            Update(groupID, groupName, groupDescription);
        }
        static public void DeleteGroup(Guid groupID)
        {
            Delete(groupID);
        }
        #endregion
    }
    public class dmdGroupTarget : ASI.Wanda.DMD.DB.Tables.Table<dmd_group_target>
    {
        #region Methods
        static public List<dmd_group_target> SelectGroupTargets(Guid groupID)
        {
            var whereString = string.Format("where group_id = '{0}'", groupID);
            var devices = SelectWhere(whereString);
            return devices;
        }
        static public void InsertGroupTarget(Guid groupID, string stationID, string AreaID, string DeviceID)
        {
            Insert(groupID, stationID, AreaID, DeviceID);
        }
        static public void DeleteGroupTarget(Guid groupID, string stationID, string AreaID, string DeviceID)
        {
            Delete(groupID, stationID, AreaID, DeviceID);
        }
        static public void DeleteGroupTargets(Guid groupID)
        {
            string whereString = string.Format("where group_id = '{0}'", groupID);
            DeleteWhere(whereString);
        }
        #endregion
    }
}
