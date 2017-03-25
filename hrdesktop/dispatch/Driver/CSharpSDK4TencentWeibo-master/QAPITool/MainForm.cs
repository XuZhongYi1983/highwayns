using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QWeiboSDK;

namespace QAPITool
{
    public partial class MainForm : Form
    {
        private string appKey = null;
        private string appSecret = null;
        private string accessKey = null;
        private string accessSecret = null;

        //�����̼߳�ͨѶ
        private int myhWnd = 0;

        //�߳�ͨѶ���ݶ���
        private Dictionary<int, string> dicData = new Dictionary<int, string>();

        public void SetAppKey(string appKey)
        {
            this.appKey = appKey;
        }
        public void SetAppSecret(string appSecret)
        {
            this.appSecret = appSecret;
        }
        public void SetAccessKey(string accessKey)
        {
            this.accessKey = accessKey;
        }
        public void SetAccessSecret(string accessSecret)
        {
            this.accessSecret = accessSecret;
        }

        public MainForm()
        {
            InitializeComponent();
            myhWnd = this.Handle.ToInt32();
        }

        public void ParamFill_HomeTimeline()
        {
            string[] item1 = { "pageflag", "0"};
            string[] item2 = { "pagetime", "0"};
            string[] item3 = { "reqnum", "20" };

            listViewHeader.DeleteAllItem();
            listViewHeader.AddItem(item1);
            listViewHeader.AddItem(item2);
            listViewHeader.AddItem(item3);
           
            
         }

        public void ParamFill_AddPic()
        {
            string[] item1 = { "content", "xxxx"};
            string[] item2 = { "clientip", "127.0.0.1" };
            string[] item3 = { "jing", ""};
            string[] item4 = { "wei", ""};
            string[] item5 = { "pic", ""};

            listViewHeader.DeleteAllItem();
            listViewHeader.AddItem(item1);
            listViewHeader.AddItem(item2);
            listViewHeader.AddItem(item3);
            listViewHeader.AddItem(item4);
            listViewHeader.AddItem(item5);
         
        }

        

        private void Form_Load(object sender, EventArgs e)
        {
            /*http����combobxo��ʼ��*/
            comboMethod.Items.Add("��ҳʱ����");
            comboMethod.Items.Add("����һ����ͼƬ��΢��");
            comboMethod.SelectedIndex = 0;

            /*����ͷ��������ؼ���ʼ��*/
            listViewHeader.GridLines = true;
            listViewHeader.Scrollable = true;
            listViewHeader.View = View.Details;
            listViewHeader.Visible = true;
            listViewHeader.Activation = ItemActivation.OneClick;

            listViewHeader.Columns.Add("", 20, HorizontalAlignment.Center);

            ColumnHeader header2 = new ColumnHeader();
            header2.Width = 120;
            header2.Text = "name";
            listViewHeader.Columns.Add(header2);

            ColumnHeader header3 = new ColumnHeader();
            header3.Width = 120;
            header3.Text = "value";
            listViewHeader.Columns.Add(header3);

            
            textOutput.ScrollBars = ScrollBars.Both;

            /*����formĬ�ϰ�ť*/
            buttonSend.NotifyDefault(true);
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            textOutput.Text = "";
           
            List<Parameter> parameters = new List<Parameter>();
            OauthKey oauthKey = new OauthKey();
            oauthKey.customKey = appKey;
            oauthKey.customSecret = appSecret;
            oauthKey.tokenKey = accessKey;
            oauthKey.tokenSecret = accessSecret;

            bool getMethod = comboMethod.SelectedIndex == 0 ? true : false;
            
            /*http���������name������*/
            List<string> arryName = new List<string>();
            /*http���������value������*/
            List<string> arryValue = new List<string>();

            listViewHeader.GetColumnItem(1, arryName);
            listViewHeader.GetColumnItem(2, arryValue);

            /*����listview�õ���value����utf8����,�������������б�*/
            

            string ret;
            UTF8Encoding utf8 = new UTF8Encoding();
            if (0 == comboMethod.SelectedIndex)
            {
              statuses st = new statuses(oauthKey, "json");
                                  
              ret = st.home_timeline(Convert.ToInt32(arryValue[0]),
                                 Convert.ToInt32(arryValue[1]),
                                 Convert.ToInt32(arryValue[2]));
            }
            else
            {
                t twit = new t(oauthKey, "json");
                
               ret = twit.add_pic(utf8.GetString(utf8.GetBytes(arryValue[0])),
                             utf8.GetString(utf8.GetBytes(arryValue[1])),
                             utf8.GetString(utf8.GetBytes(arryValue[2])),
                             utf8.GetString(utf8.GetBytes(arryValue[3])),
                             utf8.GetString(utf8.GetBytes(arryValue[4]))
                            );
                 
              //  ret = twit.add_pic("xxxx","127.0.0.1","","","E:\abc.jpg");
            }
           
            
            textOutput.Text = ret;

        }

        private void OnComboSelChanged(object sender, EventArgs e)
        {
            int nIndex = comboMethod.SelectedIndex;

            
            if (nIndex == 0)//GET
            {
                ParamFill_HomeTimeline();
            }
            else if (nIndex == 1)//POST
            {
                ParamFill_AddPic();
            }
            
        }


        [System.Runtime.InteropServices.DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(
        int hWnd, //Ŀ�괰�ڵ�handle
        int Msg, // ��Ϣ
        int wParam, // ��һ����Ϣ����
        int lParam // �ڶ�����Ϣ����
        );
        const int WM_HTTPNOTIFY = 8000;

        //�ú��������첽http�ĳ�������http�̵߳��ã�֪ͨ���߳���ʾhttp���
        protected void RequestCallback(int key, string content)
        {
            //ת���̵߳���
            lock (dicData)
            {
                Encoding utf8 = Encoding.GetEncoding(65001);
                Encoding defaultChars = Encoding.Default;
                byte[] temp = utf8.GetBytes(content);
                byte[] temp1 = Encoding.Convert(utf8, defaultChars, temp);
                string result = defaultChars.GetString(temp1);
                dicData.Add(key, result);
            }

            PostMessage(myhWnd, WM_HTTPNOTIFY, 0, 0);
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_HTTPNOTIFY:

                    //������ȡ�������棬�Լ��ټ�����ʱ��
                    Dictionary<int, string> dicText = new Dictionary<int, string>();
                    lock (dicData)
                    {
                        foreach (KeyValuePair<int, string> a in dicData)
                        {
                            dicText.Add(a.Key, a.Value);
                        }
                        dicData.Clear();
                    }
                    

                    foreach (KeyValuePair<int, string> a in dicText)
                    {
                        textOutput.Text = a.Value;
                    }

                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void OnHeaderAdd(object sender, EventArgs e)
        {
            string[] items = { "", "" };
            listViewHeader.AddItem(items);
        }

        private void OnHeaderDelSel(object sender, EventArgs e)
        {
            listViewHeader.DelSelItem();
        }
/*
        private void OnPicAdd(object sender, EventArgs e)
        {
            string[] items = { "", "" };
            listViewPic.AddItem(items);
        }

        private void OnPicDelSel(object sender, EventArgs e)
        {
            listViewPic.DelSelItem();
        }
 */
    }
}