using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mercury.business;
using mercury.data;

namespace mercury.model
{
    public class session_dto_min
    {
        public string id { get; set; }
        public string user_ { get; set; }
        public string device { get; set; }
        public string location { get; set; }
        public string dt { get; set; }
        public int status { get; set; }
        public session_dto_min(session _session)
        {
            this.id = _session.id;
            this.user_ = _users.get__name_(_session.user_id);
            this.device = _session.device;
            this.location = _session.location;
            this.dt = stringify.ltodt(_session.dt).ToString(entity.dt_format);
            this.status = _session.status;
        }
    }
    public class session_dto
    {
        public string id { get; set; }
        public string user_ { get; set; }
        public string device { get; set; }
        public string location { get; set; }
        public string dt { get; set; }
        public int status { get; set; }
        public session_dto(session _session)
        {
            this.id = _session.id;
            this.user_ = _users.get__name_(_session.user_id);
            this.device = _session.device;
            this.location = _session.location;
            this.dt = stringify.ltodt(_session.dt).ToString(entity.dt_format);
            this.status = _session.status;
        }
    }
}