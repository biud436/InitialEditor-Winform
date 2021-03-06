using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using DarkUI.Forms;
using DarkUI.Controls;

namespace Editor
{
    public partial class ScriptEditor : DarkForm
    {

        private string lastScriptPath = null;

        public ScriptEditor()
        {
            InitializeComponent();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ScriptEditor_Load(object sender, EventArgs e)
        {
            CenterToParent();
            Initialize();
        }

        /**
         * 부모 디렉토리를 반환합니다.
         */
        public string GetParentPath()
        {
            string editorRoot = DataManager.Instance.ProjectPath;
            string mainRoot = "";

            if (String.IsNullOrEmpty(editorRoot))
            {
                editorRoot = Directory.GetCurrentDirectory();
            }

            if (File.Exists(Path.Combine(editorRoot, "Editor.exe")))
            {
                mainRoot = Directory.GetParent(editorRoot).Parent.FullName;
            }
            else
            {
                mainRoot = editorRoot;
            }

            return mainRoot;
        }

        public void Initialize()
        {
            var parentDir = GetParentPath();

            var files = Directory.EnumerateFiles(Path.Combine(parentDir, "scripts"));
            foreach (var i in files)
            {
                // 스크립트 파일을 노드에 추가합니다.
                var node = new DarkTreeNode();
                node.Text = $"{Path.GetFileName(i)}";
                darkScriptTree.Nodes.Add(node);
            }

            InitWithScriptView(parentDir);
        }

        public void InitWithScriptView(string parentDir)
        {
            var currentPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            string htmlPath = Path.Combine(currentPath.FullName).Replace("\\", "/");
            webBrowser1.Url = new Uri($"file:///{htmlPath}/res/ace/editor.html");
            webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted; ;
            webBrowser1.ObjectForScripting = true;
        }

        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            LoadScript("main.lua");
        }

        public void SaveScript(string filename)
        {
            String text = webBrowser1.Document.InvokeScript(@"saveAs").ToString();
            File.WriteAllText(filename, text);
        }

        /**
         * 스크립트를 로드합니다.
         */
        public void LoadScript(string filename)
        {
            // 스크립트 파일이 있는지 확인합니다.
            string targetFile = Path.Combine(Directory.GetCurrentDirectory(), "Engine", "scripts", filename);

            if (File.Exists(targetFile))
            {
                string contents = File.ReadAllText(targetFile);
                webBrowser1.Document.InvokeScript("loadScript", new object[] { contents });

                lastScriptPath = targetFile;
                toolStripStatusLabel1.Text = $"스크립트 위치 : {lastScriptPath}";
            }
        }

        private void darkScriptTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var nodes = darkScriptTree.SelectedNodes;
            if (nodes.Count > 0)
            {
                var node = nodes.First();
                LoadScript(node.Text);

            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SaveScript(lastScriptPath);
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            SaveScript(lastScriptPath);
        }

        /// <summary>
        /// 단축키를 정의합니다.
        /// 
        /// Alt + A 를 누르면 적용 버튼을 클릭합니다.
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.A))
            {
                SaveScript(lastScriptPath);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
