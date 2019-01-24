using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceManager.Models
{
    public class Message
    {
        public DateTime Time { get; set; }

        public string Text { get; set; }


        public Message()
        {
            this.Time = DateTime.Now;
        }

    }
}
