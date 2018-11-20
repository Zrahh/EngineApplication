using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace EngineApplication
{
    public sealed partial class MainForm : Form
    {
        #region class private members
        private IMapControl3 m_mapControl = null;
        private string m_mapDocumentName = string.Empty;
        #endregion

        #region class constructor
        public MainForm()
        {
            InitializeComponent();
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            //得到MapControl
            m_mapControl = (IMapControl3)axMapControl1.Object;

            //禁用“保存”菜单（因为还没有文档）
            menuSaveDoc.Enabled = false;
        }

        #region Sự kiện
        #region Main Menu event handlers
        private void menuNewDoc_Click(object sender, EventArgs e)
        {
            //执行New Document命令
            ICommand command = new CreateNewDocument();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void menuOpenDoc_Click(object sender, EventArgs e)
        {
            //执行Open Document命令
            ICommand command = new ControlsOpenDocCommandClass();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void menuSaveDoc_Click(object sender, EventArgs e)
        {
            //执行Save Document命令
            if (m_mapControl.CheckMxFile(m_mapDocumentName))
            {
                //创建一个MapDocument的新实例
                IMapDocument mapDoc = new MapDocumentClass();
                mapDoc.Open(m_mapDocumentName, string.Empty);

                //确保MapDocument不是只读的
                if (mapDoc.get_IsReadOnly(m_mapDocumentName))
                {
                    MessageBox.Show("Map document is read only!");
                    mapDoc.Close();
                    return;
                }

                //用当前地图替换其内容
                mapDoc.ReplaceContents((IMxdContents)m_mapControl.Map);

                //保存MapDocument以保持它
                mapDoc.Save(mapDoc.UsesRelativePaths, false);

                //关闭MapDocument
                mapDoc.Close();
            }
        }

        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            //执行SaveAs Document命令
            ICommand command = new ControlsSaveAsDocCommandClass();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void menuExitApp_Click(object sender, EventArgs e)
        {
            //退出应用程序
            Application.Exit();
        }
        #endregion

        //监听MapReplaced事件以更新状态栏和“保存”菜单
        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            //从MapControl获取当前文档的名称
            m_mapDocumentName = m_mapControl.DocumentFilename;

            //如果没有MapDocument，请禁用“保存”菜单并清除状态栏
            if (m_mapDocumentName == string.Empty)
            {
                menuSaveDoc.Enabled = false;
                statusBarXY.Text = string.Empty;
            }
            else
            {
                //启用“保存”菜单并将文档名称写入状态栏
                menuSaveDoc.Enabled = true;
                statusBarXY.Text = System.IO.Path.GetFileName(m_mapDocumentName);
            }
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            statusBarXY.Text = string.Format("{0}, {1}  {2}", e.mapX.ToString("#######.##"), e.mapY.ToString("#######.##"), axMapControl1.MapUnits.ToString().Substring(4));
        }

        private void zoomĐếnĐốiTượngChọnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //拍摄地图对象
                IMap map = m_mapControl.Map;
                //在地图上选择所选对象
                IEnumFeature enumFeature = map.FeatureSelection as IEnumFeature;

                //创建envelope对象
                IEnvelope env = new EnvelopeClass();

                //获取所选对象中的对象
                IFeature feature = enumFeature.Next();
                while (feature != null)
                {
                    env.Union(feature.Shape.Envelope);
                    feature = enumFeature.Next();
                }

                //activeview对象
                IActiveView act = map as IActiveView;

                env.Expand(1.2, 1.2, true);
                act.Extent = env;
                act.Refresh();        
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());              
            }
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            try
            {
                if (e.button != 2)
                {
                    return;
                }
                contextMenuStrip1.Show(axMapControl1.PointToScreen(new System.Drawing.Point(e.x,e.y)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        private void menuFile_Click(object sender, EventArgs e)
        {

        }
    }
}