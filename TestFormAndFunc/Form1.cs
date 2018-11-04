using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestFormAndFunc
{
    public partial class Form1 : Form
    {
        protected const string FILE_PATH = "d:\\media\\CP0934900049.ts";
        protected const byte TS_HEAD_FLAG = 0x47;
        protected const int TS_PERIOD_BYTES = 188; 

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream fs = File.Open(FILE_PATH, FileMode.OpenOrCreate);

                while (true)
                {
                    int val = fs.ReadByte();
                    if (-1 == val)
                        break;

                    if (TS_HEAD_FLAG == val)//TS HEAD
                    {
                        fs.Seek(-1, SeekOrigin.Current);
                    }
                }
            }catch(IOException eio)
            {
                System.Diagnostics.Debug.WriteLine(eio.ToString());
            }


            StreamWriter writer = new StreamWriter(File.OpenWrite(FILE_PATH));

            

        }
    }
}
