using ASI.Wanda.DCU.DB.Models.DU;
using System.Collections.Generic;
using System.Linq;


namespace ASI.Wanda.DCU.DB.Tables.DCU
{
    public class dulist: ASI.Wanda.DCU.DB.Tables.Table<du_list>
    {
        public static void UpdateEquipStatus(string equip_id, string panel_id ,int equip_status)
        {
            var equip =
                 SelectWhere(string.Format("where equip_id = '{0}' AND panel_id ='{1}'", equip_id,panel_id))
                 .SingleOrDefault();
          
            Update(
                equip.equip_id
              , equip.panel_id
              , equip.is_back
              , equip_status
              , equip.ins_user
              
              );
        }

    }

    public class duMessageSetup : ASI.Wanda.DCU.DB.Tables.Table<du_message_setup>
    {
        public static List<object> SelectMSGSetting(string panel_id, string station_id, string area_id)
        {
            var content = new List<object>();
            var MSG = 
                      SelectWhere(string.Format("where panel_id = '{0}' AND station_id = '{1}' AND area_id='{2}'", panel_id, station_id, area_id))
                     .SingleOrDefault();

            content.Add(MSG.text_size);//0
            content.Add(MSG.text_font);
            content.Add(MSG.text_color);
            content.Add(MSG.priority);
            content.Add(MSG.move_type);
            content.Add(MSG.speed);//5
            content.Add(MSG.stay_time);
            content.Add(MSG.content); 
            content.Add(MSG.windowdisplaymode.ToString());
            return content;
        }
       
    }

    public class layoutType : ASI.Wanda.DCU.DB.Tables.Table<layout_type>
    {
         public  static void SelectGenerallyMessage(string  WindowDisplayMode)
        {

        }
    }

    public class trainLocationParametersSetup : ASI.Wanda.DCU.DB.Tables.Table<train_location_parameters_setup> 
    {
       public static void SelectType(int type ,List<object> data) 
        {
            var MSG =
                     SelectWhere(string.Format("where train_type = '{0}'", type)) 
                    .SingleOrDefault();
            data.Add(MSG.image_change_quantity);//0
            data.Add(MSG.dynamic_text_command);
            data.Add(MSG.starting_station_color);
            data.Add(MSG.starting_station_name);
            data.Add(MSG.image_dynamic_command);
            data.Add(MSG.image_start_index);//5
            data.Add(MSG.image_change_quantity);
            data.Add(MSG.image_color);
            data.Add(MSG.prev_dynamic_text_command);
            data.Add(MSG.prev_station_color);
            data.Add(MSG.prev_station_name);//10
            data.Add(MSG.image_dynamic_command_prev);
            data.Add(MSG.image_start_index_prev); 
            data.Add(MSG.image_change_quantity_prev);
            data.Add(MSG.image_color_prev);
            data.Add(MSG.dynamic_text_command_current);//15
            data.Add(MSG.current_station_color);
            data.Add(MSG.current_station_name);
        }
    }


    public class userControlPanel : ASI.Wanda.DCU.DB.Tables.Table<user_control_panel>
    {
        public static void SelectID(int ID )
        {
            var data = SelectWhere(string.Format("where train_type = '{0}'", ID))
            .SingleOrDefault();

        }
        public static void emergency(int ID ,int WindowDisplayMode)
        {
            Update(ID, WindowDisplayMode);
        }


    }

}
