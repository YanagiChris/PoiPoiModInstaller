using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace PoiPoi_Mod_Installer
{
    public partial class Form1 : Form
    {
        readonly string settingFileName = Directory.GetCurrentDirectory() + "\\settings.config";

        public Form1()
        {
            InitializeComponent();

            //設定ファイルが無かったら作る
            if (File.Exists(settingFileName) == false)
            {
                XmlWriter(settingFileName, "");
            }
            else
            {
                //設定ファイルの情報を呼び出す
                textBox1.Text = XmlReader(settingFileName);
            }

            //カレントディレクトリを標準で指定
            textBox2.Text = Directory.GetCurrentDirectory() + "\\content";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "WoWSクライアントフォルダを指定してください。";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = @"C:\Windows";
            fbd.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                textBox1.Text = fbd.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            //上部文言
            fbd.Description = "導入対象MODのcontentフォルダを指定してください。";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = Environment.CurrentDirectory;
            fbd.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                textBox2.Text = fbd.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            string binFolder = textBox1.Text + "\\bin";
            string modFolder = textBox2.Text;

            if (Directory.Exists(binFolder) == false)
            {
                MessageBox.Show("WoWSフォルダの読み込みに失敗しました。");
                return;
            }

            if (Directory.Exists(modFolder) == false)
            {
                MessageBox.Show("contentフォルダの読み込みに失敗しました。");
                return;
            }

            var folderList = Directory.GetDirectories(binFolder);

            if (folderList.Count() == 0)
            {
                MessageBox.Show("bin内にフォルダが存在しません。");
                return;
            }

            //降順に並び替え
            Array.Sort(folderList);
            Array.Reverse(folderList);

            string targetFolder = folderList.First();
            targetFolder += "\\res_mods";
            
            FileSystem.CopyDirectory(modFolder, targetFolder,true);

            MessageBox.Show("インストールが完了しました。");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string contentFolder = textBox2.Text;

            contentFolder = Path.GetFileName(contentFolder);

            if(contentFolder != "content")
            {
                MessageBox.Show("contentフォルダのみ指定できます。");
                return;
            }
                
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string wowsFolder = textBox1.Text;

            XmlWriter(settingFileName, wowsFolder);

        }

        private void XmlWriter(string xmlPath, string writeText)
        {

            //設定ファイルが無かったら作る
            if (File.Exists(settingFileName) == false)
            {
                File.Create(settingFileName).Close();
            }

            //＜XMLファイルに書き込む＞
            //XmlSerializerオブジェクトを作成
            //書き込むオブジェクトの型を指定する
            System.Xml.Serialization.XmlSerializer serializer1 =
                new System.Xml.Serialization.XmlSerializer(typeof(string));
            //ファイルを開く（UTF-8 BOM無し）
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                xmlPath, false, new System.Text.UTF8Encoding(false));
            //シリアル化し、XMLファイルに保存する
            serializer1.Serialize(sw, writeText);
            //閉じる
            sw.Close();
        }

        private string XmlReader(string xmlPath)
        {
            string rText = "";

            //＜XMLファイルから読み込む＞
            //XmlSerializerオブジェクトの作成
            System.Xml.Serialization.XmlSerializer serializer2 =
                new System.Xml.Serialization.XmlSerializer(typeof(string));
            //ファイルを開く
            StreamReader sr = new StreamReader(
                xmlPath, new System.Text.UTF8Encoding(false));
            //XMLファイルから読み込み、逆シリアル化する
            rText = (string)serializer2.Deserialize(sr);
            //閉じる
            sr.Close();

            return rText;
        }

    }
}
