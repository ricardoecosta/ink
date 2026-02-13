using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.GamerServices;

namespace GameLibrary.Utils
{
    public class GuideHelper
    {

        public static void ShowSyncOkButtonAlertMsgBox(string title, string msg)
        {
            if (!Guide.IsVisible)
            {
                IAsyncResult result = Guide.BeginShowMessageBox(
                    title,
                    msg,
                    new string[] { "ok" },
                    0, MessageBoxIcon.Alert, null, null);

                Guide.EndShowMessageBox(result);
            }
        }

        public static int? ShowSyncYesNoButtonAlertMsgBox(string title, string msg)
        {
            if (!Guide.IsVisible)
            {
                IAsyncResult result = Guide.BeginShowMessageBox(
                    title,
                    msg,
                    new string[] { "yes", "no" },
                    0, MessageBoxIcon.Alert, null, null);

                return Guide.EndShowMessageBox(result);
            }

            return null;
        }

        public static string ShowKeyboardInput(string title, string msg, string defaultValue)
        {
            if (!Guide.IsVisible)
            {
                IAsyncResult result = Guide.BeginShowKeyboardInput(
                        Microsoft.Xna.Framework.PlayerIndex.One,
                        title,
                        msg,
                        defaultValue,
                        null,
                        null);

                return Guide.EndShowKeyboardInput(result);
            }

            return null;
        }
    }
}
