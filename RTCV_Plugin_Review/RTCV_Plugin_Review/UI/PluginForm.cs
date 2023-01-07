namespace PLUGIN_REVIEW.UI
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using NLog;
    using RTCV.CorruptCore;
    using RTCV.NetCore;
    using RTCV.Common;
    using RTCV.UI;
    using static RTCV.CorruptCore.RtcCore;
    using RTCV.Vanguard;
    using System.IO;
    using System.Text.RegularExpressions;
    using RTCV.UI.Modular;

    public partial class PluginForm : ComponentForm
    {
        public PLUGIN_REVIEW plugin;

        public volatile bool HideOnClose = true;

        Logger logger = NLog.LogManager.GetCurrentClassLogger();

        Timer timer = new Timer();

        public PluginForm(PLUGIN_REVIEW _plugin)
        {
            plugin = _plugin;

            this.InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(this.PluginForm_FormClosing);


            this.Text = PLUGIN_REVIEW.CamelCase(nameof(PLUGIN_REVIEW).Replace("_", " ")) + $" - Version {plugin.Version.ToString()}"; //automatic window title

            timer.Interval = 1420;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(StockpileManagerUISide.CurrentStashkey != null)
            {
                var current = StockpileManagerUISide.CurrentStashkey;
                lb_CorruptionName.Text = current.Alias;
                lb_RomName.Text = Path.GetFileName(current.RomFilename);
                lb_LayerSize.Text = (current.BlastLayer?.Layer?.Count ?? 0).ToString();
                string stateFilename = current.GameName + "." + current.ParentKey + ".timejump.State";
                lb_StateName.Text = stateFilename;
                lb_GameName.Text = current.GameName;
                lb_SystemName.Text = current.SystemName + " -> " + current.SystemCore;
                
                var rnd = new Random();
                if(current.BlastLayer != null && current.BlastLayer.Layer != null && current.BlastLayer.Layer.Count > 0)
                {
                    var units = current.BlastLayer.Layer.OrderBy(it => rnd.Next()).Take(6).OrderBy(it2 => it2.Address);
                    var lines = new List<string>();
                    foreach(var unit in units)
                    {
                        string unitText =
                        $"{(unit.IsEnabled ? "" : "DISABLED ")}" +
                        $"{(!unit.IsLocked ? "" : "LOCKED ")}" +
                        $"DOM:{unit.Domain} " +
                        $"ADR:{unit.Address} " +
                        $"P:{unit.Precision} " +
                        $"LIFE:{unit.Lifetime} " +
                        $"{(!unit.Loop ? "" : $"LOOP:{unit.LoopTiming} ")}" +
                        $"{(unit.TiltValue == 0 ? "" : $"TILT:{unit.TiltValue} ")}" +
                        $"SRC:{unit.Source} " +
                        $"{(unit.Source != BlastUnitSource.VALUE ? "" : $"VAL:{BitConverter.ToString(unit.Value).Replace("-", string.Empty)} ")}" +
                        $"{(unit.Source != BlastUnitSource.STORE ? "" : $"STOR:{unit.StoreType} ")}" +
                        $"{(unit.Source != BlastUnitSource.STORE ? "" : $"STOR:{unit.StoreType} ")}" +
                        $"{(unit.Source != BlastUnitSource.STORE ? "" : $"STRTIM:{unit.StoreTime} ")}" +
                        $"{(unit.Source != BlastUnitSource.STORE ? "" : $"SRCDOM:{unit.SourceDomain} ")}" +
                        $"{(unit.Source != BlastUnitSource.STORE ? "" : $"SRCADR:{unit.SourceAddress} ")}" +
                        $"{(unit.LimiterTime != LimiterTime.NONE ? "" : $"LMTTIM:{unit.LimiterTime} ")}" +
                        $"{(unit.LimiterTime != LimiterTime.NONE ? "" : $"{(!unit.InvertLimiter ? "" : "INVERT ")}")}";
                        lines.Add(unitText);
                    }
                    //var unit = current.BlastLayer.Layer[rnd.Next(current.BlastLayer.Layer.Count)];
                    
                    lb_BlastUnitEx.Text = String.Join("\n",lines.ToArray());
                }
                else
                {
                    lb_BlastUnitEx.Text = "NONE";
                }

            }
        }

        private void PluginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(HideOnClose)
            {
                e.Cancel = true;
                this.Hide();
            }    
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void PluginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
