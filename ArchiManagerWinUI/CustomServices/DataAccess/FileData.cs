using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiManagerWinUI.CustomServices.DataAccess
{
    public class FileData
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }

        public FileData(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }
    }

}
