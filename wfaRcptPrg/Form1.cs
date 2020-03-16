using FileHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wfaRcptPrg {

   public partial class Form1 : Form {

      // filehelper
      public RcptnRecords[] strRcptnLines;

      public string[] strRcptRecords;

      public string Path = @"C:\Users\myPC\source\repos\data\rES_00.csv";

      // class AllIngrES
      public string[] strIngr;

      public List<string> ListRcptNames = new List<string>();

      public List<string> ListIngredients = new List<string>();

      public List<string> listRcptnLines = new List<string>();

      public string strIngredients = string.Empty;

      public string strPreparation = string.Empty;

      // aRcptName|bIngr|cPrep
      private readonly FileHelperEngine<RcptnRecords> engineRcptnLines = new FileHelperEngine<RcptnRecords>();

      // add statusbar
      private readonly StatusBar StatusBar = new StatusBar();

      private readonly StatusBarPanel panel1 = new StatusBarPanel();

      private readonly StatusBarPanel panel2 = new StatusBarPanel();

      private readonly StatusBarPanel panel3 = new StatusBarPanel();

      private readonly StatusBarPanel panel4 = new StatusBarPanel();

      private int intRecipeRecord;

      private int nrRcpt;

      private string strRcptName = string.Empty;

      private string strIngredient = string.Empty;

      private string strNewRcptLine = string.Empty;

      private string[] rcptPrepLines;

      private bool boolNameFound = false;

      private bool boolRcptSaved = false;

      public Form1() {
         InitializeComponent();
      }

      public static string GetSeason() {
         string strSeason = "";
         switch (DateTime.Now.Month) {
            // Spring
            case 3:
            case 4:
            case 5: strSeason = "Primavera"; break;
            // Summer
            case 6:
            case 7:
            case 8: strSeason = "Verano"; break;
            // Autumn
            case 9:
            case 10:
            case 11: strSeason = "Otoño​"; break;
            // Winter
            case 12:
            case 1:
            case 2: strSeason = "Invierno"; break;
         }
         return "Es: " + strSeason;  // Het is
      }

      public int GetRcptNr(string s) {
         var nr = 0;
         for (var i = 0; i < ListRcptNames.Count; i++) {
            if (strRcptnLines[i].aName == s) {
               nr = i; break;
            }
         }
         intRecipeRecord = nr;
         return ++nr;
      }

      public void DoBtn_A_Z_Clicked(object sender, EventArgs e) {
         // btn = (Button)sender
         Button btn = (Button)sender;
         lbxRecipeNames.Items.Clear();

         for (var i = 0; i < strRcptnLines.Length; i++) {
            var record = strRcptnLines[i];
            if (record.aName.Substring(0, 1) == btn.Text || record.aName.Substring(0, 1) == btn.Text.ToLower()) {
               lbxRecipeNames.Items.Add(record.aName);
            }
         }
         lbxRecipeNames.SelectedIndex = 0;
      }

      public void InsertLineAfter(string file, int rn, string lineToInsert) {
         var lines = File.ReadLines(file).ToList();
         var index = rn;  // TODO: Validation (if index is -1, we couldn't find it)
         lines.Insert(index + 1, lineToInsert);
         File.WriteAllLines(file, lines, Encoding.UTF8); 
      }

      //==== start
      private void Form1_Load(object sender, EventArgs e) {
         // Create a Point object that will be used as the location of the form.
         var posWindow = new Point(10, 10);
         // Set the location of the form using the Point object.
         DesktopLocation = posWindow;

         // add statusbar
         AddStatusBar();
         // add Buttons A-Z
         AddBtnsA_Z();
         // add tooltips
         AddToolTips();
         // Load recipe date
         strRcptnLines = engineRcptnLines.ReadFile(Path);   // read all the recipe records

         // get all recipe names in lbx
         foreach (var rcptRecord in strRcptnLines) {
            ListRcptNames.Add(rcptRecord.aName); // put all recipe names in listRcptnNames
            // Fill listRcptnLines
            listRcptnLines.Add(rcptRecord.aName + "|" + rcptRecord.bIngr + "|" + rcptRecord.cPrep);
         }

         // put all recipe names in lbx
         foreach (var rName in ListRcptNames) {
            lbxRecipeNames.Items.Add(rName);
         }

         // use class AllIngrES to fill lists
         strIngr = AllIngrES.strIngredients.Split('\n');
         foreach (var ingr in strIngr) {
            var sI = ingr.Split('|');
            ListIngredients.AddRange(sI);
            //  ListIngredients.Sort();
         }
         lbxRecipeNames.Focus(); // set focus on recipe names
         lbxRecipeNames.SelectedIndex = 0; // select first name
      }

      private void AddToolTips() {
         // panels
         PnlButtons();
         // buttons
         BtnSearchIngr();
         BtnYoutube();
         BtnNewRecipe();
         BtnSaveRecipe();
         BtnPrintRecipe();
         // listbox
         LbxRecipeNames();
         // richTextBoxes
         RtbxDisplayIngrProperties();
         RtbxValues();
         RtbxIngr();
         RtbxPrep();
         // menu
         MenuStrip();
      }

      private void BtnPrintRecipe() {
         var toolTipBtnPrintRecipe = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipBtnPrintRecipe.SetToolTip(btnPrintRecipe, "print Recipe...");
      }

      private void BtnSaveRecipe() {
         var toolTipBtnSaveRecipe = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipBtnSaveRecipe.SetToolTip(btnSaveRecipe, "Save Recipe...");
      }

      private void BtnNewRecipe() {
         var toolTipBtnNewRecipe = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipBtnNewRecipe.SetToolTip(btnNew, "Arrastrar y soltar el ingrediente desde la dirección web");
      }

      private void BtnSearchIngr() {
         var toolTipBtnSearchIngr = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipBtnSearchIngr.SetToolTip(btnBuscar, "Buscar recetas con este ingrediente...");
      }

      private void PnlButtons() {
         var toolTipPnlButtons = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipPnlButtons.SetToolTip(pnlButtons, "Las recetas que empiezan con la misma letra.");
      }

      private void MenuStrip() {
         var toolTipMenuStrip = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipMenuStrip.SetToolTip(menuStrip, "Menú de opciones...");
      }

      private void RtbxPrep() {
         var toolTipRtbxPrep = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipRtbxPrep.SetToolTip(rtbxPrep, "Descripción de la preparación.");
      }

      private void RtbxIngr() {
         var toolTipRtbxIngr = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipRtbxIngr.SetToolTip(rtbxIngr, "Ingredientes requerido.s");
      }

      private void RtbxValues() {
         var toolTipRtbxValues = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipRtbxValues.SetToolTip(rtbxValues, "Cantidades necesarias.");
      }

      private void LbxRecipeNames() {
         var toolTipLbxRecipeNames = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipLbxRecipeNames.SetToolTip(btnYoutube, "Haz clic en mí, entonces clic en la URL de la web con el ingrediente deseado...");
      }

      private void BtnYoutube() {
         var toolTipBtnYoutube = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };
         toolTipBtnYoutube.SetToolTip(btnYoutube, "Clic en la URL de la web con el ingrediente deseado...");
      }

      private void RtbxDisplayIngrProperties() {
         var ToolTipRtbxDisplayIngrProperties = new ToolTip {
            AutoPopDelay = 5000,
            InitialDelay = 0,
            ReshowDelay = 500,
            ShowAlways = true
         };
         ToolTipRtbxDisplayIngrProperties.SetToolTip(rtbxDisplayIngrProperties, "Necesario: acciones y anuncios");
      }

      private void AddStatusBar() {
         StatusBar.Font = new Font("Arial", 12, FontStyle.Italic);

         // First panel with a sunken border style.
         panel1.BorderStyle = StatusBarPanelBorderStyle.Sunken;
         // text recipe number
         panel1.Text = LocalizableStrings.Form1_AddStatusBar_Número_de_receta__ + GetRcptNr(lbxRecipeNames.GetItemText(lbxRecipeNames.SelectedItem));
         panel1.Width = 300;

         // Display the second panel with a raised border style.
         panel2.BorderStyle = StatusBarPanelBorderStyle.Raised;
         panel2.Text = DateTime.Today.ToLongDateString(); // Set the text of the panel to the current date.
         panel2.Width = 300;
         panel2.ToolTipText = DateTime.Now.ToShortTimeString(); // Create ToolTip text that displays time the application was started.

         panel3.BorderStyle = StatusBarPanelBorderStyle.Sunken;
         panel3.Text = LocalizableStrings.Form1_AddStatusBar_Lo_encontré_ + lbxRecipeNames.Items.Count + LocalizableStrings.Form1_AddStatusBar__Recetas;
         panel3.Width = 300;

         panel4.BorderStyle = StatusBarPanelBorderStyle.Sunken;
         panel4.Text = GetSeason();
         panel4.Width = 300;

         StatusBar.ShowPanels = true; // Display panels in the StatusBar control.

         // Add both panels to the StatusBarPanelCollection of the StatusBar.
         StatusBar.Panels.Add(panel1);
         StatusBar.Panels.Add(panel2);
         StatusBar.Panels.Add(panel3);
         StatusBar.Panels.Add(panel4);

         Controls.Add(StatusBar); // Add the StatusBar to the form.
      }

      private void AddBtnsA_Z() {
         var xPos = 0;
         var yPos = 0;
         // Declare and assign number of buttons = 26
         var btnArray = new Button[26];
         // Create (26) Buttons:
         for (int i = 0; i < 26; i++) {
            btnArray[i] = new Button(); // Initialize one variable
         }

         var n = 0;

         while (n < 26) {
            btnArray[n].Tag = n + 1; // Tag of button
            btnArray[n].Width = 30; // Width of button
            btnArray[n].Height = 30; // Height of button
            if (n == 26) {
               xPos = 18;
               yPos = 0;
            }
            // Location of button:
            btnArray[n].Left = xPos;
            btnArray[n].Top = yPos;
            // Add buttons to a Panel:
            pnlButtons.Controls.Add(btnArray[n]); // Let panel hold the Buttons
            yPos += btnArray[n].Width;   // Left of next button

            // Write English Character: btnArray[n].Text = ((char)(n + 65)).ToString();

            char[] Alphabet = {
                     'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
                     'J', 'K', 'L', 'M', 'N','O', 'P', 'Q', 'R',
                     'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
                };

            btnArray[n].Text = Alphabet[n].ToString();

            // The event of click Button
            btnArray[n].Click += DoBtn_A_Z_Clicked;
            n++;
         }
      }

      //======== Toolstrip Menu
      private void MnuSave_Click(object sender, EventArgs e) {
         rtbxDisplayIngrProperties.Clear();
         if (rtbxIngr.Text.Length > 0 && rtbxIngr.Text.Length > 0 && rtbxPrep.Text.Length > 0) {
            SaveChangedRecipe(); strRcptName = lbxRecipeNames.SelectedItem.ToString();
            nrRcpt = GetRcptNr(lbxRecipeNames.GetItemText(strRcptName)); // 0
            DisplayRecipe(nrRcpt);
         } else {
            MessageBox.Show("La receta no está completa");
         }
         lblRemarks.Text = string.Empty;
         lblRemarks.Text += LocalizableStrings.Form1_mnuSaveAs_Click_;
      }

      private void MnuNew_Click(object sender, EventArgs e) {
         // clear screen
         rtbxIngr.Clear();
         rtbxValues.Clear();
         rtbxPrep.Clear();
         rtbxDisplayIngrProperties.Clear();
         lbxRecipeNames.Items.Clear();
         // statiebalk ?

         // ask user for recipe name
         // InputBox with value validation - first define validation delegate, which
         // returns empty string for valid values and error message for invalid values
         string Validation(string valueInput) {
            if (valueInput == "") return "El valor no puede estar vacío.";
            return !(valueInput.Contains("#")) ? "estera ¿#?: Nombre de receta#" : "";
         }

         var newName = "nombre de la receta#";
         if (InputBox.Show("¿Nombre de receta?", "Nombre:", ref newName, Validation) == DialogResult.OK) {
            MessageBox.Show(newName);
            if (!ListRcptNames.Contains(newName))
               lblRecipeName.Text = LocalizableStrings.Form1_mnuNew_Click__Nombre_nuevo_receta__ + newName;
            else {
               MessageBox.Show(LocalizableStrings.Form1_mnuNew_Click_Nombre_ya_exist);
               newName = GetNewRcptName();
               MessageBox.Show(LocalizableStrings.Form1_mnuNew_Click_Nuevo_nombre__ + newName);
            }
         }
         // drag and drop new recipe name, ingredients, preparation etc
         //  SaveWithNewRcptName();
      }

      private void MnuPrint_Click(object sender, EventArgs e) {
      }

      private void MnuPrintPreview_Click(object sender, EventArgs e) {
      }

      private void MnuExit_Click(object sender, EventArgs e) {
         const string message = "¿Quieres salir?";
         const string caption = "SALIR";
         var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
         if (result == DialogResult.No) {
            Application.Exit();
         } else if (result == DialogResult.Yes) {
            this.Close();
         }
      }

      private void MnuZoekIngredient_Click(object sender, EventArgs e) {
         AutoCompleteCbx.Show("Haga su elección. . .", ref strIngredient);
         DoIngrSearch();
      }

      private void MnuKarlosWebm_Click(object sender, EventArgs e) {
         MsgRtbxPrepAnderReceptText();
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (string strIngr in ListIngredients) {
            foreach (string strIngrRcpt in strIngrInRcpt) {
               StrRemarksWeb(strIngr, strIngrRcpt, "www.hogarmania.com/buscador/index.html?q=");
            }
         }
      }

      private void MsgRtbxPrepAnderReceptText() {
         rtbxDisplayIngrProperties.Clear();
         rtbxDisplayIngrProperties.AppendText("Otro cocinero con recetas que también tiene en esta receta! Haz clic en la dirección web con el ingrediente que estás buscando.: \n\n");
      }

      private void MnuES_cookpadWebm_Click(object sender, EventArgs e) {
         MsgRtbxPrepAnderReceptText();
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (string strIngr in ListIngredients) {
            foreach (string strIngrRcpt in strIngrInRcpt) {
               StrRemarksWeb(strIngr, strIngrRcpt, "cookpad.com/es/buscar/");
            }
         }
      }

      private void MnuPequeocioWebm_Click(object sender, EventArgs e) {
         MsgRtbxPrepAnderReceptText();
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (string strIngr in ListIngredients) {
            foreach (string strIngrRcpt in strIngrInRcpt) {
               StrRemarksWeb(strIngr, strIngrRcpt, "www.pequeocio.com/?s=");
            }
         }
      }

      private void MnuGnatisWebm_Click(object sender, EventArgs e) {
         MsgRtbxPrepAnderReceptText();
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (string strIngr in ListIngredients) {
            foreach (string strIngrRcpt in strIngrInRcpt) {
               StrRemarksWeb(strIngr, strIngrRcpt, "www.recetasgratis.net/busqueda?q=");
            }
         }
      }

      private void MnuCocinaCaseraWebm_Click(object sender, EventArgs e) {
         MsgRtbxPrepAnderReceptText();
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (string strIngr in ListIngredients) {
            foreach (string strIngrRcpt in strIngrInRcpt) {
               StrRemarksWeb(strIngr, strIngrRcpt, "www.cocinacaserayfacil.net/?s=");
            }
         }
      }

      private void MnucocinarygozarWeb_Click(object sender, EventArgs e) {
         MsgRtbxPrepAnderReceptText();
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (string strIngr in ListIngredients) {
            foreach (string strIngrLine in strIngrInRcpt) {
               // StrRemarks_Web(strIngr, strIngrRcpt,"www.cocinarygozar.com /? s =" );
               if (StringExtension.ContainsWord(strIngrLine, strIngr)) {
                  switch (strIngr) {
                     default: rtbxDisplayIngrProperties.AppendText("* --> " + strIngr + "@ https://" + "www.cocinarygozar.com /? s =" + strIngr + "&post_type=receta" + "\n"); break;
                  }
               }
            }
         }
      }

      private void BtnBuscar_Click(object sender, EventArgs e) {
         AutoCompleteCbx.Show("Haga su elección. . .", ref strIngredient);
         DoIngrSearch();
      }

      private void BtnYoutube_Click(object sender, EventArgs e) {
         MsgRtbxPrepAnderReceptText();
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (var strIngr in ListIngredients) {
            foreach (var strIngrRcpt in strIngrInRcpt) {
               StrRemarksWeb(strIngr, strIngrRcpt, "www.youtube.com/results?search_query=");
            }
         }
      }

      private void Microondas_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.elespanol.com/cocinillas/recetas/microondas/20170316/consejos-cocinar-microondas-sacandole-maximo-partido/1000804969496_30.html");
      }

      private void Cazuela_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://books.google.co.uk/books?id=SDoySmumZl8C&pg=PA503&lpg=PA503&dq=usando+un+cazuela&source=bl&ots=LMF1p4Zlia&sig=ACfU3U2gDBfzORd2tVPbB7c43zwvWR0YqA&hl=nl&sa=X&ved=2ahUKEwiz_em1j43oAhXF0qQKHdITA_MQ6AEwCHoECAYQAQ#v=onepage&q=usando%20un%20cazuela&f=false");
      }

      private void Plancha_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://books.google.co.uk/books?id=qQp9iLE98xwC&pg=PA7&dq=cocinar+a+la+plancha&hl=nl&sa=X&ved=0ahUKEwjG5b3kj43oAhVMLewKHZe-A3cQ6AEINTAB#v=onepage&q=cocinar%20a%20la%20plancha&f=false");
      }

      private void Thermomix_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.thermorecetas.com/que-es-thermomix/");
      }

      private void Horno_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.google.co.uk/search?q=que+es+un+horno&newwindow=1&hl=nl&tbm=isch&sxsrf=ALeKk02A-IR0oGjI13mUQqhvDhXSALXFKw:1583748162565&source=lnms&sa=X&ved=0ahUKEwjBo5LVkY3oAhWLC-wKHWIUD3oQ_AUICigB&biw=1053&bih=517&dpr=1.71");
      }

      private void Vapor_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.google.co.uk/search?q=que+es+cocinar+al+vapor&tbm=isch&ved=2ahUKEwiwotSKko3oAhWDG-wKHRYFCbkQ2-cCegQIABAA&oq=que+es+cocinar+al+vapor&gs_l=img.3..0i19.36925.46868..47267...0.0..0.501.2359.0j14j1j5-1......0....1..gws-wiz-img.......0i8i30i19j0i30i19.5PKp7Lq7F58&ei=shRmXrCBMIO3sAeWiqTICw&bih=517&biw=1053&hl=nl");
      }

      private void Airfryer_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.google.co.uk/search?q=que+es+un+airfryer&tbm=isch&ved=2ahUKEwiwotSKko3oAhWDG-wKHRYFCbkQ2-cCegQIABAA&oq=que+es+un+airfryer&gs_l=img.3..0i19.7137.10747..11135...0.0..0.401.1419.0j5j2j0j1......0....1..gws-wiz-img.xACRfNAhUK8&ei=shRmXrCBMIO3sAeWiqTICw&bih=517&biw=1053&hl=nl");
      }

      private void Barbacoa_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.google.co.uk/search?q=que+es+un+barbacoa&tbm=isch&ved=2ahUKEwiwotSKko3oAhWDG-wKHRYFCbkQ2-cCegQIABAA&oq=que+es+un+barbacoa&gs_l=img.3...3904.6499..7202...0.0..0.413.2513.0j2j0j5j1......0....1..gws-wiz-img.......0i19.8rVvqTTXC0s&ei=shRmXrCBMIO3sAeWiqTICw&bih=517&biw=1053&hl=nl");
      }

      private void Verduras_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.clara.es/recetas/verdura-legumbres");
      }

      private void Pescado_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.google.co.uk/search?q=recatas+de+pescado+Y+Marisco&tbm=isch&ved=2ahUKEwjijtKGk43oAhWFG-wKHUqJCuAQ2-cCegQIABAA&oq=recatas+de+pescado+Y+Marisco&gs_l=img.12...5791.15186..17741...0.0..0.272.1734.0j13j1......0....1..gws-wiz-img.......0j0i67j0i19j0i10i19j0i30i19j0i5i30i19.HnmJ-OWYMbk&ei=thVmXuLbMIW3sAfKkqqADg&bih=517&biw=1053&hl=nl");
      }

      private void Carne_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.google.co.uk/search?q=recetas+de+carne&tbm=isch&ved=2ahUKEwjijtKGk43oAhWFG-wKHUqJCuAQ2-cCegQIABAA&oq=recetas+de+carne&gs_l=img.3..0i19l10.137887.143225..143611...0.0..1.630.4106.0j8j4j2j0j2......0....1..gws-wiz-img.......0j0i67j0i30.vb4NE_NAhj0&ei=thVmXuLbMIW3sAfKkqqADg&bih=517&biw=1053&hl=nl");
      }

      private void Frutas_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.google.co.uk/search?q=recatas+de+frutas&tbm=isch&ved=2ahUKEwiCs_jok43oAhXG2qQKHa14CfQQ2-cCegQIABAA&oq=recatas+de+frutas&gs_l=img.12...34466.35948..38499...0.0..0.146.776.0j6......0....1..gws-wiz-img.kGRHeXVRJaU&ei=hBZmXoLhOca1kwWt8aWgDw&bih=517&biw=1053&hl=nl");
      }

      private void Pasta_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.google.co.uk/search?q=recatas+de+pasta&tbm=isch&ved=2ahUKEwjjv_b7k43oAhUJC-wKHas7AdUQ2-cCegQIABAA&oq=recatas+de+pasta&gs_l=img.12...20225.21239..23988...0.0..0.110.527.0j5......0....1..gws-wiz-img.-Y2ro2NJKWE&ei=rBZmXuO5LomWsAer94SoDQ&bih=517&biw=1053&hl=nl");
      }

      private void Otros_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.google.co.uk/search?q=recetas+de+otros&tbm=isch&hl=nl&nfpr=1&hl=nl&ved=2ahUKEwjD5tCklI3oAhXWNuwKHeWTB3YQBXoECAEQJg&biw=1043&bih=517");
      }

      private void StrRemarksWeb(string ingrediente, string IngrInRcpt, string strWeb) {
         if (StringExtension.ContainsWord(IngrInRcpt, ingrediente)) {
            switch (ingrediente) {
               default: rtbxDisplayIngrProperties.AppendText("* --> " + ingrediente + "@ https://" + strWeb + ingrediente + "\n"); break;
            }
         }
      }

      private void MnuNestleWebm_Click(object sender, EventArgs e) {
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (string strIngr in ListIngredients) {
            foreach (string strIngrRcpt in strIngrInRcpt) {
               StrRemarksWeb(strIngr, strIngrRcpt, "www.nestlecocina.es/recetas/busqueda?c=");
            }
         }
      }

      private void MnuEnColombiaWebm_Click(object sender, EventArgs e) {
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (string strIngr in ListIngredients) {
            foreach (string strIngrRcpt in strIngrInRcpt) {
               StrRemarksWeb(strIngr, strIngrRcpt, "encolombia.com/?s=");
            }
         }
      }

      private void MnuGallinablancaWebm_Click(object sender, EventArgs e) {
         string[] strIngrInRcpt = strRcptnLines[nrRcpt - 1].bIngr.Split(';');
         foreach (string strIngr in ListIngredients) {
            foreach (string strIngrRcpt in strIngrInRcpt) {
               StrRemarksWeb(strIngr, strIngrRcpt, "www.gallinablanca.es/busqueda/?phrase=");
            }
         }
      }

      private void MnuAbout_Click(object sender, EventArgs e) {
         new AboutBox();
      }

      private void DoIngrSearch() {
         if (strIngredient == null) throw new ArgumentNullException(nameof(strIngredient));
         lbxRecipeNames.Items.Clear();
         try {
            foreach (var record in strRcptnLines) {
               if (Regex.IsMatch(record.bIngr.ToLower(), "\\b" + this.strIngredient.ToLower() + "\\b", RegexOptions.IgnoreCase)) {
                  lbxRecipeNames.Items.Add(record.aName);
               }
            }
            lbxRecipeNames.SelectedIndex = 0;
         } catch {
            ClearDisplay();
            rtbxPrep.AppendText("No se encontró receta :-)");
         }
      }

      private void ClearDisplay() {
         strIngredients = string.Empty;
         strPreparation = string.Empty;
         rtbxDisplayIngrProperties.Clear();
         rtbxPrep.Clear();
         rtbxIngr.Clear();
         rtbxValues.Clear();
      }

      private void LbxRecipeNames_SelectedIndexChanged(object sender, EventArgs e) {
         rtbxDisplayIngrProperties.Clear();
         strRcptName = lbxRecipeNames.SelectedItem.ToString();
         nrRcpt = GetRcptNr(lbxRecipeNames.GetItemText(strRcptName)); // 0
         DisplayRecipe(nrRcpt);
         // messsage for user
         lblRemarks.Text = LocalizableStrings.Form1_lbxRecipeNames_SelectedIndexChanged_Nota__Haz_clic_con_el_ratón_en_el_cuadro_de_lista_con_los_nombres_de_las_recetas_o_usa_la_búsqueda_de_ingrediente_y_receta____;
      }

      private void SaveChangedRecipe() {
         rtbxDisplayIngrProperties.Clear();
         strIngredients = string.Empty;  // !!
         strPreparation = string.Empty;
         // for \n
         rtbxPrep.Text = rtbxPrep.Text.Replace(".", ";");

         var strNewName = GetNewRcptName();
            MessageBox.Show(strNewName);
         // - 9 ?  save with same name (Xxx - 9)
         if (strNewName.Contains(" - 9")) {
            var file = new List<string>(File.ReadAllLines(Path,Encoding.UTF8));
            file.RemoveAt(nrRcpt); // - 9
            File.WriteAllLines(Path, file.ToArray(),Encoding.UTF8);
            
            // make strNewRcptLine
            for (var i = 0; i < rtbxValues.Lines.Count(); i++) {
               strIngredients += rtbxValues.Lines[i].Trim() + " " + rtbxIngr.Lines[i] + ";";
            }

            // get prep lines
            for (var i = 0; i < rtbxPrep.Lines.Count(); i++) {
               strPreparation += rtbxPrep.Lines[i];
            }
            strNewRcptLine = strRcptName + "|" + strIngredients + "|" + strPreparation;  // - 9
            //  file,  lineToFind,  lineToInsert)
            InsertLineAfter(Path,nrRcpt, strNewRcptLine);

            MessageBox.Show("Opgeslagen als " + strNewName);
         } else { 
           // save with the new name
            for (var i = 0; i < rtbxValues.Lines.Count(); i++) { // get the ingr lines (value + name)
               strIngredients += rtbxValues.Lines[i].Trim() + " " + rtbxIngr.Lines[i] + ";";
            }

            // get prep lines
            for (var i = 0; i < rtbxPrep.Lines.Count(); i++) {
               strPreparation += rtbxPrep.Lines[i];
            }
            strNewRcptLine = strNewName + "|" + strIngredients + "|" + strPreparation;

            InsertLineAfter(Path, nrRcpt, strNewRcptLine);
            strRcptnLines = engineRcptnLines.ReadFile(Path);  // read recipe records
            lbxRecipeNames.Items.Clear();

            foreach (var rcptRecord in strRcptnLines) {
               lbxRecipeNames.Items.Add(rcptRecord.aName); // put all recipe names in lbx
               ListRcptNames.Add(rcptRecord.aName); // put all recipe names in listRcptnNames
            }

            boolRcptSaved = false;
            mnuSaveRecipe.Enabled = false;

            lbxRecipeNames.Focus();
            lbxRecipeNames.Sorted = true;
            lbxRecipeNames.SelectedItem = strNewName;
         }

         lblRemarks.Text = LocalizableStrings.Form1_SaveEditedRecipe_Receta_guardada_con_nuevo_nombre__ + strRcptName;
      }

      private void SaveWithSelectedRcptName(string strRName) {
         // remove recipe and save with old recipe name
         if (RemoveRcpt.RemoveLines(Path, (l) => { return l.Contains(strRName); }) == 0) {
            MessageBox.Show(LocalizableStrings.Form1_SaveWithSelectedRcptName_);
         }
         strNewRcptLine = strRName + "|" + strIngredients + "|" + strPreparation;
         strNewRcptLine = strNewRcptLine.Replace("; ;|", "|"); // strNewRcptLine = strNewRcptLine.Replace("", "");
         strNewRcptLine = strNewRcptLine.Replace("; ;", ";");
         strNewRcptLine = strNewRcptLine.Replace(";;", ";");

       //  InsertLineAfter(Path, intRecipeRecord.ToString(), strNewRcptLine);
      }

      private string GetNewRcptName() {
         if (strRcptName.Contains(" - 9")) {
           // strRcptName = strRcptName.Replace(" - 9", " - 10");
         }
         if (strRcptName.Contains(" - 8")) {
            strRcptName = strRcptName.Replace(" - 8", " - 9");
         }
         if (strRcptName.Contains(" - 7")) {
            strRcptName = strRcptName.Replace(" - 7", " - 8");
         }
         if (strRcptName.Contains(" - 6")) {
            strRcptName = strRcptName.Replace(" - 6", " - 7");
         }

         if (strRcptName.Contains(" - 5")) {
            strRcptName = strRcptName.Replace(" - 5", " - 6");
         }
         if (strRcptName.Contains(" - 4")) {
            strRcptName = strRcptName.Replace(" - 4", " - 5");
         }
         if (strRcptName.Contains(" - 3")) {
            strRcptName = strRcptName.Replace(" - 3", " - 4");
         }
         if (strRcptName.Contains(" - 2")) {
            strRcptName = strRcptName.Replace(" - 2", " - 3");
         }
         if (strRcptName.Contains(" - 1")) {
            strRcptName = strRcptName.Replace(" - 1", " - 2");
         }
         if (strRcptName.Contains("#")) {
            strRcptName = strRcptName.Replace("#", " - 1");
         }
         return strRcptName;
      }

      private void DisplayRecipe(int rcptNr) {
         ClearDisplay();
         strRcptRecords = strRcptnLines[rcptNr - 1].bIngr.Split(';');  // Append lbxIngrList

         for (int nrIngr = 0; nrIngr < ListIngredients.Count(); nrIngr++) {
            strIngredient = ListIngredients[nrIngr];
            for (var i = 0; i < strRcptRecords.Length; i++) {
               if (strRcptRecords[i].ContainsWord(strIngredient)) {
                  DisplayIngrValues(i);
               }
            }
         }

         // display recipe name
         lblRecipeName.Text = strRcptName;

         // display preperation:
         rcptPrepLines = strRcptnLines[intRecipeRecord].cPrep.Split(';'); // ====

         for (int i = 0; i < rcptPrepLines.Length - 1; i++) { // (-1) no extra "." at last line
            string prepLines = rcptPrepLines[i];
            rtbxPrep.AppendText(prepLines + ".\n");

            // corrections
            rtbxPrep.Text = rtbxPrep.Text.Replace("!.", "!");
            rtbxPrep.Text = rtbxPrep.Text.Replace("?.", "?");
            rtbxIngr.Text = rtbxIngr.Text.Replace(" ,", ",");

            // statusbalk
            panel1.Text = LocalizableStrings.Form1_DisplayRecipe_numeración__del_receta__ + nrRcpt;
            panel3.Text = LocalizableStrings.Form1_DisplayRecipe_Encontré_ + lbxRecipeNames.Items.Count + LocalizableStrings.Form1_DisplayRecipe__Recetas;
            panel4.Text = GetSeason();

            mnuSaveRecipe.Enabled = false;
         }
      }

      private void DisplayIngrValues(int i) {
         if (!rtbxIngr.Text.Contains(strIngredient)) {
            CommaToSpace(i);
            ValuesRightSide(i);
         }
      }

      private void ValuesRightSide(int i) {
         switch (strRcptRecords[i].Length) {
            case 0: rtbxValues.AppendText("\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 1: rtbxValues.AppendText("                                  " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 2: rtbxValues.AppendText("                                " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 3: rtbxValues.AppendText("                                " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 4: rtbxValues.AppendText("                               " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 5: rtbxValues.AppendText("                              " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 6: rtbxValues.AppendText("                             " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 7: rtbxValues.AppendText("                            " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 8: rtbxValues.AppendText("                           " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 9: rtbxValues.AppendText("                          " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 10: rtbxValues.AppendText("                         " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 11: rtbxValues.AppendText("                        " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 12: rtbxValues.AppendText("                       " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 13: rtbxValues.AppendText("                      " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 14: rtbxValues.AppendText("                     " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 15: rtbxValues.AppendText("                    " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 16: rtbxValues.AppendText("                   " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 17: rtbxValues.AppendText("                  " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 18: rtbxValues.AppendText("                 " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 19: rtbxValues.AppendText("                " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 20: rtbxValues.AppendText("               " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 21: rtbxValues.AppendText("              " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 22: rtbxValues.AppendText("             " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 23: rtbxValues.AppendText("            " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 24: rtbxValues.AppendText("           " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 25: rtbxValues.AppendText("          " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 26: rtbxValues.AppendText("         " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 27: rtbxValues.AppendText("        " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 28: rtbxValues.AppendText("       " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 29: rtbxValues.AppendText("      " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 30: rtbxValues.AppendText("     " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 31: rtbxValues.AppendText("    " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 32: rtbxValues.AppendText("   " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 33: rtbxValues.AppendText("  " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 34: rtbxValues.AppendText(" " + strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
            case 35: rtbxValues.AppendText(strRcptRecords[i] + "\n"); rtbxIngr.AppendText(strIngredient + "\n"); break;
         }
      }

      private void CommaToSpace(int i) {
         strRcptRecords[i] = strRcptRecords[i].Replace(strIngredient, "").Replace(" , ", " ").Trim();
         strRcptRecords[i] = strRcptRecords[i].Replace(strIngredient, "").Replace(", ", " ").Trim();
         strRcptRecords[i] = strRcptRecords[i].Replace(strIngredient, "").Replace(" ,", " ").Trim();
         strRcptRecords[i] = strRcptRecords[i].Replace(strIngredient, "").Replace("  ", " ").Trim();
      }

      private void RtbxDisplayIngrSpec_LinkClicked(object sender, LinkClickedEventArgs e) {
         System.Diagnostics.Process.Start(e.LinkText);
      }

      private void RecetasRápidas_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.clara.es/temas/recetas-faciles");
      }

      private void MnuSemanal_Click(object sender, EventArgs e) {
         System.Diagnostics.Process.Start(@"https://www.clara.es/bienestar/alimentacion/menu-semanal-excesos-empachos-navidad_15384");
      }

      private void RtbxValues_TextChanged(object sender, EventArgs e) {
         lblRemarks.Text = string.Empty;
         lblRemarks.Text += LocalizableStrings.Form1_rtbxValues_TextChanged_;
         mnuSaveRecipe.Enabled = true;
         boolRcptSaved = false;
      }

      private void RtbxIngr_TextChanged(object sender, EventArgs e) {
         lblRemarks.Text = string.Empty;
         lblRemarks.Text += LocalizableStrings.Form1_rtbxValues_TextChanged_;
         mnuSaveRecipe.Enabled = true;
         boolRcptSaved = false;
      }

      private void RtbxPrep_TextChanged(object sender, EventArgs e) {
         lblRemarks.Text = string.Empty;
         lblRemarks.Text += LocalizableStrings.Form1_rtbxValues_TextChanged_;
         mnuSaveRecipe.Enabled = true;
         boolRcptSaved = false;
      }

      private void LbxRecipeNames_MouseDown(object sender, MouseEventArgs e) {
         if (e.Button == MouseButtons.Right) {
            RemoveSelectedRecpe(lbxRecipeNames.SelectedItem.ToString());
         }
      }

      private void RemoveSelectedRecpe(string rcptName) {
         MessageBox.Show(LocalizableStrings.Form1_RemoveSelectedRecpe_El_nombre_de_receta_seleccionado_se_elimina);
         // remove recept line in listRcptnLines
         listRcptnLines.Remove(listRcptnLines[nrRcpt - 1]);
         // delete  the file
         File.Delete(Path);
         // MessageBox.Show("Test");
         // write the new file
         File.WriteAllLines(Path, listRcptnLines, Encoding.UTF8);
         boolRcptSaved = true;
         //
         lbxRecipeNames.Items.Clear();
         // read all recipes names in lbx
         strRcptnLines = engineRcptnLines.ReadFile(Path);   // read all the recipe records
         // get all recipe names in lbx
         for (int i = 0; i < strRcptnLines.Length; i++) {
            RcptnRecords rcptRecord = strRcptnLines[i];
            lbxRecipeNames.Items.Add(rcptRecord.aName); // put all recipe names in lbx
            ListRcptNames.Add(rcptRecord.aName); // put all recipe names in listRcptnNames
            // Fill listRcptnLines
            listRcptnLines.Add(rcptRecord.aName + "|" + rcptRecord.bIngr + "|" + rcptRecord.cPrep);
         }
         DisplayRecipe(1);
      }

      private void BtnNew_Click(object sender, EventArgs e) {
         // clear screen
         rtbxIngr.Clear();
         rtbxValues.Clear();
         rtbxPrep.Clear();
         rtbxDisplayIngrProperties.Clear();
         lbxRecipeNames.Items.Clear();
         // statiebalk ?

         // ask user for recipe name
         // InputBox with value validation - first define validation delegate, which
         // returns empty string for valid values and error message for invalid values
         string InputBoxValidation(string valueInput) {
            if (valueInput == "") return "El valor no puede estar vacío.";
            if (!(valueInput.Contains("#"))) return "estera ¿#?: Nombre de receta#";
            return "";
         }

         var newName = "nombre de la receta#";
         if (InputBox.Show("¿Nombre de receta?", "Nombre:", ref newName, InputBoxValidation) == DialogResult.OK) {
            MessageBox.Show(newName);
            if (!ListRcptNames.Contains(newName))
               lblRecipeName.Text = LocalizableStrings.Form1_mnuNew_Click__Nombre_nuevo_receta__ + newName;
            else {
               MessageBox.Show(LocalizableStrings.Form1_mnuNew_Click_Nombre_ya_exist);
               newName = GetNewRcptName();
               MessageBox.Show(LocalizableStrings.Form1_mnuNew_Click_Nuevo_nombre__ + newName);
            }
         }
         // drag and drop new recipe name, ingredients, preparation etc
         // SaveWithNewRcptName();
      }

      private void BtnSaveRecipe_Click(object sender, EventArgs e) {
         rtbxDisplayIngrProperties.Clear();
         if (rtbxIngr.Text.Length > 0)
            SaveChangedRecipe();
         else {
            strRcptName = lbxRecipeNames.SelectedItem.ToString();
            nrRcpt = GetRcptNr(lbxRecipeNames.GetItemText(strRcptName)); // 0
            DisplayRecipe(nrRcpt);
         }
         lblRemarks.Text = string.Empty;
         lblRemarks.Text += LocalizableStrings.Form1_mnuSaveAs_Click_;
      }

      private void BtnPrintRecipe_Click(object sender, EventArgs e) {
      }

      public static class RemoveRcpt {

         public static int RemoveLines(string path, Predicate<string> remove) {
            var removed = 0;
            var lines = File.ReadAllLines(path);
            using (var output = new StreamWriter(path)) {
               foreach (var line in lines) {
                  if (remove(line)) {
                     removed++;
                  } else {
                     output.WriteLine(line);
                  }
               }
            }
            return removed;
         }
      }
   }
}