using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mercury.business;
using Newtonsoft.Json;
using System.IO;
using mercury.controller;

namespace mercury.model
{
    public static class dbc_mercury
    {
        public static List<user> users { get; set; } = new List<user>();
        public static List<session> sessions { get; set; } = new List<session>();
        public static void init()
        {
            users = ctrl_db.get_users();
            sessions = ctrl_db.get_sessions();
        }
    }
}