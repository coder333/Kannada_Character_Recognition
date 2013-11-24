/*

 * Form1.cs

 * 

 * A GUI that communicates with a Touch Screen Device over the USB.

 * Also Recognizes Kannada Characters drawn on the Touch Screen and displays it.

 * 

 * USB Communication done with the help of (c)Simon Inns's USB Framework, distributed 

 * under the GNU General Public License.

 * 

 * Functions made use of for USB communication:

 * 1.bool writeRawReportToDevice(Byte[])

 * 2.bool readSingleReportFromDevice(Byte[])

 * 

 * Written By: Jeevan B S

 * Project:    Touch Screen Input Device for Kannada Language Script

 */

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



namespace myFirstApp

{

    // USB Library used to communicate with the device

    using usbGenericHidCommunications;



    public partial class Form1 : Form

    {

        

        string winDir = System.Environment.GetEnvironmentVariable("windir");

        

        private void addWrite(Byte[] coor, int index)
        {

            StreamWriter writer = new StreamWriter("c:\\Users\\wind\\Desktop\\RawCoordinates.txt");

            writer.WriteLine("File created using StreamWriter class");

            writer.WriteLine(coor[index]);

        }



        

        public Form1()

        {

            

            InitializeComponent();



            // Create the USB reference device object (passing VID and PID)

            theReferenceUsbDevice = new usbReferenceDevice(0x04D8, 0x0080);



            // Add a listener for usb events

            theReferenceUsbDevice.usbEvent +=

              new usbReferenceDevice.usbEventsHandler(usbEvent_receiver);



            // Perform an initial search for the target device

            theReferenceUsbDevice.findTargetDevice();

        }



        class usbReferenceDevice : usbGenericHidCommunication

        {

            // Declared to be visible to all functions

            // temp and temp1 used at various stages to read text from Files

            string temp, temp1;

            // a and b used to hold integer values of temp and temp1 (at various stages)

            int a, b = 0, i = 0;



            // Class constructor - place any initialisation here

            public usbReferenceDevice(int vid, int pid)

                : base(vid, pid)

            {

            }



            /* Used to conform if the device is connected to PC */

            public bool test1()

            {

                // Test 1 - Send a single write packet to the USB device

                // Declare our output buffer

                Byte[] outputBuffer = new Byte[65];



                // Byte 0 must be set to 0

                outputBuffer[0] = 0;



                // Byte 1 must be set to our command

                // Command 0x80 writes a single Report to Device

                outputBuffer[1] = 0x80;



                // Fill the rest of the buffer with known data

                int bufferPointer;

                Byte data = 0;

                for (bufferPointer = 2; bufferPointer < 65; bufferPointer++)

                {

                    // We send the numbers 0 to 63 to the device

                    outputBuffer[bufferPointer] = data;

                    data++;

                }



                // Perform the write command

                bool success;

                success = writeRawReportToDevice(outputBuffer);



                // We can't tell if the device received the data ok, we are

                // only indicating that the write was error free.

                return success;

            }



            /* Gets RawCoordinates from the Device and Writes it to a File */

            public Byte[] rawCoordinates()

            {

                // Declare our output buffer

                Byte[] outputBuffer = new Byte[65];



                // Byte 0 must be set to 0

                outputBuffer[0] = 0;



                // Byte 1 must be set to our command

                // Command 0x81 gets a Single Report from Device

                outputBuffer[1] = 0x81;



                // Fill the rest of the buffer with known data

                int bufferPointer;

                Byte data = 0;

                for (bufferPointer = 2; bufferPointer < 65; bufferPointer++)

                {

                    // We send the numbers 0 to 63 to the device

                    outputBuffer[bufferPointer] = data;

                    data++;

                }



                // Perform the write command

                bool success, success1;

                success1 = writeRawReportToDevice(outputBuffer);

                // Perform the Read command

                success = readSingleReportFromDevice(ref outputBuffer);

               

                // Write Raw Coordinates to a text file (RawCoordinates.txt in this case)

                // Also use argument 'true' to append text

                StreamWriter writer = new StreamWriter("c:\\Users\\wind\\Desktop\\RawCoordinates.txt", true);

                

                // If Higher Byte is greater than 0, then 

                // RawCoordinate = Higher Byte Value + 255;

                // Otherwise RawCoordinate = Lower Byte Value;

                int o1 = Convert.ToInt32(outputBuffer[1]);

                int o2 = Convert.ToInt32(outputBuffer[2]);

                if (outputBuffer[1] > 0)

                    o2 = 255 + o2;



                writer.WriteLine(o2);

                writer.Close();



                // Send the received data to the caller

                return outputBuffer;

                

            }



