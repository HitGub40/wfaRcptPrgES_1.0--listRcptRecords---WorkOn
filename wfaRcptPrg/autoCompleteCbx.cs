// Based on: Manohar Solomon. K
// E-Mail: manoh.code@gmail.com
// <<< ------------------------------------------------------------- >>>

using System.Drawing;
using System.Windows.Forms;

namespace wfaRcptPrg {

   /// <summary>
   /// Defines the <see cref="AutoCompleteCbx" />
   /// </summary>
   public class AutoCompleteCbx {
      public static string[] strAllIngrNL_line = AllIngrES.strIngredients.Split('\n');

      /// <summary>
      /// Defines the cbxAutoComplete
      /// </summary>
      private static ComboBox cbxAutoComplete;

      private static CheckBox chkAutoCompleteMode;
      private static CheckBox chkLimitToList;

      public static void AutoComplete(ComboBox cbx, KeyPressEventArgs e, bool btnLimitToList) {
         string strFindStr;
         if (e.KeyChar == (char)13) {
            if (cbx.SelectionStart <= 1) { cbx.Text = ""; return; }
            strFindStr = cbx.SelectionLength == 0
                            ? cbx.Text.Substring(0, cbx.Text.Length - 1)
                            : cbx.Text.Substring(0, cbx.SelectionStart - 1);
         } else {
            strFindStr = cbx.SelectionLength == 0
                            ? cbx.Text + e.KeyChar
                            : cbx.Text.Substring(0, cbx.SelectionStart) + e.KeyChar;
         }

         // Search the string in the ComboBox list.

         var Idx = cbx.FindString(strFindStr);

         if (Idx != -1) {
            cbx.SelectedText = "";
            cbx.SelectedIndex = Idx;
            cbx.SelectionStart = strFindStr.Length;
            cbx.SelectionLength = cbx.Text.Length;
            e.Handled = true;
         } else {
            e.Handled = btnLimitToList;
         }
      }

      public static DialogResult? Show(string title, ref string value) {
         var buttonOk = new Button {
            Text = "OK",
            DialogResult = DialogResult.OK
         };
         buttonOk.SetBounds(228, 172, 80, 30);
         buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

         var buttonCancel = new Button {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel
         };
         buttonCancel.SetBounds(309, 172, 80, 30);

         cbxAutoComplete = new ComboBox {
            DropDownStyle = ComboBoxStyle.DropDown,
            DropDownWidth = 176,
            DropDownHeight = 106
         };

         cbxAutoComplete.SetBounds(15, 16, 176, 22);

         // get ingredients in cbx
         foreach (var t in strAllIngrNL_line) {
            var line = t.Split('|');
            cbxAutoComplete.Items.Add(line[0]);
         }

         cbxAutoComplete.SelectedIndex = 0;

         cbxAutoComplete.KeyPress += CbxAutoComplete_OnKeyPress;
         chkAutoCompleteMode = new CheckBox();
         chkAutoCompleteMode.SetBounds(208, 15, 200, 22);
         chkAutoCompleteMode.Text = "Auto Complete Mode";
         chkAutoCompleteMode.Checked = true;
         chkAutoCompleteMode.Font = new Font(chkAutoCompleteMode.Font.FontFamily, 12);

         chkLimitToList = new CheckBox();
         chkLimitToList.SetBounds(208, 39, 130, 22);
         chkLimitToList.Text = "Limit to List";
         chkLimitToList.Checked = true;
         chkLimitToList.Font = new Font(chkLimitToList.Font.FontFamily, 12);

         Form form = new Form { Text = title, ClientSize = new Size(400, 222) };
         form.Controls.AddRange(new Control[] { cbxAutoComplete, chkAutoCompleteMode, chkLimitToList, buttonOk, buttonCancel });
         form.ClientSize = new Size(form.ClientSize.Width, form.ClientSize.Height);
         form.FormBorderStyle = FormBorderStyle.FixedDialog;
         form.Font = new Font(form.Font.FontFamily, 12);
         form.StartPosition = FormStartPosition.CenterScreen;
         form.MinimizeBox = false;
         form.MaximizeBox = false;
         form.AcceptButton = buttonOk;
         form.CancelButton = buttonCancel;

         form.ShowDialog();
         value = cbxAutoComplete.Text;
         return DialogResult.OK;
      }

      public void AutoComplete(ComboBox cbx, KeyPressEventArgs e) {
         AutoComplete(cbx, e, false);
      }

      private static void CbxAutoComplete_OnKeyPress(object sender, KeyPressEventArgs e) {
         if (chkAutoCompleteMode.Checked) {
            AutoComplete(cbxAutoComplete, e, chkAutoCompleteMode.Checked);
         }
      }
   }
}