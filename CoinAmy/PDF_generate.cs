using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Windows;

namespace CoinAmy
{
    internal class PDF_generate
    {
        Document PDF_document;
        PdfWriter PDF_writer;
        Font boldFont;

        DbConnect dbConnect;

        public PDF_generate(string filePath) //A SaveFileDialog által adott mentési útvonal a paraméter
        {
            PDF_document = new Document(PageSize.A4);

            try
            {
                PDF_writer = PdfWriter.GetInstance(PDF_document, new FileStream(filePath, FileMode.Create));
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            boldFont = FontFactory.GetFont("Times New Roman", 26, Font.BOLD);

            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");
        }

        public bool Connect()
        {
            try
            {
                PDF_document.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Connect_Close()
        {
            try
            {
                PDF_document.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void GenerateDocument()
        {
            if (Connect())
            {
                PDF_document.AddAuthor("CoinAmy"); //Megadja a dokumentum készítőnek személyét

                string imageURL = @"..\..\Images\coinamy_logo_original.png"; //Megadja a logo elérési útvonalát
                Image image = Image.GetInstance(imageURL);
                image.ScalePercent(15f); //Beállítja a logó méretét százalékos arányban
                PDF_document.Add(image); //Hozzáadja a dokumentumhoz az image példányát

                PDF_document.Add(new Paragraph("Portfolióm", boldFont) //Hozzádja a dokumentumhoz a feliratot a megadott tulajdonságokkal
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20,
                });

                PdfPTable investmentsTable = new PdfPTable(6); //Megadja a táblázat oszlopainak számát

                investmentsTable = dbConnect.LoadDataToExport(); //A dbConnect osztályból érkező adatokat a példánynak adja
                investmentsTable.WidthPercentage = 100;

                PDF_document.Add(investmentsTable);

                PDF_document.Add(new Paragraph($"Exportálás dátuma: {DateTime.Now}") //Hozzádja a dokumentumhoz az exportálás dátumát a megadott tulajdonságokkal
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 20
                });

                Connect_Close();
            }
        }
    }
}