            /* Writes Database Pattern to a File for Comparison with the Output File

             * Gets the name of the Normal character file in 'string iFile'

             * Gets the name of the Sign character file in 'string iSignFile'

             * Gets the name of the Vathakshara character file in 'string iVathaksharaFile'

             */

            public void prepareCompFile(string iFile, string iSignFile, string iVathaksharaFile)           

            {        

                // 'string addresscPatternFile' contains the address of the 

                // File containing the Character Pattern (Normal characters)

                string addresscPatternFile = "c:\\Users\\wind\\Desktop\\cPattern\\";

                addresscPatternFile = addresscPatternFile + iFile + ".txt";

                StreamReader cPatternFile = new StreamReader(addresscPatternFile);



                // 'string addressSignFile' contains the address of the

                // File containing the Sign Character Pattern

                string addressSignFile = "c:\\Users\\wind\\Desktop\\cPattern\\signs\\";

                addressSignFile = addressSignFile + iSignFile + ".txt";

                StreamReader SignFile = new StreamReader(addressSignFile);

                

                // 'string addressVathaksharaFile' contains the address of the

                // File containing the Vathakshara Character Pattern

                string addressVathaksharaFile = "c:\\Users\\wind\\Desktop\\cPattern\\vathakshara\\";

                addressVathaksharaFile = addressVathaksharaFile + iVathaksharaFile + ".txt";

                StreamReader vathaksharaFile = new StreamReader(addressVathaksharaFile);



                // Address of the File used for comparison is directly given as the StreamWriter() argument

                StreamWriter addressCompFile = new StreamWriter("c:\\Users\\wind\\Desktop\\compFile.txt");



                // Write Patterns of the Normal character, Sign character, Vathakshara character respectively

                // to comparison file

                StreamReader cPatternFile1 = new StreamReader(addresscPatternFile);

                int nullIsIn = 0;

                while ((temp = cPatternFile1.ReadLine()) != null)

                    nullIsIn++;

                cPatternFile1.Close();



                while ((temp = cPatternFile.ReadLine()) != null)

                {

                    if ((iSignFile != "SnoPattern") && (iSignFile != "Su") && (iSignFile != "Suu"))

                    {                        

                        if (nullIsIn <= 2)

                            break;

                        nullIsIn--;

                        addressCompFile.WriteLine(temp);

                /*        if (temp == "0")

                        {

                            string buf = cPatternFile.ReadLine();

                            string buf1 = cPatternFile.ReadLine();

                            if ((buf == " 1") && (buf1 == null))

                                break;

                            else

                            {

                                addressCompFile.WriteLine(temp);

                                addressCompFile.WriteLine(buf);

                                addressCompFile.WriteLine(buf1);

                            }

                        }

                        else

                            addressCompFile.WriteLine(temp);*/

                    }

                    else

                        addressCompFile.WriteLine(temp);

                }

                while ((temp = SignFile.ReadLine()) != null)

                    addressCompFile.WriteLine(temp);

                while ((temp = vathaksharaFile.ReadLine()) != null)

                    addressCompFile.WriteLine(temp);



                // Remember to close all StreamWriter and StreamReader Pointers

                SignFile.Close();

                addressCompFile.Close();

                cPatternFile.Close();

                vathaksharaFile.Close();

            }



            /* Recognizes the character that was drawn on the Touch Screen

             */

            public string recognizeCharacter(string in_cPattern)

