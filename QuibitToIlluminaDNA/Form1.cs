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

namespace QuibitToIlluminaDNA
{
    public partial class MainForm : Form
    {

        public string QubitFileInString = null;
        public float dnaConcetrationFinal = (float)0.0;
        public float dnaVolumeFinal = (float)0.0;
        public float dnaVolumeInitial = (float)0.0;
        public float teVolumeInitial = (float)0.0;
        public float teVolumeFinal = (float)0.0;
        public string wellIn = "A1";
        public bool usingDiluteInput;

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\Users\SmithS3\Documents\";
            openFileDialog1.Filter = "excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((QubitFileInString = openFileDialog1.FileName) != null)
                    {
                        FileSelectedLabel.Text = QubitFileInString + "\nSelected";
                        FileSelectedLabel.Update();
                        if (testFile()) { ProcessInputButton.Enabled = true; }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private bool testFile()
        {
            foreach (string line in File.ReadAllLines(QubitFileInString)){
                if (line.Contains("Run ID")){
                    return true;
                }
            }
            return false;
        }
        public void NextWell()
        {
            string currentWell = wellIn;
            char whatatat = char.Parse(currentWell.Remove(1));
            int row = Convert.ToInt32(whatatat) - 64;
            int column = int.Parse(currentWell.Substring(1));
            if(row + 1 > 8)
            {
                column++;
                row = 1;
            } else
            {
                row++;
            }
            wellIn = (Convert.ToChar(row + 64)).ToString() + column.ToString();
            Console.WriteLine("Row : " + Convert.ToChar(row + 64));
            Console.WriteLine("Column : " + column);
            Console.WriteLine("Next Well : " + wellIn);
        }
        private void ProcessInputButton_Click(object sender, EventArgs e)
        {
            List<string[]> lineOut = new List<string[]>();

            dnaVolumeInitial = float.Parse(textBox1.Text);
            dnaConcetrationFinal = float.Parse(textBox2.Text);

            foreach (string line in File.ReadAllLines(QubitFileInString).Reverse())
            {
                double dnaConcentrationIn = (double)0.0;
                Console.WriteLine(line);
                string thisline = line.Replace("\"",string.Empty);
                if (thisline.StartsWith("Run")) { continue; }
                // if (thisline.StartsWith("Out")) { NextWell();  continue; }
                Console.WriteLine(thisline.Split(',')[6]);
                try
                {
                    dnaConcentrationIn = double.Parse(thisline.Split(',')[6]);
                }
                catch (Exception theException)
                {
                    String errorMessage;
                    errorMessage = "Error: ";
                    errorMessage = String.Concat(errorMessage, theException.Message);
                    errorMessage = String.Concat(errorMessage, " Line: ");
                    errorMessage = String.Concat(errorMessage, theException.Source);
                }
                lineOut.Add(DilutionLineFromInput(dnaConcentrationIn));
                NextWell();
            }
            dataGridViewOut.DataSource = PopulateTable(lineOut);
            ExportFilesButton.Enabled = true;
        }
        public string[] DilutionLineFromInput( double dnaConIn)
        {
            string[] lineOutput = new string[7];
            // Well     Input DNA   Dilution Ratio  TE intermediate well    TE final well   Final Volume    Final Concentration
            // 1        2           3               4                       5               6               7
            lineOutput[0] = wellIn;
            lineOutput[1] = dnaConIn.ToString();
            if (dnaConIn < dnaConcetrationFinal + 0.02 && dnaConIn > dnaConcetrationFinal - 0.02)
            {
                lineOutput[2] = "0";
                lineOutput[3] = "None";
                lineOutput[4] = "None";
                lineOutput[5] = (dnaVolumeInitial * 2).ToString();
                lineOutput[6] = dnaConIn.ToString();
            }
            else if (dnaConIn < dnaConcetrationFinal - 0.02)
            {
                if (usingDiluteInput)
                {
                    // for bad DNA < initial concentration, use original
                    lineOutput[1] = "Orig " + dnaConIn * 10;
                    lineOutput[2] = "0";
                    lineOutput[3] = "None";
                    lineOutput[4] = TEOnlyAddition(dnaVolumeInitial, dnaConIn * 10, dnaConcetrationFinal).ToString();
                    lineOutput[5] = (Convert.ToDouble(lineOutput[4]) + dnaVolumeInitial).ToString();
                    lineOutput[6] = (dnaConIn / Convert.ToDouble(lineOutput[5])).ToString();
                } else
                {
                    // assuming there is no original, use inputDNA
                    lineOutput[2] = "0";
                    lineOutput[3] = "None";
                    lineOutput[4] = "None";
                    lineOutput[5] = (dnaVolumeInitial * 2).ToString();
                    lineOutput[6] = dnaConIn.ToString();
                }
                
            }
            else if (dnaConIn > dnaConcetrationFinal + 0.02)
            {
                // tricky tricky Equation: 2^floor(log2(x))
                var dilutionFactor = Math.Pow(2, (Math.Floor(Math.Log(dnaConIn, 2))));
                if (dilutionFactor < 1) { dilutionFactor = 1; }
                Console.WriteLine("Dilution factor : " + dilutionFactor);
                if (dilutionFactor == 1)
                {
                    lineOutput[2] = "1:"+ dilutionFactor.ToString();
                    lineOutput[3] = "None";
                    lineOutput[4] = TEOnlyAddition(dnaVolumeInitial, dnaConIn, dnaConcetrationFinal).ToString();
                    lineOutput[5] = (TEOnlyAddition(dnaVolumeInitial, dnaConIn, dnaConcetrationFinal) + dnaVolumeInitial).ToString();
                    lineOutput[6] = (dnaConIn *(dnaVolumeInitial / Convert.ToDouble(lineOutput[5]))).ToString();
                }
                else if (dilutionFactor < 64)
                {
                    var dnaConcentrationIntermediate = dnaConIn / dilutionFactor;
                    string[] s = { dnaConIn.ToString(), dilutionFactor.ToString(), ((dilutionFactor * dnaVolumeInitial) - dnaVolumeInitial).ToString(), TEOnlyAddition(dnaVolumeInitial, dnaConcentrationIntermediate, dnaConcetrationFinal).ToString() };
                    lineOutput[2] = "1:" + dilutionFactor.ToString();
                    lineOutput[3] = ((dilutionFactor * dnaVolumeInitial) - dnaVolumeInitial).ToString();
                    lineOutput[4] = TEOnlyAddition(dnaVolumeInitial, dnaConcentrationIntermediate, dnaConcetrationFinal).ToString();
                    lineOutput[5] = (TEOnlyAddition(dnaVolumeInitial, dnaConcentrationIntermediate, dnaConcetrationFinal) + dnaVolumeInitial).ToString();
                    lineOutput[6] = (dnaConcentrationIntermediate * (dnaVolumeInitial / Convert.ToDouble(lineOutput[5]))).ToString();
                }
                else
                {
                    lineOutput[2] = "1:" + dilutionFactor.ToString();
                    lineOutput[3] = "None";
                    lineOutput[4] = "None";
                    lineOutput[5] = "None";
                    lineOutput[6] = "Concentration too high";
                }
            }

            return lineOutput;
        }
        public DataTable PopulateTable(List<string[]> listIn)
        {
            FileSelectedLabel.Text = "List count: " + listIn.Count();
            FileSelectedLabel.Update();
            DataTable tableOut = InitializeTable();
            int counter = 0;
            while(tableOut.Rows.Count + 1 < listIn.Count())
            {
                counter++;
                tableOut.Rows.Add();
                FileSelectedLabel.Text = "Adding Rows: " + counter.ToString();
                FileSelectedLabel.Update();
            }
            for (int i = 0; i < listIn.Count()-1; i++)
            {
                for (int j = 0; j < listIn[i].Count(); j++) {
                    if (listIn[j][j] != null && listIn.Count() > 0)
                        tableOut.Rows[i][j] = listIn[i][j];
                }
            }
            return tableOut;
        }
        /// <summary>
        /// Equation :  (TE + DNA)*[DNA] = (TE + DNA)*[DNA]
        ///             (0uL + 10uL)*[X] = (Y + 10uL)*[0.2ng/uL]
        ///             10uL*[X] / [0.2ng/uL] = (Y +10uL)
        ///             50uL^2 / ng * [X]ng/uL = YuL + 10uL
        ///             50uL * X - 10ul = Yul
        ///             50 * X - 10 = Y
        ///             {(dnaVolumeIn / finalConcetration) * dnaConcentrationIn} - dnaVolumeIn = teVolumeOut
        /// </summary>
        /// <param name="dnaVolumeIn"></param>
        /// <param name="finalConcentrationTarget"></param>
        /// <param name="dnaConcentrationIn"></param>
        /// <returns></returns>
        public float TEOnlyAddition(float dnaVolumeIn, double dnaConcentrationIn, float finalConcentrationTarget)
        {
            float teVolumeOut = (float)0.0;
            try
            {
                teVolumeOut = (float)(dnaConcentrationIn * (dnaVolumeIn / finalConcentrationTarget)) - dnaVolumeIn;
            }
            catch ( Exception e)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, e.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, e.Source);
            }
            return teVolumeOut;
        }
        static DataTable InitializeTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Well");
            table.Columns.Add("Input DNA");
            table.Columns.Add("Dilution Ratio");
            table.Columns.Add("TE intermediate well");
            table.Columns.Add("TE final well");
            table.Columns.Add("Final Volume");
            table.Columns.Add("Final Concentration");
            //Well  Input DNA   Dilution Ratio  TE intermediate well    TE final well   Final Volume    Final Concentration
            return table;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dnaVolumeInitial = float.Parse(textBox1.Text);
                FileSelectedLabel.Text = "Dna input = " + dnaVolumeInitial + "uL";
                FileSelectedLabel.Update();
            }
            catch
            {
                MessageBox.Show("Use a float", "flooooooatt");
                textBox1.Text = string.Empty;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dnaConcetrationFinal = float.Parse(textBox2.Text);
                FileSelectedLabel.Text = "Dna final concentration = " + dnaConcetrationFinal + "uL";
                FileSelectedLabel.Update();
            }
            catch
            {
                MessageBox.Show("Use a float", "flooooooatt");
                textBox2.Text = string.Empty;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            usingDiluteInput = checkBox1.Checked;
        }

        private void FinishButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ExportFilesButton_Click(object sender, EventArgs e)
        {
            // parse through datagrid for changes
            // check if there is output
            // open and save TE to intermediate and 0.2
            // open and save dna from 1:10, original & intermediate to intermediate and 0.2
            // sort the order of the last one so that intermediate to 0.2 never happens first
            DataTable exportTable = new DataTable();
            exportTable.TableName = "exportTable";
            foreach (DataGridViewColumn col in dataGridViewOut.Columns)
            {
                exportTable.Columns.Add(col.HeaderText);
            }
            foreach (DataGridViewRow row in dataGridViewOut.Rows)
            {
                DataRow thisRow = exportTable.NewRow();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    thisRow[cell.ColumnIndex] = cell.Value;
                }
                exportTable.Rows.Add(thisRow);
            }
            //DataView teDataView = new DataView(exportTable);
            //teDataView.RowFilter = "[TE final well] <> 'None' OR [TE intermediate well] <> 'None'";
            List<string> teStringList = new List<string>();

            /// turn this whole thing into a for loop
            /// for (int transfertype = 0; transfertype < 7; transfertype++){
            ///     switch (transfertype){
            ///         case 0 : 
            ///             selectString = x;
            ///             break;
            ///         case 1 : 
            ///             selectstring = y;
            ///             break;
            ///         default:
            ///             break;
            ///     }
            ///     datarow[] workingrows = exportTable.select(selectstring);
            ///     foreach (Datarow row in workingrows){
            ///         testringlist.add(eppoutstirng);
            ///     }
            ///     // save the testingstring to file
            /// }


            DataRow[] workingRows = exportTable.Select("[TE final well] <> 'None'");
            foreach (DataRow teRow in workingRows)
            {
                teStringList.Add(teRow.ItemArray[3].ToString());
            }
            workingRows = exportTable.Select("[TE intermediate well] <> 'None'");
            foreach (DataRow teRow in workingRows)
            {
                //
                // There needs to be a fix for each one of the transfer types for this to work
                //
                EppCsvString epOutstring = new EppCsvString();
                epOutstring.IntakeTableData(teRow.ItemArray, (int)TransferType.TeIntermediate);
                //teStringList.Add(teRow.ItemArray[4].ToString());
            }
        }
        /// <summary>
        /// Output string creator class.  Stores dilution data as epMotion formated csv output strings.
        /// </summary>
        private class EppCsvString
        {
            string textOut;
            bool assignProblem;

            string barcodeID;
            string sourceRack;
            string sourceWell;
            string destinationRack;
            string destinationWell;
            string volume;
            string tool;
            string name;

            public EppCsvString()
            {
                name = "";
                barcodeID = "";
                assignProblem = false;
            }
            public void IntakeTableData(object[] dataRowIn, int transferTypeIn )
            {
                assignWell(dataRowIn[0].ToString());
                assignVolume(Convert.ToDouble(dataRowIn[3]));
                assignRack(transferTypeIn);
                if (assignProblem)
                    return;
                GenerateString();
                Console.WriteLine(textOut);
            }
            private void GenerateString()
            {
                textOut = "";
                textOut += barcodeID + ",";
                textOut += sourceRack + ",";
                textOut += sourceWell + ",";
                textOut += destinationRack + ",";
                textOut += destinationWell + ",";
                textOut += volume + ",";
                textOut += tool + ",";
                textOut += name;
            }
            public void assignWell(string wellIn)
            {
                destinationWell = wellIn;
                sourceWell = wellIn;
                if (wellIn == null)
                    assignProblem = true;
            }
            public void assignVolume(double volumeIn)
            {
                volume = Math.Round(volumeIn, 2).ToString();
                volumeIn = Math.Round(volumeIn, 2);
                if (volumeIn <= 50 && volumeIn > 0)
                    tool = "TS_50";
                else if (volumeIn > 50 && volumeIn <= 300)
                    tool = "TS_300";
                else if (volumeIn > 300 && volumeIn <= 1000)
                    tool = "TS_1000";
                else {
                    tool = "";
                    assignProblem = true;
                }
            }
            public void assignRack(int sampleType)
            {
                switch (sampleType)
                {
                    case (int)TransferType.TeIntermediate:
                        sourceRack = "1";
                        destinationRack = "1";
                        break;
                    case (int)TransferType.TeFinal:
                        sourceRack = "1";
                        destinationRack = "2";
                        break;
                    case (int)TransferType.DiluteDna2Intermediate:
                        sourceRack = "1";
                        destinationRack = "1";
                        break;
                    case (int)TransferType.OriginalDna2Intermediate:
                        sourceRack = "2";
                        destinationRack = "1";
                        break;
                    case (int)TransferType.DiluteDna2Final:
                        sourceRack = "1";
                        destinationRack = "2";
                        break;
                    case (int)TransferType.OrginalDna2Final:
                        sourceRack = "2";
                        destinationRack = "2";
                        break;
                    case (int)TransferType.Intermediate2Final:
                        sourceRack = "3";
                        destinationRack = "2";
                        break;
                    default:
                        sourceRack = "";
                        destinationRack = "";
                        assignProblem = true;
                        break;
                }

            }
            
        }
        static List<string> ImportEppCsv()
        {
            List<string> eppFileHeader = new List<string>(7);
            eppFileHeader[0] = "Rack,Src.Barcode,Src.List Name,Dest.Barcode,Dest.List Name,,,";
            eppFileHeader[1] = "1,,,,,,,";
            eppFileHeader[2] = "2,,,,,,,";
            eppFileHeader[3] = "3,,,,,,,";
            eppFileHeader[4] = "4,,,,,,,";
            eppFileHeader[5] = ",,,,,,,";
            eppFileHeader[6] = "Barcode ID, Rack, Source, Rack, Destination, Volume, Tool, Name";
            //eppFileHeader[7] = ",1,A1,1,A1,173.4433098,TS_300,";   // samples for you
            //eppFileHeader[8] = ",1,A2,1,A2,182.022397,TS_300,";
            //eppFileHeader[9] = ",1,A3,1,A3,177.058856,TS_300,";
            //eppFileHeader[10] = ",1,A4,1,A4,162.3747153,TS_300,";
            return eppFileHeader;
        }
        public enum TransferType
        {
            TeIntermediate, TeFinal,
            DiluteDna2Intermediate, OriginalDna2Intermediate, DiluteDna2Final, OrginalDna2Final, Intermediate2Final
        }
    }
}
