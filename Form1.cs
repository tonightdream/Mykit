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

namespace MyKit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<string> listFullPath = new List<string>(); //存放打开过的文件全路径
        string CurrenOpenedFilePath;    //当前正在打开的文件的全路径
        Font myFont = new Font("微软雅黑", 12, FontStyle.Regular);
        private void Form1_Load(object sender, EventArgs e)
        {
            //状态栏初始化
            toolStripStatusLabel1.Text = "当前时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
            toolStripStatusLabel2.Text = "当前用户:" + Environment.UserName + " 系统:" + Environment.OSVersion + " 电脑名:" + Environment.MachineName;

            //菜单栏初始化
            侧边栏ToolStripMenuItem.Checked = true;

            //字体设置
            toolStripStatusLabel1.Font = myFont;
            toolStripStatusLabel2.Font = myFont;
            toolStripDropDownButton1.Font = myFont;
            menuStrip1.Font = myFont;
            textBox1.Font = myFont;

            //目录 下拉框初始化
            string[] drivers = Environment.GetLogicalDrives();  //获取本机的有驱动器号
            for (int i = 0; i < drivers.Length; i++)
            {
                comboBox1.Items.Add(drivers[i]);
            }
            comboBox1.SelectedIndex = 0; //设置默认选中第一条记录

            //设置listBox Item的高度 此设置没生效,
            lboxHistory.IntegralHeight = false;
            lboxHistory.ItemHeight = 18;
            listBox2.IntegralHeight = false;

            //listBox2.DrawMode = DrawMode.OwnerDrawFixed;
            listBox2.ItemHeight = 24;

            //默认隐藏listView
            listView1.Visible = false;

            MyComputer();


        }
        /// <summary>
        ///我的电脑资源管理器
        /// </summary>
        private void MyComputer()
        {
            //我的电脑TreeView初始化
            TreeNode rootNode = treeView1.Nodes[0]; //获取TreeView根节点对象
            rootNode.Expand();  //展开根节点

            TreeNode ndMyComputer = null;
            TreeNode ndMyDocument = null;
            TreeNode ndMyFav = null;
            foreach (TreeNode nd in rootNode.Nodes) //遍历根节点下的节点将对象赋值给相应对象
            {
                if (nd.Name == "ndMyComputer")
                {
                    ndMyComputer = nd;
                }
                if (nd.Name == "ndMydocument")
                {
                    ndMyDocument = nd;
                }
                if (nd.Name == "ndMyFav")
                {
                    ndMyFav = nd;
                }
            }
            //ndMyComputer.Nodes.Add("a");//将我的电脑下加子节点
            DriveInfo[] drives = DriveInfo.GetDrives(); //获取本机的有驱动器列表
            //for方法实现
            //for (int i = 0; i < drivers.Length; i++)
            //{
            //    ndMyComputer.Nodes.Add(drivers[i].ToString());  //将获取的驱动器列表加入我的电脑子节点
            //}
            //Foreach方法实现
            foreach (DriveInfo driver in drives)
            {
                //ndMyComputer.Nodes.Add(dr.ToString());

                TreeNode node = new TreeNode(driver.Name); //另一种写法
                node.ImageKey = "disk.png";
                node.SelectedImageKey = "disk.png";
                node.Tag = driver.RootDirectory.FullName;
                ndMyComputer.Nodes.Add(node);
            }

            //我的文档添加子节点
            //获取我的文档全路径
            string myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            ////方法1
            ////根据我的文档全路径获取其子所有子目录
            //string[] myDocSubFolder = Directory.GetDirectories(myDocPath);

            //方法2
            DirectoryInfo myDocumentFolder = new DirectoryInfo(myDocPath);
            DirectoryInfo[] folders = myDocumentFolder.GetDirectories();
            foreach (DirectoryInfo folder in folders)
            {
                //ndMyDocument.Nodes.Add(folder.ToString());

                TreeNode node = new TreeNode(folder.Name);   //新建子节点名为folder.Name
                node.ImageKey = "folder.ico";
                node.SelectedImageKey = "folder.ico";
                node.Tag = folder.FullName;
                ndMyDocument.Nodes.Add(node);

                ////尝试加第三层目录
                //DirectoryInfo[] subFolders = folder.GetDirectories();
                //foreach (DirectoryInfo subfolder in subFolders)
                //{
                //    TreeNode subNode = new TreeNode(subfolder.Name);
                //    subNode.ImageKey = "folder.ico";
                //    subNode.SelectedImageKey = "folder.ico";
                //    node.Nodes.Add(subNode);

                //}
            }

            //显示我的收藏
            //获取我的收藏全路径
            string myFavPath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            //获取我的收藏下子目录
            DirectoryInfo myFavSubFolder = new DirectoryInfo(myFavPath);
            DirectoryInfo[] myfavsubFolders = myFavSubFolder.GetDirectories();
            foreach (DirectoryInfo item in myfavsubFolders)
            {
                TreeNode node = new TreeNode(item.Name);
                node.Tag = node.FullPath;
                ndMyFav.Nodes.Add(node);
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "当前时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }
        /// <summary>
        /// 文件打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本|*.txt|配置文件|*.ini|批处理|*.bat";
            //ofd.InitialDirectory = Directory.GetLogicalDrives;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            ofd.ShowDialog();
            //获取要打开文件的全路径
            string path = ofd.FileName;
            CurrenOpenedFilePath = path;
            lboxHistory.Items.Add(path);
            if (path.Length == 0)
            {
                return;
            }
            listFullPath.Add(path); //将打开的文件全路径加入集合
            using (FileStream fsRead = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            {
                byte[] buffer = new byte[1024 * 1024 * 5];
                int r = fsRead.Read(buffer, 0, buffer.Length);
                textBox1.Text = Encoding.Default.GetString(buffer, 0, r);
            }

            tSSLabel3.Text = "字符数:" + textBox1.Text.Length;
        }

        private void 状态栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (状态栏ToolStripMenuItem.Checked == true)
            //如菜单项前有对勾说明是选中的也就是显示状态栏.则此时点击菜单的目的是要隐藏状态栏,并将菜单项前的对勾取消
            {
                statusStrip1.Visible = false; 状态栏ToolStripMenuItem.Checked = false;
            }
            else
            {

                statusStrip1.Visible = true; 状态栏ToolStripMenuItem.Checked = true;
            }

        }

        private void 侧边栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (侧边栏ToolStripMenuItem.Checked == true)
            {
                panel1.Visible = false;
                侧边栏ToolStripMenuItem.Checked = false;
            }
            else
            {
                panel1.Visible = true;
                侧边栏ToolStripMenuItem.Checked = true;
            }
        }

        /// <summary>
        /// 目录索引改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空listBox中的数据
            listBox2.Items.Clear();

            //在listBox2中显示选中目录中文件
            //确定已选中的目录
            string selectedPath = (string)comboBox1.SelectedItem;
            //MessageBox.Show(selectedPath);

            //获取文件或文件夹可能访问错误的异常处理
            try
            {
                //将选中目录中的所有文件添加到listBox2中
                string[] detailFile = Directory.GetFiles(selectedPath);
                for (int i = 0; i < detailFile.Length; i++)
                {
                    listBox2.Items.Add(detailFile[i]);
                }

                //将选中目录中所有的文件夹添加到listbox2中
                string[] detailFolder = Directory.GetDirectories(selectedPath);
                for (int i = 0; i < detailFolder.Length; i++)
                {
                    listBox2.Items.Add(detailFolder[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 文件的双击打开事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            //双击打开选择的文件

            //打开文件->打开哪个文件->全路径->索引
            string path = (string)listBox2.Items[listBox2.SelectedIndex];
            CurrenOpenedFilePath = path;
            listFullPath.Add(path);
            lboxHistory.Items.Add(path);
            try
            {
                //textBox中显示双击打开的文件
                using (FileStream fsRead = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    byte[] buffer = new byte[1024 * 1024 * 5];
                    int r = fsRead.Read(buffer, 0, buffer.Length);
                    textBox1.Text = Encoding.Default.GetString(buffer, 0, r);
                    tSSLabel3.Text = "字符数:" + textBox1.Text.Length; //状态栏显示字符数
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        /// <summary>
        /// 保存已打开文件的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("文件为空,不能保存");
                return;
            }
            //获取要保存文件的全路径 
            //CurrenOpenedFilePath
            if (!string.IsNullOrEmpty(CurrenOpenedFilePath))    //当前打开文件的全路径不为空说明,已经打开了文件
            {
                using (FileStream fsWite = new FileStream(CurrenOpenedFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] buffer = Encoding.Default.GetBytes(textBox1.Text);
                    fsWite.Write(buffer, 0, buffer.Length);

                }
            }
            else
            {
                MessageBox.Show("没有打开文件啊!");
            }



        }

        /// <summary>
        /// 文件另存为的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本|*.txt";
            //设置初始目录为当前用户桌面
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            sfd.ShowDialog();
            string path = sfd.FileName; //用户选择的要保存到的全路径
            if (path.Length <= 0)
            {
                return;
            }
            using (FileStream fsWrite = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = Encoding.Default.GetBytes(textBox1.Text);
                fsWrite.Write(buffer, 0, buffer.Length);
            }
        }

        private void textBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.SelectedText))
            {

                MessageBox.Show(textBox1.SelectedText);
                MessageBox.Show(textBox1.SelectedText.Length.ToString());
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tSSLabel3.Text = "字符数:" + textBox1.Text.Length.ToString();
        }

        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.ShowDialog();
            textBox1.Font = fd.Font;
        }

        private void 颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            textBox1.ForeColor = cd.Color;
        }

        private void lboxHistory_DoubleClick(object sender, EventArgs e)
        {
            string path = (string)lboxHistory.Items[lboxHistory.SelectedIndex];
            if (!string.IsNullOrEmpty(path))
            {
                using (FileStream fsRead = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    byte[] buffer = new byte[1024 * 1024 * 5];
                    int r = fsRead.Read(buffer, 0, buffer.Length);
                    textBox1.Text = Encoding.Default.GetString(buffer, 0, r);
                }
            }

        }

        private void 帮助说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 关于我们ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 winAbout = new AboutBox1();
            winAbout.ShowDialog();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_Enter(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 节点展开后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            //只响应鼠标点击展开事件
            //if (e.Action == TreeViewAction.ByMouse) 
            //{

            //}

            if (e.Node.Parent == null)
            {
                return;
            }
            //TreeNode node = e.Node; //获取当前节点

            foreach (TreeNode node in e.Node.Nodes /*<-当前节点的所有子节点*/)
            {
                if (node.Tag == null)
                {
                    continue;
                }

                DirectoryInfo folder = new DirectoryInfo(node.Tag.ToString()); //获取目录的全路径
                DirectoryInfo[] subFolders = null;
                try
                {
                    subFolders = folder.GetDirectories();   //获取子目录
                }
                catch (Exception)
                {

                    //throw;
                }
                if (subFolders != null)
                {
                    foreach (DirectoryInfo subfolder in subFolders)
                    {
                        TreeNode subNode = new TreeNode(subfolder.Name);
                        subNode.ImageKey = "folder.ico";
                        subNode.SelectedImageKey = "folder.ico";
                        subNode.Tag = subfolder.FullName;
                        node.Nodes.Add(subNode);

                    }
                }


            }


        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string GetFileSize(FileInfo file)
        {
            string result = null;
            long fileSize = file.Length;
            if (fileSize >= 1024 * 1024 * 1024)
            {
                result = string.Format("{0:#####0.00} GB", ((double)fileSize) / (1024 * 1024 * 1024));
            }
            else if (fileSize >= 1024 * 1024)
            {
                result = string.Format("{0:#####0.00} MB", ((double)fileSize) / (1024 * 1024));
            }
            else if (fileSize >= 1024)
            {
                result = string.Format("{0:#####0.00} KB", ((double)fileSize) / 1024);
            }
            else
            {
                result = string.Format("{0} byte", ((double)fileSize));
            }

            return result;
        }

        /// <summary>
        /// TreeView选中后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.textBox1.Visible = false;
            this.listView1.Visible = true;
            if (e.Action == TreeViewAction.ByKeyboard || e.Action == TreeViewAction.ByMouse)
            {
                if (e.Node.Tag == null)
                {
                    this.Text = e.Node.Text;    //设置窗体标题栏
                    this.comboBox2.Text = e.Node.Text;
                }
                else
                {
                    this.comboBox2.Text = e.Node.Text;
                    this.Text = e.Node.Tag.ToString();//设置窗体标题栏
                }
            }
            this.listView1.Items.Clear();   //清空 listView
            if (e.Node.Tag != null)
            {
                
                string folderPath = e.Node.Tag.ToString();  //获取当前选中的文件夹路径
                DirectoryInfo folder = new DirectoryInfo(folderPath);   //根据文件夹获取对象
               //添加子文件夹
                DirectoryInfo[] subFolders = folder.GetDirectories();
                foreach (DirectoryInfo subFolder  in subFolders)
                {
                    listView1.Items.Add(new ListViewItem(new string[] { subFolder.Name, "文件夹", "", subFolder.LastWriteTime.ToString() },"folder.ico"));
                }
                
                //填充文件
                
                FileInfo[] files = null;
                try
                {
                    files = folder.GetFiles(); //根据文件夹获取里面原文件列表
                }
                catch (System.UnauthorizedAccessException)
                {

                    //throw;
                }
                if (files != null)
                {
                    foreach (FileInfo file in files)    //遍历文件并加入listView
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { file.Name, "文件", GetFileSize(file), file.LastWriteTime.ToString() },"file.ico"));
                    }
                }



            }
        }
    }
}