            {                

                // 'cHold' and 'cHold1' used to hold characters read from various files

                string cHold, cHold1;

                // 'recChar' is replaced with the Unicode number of the Recognized Character (if a match is found)

                string recChar = "";

                // All files holding the character pattern are mentioned in an 'index.txt' file

                // Use this file to get the names of the files. Three layers contain three different 

                // index files

                StreamReader indexSign = new StreamReader("c:\\Users\\wind\\Desktop\\cPattern\\signs\\index.txt");

                

                string addressOutFile = "c:\\Users\\wind\\Desktop\\out.txt";

                

                // Signs database contains 17 files. Go through all of them

                for (int i = 0; i < 17; i++)

                {

                    StreamReader indexVathakshara = new StreamReader("c:\\Users\\wind\\Desktop\\cPattern\\vathakshara\\index.txt");

                    // 'sendSign' contains the present file name drawn from the index

                    string sendSign = indexSign.ReadLine();



                    // Location of the Unicode Number of Signs

                    string addressSignNum = "c:\\Users\\wind\\Desktop\\cPattern\\signs\\";

                    addressSignNum = addressSignNum + sendSign + "Num.txt";

                    StreamReader SignNum = new StreamReader(addressSignNum);



                    // Vathakshara database contains 13 files

                    for (int j = 0; j < 13; j++)

                    {

                        string sendVathakshara = indexVathakshara.ReadLine();

                        prepareCompFile(in_cPattern, sendSign, sendVathakshara);

                        StreamReader outFile = new StreamReader(addressOutFile);

                    

                        // Location of Unicode Number of Vathaksharas

                        string addressVathaksharaNum = "c:\\Users\\wind\\Desktop\\cPattern\\";

                        addressVathaksharaNum = addressVathaksharaNum + sendVathakshara + "Num.txt";

                        StreamReader VathaksharaNum = new StreamReader(addressVathaksharaNum);



                        StreamReader compFile = new StreamReader("c:\\Users\\wind\\Desktop\\compFile.txt");

                        // Compares pattern in Comparison file to the Output file

                        while (true)

                        {

                            // Hold number (represented in char) from Output file

                            cHold = outFile.ReadLine();

                            // Hold number (represented in char) from Comparison file

                            cHold1 = compFile.ReadLine();

                            // Change 'recChar' if both the Output and the Comaprison files reach their end

                            // Thereby indicating that all the numbers matched

                            if ((cHold == null) && (cHold1 == null))

                            {

                                // Method of getting Unicode number varies depending whether there is

                                // a Vathakshara present or not

                                if (sendVathakshara == "noPattern")

                                    recChar = giveNumber(in_cPattern) + SignNum.ReadLine();

                                else

                                    recChar = giveNumber(in_cPattern) + "\u0ccd" + VathaksharaNum.ReadLine() + SignNum.ReadLine();

                                outFile.Close();

                                compFile.Close();

                                return recChar;

                            }



                            // If either one of the files reaches their end first then break out of the loop

                            if ((cHold == null) || (cHold1 == null))

                                break;

                            

                            // Break out of loop if the numbers (represented as characters) donot match

                            if (cHold != cHold1)

                                break;

                        }

                    compFile.Close();

                    outFile.Close();

                    

                    VathaksharaNum.Close();

                    }

                    SignNum.Close();

                    

                    indexVathakshara.Close();

                }

                

                indexSign.Close();



                // Returns a Unicode if a character was recognized. Otherwise, 'recChar' remains

                // unchanged from its definition ("")

                return recChar;

            }



            /* Returns the Unicode number and takes the name of the recognized character file as the

             * argument

             */

            public string giveNumber(string in_cPattern)

            {

                // Append 'Num' to the file name

                string numAdd = in_cPattern + "Num";

                // Open the file to get the Unicode number

                StreamReader numRetriever = new StreamReader("c:\\Users\\wind\\Desktop\\cPattern\\" + numAdd + ".txt");

                temp = numRetriever.ReadLine();

                numRetriever.Close();

                // Return the Unicode number

                return temp;

            }



            /* Applies various techniques to make sense of the Raw Coordinates

             * Does not take any arguments and does not return anything

             */

            public void getCoordinates()

