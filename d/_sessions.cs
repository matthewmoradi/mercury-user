using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using mercury.business;
using mercury.controller;
using mercury.model;

namespace mercury.data
{
    public static class _sessions
    {
        #region get
        public static IEnumerable<session> get()
        {
            return dbc_mercury.sessions;
        }
        public static IEnumerable<session> get(int skip, int take, ref int count, bool asc = false, string sort_by = "id", string d_p = null, string s = null, long? dt_from = null, long? dt_to = null)
        {
            var propertyInfo = typeof(session).GetProperty(sort_by);
            if (propertyInfo == null)
                propertyInfo = typeof(session).GetProperty("id");
            var query = get();
            if (!string.IsNullOrEmpty(d_p))
                query = query.Where(x => x.id == d_p);
            if (!string.IsNullOrEmpty(s) && d_p == null)
                query = query.Where(x => _users.get_(x.user_id).username.Contains(s) || x.device.Contains(s) || x.location.Contains(s));
            if (dt_from != null)
                query = query.Where(x => x.dt > dt_from.Value);
            if (dt_to != null)
                query = query.Where(x => x.dt < dt_to.Value);
            // 
            count = query.Count();
            if (asc)
                query = query.OrderBy(x => propertyInfo.GetValue(x, null));
            else
                query = query.OrderByDescending(x => propertyInfo.GetValue(x, null));
            return query.Skip(skip).Take(take);
        }
        public static IEnumerable<session> get_user(string user_id)
        {
            return dbc_mercury.sessions.Where(x => x.token == user_id);
        }
        public static IEnumerable<session> get_active()
        {
            return dbc_mercury.sessions.Where(x => x.status == (int)entity.enum_session_statuses.active);
        }
        public static IEnumerable<session> get_pending()
        {
            return dbc_mercury.sessions.Where(x => x.status == (int)entity.enum_session_statuses.pending);
        }
        public static IEnumerable<session> get_status(int status)
        {
            return dbc_mercury.sessions.Where(x => x.status == status);
        }
        public static session get_user_pending_code(string user_id, string code)
        {
            return get_user_status(user_id, (int)entity.enum_session_statuses.pending).FirstOrDefault(x => x.code == code);
        }
        public static IEnumerable<session> get_user_status(string user_id, int status)
        {
            return dbc_mercury.sessions.Where(x => x.user_id == user_id && x.status == status);
        }
        #endregion
        #region get_
        public static session get_(string id)
        {
            session _session = null;
            _session = dbc_mercury.sessions.FirstOrDefault(x => x.id == id);
            return _session;
        }
        public static bool is_token_active(string user_id, string token)
        {
            return dbc_mercury.sessions.Any(x => x.user_id == user_id && x.token == token && x.status == (int)entity.enum_session_statuses.active);
        }
        public static session get_user_code(string user_id, string code)
        {
            return dbc_mercury.sessions.FirstOrDefault(x => x.user_id == user_id && x.code == code);
        }
        public static session generate(string user_id, string location, string device)
        {
            session item = new session();
            item.id = entity.id_new;
            item.user_id = user_id;
            item.location = location;
            item.device = device;
            item.code = entity.code_new;
            item.status = (int)entity.enum_session_statuses.pending;
            item.dt = stringify.dttol(DateTime.Now);
            if (ctrl_db.insert_session(item) == null)
                return null;
            dbc_mercury.sessions.Add(item);
            return item;
        }
        #endregion
        #region set
        public static string activate(user _user, session _session)
        {
            if (_session.status != (int)entity.enum_session_statuses.pending)
                return null;
            _session.token = stringify.calc_token(_user, _session.id);
            _session.dt_active = stringify.dttol(DateTime.Now);
            set_as_active(_session.id);
            return _session.token;
        }
        public static void set_as_active(string id)
        {
            set_flag(id, (int)entity.enum_session_statuses.active);
        }
        public static void set_as_expired(string id)
        {
            set_flag(id, (int)entity.enum_session_statuses.expired);
        }
        public static void set_flag(string id, int status)
        {
            session item = get_(id);
            item.status = status;
            ctrl_db.update_session(item);
        }
        public static bool close(user _user, string id, string token, ref string str)
        {
            session item = get_(id);
            if (item == null)
            {
                str = message_sys.null_refrence;
                return false;
            }
            if (item.user_id == _user.id)
            {
                str = message_sys.not_your_item;
                return false;
            }
            if (item.token == token)
            {
                str = message_sys.cant_close_current_session;
                return false;
            }
            set_as_expired(item.id);
            return true;
        }
        public static bool close_all_other(user _user, string token, ref string str)
        {
            var items = get_user(_user.id);
            foreach(var item in items)
            {
                if (item.token != token)
                    set_as_expired(item.id);
            }
            return true;
        }
        #endregion
        #region delete
        public static bool delete(string id)
        {
            session item = get_(id);
            if (item == null)
                return false;
            dbc_mercury.sessions.Remove(item);
            ctrl_db.delete_session(item);
            return true;
        }
        #endregion
    }
}