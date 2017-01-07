using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.XML
{
    public class FileLoader : ILoader
    {
        String Path;

        public string Load()
        {
            String content = String.Empty;

            try
            {
                FileStream fs = new FileStream(Path, FileMode.Open);
                using (StreamReader sr = new StreamReader(fs))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
                content = String.Empty;
            }

            return content;
        }

        public void SetPath(string Path)
        {
            this.Path = Path;
        }
    }
}