            {

                // See if the Next coordinate is within the range of +5 or -5 from the Previous coordinate

                StreamReader reader1 = new StreamReader("c:\\Users\\wind\\Desktop\\RawCoordinates.txt");

                StreamWriter writer1 = new StreamWriter("c:\\Users\\wind\\Desktop\\RawCoordinates1.txt", true);

                bool loopEntered = false;

                // If facing problems in the future: Look at the following while loop, used 

                // to remove 0s induced by Erasing process. Might be loosing one coordinate here

                while ((temp = reader1.ReadLine()) != null)

                {

                    // Conversion from string to int is required to apply mathematical functions

                    a = Convert.ToInt32(temp);

                    b = Convert.ToInt32(temp1);

                    // Takes care of a loophole in the kit

                    if (a < 20)

                    {

                        a = 0;

                        temp = Convert.ToString(a);

                    }

                    // Perform calculations only if the Present and the Previous values are present

                    // Which is not the case in the first round of execution. So wait till the second

                    // round of execution to begin calculations

                    if (loopEntered)

                    {

                        if (((a - b) <= 5) && ((a - b) >= -5))

                        {

                            a = b;

                            temp = Convert.ToString(a);

                        }

                        else if (a == b)

                        {

                            a = b;

                            temp = Convert.ToString(a);

                        }

                        else

                            a = a;

                    }

                    writer1.WriteLine(temp);

                    temp1 = temp;

                    loopEntered = true;

                }

                reader1.Close();

                writer1.Close();



                // Makes note of all those coordinates that appear in blocks of three

                StreamReader reader2 = new StreamReader("c:\\Users\\wind\\Desktop\\RawCoordinates1.txt");

                StreamWriter writer2 = new StreamWriter("c:\\Users\\wind\\Desktop\\in.txt", true);

                bool loopEntered2 = false;

                int index = 1;

                while ((temp = reader2.ReadLine()) != null)

                {

                    if (loopEntered2)

                    {

                        if (temp == temp1)

                            index++;



                        if (temp != temp1)

                            index = 1;



                        if (index == 4)

                        {

                            writer2.WriteLine(temp);

                            index = 1;

                        }

                    }

                    temp1 = temp;

                    loopEntered2 = true;

                }

                reader2.Close();

                writer2.Close();



                // If the same coordinates appears consecutively, then consider it as a single value

                // not as multiple different values

                StreamReader reader3 = new StreamReader("c:\\Users\\wind\\Desktop\\in.txt");

                StreamWriter writer3 = new StreamWriter("c:\\Users\\wind\\Desktop\\in1.txt", true);

                bool loopEntered3 = false;

                while ((temp = reader3.ReadLine()) != null)

                {

                    if (!loopEntered3)

                        writer3.WriteLine(temp);

                    

                    if (loopEntered3)

                    {

                        if (temp == temp1)

                            continue;

                        else

                            writer3.WriteLine(temp);

                    }

                    loopEntered3 = true;

                    temp1 = temp;



                }

                reader3.Close();

                writer3.Close();



                // Does the first set of calculations to ascertain the direction of movement

                // on the touchscreen

                StreamReader reader4 = new StreamReader("c:\\Users\\wind\\Desktop\\in1.txt");

                StreamWriter writer4 = new StreamWriter("c:\\Users\\wind\\Desktop\\in2.txt", true);

                bool loopEntered4 = false;

                while ((temp = reader4.ReadLine()) != null)

                {

                    // Again wait till the second round of executions to perform calculations

                    if (loopEntered4)

                    {

                        a = Convert.ToInt32(temp);

                        b = Convert.ToInt32(temp1);

                        

                        // If the current or the previous value is 0, then write them as it is

                        // Do not perform any calculations with them. It leads to errors

                        if (temp1 == "0")

                        {

                            writer4.WriteLine(temp1);

                            temp1 = temp;                            

                            continue;

                        }

                        if (temp == "0")

                        {

                            writer4.WriteLine(temp);

                            temp1 = temp;                           

                            continue;

                        }



                        // Movement is to the right or upwards

                        if ((a - b) > 0)

                        {

                            writer4.WriteLine(" 1");

                        }

                        // Movement is to the left or downwards

                        else if ((a - b) < 0)

                        {

                            writer4.WriteLine("-1");

                        }

                    }

                    loopEntered4 = true;

                    temp1 = temp;

                }

                reader4.Close();

                writer4.Close();



                // Compress the 1s, -1s and 0s obtained through the previous set of calculations

                StreamReader reader5 = new StreamReader("c:\\Users\\wind\\Desktop\\in2.txt");

                StreamWriter writer5 = new StreamWriter("c:\\Users\\wind\\Desktop\\outp.txt", true);

                bool loopEntered5 = false;

                while ((temp = reader5.ReadLine()) != null)

                {

                    if (!loopEntered5)

                        writer5.WriteLine(temp);



                    if (loopEntered5)

                    {

                        if (temp != temp1)

                            writer5.WriteLine(temp);

                    }

                    loopEntered5 = true;

                    temp1 = temp;

                }

                writer5.Close();

                reader5.Close();



                // Removing the Trailing 0

                StreamReader reader6 = new StreamReader("c:\\Users\\wind\\Desktop\\outp.txt");

                StreamWriter writer6 = new StreamWriter("c:\\Users\\wind\\Desktop\\out.txt", true);

                bool loopEntered6 = false;

                while (true)

                {

                    string prsnt = reader6.ReadLine();

                    string nxt = reader6.ReadLine();

                    if (prsnt == null)

                        break;

                    if ((prsnt == "0") || (nxt == "0"))

                    {

                        if ((nxt == null) || (prsnt == null))

                            break;

                        else

                        {

                            if (loopEntered6)

                                writer6.WriteLine(prsnt);

                            writer6.WriteLine(nxt);

                        }

                    }

                    else

                    {

                        writer6.WriteLine(prsnt);

                        if (nxt != null)

                            writer6.WriteLine(nxt);

                    }



                    



                    loopEntered6 = true;

                }

                writer6.Close();

                reader6.Close();

            }

        }



