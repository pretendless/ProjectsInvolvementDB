using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    class Involvement
    {
        int id;
        int memberid;
        int projectid;

        public int Id { get => id; set => id = value; }
        public int Memberid { get => memberid; set => memberid = value; }
        public int Projectid { get => projectid; set => projectid = value; }
    }
}
