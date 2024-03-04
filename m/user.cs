using System;
using System.Linq;
using mercury.business;
using mercury.data;
using Newtonsoft.Json;

namespace mercury.model
{
    [Serializable]
    public class user_dto_min
    {
        public string id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string dt_register { get; set; }
        public string dt_last_login { get; set; } = null;
        public string dt_login_fail { get; set; } = null;
        public int status { get; set; }
        public bool valid { get; set; }
        public user_dto_min(user _user)
        {
            this.id = _user.id;
            this.name = _user.get_name();
            this.dt_register = stringify.ltodt(_user.dt_register).ToString(entity.dt_format);
            this.dt_last_login = stringify.ltodt(_user.dt_last_login).ToString(entity.dt_format);
            this.dt_login_fail = stringify.ltodt(_user.dt_login_fail).ToString(entity.dt_format);
            this.phone = _user.phone;
            this.email = _user.email;
            this.status = _user.status;
            this.valid = stringify.hash(_user) == _user.hash;
        }
    }

    [Serializable]
    public class user_dto
    {
        public string id { get; set; }
        public string avatar { set; get; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string dt_register { get; set; }
        public string dt_last_login { get; set; }
        public string dt_login_fail { get; set; }
        public int logins_failed_count { get; set; }
        public int status { get; set; }
        public bool valid { get; set; }
        public user_dto(user _user)
        {
            this.id = _user.id;
            this.avatar = _user.avatar;
            this.name = _user.get_name();
            this.phone = _user.phone;
            this.email = _user.email;
            this.dt_register = stringify.ltodt(_user.dt_register).ToString(entity.dt_format);
            this.dt_last_login = stringify.ltodt(_user.dt_last_login).ToString(entity.dt_format);
            this.dt_login_fail = stringify.ltodt(_user.dt_login_fail).ToString(entity.dt_format);
            this.logins_failed_count = _user.logins_failed_count;
            this.status = _user.status;
            this.valid = stringify.hash(_user) == _user.hash;
        }
    }
    public class user_dto_info
    {
        public string id { get; set; }
        public string avatar { set; get; }
        public string name { get; set; }
        public string username { get; set; }
        public string phone { get; set; }
        public string last_seen { get; set; }
        public string chat_id { get; set; }
        //
        public int shared_photos { get; set; } = 0;
        public int shared_videos { get; set; } = 0;
        public int shared_files { get; set; } = 0;
        public int shared_audios { get; set; } = 0;
        public int shared_links { get; set; } = 0;
        public int shared_voices { get; set; } = 0;
        public int shared_groups { get; set; } = 0;
        public user_dto_info(user _user_target, user _user_current)
        {
            this.id = _user_target.id;
            this.avatar = _user_target.avatar;
            this.name = _user_target.get_name();
            this.username = _user_target.username;
            this.phone = _user_target.phone;
            this.last_seen = "Last seen recently";
            // var _chat = _chats.get_(_user_target.id, _user_current.id);
            // if(_chat == null)
            //     return;
            // this.chat_id = _chat.id;
            // this.shared_photos = _messages.get_chat(_chat.id, (int)entity.enum_message_types.photo).Count();
            // this.shared_videos = _messages.get_chat(_chat.id, (int)entity.enum_message_types.video).Count();
            // this.shared_files = _messages.get_chat(_chat.id, (int)entity.enum_message_types.file).Count();
            // this.shared_audios = _messages.get_chat(_chat.id, (int)entity.enum_message_types.audio).Count();
            // this.shared_links = _messages.get_chat(_chat.id).Count(x => x.has_link);
            // this.shared_voices = _messages.get_chat(_chat.id, (int)entity.enum_message_types.voice).Count();
            // this.shared_groups = _group_users.get_shared(_user_target.id, _user_current.id);
        }
    }
}