        /*

         * Takes care of button clicks

         */



        string sentences = "";



        // Test button clicked (button1)

        private void button1_Click(object sender, EventArgs e)

        {

            if (theReferenceUsbDevice.test1()) this.label1.Text = "Test passed";

            else this.label1.Text = "Test failed";

        }



        // Upper Byte button clicked (button2)

        private void button2_Click(object sender, EventArgs e)

        {

            Byte[] m = new Byte[65];

            m = theReferenceUsbDevice.rawCoordinates();

            this.label2.Text = m[1].ToString();

        }



        // Record button clicked (button3)

        private void button3_Click(object sender, EventArgs e)

        {

            Byte[] n = new Byte[65];

            n = theReferenceUsbDevice.rawCoordinates();

            int oi1 = Convert.ToInt32(n[1]);

            int oi2 = Convert.ToInt32(n[2]);

            if (n[1] > 0)

                oi2 = 255 + oi2;

            this.label3.Text = oi2.ToString();

        }

        

        // Filter button clicked (button4)

        private void button4_Click(object sender, EventArgs e)

        {

            theReferenceUsbDevice.getCoordinates();

        }

        

        // Erase button clicked (button5)

        private void button5_Click(object sender, EventArgs e)

        {

            // Writer a 0 in place of all the other text. Thereby effectively erasing all text

            // Note that StreamWriter() doesnot contain 'true' as the second argument.

            // That is , no appending is done.

            StreamWriter eraser1 = new StreamWriter("c:\\Users\\wind\\Desktop\\RawCoordinates.txt");

            eraser1.WriteLine(0);

            eraser1.Close();

            

            StreamWriter eraser2 = new StreamWriter("c:\\Users\\wind\\Desktop\\RawCoordinates1.txt");

            eraser2.WriteLine(0);

            eraser2.Close();



            StreamWriter eraser3 = new StreamWriter("c:\\Users\\wind\\Desktop\\in.txt");

            eraser3.WriteLine(0);

            eraser3.Close();



            StreamWriter eraser4 = new StreamWriter("c:\\Users\\wind\\Desktop\\in1.txt");

            eraser4.WriteLine(0);

            eraser4.Close();



            StreamWriter eraser5 = new StreamWriter("c:\\Users\\wind\\Desktop\\in2.txt");

            eraser5.WriteLine(0);

            eraser5.Close();



            StreamWriter eraser6 = new StreamWriter("c:\\Users\\wind\\Desktop\\out.txt");

            eraser6.WriteLine(0);

            eraser6.Close();



            StreamWriter eraser7 = new StreamWriter("c:\\Users\\wind\\Desktop\\outp.txt");

            eraser7.WriteLine(0);

            eraser7.Close();

        }



