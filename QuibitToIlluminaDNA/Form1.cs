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
        public bool useLowDNA;
        public bool useHighDNA;
        public bool use12x8Plate;

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
            if (!use12x8Plate)
            {
                if (row + 1 > 8)
                {
                    column++;
                    row = 1;
                }
                else
                {
                    row++;
                }
            } else
            {
                if (column + 1 > 12)
                {
                    row++;
                    column = 1;
                }
                else
                {
                    column++;
                }
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
                catch (FormatException formatExpt)
                {
                    try
                    {
                        double nearControl = double.Parse(thisline.Split(',')[10]);
                        double farControl = double.Parse(thisline.Split(',')[11]);
                        double farReading = double.Parse(thisline.Split(',')[15]);
                        if (farReading + 2000 > farControl)
                        {
                            lineOut.Add(DilutionLineFromInput(false));
                        } else if (farReading < (nearControl * 2))
                        {
                            lineOut.Add(DilutionLineFromInput(true));
                        }
                    }
                    catch (Exception formatDoubleException)
                    {
                        System.AggregateException ae = new AggregateException(formatExpt, formatDoubleException);
                        throw ae.InnerException;
                    }
                }
                catch (Exception theException)
                {
                    String errorMessage;
                    errorMessage = "Error: ";
                    errorMessage = String.Concat(errorMessage, theException.Message);
                    errorMessage = String.Concat(errorMessage, " Line: ");
                    errorMessage = String.Concat(errorMessage, theException.Source);
                    Console.Write(errorMessage);
                }
                if(dnaConcentrationIn != 0)
                    lineOut.Add(DilutionLineFromInput(dnaConcentrationIn));
                
                NextWell();
            }
            lineOut.RemoveAll(x => x == null);
            dataGridViewOut.DataSource = PopulateTable(lineOut);
            ExportFilesButton.Enabled = true;
        }
        public string[] DilutionLineFromInput(bool tooLow)
        {
            string[] lineOutput = new string[7];
            // Well     Input DNA   Dilution Ratio  TE intermediate well    TE final well   Final Volume    Final Concentration
            // 1        2           3               4                       5               6               7
            lineOutput[0] = wellIn;

            if (tooLow  && !useLowDNA)
            {
                lineOutput[1] = "Low";
                lineOutput[2] = "0";
                lineOutput[3] = "None";
                lineOutput[4] = "None";
                lineOutput[5] = "None";
                lineOutput[6] = "Low";
            } else if (!tooLow && useHighDNA)
            {
                lineOutput[1] = "High";
                lineOutput[2] = "1:100";
                lineOutput[3] = "1000";
                lineOutput[4] = "40";
                lineOutput[5] = "50";
                lineOutput[6] = "High";
            } else if (tooLow && useLowDNA)
            {
                lineOutput[1] = "Low";
                lineOutput[2] = "0";
                lineOutput[3] = "None";
                lineOutput[4] = "None";
                lineOutput[5] = "20";
                lineOutput[6] = "Low";
            } else if (!tooLow && !useHighDNA)
            {
                lineOutput[1] = "High";
                lineOutput[2] = "0";
                lineOutput[3] = "None";
                lineOutput[4] = "None";
                lineOutput[5] = "None";
                lineOutput[6] = "High";
            }
            return lineOutput;
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
                if (usingDiluteInput && useLowDNA)
                {
                    float TEAddFloat = TEOnlyAddition(dnaVolumeInitial, dnaConIn * 10, dnaConcetrationFinal);
                    // for bad DNA < initial concentration, use original
                    lineOutput[1] = "Orig " + dnaConIn * 10;
                    lineOutput[2] = "0";
                    lineOutput[3] = "None";
                    if (TEAddFloat > 0)
                    {
                        lineOutput[4] = TEAddFloat.ToString();
                        lineOutput[5] = (Convert.ToDouble(lineOutput[4]) + dnaVolumeInitial).ToString();
                        lineOutput[6] = ((dnaConIn * 10) * ( dnaVolumeInitial / Convert.ToDouble(lineOutput[5]))).ToString();
                    }
                    else {
                        lineOutput[4] = "None";
                        lineOutput[5] = (dnaVolumeInitial).ToString();
                        lineOutput[6] = (dnaConIn * 10).ToString();
                    }
                    
                    ;
                } else
                {
                   
                    lineOutput[2] = "0";
                    lineOutput[3] = "None";
                    lineOutput[4] = "None";
                    lineOutput[5] = (dnaVolumeInitial * 2).ToString();
                    lineOutput[6] = dnaConIn.ToString();
                }
                
            }
            else if (dnaConIn > dnaConcetrationFinal + 0.02)
            {
                // tricky tricky Equation: 2^floor(log2(x) - ([final] - 1)) // last part only if final > 1
                float diltionFix = dnaConcetrationFinal - 1;
                if (diltionFix < 0) diltionFix = 0;
                var dilutionFactor = Math.Pow(2, ((Math.Floor(Math.Log(dnaConIn, 2))) - diltionFix));
                if (dilutionFactor < 1) { dilutionFactor = 1; }
                Console.WriteLine("Dilution factor : " + dilutionFactor);
                if (dilutionFactor == 1)
                {
                    float TEAddFloat = TEOnlyAddition(dnaVolumeInitial, dnaConIn, dnaConcetrationFinal);
                    lineOutput[2] = "1:"+ dilutionFactor.ToString();
                    lineOutput[3] = "None";
                    if (TEAddFloat > 0)
                    {
                        lineOutput[4] = TEAddFloat.ToString();
                        lineOutput[5] = (TEAddFloat + dnaVolumeInitial).ToString();
                        lineOutput[6] = (dnaConIn * (dnaVolumeInitial / Convert.ToDouble(lineOutput[5]))).ToString();
                    }
                    else
                    {
                        lineOutput[4] = "None";
                        lineOutput[5] = (dnaVolumeInitial).ToString();
                        lineOutput[6] = (dnaConIn).ToString();
                    }
                }
                else if (dilutionFactor <= 64)
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
                    if (!useHighDNA)
                    {
                        lineOutput[2] = "1:" + dilutionFactor.ToString();
                        lineOutput[3] = "None";
                        lineOutput[4] = "None";
                        lineOutput[5] = "None";
                        lineOutput[6] = "Concentration too high";
                    } else
                    {
                        var dnaConcentrationIntermediate = dnaConIn / 100;
                        string[] s = { dnaConIn.ToString(), dilutionFactor.ToString(), ((dilutionFactor * dnaVolumeInitial) - dnaVolumeInitial).ToString(), TEOnlyAddition(dnaVolumeInitial, dnaConcentrationIntermediate, dnaConcetrationFinal).ToString() };
                        lineOutput[2] = "1:100";
                        // TODO : turn this into math
                        lineOutput[3] = ((100 * dnaVolumeInitial) - dnaVolumeInitial).ToString();
                        lineOutput[4] = "40";
                        lineOutput[5] = (40 + dnaVolumeInitial).ToString();
                        lineOutput[6] = (dnaConcentrationIntermediate * (dnaVolumeInitial / Convert.ToDouble(lineOutput[5]))).ToString();
                    }
                    
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
            while(tableOut.Rows.Count < listIn.Count())
            {
                counter++;
                tableOut.Rows.Add();
                FileSelectedLabel.Text = "Adding Rows: " + counter.ToString();
                FileSelectedLabel.Update();
            }
            for (int i = 0; i < listIn.Count(); i++)
            {
                if (listIn[i] == null)
                    continue;
                for (int j = 0; j < listIn[i].Count(); j++) {
                    if (listIn[i][j] != null && listIn.Count() > 0)
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
            if (teVolumeOut < 0)
            {
                teVolumeOut = 0;
                Console.WriteLine("Issue with TE addition, produced : " + teVolumeOut);
                Console.WriteLine("DNA volume in : " + dnaVolumeIn);
                Console.WriteLine("DNA concentration in : " + dnaConcentrationIn);
                Console.WriteLine("DNA concentration final : " + finalConcentrationTarget);
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
           
            List<string> teStringList = new List<string>();
            List<string> dnaStringList = new List<string>();
            // open a TE file to write to
            // open a DNA file to write to 
            // open a Intermediate file to write to
            string selectString = "";
            for (int transfertype = 0; transfertype < 7; transfertype++){
                switch (transfertype){
                    //Well  Input DNA   Dilution Ratio  TE intermediate well    TE final well   Final Volume    Final Concentration
                    case 0 :
                        // Te to final well
                        selectString = "[TE final well] <> 'None'";
                         break;
                    case 1:
                        // te to intermediate well
                        selectString = "[TE intermediate well] <> 'None'";
                        break;
                    case 2:
                        // dilute dna to intermediate
                        selectString = "[Input DNA] NOT LIKE 'Orig*' AND [TE intermediate well] <> 'None'";
                        break;
                    case 3:
                        // original dna to intermediate
                        selectString = "[Input DNA] LIKE 'Orig*' AND [TE intermediate well] <> 'None'";
                        break;
                    case 4:
                        // dilute dna to final
                        selectString = "[Input DNA] NOT LIKE 'Orig*' AND [TE intermediate well] >= 'None'";
                        break;
                    case 5:
                        // original dna to final
                        selectString = "[Input DNA] LIKE 'Orig*' AND [TE intermediate well] >= 'None'";
                        break;
                    case 6:
                        // intermediate dna to final
                        selectString = "[TE intermediate well] <> 'None'";
                        break;
                    case -1:
                        //case reserved for wtf cases
                        selectString = "[orginal well] = 0";
                        break;
                    default:
                         break;
                 }
                if (transfertype == 0 || transfertype == 1)
                {
                    DataRow[] workingrows = exportTable.Select(selectString);
                    foreach (DataRow row in workingrows)
                    {
                        EppCsvString epOutstring = new EppCsvString();
                        epOutstring.DataIn(Convert.ToDouble(row.ItemArray[4 - transfertype]), transfertype, row.ItemArray[0].ToString());
                        teStringList.Add(epOutstring.textOut);
                    }
                }
                else if (transfertype == 4 || transfertype == 5)
                {
                    DataRow[] workingrows = exportTable.Select(selectString);
                    foreach (DataRow row in workingrows) {
                        EppCsvString epOutString = new EppCsvString();
                        try
                        {
                            if (row.ItemArray[3].ToString() == "None") { 
                                double largerFinalDNA = Convert.ToDouble(row.ItemArray[5]) - Convert.ToDouble(row.ItemArray[4]);
                                epOutString.DataIn(largerFinalDNA, transfertype, row.ItemArray[0].ToString());
                                dnaStringList.Add(epOutString.textOut);
                            } else
                            {
                                Console.WriteLine("Cannot find none in " + row.ItemArray[4]);
                            }
                        } catch (FormatException)
                        {
                            epOutString.DataIn(dnaVolumeInitial, transfertype, row.ItemArray[0].ToString());
                            dnaStringList.Add(epOutString.textOut);
                        }
                        catch (Exception otherException)
                        {
                            String errorMessage;
                            errorMessage = "Error: ";
                            errorMessage = String.Concat(errorMessage, otherException.Message);
                            errorMessage = String.Concat(errorMessage, " Line: ");
                            errorMessage = String.Concat(errorMessage, otherException.Source);
                            Console.WriteLine(errorMessage);
                        }
                    }
                } else
                {
                    DataRow[] workingrows = exportTable.Select(selectString);
                    foreach (DataRow row in workingrows)
                    {
                        EppCsvString epOutString = new EppCsvString();
                        epOutString.DataIn(dnaVolumeInitial, transfertype, row.ItemArray[0].ToString());
                        dnaStringList.Add(epOutString.textOut);
                    }
                }
             }
            // sort that list
            dnaStringList.Sort(delegate (string x, string y)
            {
                int rackCompare = x.Split(',')[1].CompareTo(y.Split(',')[1]);
                if (x.Split(',')[1] == null && y.Split(',')[1] == null) return 0;
                else if (x.Split(',')[1] == null) return -1;
                else if (y.Split(',')[1] == null) return 1;
                else if (rackCompare != 0 ) return rackCompare;
                else return x.Split(',')[2].CompareTo(y.Split(',')[2]);
            });
            teStringList.Sort(delegate (string x, string y)
            {
                int rackCompare = x.Split(',')[1].CompareTo(y.Split(',')[1]);
                if (x.Split(',')[1] == null && y.Split(',')[1] == null) return 0;
                else if (x.Split(',')[1] == null) return -1;
                else if (y.Split(',')[1] == null) return 1;
                else if (rackCompare != 0) return rackCompare;
                else return x.Split(',')[2].CompareTo(y.Split(',')[2]);
            });

            DataTable outputCSVOne = new DataTable();
            int columns = 8;
            // Add columns.
            for (int i = 0; i < columns; i++)
            {
                outputCSVOne.Columns.Add();
            }

            // Add rows.
            foreach (string thisString in dnaStringList)
            {
                outputCSVOne.Rows.Add(thisString.Split(','));
            }
            dataGridViewOut.DataSource = outputCSVOne;
            OutputCSV(dnaStringList, 1);
            OutputCSV(teStringList, 2);
        }
        public void OutputCSV(List<string> epFormattedIn, int formatIn)
        {
            string outputName = "";
            if (formatIn == 1)
                outputName = Path.GetFileNameWithoutExtension(QubitFileInString) + "_DNA_EP.csv";
            else if (formatIn == 2)
                outputName = Path.GetFileNameWithoutExtension(QubitFileInString) + "_TE_EP.csv";
            string outputPath = Path.GetDirectoryName(QubitFileInString) + Path.DirectorySeparatorChar + outputName;
            // TODO: see if this can be identified as open already and save as secondary name
            using (StreamWriter outfile = new StreamWriter(outputPath))
            {
                foreach (string s in ImportEppCsv())
                {
                    outfile.WriteLine(s);
                }
                foreach (string s in epFormattedIn)
                {
                    outfile.WriteLine(s);
                }
            }
        }

        /// <summary>
        /// epMotion CSV input string creator class.  Stores dilution data as epMotion formated csv input strings.
        /// </summary>
        private class EppCsvString
        {
            public string textOut;
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
            /// <summary>
            /// Collects data for a substantiated eppCsvString. Performs internal calculations to provide textOut.
            /// </summary>
            /// <param name="volumeIn"> takes in te or dna volume </param>
            /// <param name="transferTypeIn"> takes in enum TransferType </param>
            /// <param name="wellIn"> take in well number </param>
            public void DataIn (double volumeIn, int transferTypeIn, string wellIn )
            {
                assignWell(wellIn, transferTypeIn);
                assignVolume(volumeIn);
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
            public void assignWell(string wellIn, int transferType)
            {
                // TODO : add in a 1A as the well for te, and check for input type
                if (transferType == 0 || transferType == 1)
                    sourceWell = "1A";
                else
                    sourceWell = wellIn;
                destinationWell = wellIn;
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
        /// <summary>
        /// Import header format for a epMotion file.  Used for writting header sequence of CSV outputs.
        /// </summary>
        /// <returns></returns>
        static List<string> ImportEppCsv()
        {
            List<string> eppFileHeader = new List<string>(new string[7]);
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

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            useLowDNA = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            useHighDNA = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            use12x8Plate = checkBox4.Checked;
        }
    }
}
