using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using mercury.model;
using mercury.business;

namespace mercury.controller
{
    public class ctrl_db : Controller
    {
        #region users
        public static dto.msg push_user(string path, user item = null)
        {
            string url = _io._config_value("url_database") + path;
            string token = _io._config_value("token_database");
            Dictionary<string, string> data = new() { { "token", token }, { "action", "users" } };
            if (item != null)
            {
                string item_str = JsonConvert.SerializeObject(item);
                data.Add("user", item_str);
            }
            var res = api.post(url, JsonConvert.SerializeObject(data));
            if (res == null)
            {
                return null;
            }
            dto.msg msg = JsonConvert.DeserializeObject<dto.msg>(res);
            return msg;
        }
        public static List<user> get_users()
        {
            dto.msg msg = push_user("get");
            if (msg.success)
                return JsonConvert.DeserializeObject<List<user>>(msg.data);
            return null;
        }
        public static user insert_user(user item)
        {
            dto.msg msg = push_user("insert", item);
            if (msg.success)
                return item;
            return null;
        }
        public static user update_user(user item)
        {
            dto.msg msg = push_user("update", item);
            if (msg.success)
                return item;
            return null;
        }
        public static user delete_user(user item)
        {
            dto.msg msg = push_user("delete", item);
            if (msg.success)
                return item;
            return null;
        }
        #endregion users
        
        #region sessions
        public static dto.msg push_session(string path, session item = null)
        {
            string url = _io._config_value("url_database") + path;
            string token = _io._config_value("token_database");
            Dictionary<string, string> data = new() { { "token", token }, { "action", "sessions" } };
            if (item != null)
            {
                string item_str = JsonConvert.SerializeObject(item);
                data.Add("session", item_str);
            }
            var res = api.post(url, JsonConvert.SerializeObject(data));
            if (res == null)
            {
                return null;
            }
            dto.msg msg = JsonConvert.DeserializeObject<dto.msg>(res);
            return msg;
        }
        public static List<session> get_sessions()
        {
            dto.msg msg = push_session("get");
            if (msg.success)
                return JsonConvert.DeserializeObject<List<session>>(msg.data);
            return null;
        }
        public static session insert_session(session item)
        {
            dto.msg msg = push_session("insert", item);
            if (msg.success)
                return item;
            return null;
        }
        public static session update_session(session item)
        {
            dto.msg msg = push_session("update", item);
            if (msg.success)
                return item;
            return null;
        }
        public static session delete_session(session item)
        {
            dto.msg msg = push_session("delete", item);
            if (msg.success)
                return item;
            return null;
        }
        #endregion sessions
    }
}