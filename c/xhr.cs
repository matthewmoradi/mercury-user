using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using mercury.business;
using mercury.data;
using mercury.model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mercury.controller
{
    public class ctrl_xhr : Controller
    {
        #region tools
        private ActionResult ret(staff _staff, dto.msg obj)
        {
            string serd = JsonConvert.SerializeObject(obj);
            string ret = Convert.ToBase64String(Encoding.UTF8.GetBytes(serd));
            Console.WriteLine("Response Len: " + ret.Length);
            return Content(ret);
        }
        private ActionResult ret(dto.msg obj)
        {
            string serd = JsonConvert.SerializeObject(obj);
            // Console.WriteLine("Response Len: " + ret.Length);
            return Content(serd);
        }
        private bool contains(Dictionary<string, string> dic, string[] keys)
        {
            foreach (var key in keys)
                if (!dic.ContainsKey(key))
                    return false;
            return true;
        }
        private void cookie_set_user(user _user, string token)
        {
            ctrl_default.cookie_set(Response, "token", token, entity.cookie_expire_day_user);
            ctrl_default.cookie_set(Response, "user_id", _user.id, entity.cookie_expire_day_user);
        }
        private void cookie_set_staff(staff _staff)
        {
            string token_ = stringify.calc_token(_staff);
            ctrl_default.cookie_set(Response, "token", token_, entity.cookie_expire_day_staff);
            ctrl_default.cookie_set(Response, "staff_id", _staff.id, entity.cookie_expire_day_staff);
        }
        private user get_user()
        {
            string user_id = ctrl_default.cookie_get(Request, "user_id");
            string token = ctrl_default.cookie_get(Request, "token");
            if (user_id == null || token == null)
                return null;
            user _user = _users.get_(user_id);
            if (_user == null || !_sessions.is_token_active(user_id, token))
                return null;
            return _user;
        }
        #endregion

        #region root //all operations passes through this gate except the auth operations.
        [HttpPost]
        [EnableCors("cors_mercury")]
        [Route("/wind")]
        public ActionResult wind()
        {
            string body = new StreamReader(HttpContext.Request.Body, Encoding.UTF8, true, 1024, true).ReadToEnd();
            user _user = get_user();
            if (_user == null)
                return ret(dto.msg.error_unauth());
            Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            if (parameters == null || parameters.Count == 0)
                return ret(dto.msg.error_500());
            if (!contains(parameters, new string[] { "action" }))
                return ret(dto.msg.error_500());
            switch (parameters["action"])
            {
                // preference
                case "preference_get_":
                    return preference_get_();
                // user
                case "user_set_avatar":
                    return user_set_avatar(_user, parameters);
                case "user_set_name":
                    return user_set_name(_user, parameters);
                case "user_set_phone_email":
                    return user_set_phone_email(_user, parameters);
                case "user_get_":
                    return user_get_(_user, parameters);
                case "user_get_settings":
                    return user_get_settings(_user, parameters);
                // session
                case "session_get":
                    return session_get(_user, parameters);
                case "session_close":
                    return session_close(_user, parameters);
                case "session_close_all_other":
                    return session_close_all_other(_user, parameters);
            }
            return ret(new dto.msg(dto.msg.response_code_valid, message_sys.action_not_found, "", false));
        }
        #endregion root
        
        #region preference
        public ActionResult preference_get_()
        {
            return ret(dto.msg.success_data(JsonConvert.SerializeObject(new preference("Mercury"))));
        }
        #endregion preference

        #region user

        #region user_auth
        [HttpPost]
        [EnableCors("cors_mercury")]
        [Route("/register")]
        public ActionResult register()
        {
            string body = new StreamReader(HttpContext.Request.Body, Encoding.UTF8, true, 1024, true).ReadToEnd();
            Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            if (parameters == null || parameters.Count == 0)
                return ret(dto.msg.error_500());
            if (!contains(parameters, new string[] { "username", "location" }))
                return ret(dto.msg.error_500());
            // 
            string device = Request.Headers["User-Agent"].ToString();
            if (parameters["username"].Length < 2)
                return ret(new dto.msg(dto.msg.response_code_valid, "Username is not valid!", ""));
            int error_code = -1;
            string token = "";
            user _user = _users.register(parameters["username"], parameters["location"], device, ref token, ref error_code);
            if (error_code == 1)
                return ret(new dto.msg(dto.msg.response_code_valid, "Username already taken.", ""));
            cookie_set_user(_user, token);
            return ret(dto.msg.success_());
        }

        [HttpPost]
        [EnableCors("cors_mercury")]
        [Route("/login_request")]
        public ActionResult login_request()
        {
            string body = new StreamReader(HttpContext.Request.Body, Encoding.UTF8, true, 1024, true).ReadToEnd();
            Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            if (parameters == null || parameters.Count == 0)
                return ret(dto.msg.error_500());
            if (!contains(parameters, new string[] { "username", "location" }))
                return ret(dto.msg.error_500());
            // 
            string device = Request.Headers["User-Agent"].ToString();
            string code = _users.request_code(parameters["username"], parameters["location"], device);
            if (code == null)
                return ret(new dto.msg(dto.msg.response_code_valid, "Wrong username!", "", false));
            System.Console.WriteLine("User: " + parameters["username"] + ", Code: " + code);
            return ret(new dto.msg(dto.msg.response_code_valid, "Code sent to your phone or email!", ""));
        }

        [HttpPost]
        [EnableCors("cors_mercury")]
        [Route("/login")]
        public ActionResult login()
        {
            string body = new StreamReader(HttpContext.Request.Body, Encoding.UTF8, true, 1024, true).ReadToEnd();
            Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            if (parameters == null || parameters.Count == 0)
                return ret(dto.msg.error_500());
            if (!contains(parameters, new string[] { "username", "code" }))
                return ret(dto.msg.error_500());
            // 
            string token = "";
            user _user = _users.login(parameters["username"], parameters["code"], ref token);
            if (_user == null)
                return ret(new dto.msg(dto.msg.response_code_valid, "Wrong username or code.", "", false));
            if (_user.status == (int)entity.enum_user_flags.block)
                return ret(new dto.msg(dto.msg.response_code_valid, "User is in block status!", "", false));
            cookie_set_user(_user, token);
            var res_data = new { self = new user_dto(_user) };
            return ret(new dto.msg(dto.msg.response_code_valid, "User Authenticated.", JsonConvert.SerializeObject(res_data)));
        }

        #endregion user_auth

        #region user_profile
        public ActionResult user_set_avatar(user _user, Dictionary<string, string> parameters)
        {
            if (!contains(parameters, new string[] { "avatar" }))
                return ret(dto.msg.error_500());
            //check avatar, must be base64
            _users.set_avatar(_user, parameters["avatar"]);
            return ret(dto.msg.success_());
        }

        public ActionResult user_set_name(user _user, Dictionary<string, string> parameters)
        {
            if (!contains(parameters, new string[] { "name_first", "name_last" }))
                return ret(dto.msg.error_500());
            _users.set_name(_user, parameters["name_first"], parameters["name_last"]);
            return ret(dto.msg.success_());
        }

        public ActionResult user_set_phone_email(user _user, Dictionary<string, string> parameters)
        {
            if (!contains(parameters, new string[] { "phone", "email" }))
                return ret(dto.msg.error_500());
            _users.set_phone_email(_user, parameters["phone"], parameters["email"]);
            return ret(dto.msg.success_());
        }
        public ActionResult user_get_(user _user, Dictionary<string, string> parameters)
        {
            if (!contains(parameters, new string[] { "username" }))
                return ret(dto.msg.error_500());
            var _user_target = _users.get__username(parameters["username"]);
            var res = new user_dto_info(_user_target, _user);
            return ret(dto.msg.success_data(JsonConvert.SerializeObject(res)));
        }
        public ActionResult user_get_settings(user _user, Dictionary<string, string> parameters)
        {
            return ret(dto.msg.success_());
        }

        #endregion user_profile

        #endregion user

        #region session
        public ActionResult session_get(user _user, Dictionary<string, string> parameters)
        {
            var query = _sessions.get_user(_user.id);
            var res = query.Select(x => new session_dto(x)).ToList();
            return ret(dto.msg.success_data(JsonConvert.SerializeObject(res)));
        }
        public ActionResult session_close(user _user, Dictionary<string, string> parameters)
        {
            string token = ctrl_default.cookie_get(Request, "token");
            if (!contains(parameters, new string[] { "id" }))
                return ret(dto.msg.error_500());
            string str = "";
            bool res = _sessions.close(_user, parameters["id"], token, ref str);
            if (!res)
                return ret(dto.msg.fail_(str));
            return ret(dto.msg.success_());
        }
        public ActionResult session_close_all_other(user _user, Dictionary<string, string> parameters)
        {
            string token = ctrl_default.cookie_get(Request, "token");
            if (!contains(parameters, new string[] { "id" }))
                return ret(dto.msg.error_500());
            string str = "";
            _sessions.close_all_other(_user, token, ref str);
            return ret(dto.msg.success_());
        }

        #endregion session

    }
}