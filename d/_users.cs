using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using mercury.controller;
using mercury.model;
using mercury.business;

namespace mercury.data
{
    public static class _users
    {
        #region get
        public static IEnumerable<user> get()
        {
            return dbc_mercury.users;
        }
        public static IEnumerable<user> get_search(string s, IEnumerable<user> query = null)
        {
            if (query == null)
                query = dbc_mercury.users;
            return query.Where(x => get__search(x, s));
        }
        public static IEnumerable<user> get(int skip, int take, ref int count, bool asc = false, string sort_by = "id", string d_p = null, string s = null, long? dt_from = null, long? dt_to = null)
        {
            var propertyInfo = typeof(user).GetProperty(sort_by);
            if (propertyInfo == null)
                propertyInfo = typeof(user).GetProperty("id");
            var query = get();
            if (!string.IsNullOrEmpty(d_p))
                query = query.Where(x => x.id == d_p);
            if (!string.IsNullOrEmpty(s) && d_p == null)
            {
                query = get_search(s, query);
            }
            if (dt_from != null)
                query = query.Where(x => x.dt_register > dt_from.Value);
            if (dt_to != null)
                query = query.Where(x => x.dt_register < dt_to.Value);
            count = query.Count();
            if (asc)
                query = query.OrderBy(x => propertyInfo.GetValue(x, null));
            else
                query = query.OrderByDescending(x => propertyInfo.GetValue(x, null));
            return query.Skip(skip).Take(take);
        }
        #endregion
        #region get_
        public static user get_(string id)
        {
            return dbc_mercury.users.FirstOrDefault(x => x.id == id);
        }
        public static user get__username(string username)
        {
            return dbc_mercury.users.FirstOrDefault(x => x.username == username);
        }
        public static bool get__search(string id, string s)
        {
            var item = get_(id);
            if (item == null)
                return false;
            return get__search(item, s);
        }
        public static bool get__search(user item, string s)
        {
            return item.username.Contains(s) || item.name_first.Contains(s) || item.name_last.Contains(s) || item.email.Contains(s) || item.phone.Contains(s);
        }
        public static string get__last_seen_(string id)
        {
            var item = get_(id);
            if (item != null)
                return "last seen recently";
            return message_sys.unknow;
        }
        public static string get__name_(string id)
        {
            var item = get_(id);
            if (item != null)
                return item.get_name();
            return message_sys.unknow;
        }
        #endregion
        #region set
        public static string request_code(string username, string location, string device)
        {
            var item = get__username(username);
            if (item == null)
                return null;
            session _session = _sessions.generate(item.id, location, device);
            //email
            // email.
            return _session.code;
        }
        public static user login(string username, string code, ref string token)
        {
            var item = get().FirstOrDefault(x => x.username == username);
            if (item == null || item.status == (int)entity.enum_user_flags.block)
                return null;
            session _session = _sessions.get_user_pending_code(item.id, code);
            if (_session == null || _session.code != code)
            {
                item.logins_failed_count += 1;
                item.dt_login_fail = stringify.dttol(DateTime.Now);
                if (item.logins_failed_count >= entity.max_fails)
                    item.status = (int)entity.enum_user_flags.block;
                ctrl_db.update_user(item);
                return null;
            }
            token = _sessions.activate(item, _session);
            if (token == null)
                return null;
            item.dt_last_login = stringify.dttol(DateTime.Now);
            item.logins_failed_count = 0;
            ctrl_db.update_user(item);
            return item;
        }
        public static user register(string username, string location, string device, ref string token, ref int error_code)
        {
            if (get__username(username) != null)
            {
                error_code = 1;
                return null;
            }
            user item = new user();
            item.username = username;
            item.dt_last_login = 0;
            item.dt_login_fail = 0;
            item.logins_failed_count = 0;
            item.status = (int)entity.enum_user_flags.ok;
            item.id = entity.id_new;
            item.hash = stringify.hash(item);
            if (ctrl_db.insert_user(item) == null)
                return null;
            dbc_mercury.users.Add(item);
            session _session = _sessions.generate(item.id, location, device);
            token = _sessions.activate(item, _session);
            return item;
        }
        public static void set_avatar(user item, string avatar)
        {
            // attachment _attachment = _attachments.add_avatar(item.username + "_" + avatar, avatar);
            // item.avatar = _attachment.id;
            // dbc.update(item);
        }
        public static void set_phone_email(user item, string phone, string email)
        {
            item.phone = phone;
            item.email = email;
            ctrl_db.update_user(item);
        }
        public static void set_name(user item, string name_first, string name_last)
        {
            item.name_first = name_first;
            item.name_last = name_last;
            ctrl_db.update_user(item);
        }
        #endregion
        #region del
        public static bool delete(string id)
        {
            user item = get_(id);
            if (item == null)
            {
                return false;
            }
            // _contacts.delete_by_user(item);
            dbc_mercury.users.Remove(item);
            ctrl_db.delete_user(item);
            return true;
        }
        #endregion
    }
}