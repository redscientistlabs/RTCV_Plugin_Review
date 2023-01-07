using PLUGIN_REVIEW.UI;
using NLog;
using RTCV.Common;
using RTCV.CorruptCore;
using RTCV.NetCore;
using System;
using System.Windows.Forms;

namespace PLUGIN_REVIEW
{
    internal class PluginConnectorEMU : IRoutable
    {
        PLUGIN_REVIEW plugin;

        public PluginConnectorEMU(PLUGIN_REVIEW _plugin)
        {
            plugin = _plugin;
            LocalNetCoreRouter.registerEndpoint(this, Ep.EMU_SIDE);
        }

        public object OnMessageReceived(object sender, NetCoreEventArgs e)
        {
            NetCoreAdvancedMessage message = e.message as NetCoreAdvancedMessage;
            switch (e.message.Type)
            {
                case Commands.SHOW_WINDOW:
                    try
                    {
                        SyncObjectSingleton.FormExecute(() =>
                        {
                            if (((Control)S.GET<PluginForm>()).IsDisposed)
                            {
                                PLUGIN_REVIEW.PluginForm = new PluginForm(plugin);
                                S.SET<PluginForm>(PLUGIN_REVIEW.PluginForm);
                            }
                            ((Control)S.GET<PluginForm>()).Show();
                            ((Form)S.GET<PluginForm>()).Activate();
                        });
                        break;
                    }
                    catch
                    {
                        Logging.GlobalLogger.Error($"{nameof(PluginConnectorEMU)}: SHOW_WINDOW failed Reason:\r\n" + e.ToString());
                        break;
                    }
                //case Commands.BYTESGET:
                //    try
                //    {
                //        Tuple<long, long, string> objectValue = (Tuple<long, long, string>)message.objectValue;
                //        e.setReturnValue((object)this.GetByteArr(objectValue.Item1, objectValue.Item2, objectValue.Item3));
                //        break;
                //    }
                //    catch (Exception ex)
                //    {
                //        if (!(ex is ArgumentNullException))
                //        {
                //            NullReferenceException referenceException = ex as NullReferenceException;
                //            break;
                //        }
                //        break;
                //    }
            }
            return e.returnMessage;
        }

        //private byte[] GetByteArr(long start, long end, string domain)
        //{
        //    MemoryInterface memoryInterface = MemoryDomains.GetInterface(domain);
        //    return end >= memoryInterface.get_Size() ? (byte[])null : memoryInterface.PeekBytes(start, end, true);
        //}
    }
}
