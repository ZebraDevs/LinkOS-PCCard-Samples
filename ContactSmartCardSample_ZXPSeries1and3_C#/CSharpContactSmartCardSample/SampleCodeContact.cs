/**********************************************
* CONFIDENTIAL AND PROPRIETARY
*
* The source code and other information contained herein is the confidential and the exclusive property of
* ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
* This source code, and any other information contained herein, shall not be copied, reproduced, published,
* displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
* expressly permitted under such license agreement.
*
* Copyright ZIH Corp. 2011
*
* ALL RIGHTS RESERVED
***********************************************
File: SampleCodeCONTACT.cs
Description: Example code showing how to apply CONTACT smart card encoding.
$Revision: 1 $
$Date: 2010/12/06 $
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CSharpContactSmartCardSample
{
    class SampleCodeCONTACT
    {
        public static int CONTACT = 1;
        
        #region Constructor

        // Constructor --------------------------------------------------------------------------------------

        public SampleCodeCONTACT()
        {
        }

        #endregion
 

        #region CONTACT Sample Code

        // CONTACT Sample Code ----------------------------------------------------------------------------------

        public void ContactEncode(string drvName, string _contactReader, int eject, out string msg)
        {
            ZBRPrinter printer = null; 
            int errValue = 0; 
            msg = "";
            try
            {
                printer = new ZBRPrinter(); 

                // Opens a connection to a printer driver
                if (!printer.Open(drvName, out msg)  )
                {
                    msg = "CONTACT Open Error: " + ZBRUtil.TranslateErrorCode(errValue) + "\n" + "Error Code : " + errValue.ToString();
                    return;
                }

                // Moves the card into position for encoding 
                if (printer.MoveCardToSmartCardEncoder(CONTACT, out errValue) == 0)
                {
                    msg = "CONTACT StartCard Error:" + ZBRUtil.TranslateErrorCode(errValue) + "\n" + "Error Code : " + errValue.ToString();
                    return;
                }
  
                 byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
  

                //perform the smart card encoding:
                ContactSupport contact = new ContactSupport();
                string cardType = "PVC,SLE4428";
                contact.ContactTest(ref cardType, _contactReader, key, true, out msg);

                if (msg.Length > 0)
                    return;

                if (printer.MoveSmartCardToPrintReadyPosition(CONTACT,   out errValue) == 0)
                {
                    msg = "CONTACT Prnt Card Error: " + errValue.ToString();
                    return;
                }
                
                if (printer.EjectCard(out errValue) == 0)
                {
                    msg = "CONTACT Eject Card Error: " + errValue.ToString();
                    return;

                }
            }
            catch (Exception ex)
            {
                msg += ex.Message;
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "CONTACTCode threw exception");
            }
            finally
            {  
                if (printer != null)
                {
                    printer.EjectCard(out errValue);
                    if (msg.Length == 0)
                        printer.Close(out msg);
                    else
                    {
                        string tempmsg = "";
                        printer.Close(out tempmsg);

                    }
                    printer = null;
                }
            }
        }

        #endregion
    }
}
