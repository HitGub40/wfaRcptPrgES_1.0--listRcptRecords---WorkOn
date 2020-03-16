using System;
using System.Drawing;
using System.Windows.Forms;

namespace wfaRcptPrg {
   public partial class AboutBox : Form {
      public String Maker = "Rudolf Albert";
      public String AppNaam = "Recepten Programma";
      public String Versie = "1.0_02-02-2020";

      public AboutBox() {
         InitDialog();
      }

      private void InitDialog() {
         ClientSize = new Size(250, 140);
         Text = "Over het programma";
         FormBorderStyle = FormBorderStyle.FixedDialog;
         ControlBox = false;
         MinimizeBox = false;
         MaximizeBox = false;

         Button wndClose = new Button {
            Text = "OK",
            Location = new Point(90, 100),
            Size = new Size(72, 24)
         };

         wndClose.Click += new EventHandler(About_OK);

         Label wndAuthorLabel = new Label {
            Text = "Maker:",
            Location = new Point(5, 5),
            Size = new Size(72, 24)
         };

         Label wndAuthor = new Label {
            Text = Maker,
            Location = new Point(80, 5),
            Size = new Size(80, 24)
         };

         Label wndProdNameLabel = new Label {
            Text = "Produkt:",
            Location = new Point(5, 30),
            Size = new Size(72, 24)
         };

         Label wndProdName = new Label {
            Text = AppNaam,
            Location = new Point(80, 30),
            Size = new Size(120, 24)
         };

         Label wndVersionLabel = new Label {
            Text = "Versie:",
            Location = new Point(5, 55),
            Size = new Size(72, 24)
         };

         Label wndVersion = new Label {
            Text = Versie,
            Location = new Point(80, 55),
            Size = new Size(72, 24)
         };

         Controls.AddRange(new Control[] {
                        wndClose,
                        wndAuthorLabel,
                        wndProdNameLabel,
                        wndVersionLabel,
                        wndAuthor,
                        wndProdName,
                        wndVersion
                        });
         StartPosition = FormStartPosition.CenterParent;
         ShowDialog();
      }

      private void About_OK(Object source, EventArgs e) {
         Control wndCtrl = ((Button)source).Parent;
         ((Form)wndCtrl).Close();
      }
   }
}