        // Create an instance of the USB reference device

        private usbReferenceDevice theReferenceUsbDevice;



        // Listener for USB events

        private void usbEvent_receiver(object o, EventArgs e)

        {

            this.button1.Enabled = true;



        }

        

        // Recognize button pressed (button6)

        private void button6_Click(object sender, EventArgs e)

        {

            // Outputs a Unicode number if a character is recognized

            // Other wise outputs ""

            string charTransfer = "", recognized1 = "", recognized2 = "",recognized = "", recognized3 = "";

            string in_cPattern = "";

            StreamReader indexFile = new StreamReader("c:\\Users\\wind\\Desktop\\cPattern\\index.txt");

            for (int i = 0; i < 47; i++)

            {

                in_cPattern = indexFile.ReadLine();

                charTransfer = theReferenceUsbDevice.recognizeCharacter(in_cPattern);

                if (charTransfer != "")

                {

                    if (recognized == "")

                    {

                        recognized = charTransfer;

                        charTransfer = "";

                        this.richTextBox1.Text = this.richTextBox1.Text + recognized;

                        richTextBox1.ZoomFactor = 3;

                    }

                    else if (recognized1 == "")

                    {

                        recognized1 = charTransfer;

                        charTransfer = "";

                        this.richTextBox2.Text = recognized1;

                        richTextBox2.ZoomFactor = 2;

                    }

                    else if (recognized2 == "")

                    {

                        recognized2 = charTransfer;

                        charTransfer = "";

                        this.richTextBox3.Text = recognized2;

                        richTextBox3.ZoomFactor = 2;

                    }

                    else

                    {

                        recognized3 = charTransfer;

                        charTransfer = "";

                        this.richTextBox4.Text = recognized3;

                        richTextBox4.ZoomFactor = 2;

                    }

                }

            }

            // A 'rich text box' is used for display

            //this.richTextBox1.Text = this.richTextBox1.Text + recognized;

            //this.richTextBox2.Text = recognized1;

            //this.richTextBox3.Text = recognized2;

            indexFile.Close();

        }



        private void button7_Click(object sender, EventArgs e)

        {

            sentences = this.richTextBox1.Text;

            int lngth = sentences.Length;

            if (lngth == 4)

                sentences = sentences.Remove(lngth - 4);

            else if (lngth == 3)

                sentences = sentences.Remove(lngth - 3);

            else if (lngth == 2)

                sentences = sentences.Remove(lngth - 2);

            else if (lngth == 1)

                sentences = sentences.Remove(lngth - 1);

            else

                sentences = sentences.Remove(lngth - 4);

            sentences = sentences + this.richTextBox2.Text;

            this.richTextBox1.Text = sentences;

        }



        private void button8_Click(object sender, EventArgs e)

        {

            sentences = this.richTextBox1.Text;

            int lngth = sentences.Length;

            if (lngth == 4)

                sentences = sentences.Remove(lngth - 4);

            else if (lngth == 3)

                sentences = sentences.Remove(lngth - 3);

            else if (lngth == 2)

                sentences = sentences.Remove(lngth - 2);

            else if (lngth == 1)

                sentences = sentences.Remove(lngth - 1);

            else

                sentences = sentences.Remove(lngth - 4);

            sentences = sentences + this.richTextBox3.Text;

            this.richTextBox1.Text = sentences;

        }



        private void button9_Click(object sender, EventArgs e)

        {

            sentences = this.richTextBox1.Text;

            int lngth = sentences.Length;

            if (lngth == 4)

                sentences = sentences.Remove(lngth - 4);

            else if (lngth == 3)

                sentences = sentences.Remove(lngth - 3);

            else if (lngth == 2)

                sentences = sentences.Remove(lngth - 2);

            else if (lngth == 1)

                sentences = sentences.Remove(lngth - 1);

            else

                sentences = sentences.Remove(lngth - 4);

            sentences = sentences + this.richTextBox4.Text;

            this.richTextBox1.Text = sentences;

        }

        

    }

}
